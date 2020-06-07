﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceInvaderGameManager : MiniGameManager
{
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI infoLabel;
    [SerializeField] private Button tryAgainButton;

    private Coroutine blinking;
    private float lastTimeShot;

    protected override void InitGameState()
    {
      switch(state)
      {
        case GameState.Start:
          blinking = StartCoroutine(FadeBlink(infoLabel));
          tryAgainButton.gameObject.SetActive(false);
        break;
        case GameState.Gameplay:
          infoLabel.gameObject.SetActive(false);
          startButton.gameObject.SetActive(false);
        break;
        case GameState.GameOver:
          infoLabel.gameObject.SetActive(true);
          StopCoroutine(blinking);
          infoLabel.text = "GAME OVER";
          tryAgainButton.gameObject.SetActive(true);
        break;
      }
    }
}
