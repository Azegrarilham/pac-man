using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class playerController : MonoBehaviour
{
    [SerializeField] InputAction MoveAction;
    Vector2 move;
    MovController movControl;
    Animator anim;
    SpriteRenderer sprite;

    public GameObject startNode;
    public Vector2 StartPos;

    GameManager gameManager;
    void Awake()
    {
        
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartPos = new Vector2(0.18f, -1.7584f);
        MoveAction.Enable();
        movControl = GetComponent<MovController>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
       

        startNode = movControl.currentNode;
        
    }
    public void Setup()
    {
       
        if (movControl == null || startNode == null || anim == null)
        {
            Debug.Log("Required components are missing in Setup!");
            return;
        }
        anim.SetBool("death", false);
        sprite.flipX = false;
        movControl.currentNode = startNode;
        transform.position = StartPos;
       
        movControl.lastMoveDirec = "";
        movControl.direction = "left";
        anim.speed = 1;
        anim.SetBool("move", false);
    }
    void Update()
    {
        if (!gameManager.GameRuning)
        {
            return; 
        }
        move = MoveAction.ReadValue<Vector2>();
        anim.SetBool("move", true);
        if (move.y > 0) movControl.setdirction("up");
        else if (move.y < 0) movControl.setdirction("down");
        else if (move.x > 0) movControl.setdirction("right");
        else if (move.x < 0) movControl.setdirction("left");

        flipSprit(movControl.lastMoveDirec);
        
    }
    public void Stopanim()
    {
        anim.speed = 0;
    }
    public void flipSprit(string lastMove)
    {
        
        bool flipX = false;
        bool flipY = false;

        if (lastMove == "left")
        {
            anim.SetInteger("deriction", 0);
        }
        else if (lastMove == "right")
        {
            anim.SetInteger("deriction", 0);
            flipX = true;
        }
        else if (lastMove == "up")
        {
            anim.SetInteger("deriction", 1);
        }
        else if (lastMove == "down")
        {
            anim.SetInteger("deriction", 1);
            flipY = true;
        }
        
        sprite.flipY = flipY;
        sprite.flipX = flipX;
        
    }
    public void Death()
    {
        anim.SetBool("move", false);
        anim.speed = 1;
        anim.SetBool("death", true);
    }
}
