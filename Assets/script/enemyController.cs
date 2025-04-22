using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movController = GetComponent<MovController>();
        anim = GetComponent<Animator>();
        
        ghostSprit = GetComponent<SpriteRenderer>();
        ghostEyesSprit = transform.Find("eyes").GetComponent<SpriteRenderer>();
        color = ghostSprit.color;
        if (ghostType == GhostType.Blinky)
        {
            startGhostStat = GhostNodeStat.startNode;
            respawnState = GhostNodeStat.centerNode;
            GhoststartingNode = NodeStart;
        }
        else if(ghostType == GhostType.Pinky)
        {
            startGhostStat = GhostNodeStat.centerNode;
            respawnState = GhostNodeStat.centerNode;
            GhoststartingNode = NodeStart;  // For respawning
           
        }
        else if(ghostType == GhostType.Inky)
        {
            startGhostStat = GhostNodeStat.leftNode;
            respawnState = GhostNodeStat.leftNode;
            GhoststartingNode = NodeLeft;
        }
        else if(ghostType == GhostType.Clyde)
        {
            startGhostStat = GhostNodeStat.rightNode;
            respawnState = GhostNodeStat.rightNode;
            GhoststartingNode = NodeRight;
        }
        
    }
    public void Setup()
    {
        nodeStat = startGhostStat;
        readyToLeaveHome = false;
        anim.SetBool("move", false);
        //reset ghosts back to home
        movController.currentNode = GhoststartingNode;
        transform.position = GhoststartingNode.transform.position;

        movController.direction = "";
        movController.lastMoveDirec = "";
        isFrightened = false;

        leftHomBefor = false;
        
        if(ghostType == GhostType.Pinky )
        {
            movController.currentNode = NodeCenter;  // Start at center
            transform.position = NodeCenter.transform.position;  // Start at center
            readyToLeaveHome = true;
            leftHomBefor = true;
        }
        else if(ghostType == GhostType.Blinky)
        {
            readyToLeaveHome = true;
            leftHomBefor = true;
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
            ghostSprit.color = new Color(225, 225, 225, 255);
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
        if(nodeStat == GhostNodeStat.movingNode)
        {
            leftHomBefor = true;
            if(gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {
                //scatter mode
                ScatterMode();
            }
            else if (isFrightened)//frighten mode
            {
                string direction = getRandomDirection();
                movController.setdirction(direction);
            }
            else
            { //chase mode

                //Determine next game node to go to
                if (ghostType == GhostType.Blinky)
                {
                    redGhostDirection();
                }
                else if(ghostType == GhostType.Pinky)
                {
                    pinkGhostDirection();
                }
                else if(ghostType == GhostType.Inky)
                {
                    blueGhostDirection();
                }
                else if(ghostType == GhostType.Clyde)
                {
                    orangeGhostDirection();
                }
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
    void redGhostDirection()
    {
        string dirction = ClosestDirection(gameManager.pacman.transform.position);
        movController.setdirction(dirction);
    }
    void pinkGhostDirection()
    {
        string pacmanDirection = gameManager.pacman.GetComponent<MovController>().lastMoveDirec;
        float distenceBetweenNodes = 0.82f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmanDirection == "left") target.x -= distenceBetweenNodes * 2;
        else if (pacmanDirection == "right") target.x += distenceBetweenNodes * 2;
        else if (pacmanDirection == "up") target.y += distenceBetweenNodes * 2;
        else if (pacmanDirection == "down") target.y -= distenceBetweenNodes * 2;

        string direction = ClosestDirection(target);
        movController.setdirction(direction);
    }
    void blueGhostDirection()
    {
        string pacmanDirection = gameManager.pacman.GetComponent<MovController>().lastMoveDirec;
        float distenceBetweenNodes = 0.82f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmanDirection == "left") target.x -= distenceBetweenNodes * 2;
        else if (pacmanDirection == "right") target.x += distenceBetweenNodes * 2;
        else if (pacmanDirection == "up") target.y += distenceBetweenNodes * 2;
        else if (pacmanDirection == "down") target.y -= distenceBetweenNodes * 2;

        GameObject redGhost = gameManager.blinky;
        float xDistense = target.x - redGhost.transform.position.x;
        float yDistense = target.y - redGhost.transform.position.y;

        Vector2 inkyTarget = new Vector2(target.x + xDistense, target.y + yDistense);
        string direction = ClosestDirection(inkyTarget);
        movController.setdirction(direction);
    }
    void orangeGhostDirection()
    {
        float distenceBetweenNodes = 0.82f;
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        if (distance < 0) distance *= -1; //bax ta lakan salib nrj3oh mojab bax tshal 3lina l5dma
        
        //if we'r within 8 nodes from pacman chase using red chase mode
        if(distance <= distenceBetweenNodes * 8)
        {
            redGhostDirection();
        }
        else
        {
            //scater mode 
            ScatterMode();
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
    void ScatterMode()
    {
        string dirction = ClosestDirection(ScaterNode.transform.position);
        movController.setdirction(dirction);
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
        isVisibel = isItVisibel;
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && nodeStat != GhostNodeStat.respawning)
        {
            //player eat ghost
            if (isFrightened)
            {
                nodeStat = GhostNodeStat.respawning;
                gameManager.GhostEaten();
                
            }
            else //ghost eat player
            {
                StartCoroutine(gameManager.PlayerEaten());
            }
        }
    }
}
