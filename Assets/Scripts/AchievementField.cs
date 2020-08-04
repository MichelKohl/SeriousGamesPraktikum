using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementField : MonoBehaviour
{
    [SerializeField] private Image trophy;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI gameName;

    public AchievementField Init(bool wasAchieved, string title, string description, string gameName, Sprite trophy)
    {
        this.trophy.sprite = trophy;
        this.trophy.color = new Color(255, 255, 255, wasAchieved ? 1 : 0.5f);
        this.title.text = title;
        this.description.text = description;
        this.gameName.text = gameName;
        return this;
    }
}
