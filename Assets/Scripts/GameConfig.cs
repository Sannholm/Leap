using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Leap/GameConfig")]
public class GameConfig : ScriptableObject
{
    public SceneReference mainMenuScene;
    public SceneReference gameLevelScene;
    
    public GameObject platformPrefab;
}