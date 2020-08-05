using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

enum State
{
    PLAY, SETTINGS, SCOREBOARD
}

public class MainMenu : MonoBehaviour
{
    public GameConfig gameConfig;

    public GameObject play;
    public GameObject settings;
    public GameObject scoreboard;

    private State state;

    void Start()
    {
        SetState(State.PLAY);
    }

    void Update()
    {
        if (state == State.PLAY && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(gameConfig.gameLevelScene);
        }
    }

    private void SetState(State state)
    {
        this.state = state;

        play.SetActive(false);
        settings.SetActive(false);
        scoreboard.SetActive(false);

        switch (state)
        {
            case State.PLAY:
                play.SetActive(true);
                break;
            case State.SETTINGS:
                settings.SetActive(true);
                break;
            case State.SCOREBOARD:
                scoreboard.SetActive(true);
                break;
        }
    }

    public void OnClickSettingsBtn()
    {
        if (state != State.SETTINGS)
        {
            SetState(State.SETTINGS);
        }
        else
        {
            SetState(State.PLAY);
        }
    }

    public void OnClickScoreboardBtn()
    {
        if (state != State.SCOREBOARD)
        {
            SetState(State.SCOREBOARD);
        }
        else
        {
            SetState(State.PLAY);
        }
    }
}
