using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public GameObject scoreboardBtn;

    private State state;

    void Start()
    {
        int nextLevel = Persistence.LoadScoreboard().completedLevels.Select(l => l.level).DefaultIfEmpty(0).Max() + 1;
        play.GetComponent<TMP_Text>().SetText("Press SPACE to play level {0}", nextLevel);
        scoreboardBtn.SetActive(nextLevel > 1);

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
