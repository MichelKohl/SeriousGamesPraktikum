using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class AchievementPopUp : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI reward;
    [SerializeField] private Image trophy;
    [SerializeField] private Sprite bronzeTrophy;
    [SerializeField] private Sprite silverTrophy;
    [SerializeField] private Sprite goldTrophy;
    [SerializeField] private Sprite platinumTrophy;

    public void SetupPopUp(Achievement achievement)
    {
        switch (achievement.trophyType)
        {
            case TrophyType.Bronze:
                trophy.sprite = bronzeTrophy;
                break;
            case TrophyType.Silver:
                trophy.sprite = silverTrophy;
                break;
            case TrophyType.Gold:
                trophy.sprite = goldTrophy;
                break;
            case TrophyType.Platinum:
                trophy.sprite = platinumTrophy;
                break;
        }
        title.text = achievement.achievementName;
        reward.text = achievement.reward.ToString();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut(float speed = 0.5f, float waitTime = 2f)
    {
        yield return new WaitForSeconds(waitTime);
        while (title.color.a > 0.0f)
        {
            Color color = new Color(title.color.r, title.color.g, title.color.b,
                title.color.a - (Time.deltaTime * speed));
            title.color = color;
            reward.color = color;
            trophy.color = color;
            Image panel = GetComponent<Image>();
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b,
               Mathf.Max(0, panel.color.a - (Time.deltaTime * speed)));
        }
        gameObject.SetActive(false);
        yield return null;
    }
}
