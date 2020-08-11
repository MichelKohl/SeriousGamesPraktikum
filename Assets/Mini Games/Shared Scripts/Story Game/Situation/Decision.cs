using System;
using UnityEngine;
using TMPro;

public class Decision : MonoBehaviour
{
    [SerializeField] private StoryManager storyManager;
    [SerializeField] private TextMeshProUGUI description;

    public string Description { get; private set; }
    public int NextSituationID { get; private set; }
    public double Condition { get; private set; }
    public bool StartBattle { get; private set; }


    public void Init(string description, int nextSituationID, double condition)
    {
        Description = description;
        NextSituationID = nextSituationID;
        Condition = condition;
        StartBattle = false;

        this.description.text = description + (condition > 0 ? $" [{condition} km]" : "");

        gameObject.SetActive(true);
    }

    public void Init(string description, int nextSituationID, bool startBattle)
    {
        Description = description;
        NextSituationID = nextSituationID;
        Condition = 0;
        StartBattle = startBattle;

        this.description.text = description;

        gameObject.SetActive(true);
    }

    public void OnClick()
    {
        storyManager.ChangeSituation(toID: NextSituationID, Condition, StartBattle);
    }
}