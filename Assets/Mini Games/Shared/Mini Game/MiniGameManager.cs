using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MiniGameManager : AchievementManager
{
    [SerializeField] protected TextMeshProUGUI scoreLabel;

    protected GameState state;
    private bool initGameState;
    public int Score { get => observableInts["score"]; set => observableInts["score"] = value; }

    public override void InitializeObservables()
    {
        base.InitializeObservables();
        Score = 0;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //PlayerPrefs.DeleteAll();

        state = GameState.Start;
        initGameState = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (initGameState)
        {
            InitGameState();
            initGameState = false;
        }
        else scoreLabel.text = $"{Score}";
        DoUpdate();
    }
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
        Profile playerProfile = GameManager.INSTANCE.profile;
        playerProfile.SetHighscore(gameName,
            Mathf.Max(playerProfile.GetHighscore(gameName), Score));
        // add earned coins to profile
        GameManager.INSTANCE.profile.SetCoins((int) GameManager.INSTANCE.profile.GetCoins() + (Score / 20));
        // save progress
        GameManager.INSTANCE.SaveProfile(GameManager.INSTANCE.profile);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }
    /// <summary>
    /// Exits the game and returns to (default screen of) app.
    /// </summary>
    public void ExitGame()
    {
        // add earned coins to profile
        GameManager.INSTANCE.profile.SetCoins((int)GameManager.INSTANCE.profile.GetCoins() + (Score / 20));
        // save progress
        GameManager.INSTANCE.SaveProfile(GameManager.INSTANCE.profile);
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Scenes/DefaultScreen");
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
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b,
                text.color.a - (Time.deltaTime * speed));
            yield return null;
        }
    }
}