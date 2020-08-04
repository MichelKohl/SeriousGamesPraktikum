using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceInvaderGameManager : MiniGameManager
{
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI infoLabel;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private TextMeshProUGUI levelLabel;

    private Coroutine blinking;
    private int killCounter = 0;
    private int nrOfEnemies = 0;

    protected override void InitGameState()
    {
      switch(state)
        {
            case GameState.Start:
                blinking = StartCoroutine(FadeBlink(infoLabel));
                tryAgainButton.gameObject.SetActive(false);
                Score += PlayerPrefs.GetInt("Space Invaders Current Score", 0);
                foreach (Transform row in GameObject.Find("Alien Invaders").transform)
                    nrOfEnemies += row.childCount;
                levelLabel.text = $"Level {PlayerPrefs.GetInt("Space Invaders Level", 1)}";
            break;
            case GameState.Gameplay:
                infoLabel.gameObject.SetActive(false);
                startButton.gameObject.SetActive(false);
                levelLabel.gameObject.SetActive(false);
            break;
            case GameState.GameOver:
                PlayerPrefs.SetInt("Space Invaders Level", 1);
                PlayerPrefs.SetInt("Space Invaders Current Score", 0);
                infoLabel.gameObject.SetActive(true);
                StopCoroutine(blinking);
                infoLabel.text = "GAME OVER";
                infoLabel.color = new Color(infoLabel.color.r, infoLabel.color.g, infoLabel.color.b, 1);
                tryAgainButton.gameObject.SetActive(true);
            break;
      }
    }

    protected override void DoUpdate()
    {
        if(killCounter == nrOfEnemies)
        {
            PlayerPrefs.SetInt("Space Invaders Current Score", Score);
            PlayerPrefs.SetInt("Space Invaders Level", PlayerPrefs.GetInt("Space Invaders Level", 1) + 1);
            ResetGame();
        }
    }

    public void IncreaseKillCount()
    {
        killCounter++;
    }
}
