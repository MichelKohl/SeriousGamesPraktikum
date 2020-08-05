using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentSituation;
    [SerializeField] private DecisionsPanel decisionsPanel;
    [SerializeField] private Situation[] situations;
    [SerializeField] private Decision decisionPrefab;

    private int currentSituationID = 0;

    public void ChangeSituation(int toID = 0)
    {
        Debug.Log($"Change to {toID}");
        currentSituationID = toID;
        Situation current = situations[currentSituationID];
        currentSituation.text = current.description;

        foreach (Transform child in decisionsPanel.transform)
            GameObject.Destroy(child.gameObject);
        foreach (DecisionInfo info in current.decisions)
            Instantiate(decisionPrefab, decisionsPanel.transform).
                Init(info.description, info.nextSituationID, info.condition);

    }

    void Start()
    {
        ChangeSituation();
    }
}
