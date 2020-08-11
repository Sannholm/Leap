using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Leap/GameConfig")]
public class GameConfig : ScriptableObject
{
    public SceneReference mainMenuScene;
    public SceneReference gameLevelScene;
    
    public GameObject platformPrefab;
    public LayerMask groundLayerMask;

    public AudioMixer mixer;
}