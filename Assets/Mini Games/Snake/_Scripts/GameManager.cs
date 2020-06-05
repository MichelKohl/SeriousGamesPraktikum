using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private TextMeshProUGUI pointsLabel;
    [SerializeField] private TextMeshProUGUI startLabel;
    [SerializeField] private TextMeshProUGUI gameOverLabel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Joystick joystick;

    private string gameName = "Snake";
    private int score;
    private GameState state;
    private bool initGameState;

    enum GameState
    {
        Start,
        Gameplay,
        GameOver
    }

    // Start is called before the first frame update
    void Start()
    {
      score = 0;
      state = GameState.Start;
      initGameState = true;
      scoreLabel.gameObject.SetActive(false);
      pointsLabel.gameObject.SetActive(false);
      gameOverLabel.gameObject.SetActive(false);
      joystick.gameObject.SetActive(false);
      playButton.gameObject.SetActive(false);
      StartCoroutine(FadeBlink(3f, startLabel));
    }

    void Update()
    {
      if(initGameState){
        switch(state){
          case GameState.Start: break;
          case GameState.Gameplay:
            startLabel.gameObject.SetActive(false);
            scoreLabel.gameObject.SetActive(true);
            pointsLabel.gameObject.SetActive(true);
            joystick.gameObject.SetActive(true);
          break;
          case GameState.GameOver:
            joystick.gameObject.SetActive(false);
            gameOverLabel.gameObject.SetActive(true);
            playButton.gameObject.SetActive(true);
          break;
        }
        initGameState = false;
      } else {
        if(state == GameState.Gameplay) pointsLabel.text = $"{score}";
        if(state == GameState.Start && (Input.touchCount > 0 || (Application.isEditor && Input.GetAxis("Fire1") > 0)))
            StartGamePlay();
      }
    }

    public void StartGamePlay()
    {
      state = GameState.Gameplay;
      initGameState = true;
    }

    public void ResetGame()
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
      //TODO
    }

    public void GameOver()
    {
      PlayerPrefs.SetInt(gameName, Mathf.Max(PlayerPrefs.GetInt(gameName, 0), score));
      state = GameState.GameOver;
      initGameState = true;
    }

    public bool isGameplay()
    {
      return state == GameState.Gameplay;
    }

    public bool isGameOver()
    {
      return state == GameState.GameOver;
    }

    public void IncreaseScoreBy(int points)
    {
      score += points;
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

    private IEnumerator FadeBlink(float speed, TextMeshProUGUI text)
    {
      while(true){
        yield return StartCoroutine(FadeInText(speed, text));
        yield return StartCoroutine(FadeOutText(speed, text));
      }
    }
}
