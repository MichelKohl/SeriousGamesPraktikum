using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using static Achievement;

public class MiniGameManager : MonoBehaviour
{   // TODO delete all player prefs on app start.
    [SerializeField] protected string gameName;
    [SerializeField] protected TextMeshProUGUI scoreLabel;
    [SerializeField] protected Achievement[] achievements;

    private Dictionary<string, int> observableInts = new Dictionary<string, int>();
    private Dictionary<string, float> observableFloats = new Dictionary<string, float>();
    private Dictionary<string, bool> observableBools = new Dictionary<string, bool>();

    protected GameState state;
    private bool initGameState;
    public int Score { get => observableInts["score"]; set => observableInts["score"] = value; }

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
         Mathf.Max(PlayerPrefs.GetInt($"{gameName} Score", 0), Score));
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
        while (text.color.a > 0.0f){
            text.color = new Color(text.color.r, text.color.g, text.color.b,
                text.color.a - (Time.deltaTime * speed));
            yield return null;
        }
    }

    protected int ObservableInt(string name)
    {
        return observableInts[name];
    }

    protected void ObservableInt(string name, int value)
    {
        observableInts[name] = value;
    }

    protected float ObservableFloat(string name)
    {
        return observableFloats[name];
    }

    protected void ObservableFloat(string name, float value)
    {
        observableFloats[name] = value;
    }

    protected bool ObservableBool(string name)
    {
        return observableBools[name];
    }

    protected void ObservableBool(string name, bool value)
    {
        observableBools[name] = value;
    }

    public List<string> GetAllObservablesNames()
    {
        List<string> list = new List<string>();
        foreach (string key in observableInts.Keys)     list.Add($"{key}|integer");
        foreach (string key in observableFloats.Keys)   list.Add($"{key}|float");
        foreach (string key in observableBools.Keys)    list.Add($"{key}|boolean");
        return list;
    }

    public virtual void InitializeObservables()
    {
        Score = 0;
    }

    private void CheckAchievements()
    {
        foreach (Achievement achievement in achievements)
        {
            bool achieved = true;
            foreach (Condition condition in achievement.conditions)
            {
                switch (condition.varType)
                {
                    case VarType.Integer:
                        if(!observableInts.ContainsKey(condition.variableName))
                            Debug.Log($"{condition.variableName} is not an observable integer variable.");
                        switch (condition.condition)
                        {
                            case Conditional.IsLessThan:
                                achieved = observableInts[condition.variableName] < int.Parse(condition.value);
                                break;
                            case Conditional.IsBiggerThan:
                                achieved = observableInts[condition.variableName] > int.Parse(condition.value);
                                break;
                            case Conditional.IsEqual:
                                achieved = observableInts[condition.variableName] == int.Parse(condition.value);
                                break;
                            default:
                                Debug.Log($"condition ({condition.variableName}) of achievement ({achievement.achievementName}) is inconsistent.");
                                achieved = false;
                                break;
                        }
                        break;
                    case VarType.Float:
                        if (!observableFloats.ContainsKey(condition.variableName))
                            Debug.Log($"{condition.variableName} is not an observable float variable.");
                        switch (condition.condition)
                        {
                            case Conditional.IsLessThan:
                                achieved = observableFloats[condition.variableName] < float.Parse(condition.value);
                                break;
                            case Conditional.IsBiggerThan:
                                achieved = observableFloats[condition.variableName] > float.Parse(condition.value);
                                break;
                            case Conditional.IsEqual:
                                achieved = observableFloats[condition.variableName] == float.Parse(condition.value);
                                break;
                            default:
                                Debug.Log($"condition ({condition.variableName}) of achievement ({achievement.achievementName}) is inconsistent.");
                                achieved = false;
                                break;
                        }
                        break;
                    case VarType.Boolean:
                        if (!observableBools.ContainsKey(condition.variableName))
                            Debug.Log($"{condition.variableName} is not an observable boolean variable.");
                        switch (condition.condition)
                        {
                            case Conditional.Is:
                                achieved = observableBools[condition.variableName] == bool.Parse(condition.value);
                                break;
                            default:
                                Debug.Log($"condition ({condition.variableName}) of achievement ({achievement.achievementName}) is inconsistent.");
                                achieved = false;
                                break;
                        }
                        break;
                }
                if (!achieved) break;
            }
            if (achieved) achievement.Achieved();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.Start;
        initGameState = true;
        InitializeObservables();
    }

    // Update is called once per frame
    void Update()
    {
        if(initGameState) {
            InitGameState();
            initGameState = false;
        } else scoreLabel.text = $"{Score}";
        CheckAchievements();
        DoUpdate();
    }
}