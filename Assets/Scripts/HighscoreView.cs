using System.Collections.Generic;
using UnityEngine;

public class HighscoreView : MonoBehaviour
{
    [SerializeField] private HighscoreField prefab;

    private bool changedToView = false;
    // Update is called once per frame
    void Update()
    {
        if (changedToView)
        {
            changedToView = false;

            GameManager gameManager = GameManager.INSTANCE;
            Dictionary<string, int> highscores = gameManager.profile.GetHighscores();

            // TODO get game icons for highscore view
            foreach (string gameTitle in highscores.Keys)
                _ = Instantiate(prefab, transform).Init(gameTitle, highscores[gameTitle]);
        }
    }

    public void ChangeToHighscoreView()
    {
        changedToView = true;
    }
}
