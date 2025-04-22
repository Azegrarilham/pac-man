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
        InitializeGame();
    }

    private void InitializeGame()
    {
        background.enabled = false;
        GameOverText.enabled = false;
        newGame = true;
        ClearLevel = false;

        InitializeGhostControllers();

        lives = 3;
        GhostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
        pacman = GameObject.Find("PacMan");
        StartCoroutine(Setup());
    }

    private void InitializeGhostControllers()
    {
        blinkyController = blinky.GetComponent<enemyController>();
        pinkyController = pinky.GetComponent<enemyController>();
        inkyController = inky.GetComponent<enemyController>();
        clydeController = clyde.GetComponent<enemyController>();
    }

    private void Update()
    {
        if (!GameRuning) return;

        UpdateGhostMode();
        UpdatePowerPellet();
    }

    private void UpdatePowerPellet()
    {
        if (!isPowerPelletRuning) return;

        curentPowerPelletTime += Time.deltaTime;
        if (curentPowerPelletTime >= powerPelletTimer)
        {
            isPowerPelletRuning = false;
            curentPowerPelletTime = 0;
            powerPelletMultiplyer = 1;

            // Reset ghost frightened states
            blinkyController.setFrightened(false);
            pinkyController.setFrightened(false);
            inkyController.setFrightened(false);
            clydeController.setFrightened(false);
        }
    }

    private void UpdateGhostMode()
    {
        if (completedTimer || !runningTimer) return;

        ghostTimer += Time.deltaTime;
        if (ghostTimer >= ghostModeTimer[ghostTimerIndex])
        {
            ghostTimer = 0;
            ghostTimerIndex++;
            currentGhostMode = currentGhostMode == GhostMode.chase ? GhostMode.scatter : GhostMode.chase;

            if (ghostTimerIndex == ghostModeTimer.Length)
            {
                completedTimer = true;
                runningTimer = false;
                currentGhostMode = GhostMode.chase;
            }
        }
    }

    public IEnumerator Setup()
    {
        ResetGameState();

        if (ClearLevel)
        {
            background.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        background.enabled = false;

        float waitTimer = ClearLevel || newGame ? 4f : 1f;

        if (ClearLevel || newGame)
        {
            RespawnAllPellets();
        }

        if (newGame)
        {
            ResetGameForNewGame();
        }

        SetupAllCharacters();

        newGame = false;
        ClearLevel = false;
        yield return new WaitForSeconds(waitTimer);
        StartGame();
    }

    private void ResetGameState()
    {
        ghostTimerIndex = 0;
        ghostTimer = 0;
        completedTimer = false;
        runningTimer = true;
        GameOverText.enabled = false;
        palletColectedInLife = 0;
        currentGhostMode = GhostMode.scatter;
        GameRuning = false;
    }

    private void RespawnAllPellets()
    {
        foreach (var nodeController in nodeControllers)
        {
            nodeController.RespawnPellet();
        }
    }

    private void ResetGameForNewGame()
    {
        scoreCount = 0;
        score.text = scoreCount.ToString();
        lives = 3;
        currentLevel = 1;
        GameOverText.enabled = false;
    }

    private void SetupAllCharacters()
    {
        pacman.GetComponent<PlayerController>().Setup();
        blinkyController.Setup();
        pinkyController.Setup();
        inkyController.Setup();
        clydeController.Setup();
    }

    void StartGame()
    {
        GameRuning = true;
    }

    void stopGame()
    {
        GameRuning = false;
        pacman.GetComponent<PlayerController>().StopAnimation();
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

        UpdateGhostRelease();

        if (palletLeft == 0)
        {
            yield return HandleLevelComplete();
        }

        if (nodCTR.isPowerPellet)
        {
            ActivatePowerPellet();
        }
    }

    private void UpdateGhostRelease()
    {
        int inkyRequirePellet = hasDeathinThisLevel ? 12 : 30;
        int clydeRequirePellet = hasDeathinThisLevel ? 32 : 60;

        if (palletColectedInLife >= inkyRequirePellet && !inkyController.leftHomBefor)
        {
            inkyController.readyToLeaveHome = true;
           
        }
        if (palletColectedInLife >= clydeRequirePellet && !clydeController.leftHomBefor)
        {
            clydeController.readyToLeaveHome = true;
            
        }
    }


    private IEnumerator HandleLevelComplete()
    {
        currentLevel++;
        ClearLevel = true;
        stopGame();
        yield return new WaitForSeconds(1);
        palletLeft = totalPallets;
        StartCoroutine(Setup());
    }

    private void ActivatePowerPellet()
    {
        isPowerPelletRuning = true;
        curentPowerPelletTime = 0;

        blinkyController.setFrightened(true);
        pinkyController.setFrightened(true);
        inkyController.setFrightened(true);
        clydeController.setFrightened(true);
    }

    public IEnumerator PauseGame(float timeToPause)
    {
        GameRuning = false;
        yield return new WaitForSeconds(timeToPause);
        GameRuning = true;
    }

    public void GhostEaten()
    {
        AddScore(400 * powerPelletMultiplyer);
        powerPelletMultiplyer++;
        StartCoroutine(PauseGame(1));
    }

    public IEnumerator PlayerEaten()
    {
        hasDeathinThisLevel = true;
        stopGame();
        yield return new WaitForSeconds(1);

        SetGhostsVisibility(false);
        pacman.GetComponent<PlayerController>().Death();
        yield return new WaitForSeconds(3);

        lives--;
        if (lives <= 0)
        {
            yield return HandleGameOver();
        }
        StartCoroutine(Setup());
    }

    private void SetGhostsVisibility(bool visible)
    {
        blinkyController.setVisibel(visible);
        pinkyController.setVisibel(visible);
        inkyController.setVisibel(visible);
        clydeController.setVisibel(visible);
    }

    private IEnumerator HandleGameOver()
    {
        newGame = true;
        GameOverText.enabled = true;
        yield return new WaitForSeconds(3);
    }
}
