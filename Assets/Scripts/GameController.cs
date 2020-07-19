using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject generatedLevelRoot;
    public GameObject platformPrefab;
    public GameObject guideCharacter;

    private IList<PlatformInfo> platforms;
    private MovementFunc guidePath;

    void Start()
    {
        //Time.timeScale = 0.1f;

        ConstructLevel();
        guideCharacter.GetComponent<GuideController>().Follow(guidePath, 3);
    }

    private void ConstructLevel()
    {
        LevelGenerator levelGen = new LevelGenerator();
        platforms = levelGen.GeneratePlatforms(PlatformInfo.FromLength(Vector2.zero, 20), new System.Random());
        guidePath = levelGen.GenerateGuidePath(platforms);
        PlacePlatforms(platforms);
    }

    private void PlacePlatforms(IList<PlatformInfo> platforms)
    {
        foreach (var platform in platforms)
        {
            GameObject platformObj = Instantiate(platformPrefab, generatedLevelRoot.transform, false);
            platformObj.GetComponent<Platform>().Construct(platform);
        }
    }

    void Update()
    {
        
    }
}
