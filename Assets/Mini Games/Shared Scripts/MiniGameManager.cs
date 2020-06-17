using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MiniGameManager : MonoBehaviour
{   // TODO delete all player prefs on app start.
    [SerializeField] protected string gameName;
    [SerializeField] protected TextMeshProUGUI scoreLabel;

    protected GameState state;
    private int score;
    private bool initGameState;
    /// <summary>
    /// Initializes a gamestate. Should be implemented by inheriting class.
    /// Is called after a change of game state (used to toggle UI etc.).
    /// </summary>
    protected virtual void InitGameState(){}
    protected virtual void DoUpdate(){}
    
    protected enum GameState{
      Start,
      Gameplay,
      GameOver
    }
    /// <summary>
    /// Function to be used by a UI element. Changes game state to "Start Menu". 
    /// </summary>
    public void StartMenu()
    {
        state = GameState.Start;
        initGameState = true;
    }
    /// <summary>
    /// Function to be used by a UI element. Changes game state to "Gameplay". 
    /// </summary>
    public void Gameplay()
    {
        state = GameState.Gameplay;
        initGameState = true;
    }
    /// <summary>
    /// Function to be used by a UI element. Changes game state to "Game Over". 
    /// </summary>
    public void GameOver()
    {
        state = GameState.GameOver;
        initGameState = true;
        // save highscore
        PlayerPrefs.SetInt($"{gameName} Score",
         Mathf.Max(PlayerPrefs.GetInt($"{gameName} Score", 0), score));
    }
   /// <summary>
   /// Returns whether current state is "Start Menu".
   /// </summary>
   /// <returns>true if current state is "Start Menu"</returns>
    public bool IsStartMenu()
    {
        return state == GameState.Start;
    }
    /// <summary>
    /// Returns whether current state is "Gameplay".
    /// </summary>
    /// <returns>true if current state is "Gameplay"</returns>
    public bool IsGameplay()
    {
        return state == GameState.Gameplay;
    }
    /// <summary>
    /// Returns whether current state is "Game Over".
    /// </summary>
    /// <returns>true if current state is "Game Over"</returns>
    public bool IsGameOver()
    {
        return state == GameState.GameOver;
    }
    /// <summary>
    /// Resets game by reloading the scene.
    /// </summary>
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// Exits the game and returns to (default screen of) app.
    /// </summary>
    public void ExitGame()
    {
        SceneManager.LoadScene("Scenes/DefaultScreen");
    }
    /// <summary>
    /// Increases current score.
    /// </summary>
    /// <param name="points">Points by which the score is increased.</param>
    public void IncreaseScoreBy(int points)
    {
        score += points;
    }

    public int GetScore()
    {
        return score;
    }
    /// <summary>
    /// Coroutine used to make a text blink (by fading in and out).
    /// </summary>
    /// <param name="text">UI text element to make blink</param>
    /// <param name="speed">Blink speed</param>
    /// <returns></returns>
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
            text.color = new Color(text.color.r, text.color.g, text.color.b,
                text.color.a + (Time.deltaTime * speed));
            yield return null;
        }
    }

    private IEnumerator FadeOutText(float speed, TextMeshProUGUI text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f){
            text.color = new Color(text.color.r, text.color.g, text.color.b,
                text.color.a - (Time.deltaTime * speed));
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
        DoUpdate();
    }
}
