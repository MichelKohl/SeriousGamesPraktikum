using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SnakeGameManager : MiniGameManager
{
    [SerializeField] private TextMeshProUGUI gameOverLabel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Joystick joystick;

    protected override void InitGameState()
    {
      switch (state)
      {
            case GameState.Start:
                    joystick.gameObject.SetActive(false);
                    gameOverLabel.gameObject.SetActive(false);
                    playAgainButton.gameObject.SetActive(false);
                    StartCoroutine(FadeBlink(startButton.transform.GetChild(0).
                            GetComponent<TextMeshProUGUI>()));
            break;
            case GameState.Gameplay:
                    joystick.gameObject.SetActive(true);
                    startButton.gameObject.SetActive(false);
            break;
            case GameState.GameOver:
                    playAgainButton.gameObject.SetActive(true);
                    joystick.gameObject.SetActive(false);
            break;
      }
    }
}
