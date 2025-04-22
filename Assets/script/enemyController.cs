using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static enemyController;

public class enemyController : MonoBehaviour
{
    public enum GhostNodeStat
    {
        /*enum is a special "class" that represents a group
        of constants(unchangeable/read-only variables).*/
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingNode,
    }
    public GhostNodeStat nodeStat;
    public GhostNodeStat respawnState;
    public GhostNodeStat startGhostStat;
    public enum GhostType
    {
        Blinky,
        Pinky,
        Inky,
        Clyde,
    }
    public GhostType ghostType;
   

    public GameObject NodeLeft;
    public GameObject NodeRight;
    public GameObject NodeCenter;
    public GameObject NodeStart;

    public MovController movController;
    public GameObject GhoststartingNode; //to respawn in when needed

    public bool readyToLeaveHome = false;
    GameManager gameManager;
    public bool testRespawn;
    public bool isFrightened;

    public GameObject ScaterNode;

    public bool leftHomBefor;
    public bool isVisibel = true;

    SpriteRenderer ghostSprit;
    SpriteRenderer ghostEyesSprit;
    Animator anim;
    public Color color;



    private float lastDirectionChangeTime = 0f;
    private const float minTimeBetweenDirectionChanges = 0.1f; // Adjust this value
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movController = GetComponent<MovController>();
        anim = GetComponent<Animator>();
        
        ghostSprit = GetComponent<SpriteRenderer>();
        ghostEyesSprit = transform.Find("eyes").GetComponent<SpriteRenderer>();
        color = ghostSprit.color;
        InitializeGhostType();
    }
    public void Setup()
    {
        nodeStat = startGhostStat;
        readyToLeaveHome = false;
        anim.SetBool("move", false);

        movController.currentNode = GhoststartingNode;
        transform.position = GhoststartingNode.transform.position;

        movController.direction = "";
        movController.lastMoveDirec = "";
        isFrightened = false;
        leftHomBefor = false;

        if (ghostType == GhostType.Pinky)
        {
            movController.currentNode = NodeCenter;
            transform.position = NodeCenter.transform.position;
            readyToLeaveHome = true;
            leftHomBefor = true;
            nodeStat = GhostNodeStat.centerNode;
        }
        else if (ghostType == GhostType.Blinky)
        {
            readyToLeaveHome = true;
            leftHomBefor = true;
            nodeStat = GhostNodeStat.startNode;
        }
        else if (ghostType == GhostType.Inky)
        {
            nodeStat = GhostNodeStat.leftNode;
        }
        else if (ghostType == GhostType.Clyde)
        {
            nodeStat = GhostNodeStat.rightNode;
        }

        setVisibel(true);
    }

    private void Update()
    {
        if (nodeStat != GhostNodeStat.movingNode || !gameManager.isPowerPelletRuning) isFrightened = false;
        
        if(nodeStat == GhostNodeStat.movingNode) anim.SetBool("move", true);
        if (isVisibel)//show sprites
        {
            if(nodeStat != GhostNodeStat.respawning) ghostSprit.enabled = true;
            else ghostSprit.enabled = false;

            ghostEyesSprit.enabled = true;
        }
        else //hide sprit
        {
            ghostSprit.enabled = false;
            ghostEyesSprit.enabled = false;
        }

        if (!gameManager.GameRuning)
        {
            return;
        }
        if (isFrightened)
        {
            anim.SetBool("frightened", true);
            ghostEyesSprit.enabled = false;
            ghostSprit.color = Color.white;
        }
        else
        {
            anim.SetBool("frightened", false);
            ghostSprit.color = color;
        }
       
        if (testRespawn)
        {
            readyToLeaveHome = false;
            nodeStat = GhostNodeStat.respawning;
            testRespawn = false;
        }
        if (movController.currentNode.GetComponent<NodeController>().isSideNode) movController.setSpeed(1);
        else movController.setSpeed(3);
    }
    public void setFrightened(bool isIt)
    {
        isFrightened = isIt;
    }
    public void ReachedCenterNode(NodeController node)
    {
        // Add a small delay between direction changes to prevent jittering
        if (Time.time - lastDirectionChangeTime < minTimeBetweenDirectionChanges) return;

        if (nodeStat == GhostNodeStat.movingNode)
        {
            leftHomBefor = true;
            string newDirection = "";

            // Calculate new direction based on mode
            if (gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {
                newDirection = GetScatterModeDirection();
            }
            else if (isFrightened)
            {
                newDirection = getRandomDirection();
            }
            else
            {
                newDirection = GetChaseModeDirection();
            }

            // Only change direction if it's different and valid
            if (!string.IsNullOrEmpty(newDirection) && newDirection != movController.direction)
            {
                movController.setdirction(newDirection);
                lastDirectionChangeTime = Time.time;
            }
        }
        else if(nodeStat == GhostNodeStat.respawning)
        {
            string direction = "";
            //if we reached our start node move to the center node;
            if(transform.position.x == GhoststartingNode.transform.position.x && transform.position.y == GhoststartingNode.transform.position.y)
            {
                direction = "down";
             
            }
            //we reached center either finish or go left/right node
            else if(transform.position.x == NodeCenter.transform.position.x && transform.position.y == NodeCenter.transform.position.y)
            {
                if(respawnState == GhostNodeStat.centerNode)
                {
                    nodeStat = respawnState;
                }
                else if(respawnState == GhostNodeStat.leftNode)
                {
                    direction = "left";
                }
                else if(respawnState == GhostNodeStat.rightNode)
                {
                    direction = "right";
                }
            }
            //ila wslna fin b4ina leave home agin
            else if
                (
                 (transform.position.x == NodeLeft.transform.position.x && transform.position.y == NodeLeft.transform.position.y)
                 || (transform.position.x == NodeRight.transform.position.x && transform.position.y == NodeRight.transform.position.y)
                )
            {
                nodeStat = respawnState;
            }
            else//mazal mawslanax l home 
            {
                //Determin quickest direction to home
                direction = ClosestDirection(GhoststartingNode.transform.position);
            }
            movController.setdirction(direction);
        }
        else
        {
            //if we are ready to leavr home
            if (readyToLeaveHome)
            {
                if(nodeStat == GhostNodeStat.leftNode)
                {
                    nodeStat = GhostNodeStat.centerNode;
                    movController.setdirction("right");
                }
                else if(nodeStat == GhostNodeStat.rightNode)
                {
                    nodeStat = GhostNodeStat.centerNode;
                    movController.setdirction("left");
                }
                else if(nodeStat == GhostNodeStat.centerNode)
                {
                    nodeStat = GhostNodeStat.startNode;
                    movController.setdirction("up");
                }
                else if(nodeStat == GhostNodeStat.startNode)
                {
                    nodeStat = GhostNodeStat.movingNode;
                    movController.setdirction("left");
                }
            }
        }
    }
    private string GetChaseModeDirection()
    {
        // Return appropriate direction based on ghost type
        switch (ghostType)
        {
            case GhostType.Blinky:
                return ClosestDirection(gameManager.pacman.transform.position);
            case GhostType.Pinky:
                return GetPinkyTarget();
            case GhostType.Inky:
                return GetInkyTarget();
            case GhostType.Clyde:
                return GetClydeTarget();
            default:
                return "";
        }
    }

    private string GetScatterModeDirection()
    {
        // Return direction to scatter target (corner)
        return ClosestDirection(ScaterNode.transform.position);
    }

    // Helper methods for specific ghost behaviors
    private string GetPinkyTarget()
    {
        string pacmanDirection = gameManager.pacman.GetComponent<MovController>().lastMoveDirec;
        float distanceBetweenNodes = 0.82f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmanDirection == "left") target.x -= distanceBetweenNodes * 2;
        else if (pacmanDirection == "right") target.x += distanceBetweenNodes * 2;
        else if (pacmanDirection == "up") target.y += distanceBetweenNodes * 2;
        else if (pacmanDirection == "down") target.y -= distanceBetweenNodes * 2;

        return ClosestDirection(target);
    }

    private string GetInkyTarget()
    {
        string pacmanDirection = gameManager.pacman.GetComponent<MovController>().lastMoveDirec;
        float distanceBetweenNodes = 0.82f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmanDirection == "left") target.x -= distanceBetweenNodes * 2;
        else if (pacmanDirection == "right") target.x += distanceBetweenNodes * 2;
        else if (pacmanDirection == "up") target.y += distanceBetweenNodes * 2;
        else if (pacmanDirection == "down") target.y -= distanceBetweenNodes * 2;

        GameObject redGhost = gameManager.blinky;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 inkyTarget = new Vector2(target.x + xDistance, target.y + yDistance);
        return ClosestDirection(inkyTarget);
    }

    private string GetClydeTarget()
    {
        float distanceBetweenNodes = 0.82f;
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);

        // If Clyde is far from Pacman, chase directly
        if (distance > distanceBetweenNodes * 8)
        {
            return ClosestDirection(ScaterNode.transform.position);
        }
        // If Clyde is close to Pacman, scatter
        else
        {
            return ClosestDirection(gameManager.pacman.transform.position);
        }
    }

    string getRandomDirection()
    {
        List<string> posibelDirections = new List<string>();
        NodeController nodeCntrl = movController.currentNode.GetComponent<NodeController>();

        if(nodeCntrl.canMoveDown && movController.lastMoveDirec != "up")
        {
            posibelDirections.Add("down");
        }
        if(nodeCntrl.canMoveUP && movController.lastMoveDirec != "down")
        {
            posibelDirections.Add("up");
        }
        if(nodeCntrl.canMoveRight && movController.lastMoveDirec != "left")
        {
            posibelDirections.Add("right");
        }
        if(nodeCntrl.canMoveLeft && movController.lastMoveDirec != "right")
        {
            posibelDirections.Add("left");
        }

        string direction = "";
        int directionIndex = Random.Range(0, posibelDirections.Count - 1);
        direction = posibelDirections[directionIndex];
        return direction;
    }
    string ClosestDirection(Vector2 target)
    {
        float shortDistense = 0;
        string lastMovDirection = movController.lastMoveDirec;
        string newDirection = "";
        NodeController nodeController = movController.currentNode.GetComponent<NodeController>();
        //if we can move up and we'r not reversing
        if(nodeController.canMoveUP && lastMovDirection != "down")
        {
            //get the node above us
            GameObject nodeUp = nodeController.NodeUp;
            //get the distance between our top node , and pacman
            float distance = Vector2.Distance(nodeUp.transform.position, target);

            //if this is the shortest distance so far, set our direction
            if(distance < shortDistense || shortDistense == 0)
            {
                shortDistense = distance;
                newDirection = "up";
            }
        }
        //if we can move down and we'r not reversing
        if (nodeController.canMoveDown && lastMovDirection != "up")
        {
            //get the node below us
            GameObject nodeDown = nodeController.NodeDown;
            //get the distance between our down node , and pacman
            float distance = Vector2.Distance(nodeDown.transform.position, target);

            //if this is the shortest distance so far, set our direction
            if (distance < shortDistense || shortDistense == 0)
            {
                shortDistense = distance;
                newDirection = "down";
            }
        }
        //if we can move left and we'r not reversing
        if (nodeController.canMoveLeft && lastMovDirection != "right")
        {
            //get the node beside us
            GameObject nodeLeft = nodeController.NodeLeft;
            //get the distance between our left node , and pacman
            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            //if this is the shortest distance so far, set our direction
            if (distance < shortDistense || shortDistense == 0)
            {
                shortDistense = distance;
                newDirection = "left";
            }
        }
        //if we can move right and we'r not reversing
        if (nodeController.canMoveRight && lastMovDirection != "left")
        {
            //get the node beside us
            GameObject nodeRight = nodeController.NodeRight;
            //get the distance between our right node , and pacman
            float distance = Vector2.Distance(nodeRight.transform.position, target);

            //if this is the shortest distance so far, set our direction
            if (distance < shortDistense || shortDistense == 0)
            {
                shortDistense = distance;
                newDirection = "right";
            }
        }
        return newDirection;
    }
    public void setVisibel(bool isItVisibel)
    {
        this.isVisibel = isItVisibel; //  assign to the class field
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && nodeStat != GhostNodeStat.respawning)
        {
            //player eat ghost
            if (isFrightened)
            {
                nodeStat = GhostNodeStat.respawning;
                gameManager.GhostEaten();

                // Add this to make sure ghosts can leave home after respawning
                StartCoroutine(PrepareToLeaveHome());
            }
            else //ghost eat player
            {
                StartCoroutine(gameManager.PlayerEaten());
            }
        }
    }
    private IEnumerator PrepareToLeaveHome()
    {
        yield return new WaitForSeconds(2f);
        if (!gameManager.GameRuning) yield break;
        // Reset position to proper starting node based on ghost type
        switch (ghostType)
        {
            case GhostType.Inky:
                movController.currentNode = NodeLeft;
                transform.position = NodeLeft.transform.position;
                nodeStat = GhostNodeStat.leftNode;
                break;
            case GhostType.Clyde:
                movController.currentNode = NodeRight;
                transform.position = NodeRight.transform.position;
                nodeStat = GhostNodeStat.rightNode;
                break;
            default:
                movController.currentNode = NodeCenter;
                transform.position = NodeCenter.transform.position;
                nodeStat = GhostNodeStat.centerNode;
                break;
        }

        readyToLeaveHome = true;
        leftHomBefor = false;

    }
    private void InitializeGhostType()
    {
        switch (ghostType)
        {
            case GhostType.Blinky:
                startGhostStat = GhostNodeStat.startNode;
                respawnState = GhostNodeStat.centerNode;
                GhoststartingNode = NodeStart;
                break;
            case GhostType.Pinky:
                startGhostStat = GhostNodeStat.centerNode;
                respawnState = GhostNodeStat.centerNode;
                GhoststartingNode = NodeStart;
                break;
            case GhostType.Inky:
                startGhostStat = GhostNodeStat.leftNode;
                respawnState = GhostNodeStat.leftNode;
                GhoststartingNode = NodeLeft;
                break;
            case GhostType.Clyde:
                startGhostStat = GhostNodeStat.rightNode;
                respawnState = GhostNodeStat.rightNode;
                GhoststartingNode = NodeRight;
                break;
        }
    }

}
