using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject generatedLevelRoot;
    public GameObject platformPrefab;

    private IList<PlatformInfo> platforms;

    void Start()
    {
        ConstructLevel();
    }

    private void ConstructLevel()
    {
        LevelGenerator levelGen = new LevelGenerator();
        platforms = levelGen.GeneratePlatforms(PlatformInfo.FromLength(Vector2.zero, 10), new System.Random());
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
