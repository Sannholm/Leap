using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject generatedLevelRoot;
    public GameObject platformPrefab;
    public GameObject player;
    public GameObject guideCharacter;

    public bool allPlatformsOn = false;
    public float startPlatformLength = 20;
    public float playerStartDelay = 2;
    public float guideRunSpeed = 6.9f;

    private IList<PlatformInfo> platforms;
    private MovementFunc guidePath;

    void Start()
    {
        //Time.timeScale = 0.1f;

        ConstructLevel();
        guideCharacter.GetComponent<GuideController>().Follow(guidePath, 0);

        player.SetActive(false);
        StartCoroutine(StartPlayerRun(playerStartDelay));
    }

    private IEnumerator StartPlayerRun(float delay) {
        yield return new WaitForSeconds(delay);
        Debug.Log("Run");
        player.SetActive(true);
    }

    private void ConstructLevel()
    {
        LevelGenerator levelGen = new LevelGenerator();
        platforms = levelGen.GeneratePlatforms(PlatformInfo.FromLength(Vector2.zero, startPlatformLength), new System.Random());
        guidePath = levelGen.GenerateGuidePath(platforms, guideRunSpeed);
        PlacePlatforms(platforms);
    }

    private void PlacePlatforms(IList<PlatformInfo> platforms)
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            Platform platform = Instantiate(platformPrefab, generatedLevelRoot.transform, false).GetComponent<Platform>();
            bool alwaysOn = i == 0 || allPlatformsOn;
            platform.Construct(platforms[i], alwaysOn);
        }
    }

    void Update()
    {
    }
}
