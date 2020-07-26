using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject generatedLevelRoot;
    public GameObject platformPrefab;
    public PlayerController playerController;
    public GuideController guideController;

    public bool allPlatformsOn = false;
    public float levelDuration = 60;
    public float startPlatformLength = 20;
    public float playerStartDelay = 2;
    public float guideRunSpeed = 6.9f;
    public float playerRunSpeed = 7f;

    private IList<Platform> platforms;
    private MovementFunc guidePath;

    private bool gameOver = false;

    void Start()
    {
        //Time.timeScale = 0.1f;

        ConstructLevel();
        guideController.Follow(guidePath, 0);

        playerController.Landed += OnPlayerLand;
        StartCoroutine(StartPlayerRun(playerStartDelay));
    }

    private void ConstructLevel()
    {
        LevelGeneratorParams p = new LevelGeneratorParams
        {
            levelDuration = levelDuration,
            runSpeed = playerRunSpeed,

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

        LevelGenerator levelGen = new LevelGenerator();
        IList<PlatformInfo> platformInfos = levelGen.GeneratePlatforms(new System.Random(), PlatformInfo.FromLength(Vector2.zero, startPlatformLength), p);
        guidePath = levelGen.GenerateGuidePath(platformInfos, guideRunSpeed);
        PlacePlatforms(platformInfos);
    }

    private void PlacePlatforms(IList<PlatformInfo> platformInfos)
    {
        platforms = new List<Platform>(platformInfos.Count);
        for (int i = 0; i < platformInfos.Count; i++)
        {
            Platform platform = Instantiate(platformPrefab, generatedLevelRoot.transform, false).GetComponent<Platform>();
            bool alwaysOn = i == 0 || allPlatformsOn;
            platform.Construct(platformInfos[i], alwaysOn);
            platforms.Add(platform);
        }
    }

    private IEnumerator StartPlayerRun(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Run");
        playerController.Run(playerRunSpeed);
    }

    void Update()
    {
        if (!gameOver)
        {
            CheckGameOver();
        }
    }

    private void CheckGameOver()
    {
        float margin = 50;
        float playerX = playerController.transform.position.x;
        float nearbyPlatformsMinY = platforms.Where(p => playerX - margin < p.transform.position.x && p.transform.position.x < playerX + margin)
                                            .Select(p => p.transform.position.y)
                                            .Min();
        
        if (playerController.transform.position.y < nearbyPlatformsMinY - 10)
        {
            gameOver = true;
            playerController.Fall();
            
            Debug.Log("Game over " + (Time.timeSinceLevelLoad - playerStartDelay));

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnPlayerLand(Platform platform)
    {
        bool isLastPlatform = platforms[platforms.Count - 1] == platform;
        if (isLastPlatform)
        {
            Debug.Log("Win! " + (Time.timeSinceLevelLoad - playerStartDelay));
            playerController.Run(0);
        }
    }
}
