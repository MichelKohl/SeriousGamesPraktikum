using System.Collections;
using UnityEngine;
using TMPro;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentSituation;
    [SerializeField] private DecisionsPanel decisionsPanel;
    [SerializeField] private Situation[] situations;
    [SerializeField] private Decision decisionPrefab;
    [SerializeField] private bool turnOffWalkingRequirement = false;
    [SerializeField] private WalkingText textWhenWalking;
    [SerializeField] private float skyboxRotationSpeed = 0.2f;

    private int currentSituationID = 0;
    private GameManager manager;
    private double distanceToWalk;
    private Transform camTransform;

    public void ChangeSituation(int toID = 0, double distanceToWalk = 0)
    {
        Debug.Log($"Change to {toID}");
        StartCoroutine(WaitTillDistanceWalked(toID, distanceToWalk));
    }

    protected virtual void Start()
    {
        manager = GameManager.INSTANCE;
        camTransform = Camera.main.transform;
        distanceToWalk = 0;
        ChangeSituation();
    }

    protected virtual void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyboxRotationSpeed);

        if (manager == null) manager = GameManager.INSTANCE;
    }

    private IEnumerator WaitTillDistanceWalked(int id, double distanceToWalk)
    {
        if (!turnOffWalkingRequirement)
        {
            textWhenWalking.gameObject.SetActive(true);
            decisionsPanel.gameObject.SetActive(false);
            currentSituation.gameObject.SetActive(false);

            textWhenWalking.SetStart(manager.profile.getDistanceTraveled(), distanceToWalk);

            while (distanceToWalk - textWhenWalking.Distance <= 0 && !turnOffWalkingRequirement)
                yield return null;

            textWhenWalking.gameObject.SetActive(false);
            decisionsPanel.gameObject.SetActive(true);
            currentSituation.gameObject.SetActive(true);
        }
        currentSituationID = id;
        Situation current = situations[currentSituationID];
        currentSituation.text = current.description;

        foreach (Transform child in decisionsPanel.transform)
            GameObject.Destroy(child.gameObject);
        foreach (DecisionInfo info in current.decisions)
            Instantiate(decisionPrefab, decisionsPanel.transform).
                Init(info.description, info.nextSituationID, info.conditionDistance);

        camTransform.position = current.cameraPosition;
        camTransform.rotation = Quaternion.Euler(current.cameraRotation);
    }
}
