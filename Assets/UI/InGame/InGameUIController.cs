using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIController : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject loseMenu;
    [SerializeField]
    private GameObject winMenu;

    void Start()
    {
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);

        gameController.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);

        switch (state)
        {
            case GameState.PAUSED:
                pauseMenu.SetActive(true);
                break;
            case GameState.LOST:
                loseMenu.SetActive(true);
                break;
            case GameState.WON:
                winMenu.SetActive(true);
                break;
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            switch (gameController.GetGameState())
            {
                case GameState.PLAYING:
                    gameController.PauseGame();
                    break;
                case GameState.PAUSED:
                    gameController.UnPauseGame();
                    break;
            }
        }
    }
}
