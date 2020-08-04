using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighscoreField : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI score;

    public HighscoreField Init(string gameTitle, int score, Sprite gameIconSprite = null)
    {
        if(gameIconSprite != null) icon.sprite = gameIconSprite;
        title.text = gameTitle;
        this.score.text = score.ToString();
        return this;
    }
}
