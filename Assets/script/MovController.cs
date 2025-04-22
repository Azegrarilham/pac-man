using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovController : MonoBehaviour
{
    public GameObject currentNode;
    public float speed = 4f;

    public string direction = "";
    public string lastMoveDirec = "";

    public GameManager gameManager;
    public bool canWrap = true;
    public bool isGhost = false;
    void Start()
    {
        if (gameObject.CompareTag("Ghost")) isGhost = true;
    }

    
    void Update()
    {
        if (!gameManager.GameRuning)
        {
            return;
        }

       
        NodeController currentNdC = currentNode.GetComponent<NodeController>();
        transform.position = Vector2.MoveTowards(transform.position, currentNdC.transform.position, speed * Time.deltaTime);

        bool reverseDeriction = IsReverseDirection();

        //check if we're at the center of the current node
        if (AtNodeCenter(currentNdC) || reverseDeriction)
        {
            if (isGhost)
            {
                GetComponent<enemyController>().ReachedCenterNode(currentNdC);
            }
            if (currentNdC.isLeftwrap && canWrap)
            {
                currentNode = gameManager.RightNodewrap;
                direction = "left";
                lastMoveDirec = "left";
                transform.position = currentNode.transform.position;
                canWrap = false;
            }
            else if (currentNdC.isRightwrap && canWrap)
            {
                currentNode = gameManager.leftNodewrap;
                direction = "right";
                lastMoveDirec = "right";
                transform.position = currentNode.transform.position;
                canWrap = false;
            }
            else
            {
                //if we'r not a respawning ghost and we trying to move down don't
                if(currentNdC.isGhostStartingNode && direction == "down"
                   && (!isGhost || GetComponent<enemyController>().nodeStat != enemyController.GhostNodeStat.respawning))
                {
                    direction = lastMoveDirec;
                }
                //get the next node from node controller using our current direction
                GameObject nextNode = currentNdC.GetNodefromDirection(direction);
                //if we can move to the direction
                if (nextNode != null)
                {
                    currentNode = nextNode;
                    lastMoveDirec = direction;
                }
                else
                {//try to keep going to the last direction
                    direction = lastMoveDirec;
                    nextNode = currentNdC.GetNodefromDirection(direction);
                    if (nextNode != null)
                    {
                        currentNode = nextNode;
                    }
                }
            }
        }
        else canWrap = true;
    }
    public void setdirction(string ToDeriction)
    {
        direction = ToDeriction;
    }
    private bool IsReverseDirection()
    {
        return (direction == "left" && lastMoveDirec == "right") ||
               (direction == "right" && lastMoveDirec == "left") ||
               (direction == "up" && lastMoveDirec == "down") ||
               (direction == "down" && lastMoveDirec == "up");
    }


    // Helper to check if at the center of the node
    private bool AtNodeCenter(NodeController node)
    {
        return Mathf.Approximately(transform.position.x, node.transform.position.x) &&
               Mathf.Approximately(transform.position.y, node.transform.position.y);
    }
    public void setSpeed(float newspeed)
    {
        speed = newspeed;
    }
}
