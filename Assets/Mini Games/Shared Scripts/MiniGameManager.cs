using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] protected string gameName;
    [SerializeField] protected TextMeshProUGUI scoreLabel;

    protected GameState state;
    private int score;
    private bool initGameState;

    protected virtual void InitGameState(){}

    protected enum GameState{
      Start,
      Gameplay,
      GameOver
    }

    public void StartMenu()
    {
      state = GameState.Start;
      initGameState = true;
    }

    public void Gameplay()
    {
      state = GameState.Gameplay;
      initGameState = true;
    }

    public void GameOver()
    {
      state = GameState.GameOver;
      initGameState = true;
      // save highscore
      PlayerPrefs.SetInt($"{gameName} Score",
        Mathf.Max(PlayerPrefs.GetInt($"{gameName} Score", 0), score));
    }

    public bool isStartMenu()
    {
      return state == GameState.Start;
    }

    public bool isGameplay()
    {
      return state == GameState.Gameplay;
    }

    public bool isGameOver()
    {
      return state == GameState.GameOver;
    }

    public void ResetGame()
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
      //TODO
    }

    public void IncreaseScoreBy(int points)
    {
      score += points;
    }

    protected IEnumerator FadeBlink(TextMeshProUGUI text, float speed = 1f)
    {
      while(true){
        yield return StartCoroutine(FadeInText(speed, text));
        yield return StartCoroutine(FadeOutText(speed, text));
      }
    }

    private IEnumerator FadeInText(float speed, TextMeshProUGUI text)
    {
      text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
      while (text.color.a < 1.0f){
          text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime * speed));
          yield return null;
      }
    }

    private IEnumerator FadeOutText(float speed, TextMeshProUGUI text)
    {
      text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
      while (text.color.a > 0.0f){
          text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * speed));
          yield return null;
      }
    }

    // Start is called before the first frame update
    void Start()
    {
      score = 0;
      state = GameState.Start;
      initGameState = true;
    }

    // Update is called once per frame
    void Update()
    {
      if(initGameState) {
        InitGameState();
        initGameState = false;
      } else scoreLabel.text = $"{score}";
    }
}
