using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public enum GameState
{
    PLAYING, PAUSED, WON, LOST
}

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameConfig gameConfig;

    [SerializeField]
    private GameObject generatedLevelRoot;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GuideController guideController;

    [SerializeField]
    private bool allPlatformsOn = false;
    [SerializeField]
    private float startPlatformLength = 20;
    [SerializeField]
    private float playerStartDelay = 2;
    [SerializeField]
    private float playerRunSpeed = 7f;
    [SerializeField]
    private float goodJumpThreshold = 0.5f;

    [SerializeField]
    private LevelGeneratorParams levelGenParams = new LevelGeneratorParams
    {
        levelDuration = 60,

        startAvgLength = 7, endAvgLength = 3,
        lengthVariation = 2,
        minSpacing = 5, maxSpacing = 20,
        minOffsetY = -10, maxOffsetY = 10,
        minTiltAngle = 0, maxTiltAngle = 0,

        minEarlyLandMargin = 0f, maxEarlyLandMargin = 0.1f,
        minLateJumpMargin = 0.05f, maxLateJumpMargin = 0.8f,

        jumpHeightRatio = 0.25f,
        jumpTimePerDistance = 0.1f,
    };


    private int level;
    private GameState state = GameState.PLAYING;
    public event Action<GameState> GameStateChanged;
    private IList<Platform> platforms;
    private float accuracy = 0;

    void Start()
    {
        Time.timeScale = 1;

        level = Persistence.LoadScoreboard().completedLevels.Select(l => l.level).DefaultIfEmpty(0).Max() + 1;
        levelGenParams.runSpeed = playerRunSpeed;
        levelGenParams.levelDuration *= level;

        // Setup guide character
        MovementFunc guidePath;
        (platforms, guidePath) = ConstructLevel(levelGenParams);
        guideController.Follow(guidePath, 0);

        // Setup player
        playerController.Jumped.AddListener(OnPlayerJump);
        playerController.Landed.AddListener(OnPlayerLand);
        StartCoroutine(StartPlayerRun(playerStartDelay));
    }

    private (IList<Platform> platforms, MovementFunc guidePath) ConstructLevel(LevelGeneratorParams levelGenParams)
    {
        LevelGenerator levelGen = new LevelGenerator();

        // Generate platforms
        IList<PlatformInfo> platformInfos = levelGen.GeneratePlatforms(new System.Random(), PlatformInfo.FromLength(Vector2.zero, startPlatformLength), levelGenParams);
        IList<Platform> platforms = new List<Platform>(platformInfos.Count);
        PlacePlatforms(platforms, platformInfos);
        
        // Generate guide path
        float jumpDuration = platformInfos.Sum(p => p.GetJumpFunction().duration);
        float runDuration = levelGenParams.levelDuration - jumpDuration;
        float runDistance = platformInfos.Sum(p => Vector3.Distance(p.GetLandPoint(), p.GetJumpPoint()));
        float guideRunSpeed = runDistance / runDuration;
        MovementFunc guidePath = levelGen.GenerateGuidePath(platformInfos, guideRunSpeed);

        return (platforms, guidePath);
    }

    private void PlacePlatforms(IList<Platform> platforms, IList<PlatformInfo> platformInfos)
    {
        for (int i = 0; i < platformInfos.Count; i++)
        {
            Platform platform = Instantiate(gameConfig.platformPrefab, generatedLevelRoot.transform, false).GetComponent<Platform>();
            bool alwaysOn = i == 0 || allPlatformsOn;
            platform.Construct(platformInfos[i], alwaysOn);
            platforms.Add(platform);
        }
    }

    private IEnumerator StartPlayerRun(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Run");
        playerController.SetSpeed(playerRunSpeed);
    }

    void Update()
    {
        if (state == GameState.PLAYING)
        {
            CheckPlayerFell();
        }
    }

    private void CheckPlayerFell()
    {
        float margin = 50;
        float playerX = playerController.transform.position.x;
        float nearbyPlatformsMinY = platforms.Where(p => playerX - margin < p.transform.position.x && p.transform.position.x < playerX + margin)
                                            .Select(p => p.transform.position.y)
                                            .Min();
        
        if (playerController.transform.position.y < nearbyPlatformsMinY - 10)
        {
            EndGame(success: false);
        }
    }

    private void OnPlayerJump(Platform platform)
    {
        Assert.IsTrue(platforms.Count > 1);

        float dist = Mathf.Abs(playerController.gameObject.transform.position.x - platform.transform.parent.TransformPoint(platform.GetJumpPoint()).x);
        if (dist <= goodJumpThreshold)
        {
            accuracy += 1f / (platforms.Count - 1);
            Debug.Log("Good jump!");
            Debug.Log("Total jump accuracy: " + accuracy*100 + " %");
        }
    }

    private void OnPlayerLand(Platform platform)
    {
        bool isLastPlatform = platforms[platforms.Count - 1] == platform;
        if (isLastPlatform)
        {
            EndGame(success: true);
        }
    }

    /// Preconditon: Game is running.
    public void PauseGame()
    {
        Assert.AreEqual(state, GameState.PLAYING);

        playerController.GetComponent<PlayerInput>().DeactivateInput();
        Time.timeScale = 0;

        SetGameState(GameState.PAUSED);
    }

    /// Preconditon: Game is paused.
    public void UnPauseGame()
    {
        Assert.AreEqual(state, GameState.PAUSED);

        playerController.GetComponent<PlayerInput>().ActivateInput();
        Time.timeScale = 1;

        SetGameState(GameState.PLAYING);
    }

    // Precondition: Game is running.
    private void EndGame(bool success)
    {
        Assert.AreEqual(state, GameState.PLAYING);

        playerController.GetComponent<PlayerInput>().DeactivateInput();

        if (success)
        {
            Debug.Log("Win! " + (Time.timeSinceLevelLoad - playerStartDelay));

            playerController.SetSpeed(0);

            var scoreboard = Persistence.LoadScoreboard();
            scoreboard.completedLevels.Add(new CompletedLevel
            {
                level = level,
                accuracy = accuracy,
                date = DateTime.UtcNow
            });
            Persistence.SaveScoreboard(scoreboard);

            SetGameState(GameState.WON);
        }
        else
        {
            Debug.Log("Game over " + (Time.timeSinceLevelLoad - playerStartDelay));

            playerController.Fall();
            
            SetGameState(GameState.LOST);
        }
    }

    private void SetGameState(GameState state)
    {
        Debug.Log(String.Format("Game state changed from {0} to {1}", this.state, state));

        bool stateChanged = this.state != state;
        this.state = state;

        if (stateChanged && GameStateChanged != null)
        {
            GameStateChanged.Invoke(state);
        }
    }

    public GameState GetGameState()
    {
        return state;
    }

    public void OpenStartMenu()
    {
        SceneManager.LoadScene(gameConfig.mainMenuScene);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(gameConfig.gameLevelScene);
    }
}
