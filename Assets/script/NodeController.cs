using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public bool canMoveUP, canMoveDown, canMoveRight, canMoveLeft;

    public GameObject NodeUp, NodeDown, NodeLeft, NodeRight;
    float disLength = 0.7f;
    public bool isLeftwrap = false;
    public bool isRightwrap = false;

    public bool isPelletNode = false;
    public bool hasPellet = false;
    SpriteRenderer pelletSprit;

    public bool isGhostStartingNode;
    public bool isSideNode;

    public bool isPowerPellet = false;
    public float powerPelletBlinkingtimer = 0;
    GameManager gameManager;
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(transform.childCount > 0)
        {
            gameManager.GotPallet(this);
            hasPellet = true;
            isPelletNode = true;
            pelletSprit = GetComponentInChildren<SpriteRenderer>();
        }
        RaycastHit2D[] hitDown;
        //shoot the ray line down
        hitDown = Physics2D.RaycastAll(transform.position, Vector2.down);
        for(int i =0; i < hitDown.Length; i++)
        {
            float distance = Mathf.Abs(hitDown[i].point.y - transform.position.y);
            if(distance < disLength && hitDown[i].collider.tag == "Node")
            {
                canMoveDown = true;
                NodeDown = hitDown[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitUp;
        //shoot the ray line up
        hitUp = Physics2D.RaycastAll(transform.position, Vector2.up);
        for(int i = 0; i < hitUp.Length; i++)
        {
            float distance = Mathf.Abs(hitUp[i].point.y - transform.position.y);
            if(distance < disLength && hitUp[i].collider.tag == "Node")
            {
                canMoveUP = true;
                NodeUp = hitUp[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitRight;
        //shoot the ray line right
        hitRight = Physics2D.RaycastAll(transform.position, Vector2.right);
        for(int i = 0; i < hitRight.Length; i++)
        {
            float distance = Mathf.Abs(hitRight[i].point.x - transform.position.x);
            if(distance < disLength && hitRight[i].collider.tag == "Node")
            {
                canMoveRight = true;
                NodeRight = hitRight[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitLeft;
        //shoot the ray line Left
        hitLeft = Physics2D.RaycastAll(transform.position, Vector2.left);
        for(int i = 0; i < hitLeft.Length; i++)
        {
            float distance = Mathf.Abs(hitLeft[i].point.x - transform.position.x);
            if(distance < disLength && hitLeft[i].collider.tag == "Node")
            {
                canMoveLeft = true;
                NodeLeft = hitLeft[i].collider.gameObject;
            }
        }
        if (isGhostStartingNode)
        {
            canMoveDown = true;
            NodeDown = gameManager.GhostNodeCenter;
        }
    }
    private void Update()
    {
        //if (!gameManager.GameRuning) return;

        //if(isPowerPellet && hasPellet)
        //{
        //    powerPelletBlinkingtimer += Time.deltaTime;
        //    if(powerPelletBlinkingtimer >= 0.1f)
        //    {
        //        powerPelletBlinkingtimer = 0;
        //        pelletSprit.enabled = !pelletSprit.enabled;
        //    }
        //}
        if (!gameManager.GameRuning) return;

        if (isPowerPellet && hasPellet)
        {
            float alpha = Mathf.PingPong(Time.time * 10, 1);
            pelletSprit.color = new Color(1, 1, 1, alpha);
        }
    }
    public GameObject GetNodefromDirection(string direction)
    {
        switch (direction)
        {
            case "left":
                return canMoveLeft ? NodeLeft : null;
            case "right":
                return canMoveRight ? NodeRight : null;
            case "up":
                return canMoveUP ? NodeUp : null;
            case "down":
                return canMoveDown ? NodeDown : null;
            default:
                return null;
        }
    }
    public void RespawnPellet()
    {
        if (isPelletNode)
        {
            hasPellet = true;
            pelletSprit.enabled = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && hasPellet)
        {
           
            hasPellet = false;
            pelletSprit.enabled = false;

            StartCoroutine(gameManager.CollectedPallets(this));
        }
    }
}
