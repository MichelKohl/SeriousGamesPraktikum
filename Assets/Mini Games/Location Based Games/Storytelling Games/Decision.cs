﻿using System;
using UnityEngine;
using TMPro;

public class Decision : MonoBehaviour
{
    [SerializeField] private StoryManager storyManager;
    [SerializeField] private TextMeshProUGUI description;

    public string Description { get; private set; }
    public int NextSituationID { get; private set; }
    public double Condition { get; private set; }

    public Decision Init(string description, int nextSituationID, double condition)
    {
        Description = description;
        NextSituationID = nextSituationID;
        Condition = condition;

        this.description.text = description + $" [{condition} km]";

        gameObject.SetActive(true);
        return this;
    }

    public void OnClick()
    {
        storyManager.ChangeSituation(toID: NextSituationID, Condition);
    }
}