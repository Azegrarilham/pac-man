using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject leftNodewrap;
    public GameObject RightNodewrap;
    public GameObject pacman;

    public GameObject GhostNodeLeft;
    public GameObject GhostNodeRight;
    public GameObject GhostNodeCenter;
    public GameObject GhostNodeStart;

    public GameObject blinky;
    public GameObject inky;
    public GameObject pinky;
    public GameObject clyde;

    public int totalPallets;
    public int palletLeft;
    public int palletColectedInLife;
    public bool hasDeathinThisLevel;
    public bool GameRuning;

    public List<NodeController> nodeControllers = new List<NodeController>();
    public enum GhostMode
    {
        chase, scatter,
    }
    public GhostMode currentGhostMode;

    enemyController blinkyController;
    enemyController pinkyController;
    enemyController inkyController;
    enemyController clydeController;

    public bool ClearLevel;
    public bool newGame;

    public AudioSource startGame;
    public int lives;
    public int currentLevel;

    public Image background;
    int scoreCount = 0;
    public TextMeshProUGUI score;
    public TextMeshProUGUI GameOverText;

    public int[] ghostModeTimer = new int[] { 7, 20, 7, 20, 5, 20, 5};
    public int ghostTimerIndex;
    public float ghostTimer = 0;
    public bool runningTimer;
    public bool completedTimer;

    public bool isPowerPelletRuning = false;
    public float curentPowerPelletTime = 0;
    public float powerPelletTimer = 8f;
    int powerPelletMultiplyer = 1;
    void Start()
    {
        background.enabled = false;
        GameOverText.enabled = false;
        newGame = true;
        ClearLevel = false;
        blinkyController = blinky.GetComponent<enemyController>();
        pinkyController = pinky.GetComponent<enemyController>();
        inkyController = inky.GetComponent<enemyController>();
        clydeController = clyde.GetComponent<enemyController>();
        lives = 3;
        GhostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
        pacman = GameObject.Find("PacMan");
        StartCoroutine(Setup());
    }

    private void Update()
    {
        if (!GameRuning) return;
        if(!completedTimer && runningTimer)
        {
            ghostTimer += Time.deltaTime;
            if(ghostTimer >= ghostModeTimer[ghostTimerIndex])
            {
                ghostTimer = 0;
                ghostTimerIndex++;
                if(currentGhostMode == GhostMode.chase)
                {
                    currentGhostMode = GhostMode.scatter;
                }
                else
                {
                    currentGhostMode = GhostMode.chase;
                }

                if(ghostTimerIndex == ghostModeTimer.Length)
                {
                    completedTimer = true;
                    runningTimer = false;
                    currentGhostMode = GhostMode.chase;
                }
            }
        }

        if (isPowerPelletRuning)
        {
            curentPowerPelletTime += Time.deltaTime;
            if(curentPowerPelletTime >= powerPelletTimer)
            {
                isPowerPelletRuning = false;
                curentPowerPelletTime = 0;
                powerPelletMultiplyer = 1;
                //stop frighten sound and play the normal sound

            }
        }
    }
    public IEnumerator Setup()
    {
        ghostTimerIndex = 0;
        ghostTimer = 0;
        completedTimer = false;
        runningTimer = true;
        GameOverText.enabled = false;
        //if pacman clears a level a backgroun aill cover the screen and the game will be paused for 0.1s
        if (ClearLevel)
        {
            //active background
            background.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        background.enabled = false;
        palletColectedInLife = 0;
        currentGhostMode = GhostMode.scatter;
        GameRuning = false;

        float waitTimer = 1f;

        if(ClearLevel || newGame)
        {
            waitTimer = 4f;
            //respawn pellet when clear level or start a new game
            for(int i =0; i< nodeControllers.Count; i++)
            {
                nodeControllers[i].RespawnPellet();
            }
        }
        if (newGame)
        {
            //play start odio 

            //reset Score and score text to 0
            scoreCount = 0;
            score.text = scoreCount.ToString();
            lives = 3;
            currentLevel = 1;
            GameOverText.enabled = false;
        }
        pacman.GetComponent<playerController>().Setup();

        blinkyController.Setup();
        pinkyController.Setup();
        inkyController.Setup();
        clydeController.Setup();

        newGame = false;
        ClearLevel = false;
        yield return new WaitForSeconds(waitTimer);
        StartGame();
    }
    void StartGame()
    {
        GameRuning = true;
    }
    void stopGame()
    {
        GameRuning = false;
        //stop music

        //stop pacman animation
        pacman.GetComponent<playerController>().Stopanim();
    }
    public void AddScore(int amount)
    {
        scoreCount += amount;
        score.text = scoreCount.ToString();
    }
    public void GotPallet(NodeController nodCTRL)
    {
        nodeControllers.Add(nodCTRL);
        totalPallets++;
        palletLeft++;
    }
    public IEnumerator CollectedPallets(NodeController nodCTR)
    {
        palletLeft--;
        palletColectedInLife++;
        AddScore(5);

        int inkyRequirePellet = 0;
        int clydeRequirePellet = 0;

        if (hasDeathinThisLevel)
        {
            inkyRequirePellet = 12;
            clydeRequirePellet = 32;
        }
        else
        {
            inkyRequirePellet = 30;
            clydeRequirePellet = 60;
        }
       
        if (palletColectedInLife >= inkyRequirePellet && !inkyController.leftHomBefor)
        {
            inkyController.readyToLeaveHome = true;
        }
        if(palletColectedInLife >= clydeRequirePellet && !clydeController.leftHomBefor)
        {
            clydeController.readyToLeaveHome = true;
        }

        //check if there any pellet left
        if(palletLeft == 0)
        {
            currentLevel++;
            ClearLevel = true;
            stopGame();
            yield return new WaitForSeconds(1);
            palletLeft = totalPallets;
            StartCoroutine(Setup());
        }

        //if this a power pellet
        if (nodCTR.isPowerPellet)
        {
            //stop sound and play the other for frighten

            isPowerPelletRuning = true;
            curentPowerPelletTime = 0;
            

            blinkyController.setFrightened(true);
            pinkyController.setFrightened(true);
            inkyController.setFrightened(true);
            clydeController.setFrightened(true);
        }
    }
    public IEnumerator PauseGame(float timeToPause)
    {
        GameRuning = false;
        yield return new WaitForSeconds(timeToPause);
        GameRuning = true;
    }
    public void GhostEaten()
    {
        //ghosteaten audio
        
        AddScore(400 * powerPelletMultiplyer);
        powerPelletMultiplyer++;
        StartCoroutine(PauseGame(1));
    }
    public IEnumerator PlayerEaten()
    {
        hasDeathinThisLevel = true;
        stopGame();
        yield return new WaitForSeconds(1);

        blinkyController.setVisibel(false);
        pinkyController.setVisibel(false);
        inkyController.setVisibel(false);
        clydeController.setVisibel(false);
        //play death animation
        pacman.GetComponent<playerController>().Death();
        yield return new WaitForSeconds(3); //a pause for the animation to finish

        lives--;
        if(lives <= 0)
        {
            newGame = true;
            //Display game over text
            GameOverText.enabled = true;
            yield return new WaitForSeconds(3); // wait to restart
        }
        StartCoroutine(Setup());
    }
}
