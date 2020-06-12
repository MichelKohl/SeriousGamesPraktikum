using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceInvaderGameManager : MiniGameManager
{
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI infoLabel;
    [SerializeField] private Button tryAgainButton;

    private Coroutine blinking;

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
            infoLabel.color = new Color(infoLabel.color.r, infoLabel.color.g, infoLabel.color.b, 1);
            tryAgainButton.gameObject.SetActive(true);
        break;
      }
    }
}
