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

    private int chapter;
    private int startSituation;


    public void Init(string description, int nextSituationID, double condition, int chapter = -1, int startSituation = 0)
    {
        Description = description;
        NextSituationID = nextSituationID;
        Condition = condition;
        StartBattle = false;
        this.chapter = chapter;
        this.startSituation = startSituation;

        this.description.text = description + (condition > 0 ? $" [{condition} km]" : "");

        gameObject.SetActive(true);
    }

    public void Init(string description, int nextSituationID, bool startBattle, int chapter = -1, int startSituation = 0)
    {
        Description = description;
        NextSituationID = nextSituationID;
        Condition = 0;
        StartBattle = startBattle;
        this.chapter = chapter;
        this.startSituation = startSituation;

        this.description.text = description;

        gameObject.SetActive(true);
    }

    public void OnClick()
    {
        if (chapter == -1)
            storyManager.ChangeSituation(toID: NextSituationID, Condition, StartBattle);
        else
            storyManager.ChangeChapter(chapter, startSituation);
    }
}