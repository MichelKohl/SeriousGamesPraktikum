using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private Cam cam;
    [SerializeField] private StarterClass[] starterClasses;
    [SerializeField] private DCPlayer[] starterModels;
    [SerializeField] private Lifebar playerLifebar;
    [SerializeField] private int startChapter = 0;
    [SerializeField] private int startSituationID = 0;
    [SerializeField] private TextMeshProUGUI currentSituation;
    [SerializeField] private ScrollPanel decisionsPanel;

    [SerializeField] private Chapter[] chapters;
    //[SerializeField] private Situation[] situations;

    [SerializeField] private Decision decisionPrefab;
    [SerializeField] private bool turnOffWalkingRequirement = false;
    [SerializeField] private WalkingText textWhenWalking;
    [SerializeField] private float skyboxRotationSpeed = 0.2f;

    private DCPlayer player;
    private int currentSituationID = 0;
    private Chapter currentChapter;
    private GameManager manager;
    private BattleManager battleManager;
    private bool characterChosen = false;// must be set to true when loading a saved game state.
    private List<GameObject> destroyOnSituationChange = new List<GameObject>();

    public void ChangeSituation(int toID = 0, double distanceToWalk = 0, bool startBattle = false)
    {
        if (startBattle) battleManager.StartBattle();
        // TODO save coroutine in profile

        StartCoroutine(WaitTillDistanceWalked(toID, distanceToWalk, startBattle));
    }

    public void ChangeChapter(int toChapter, int startSituation = 0)
    {
        currentChapter = chapters[toChapter];
        ChangeSituation(startSituation);
    }

    protected virtual void Start()
    {
        manager = GameManager.INSTANCE;
        battleManager = GetComponent<BattleManager>();
        ChangeChapter(startChapter);
    }

    protected virtual void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyboxRotationSpeed);

        if (manager == null) manager = GameManager.INSTANCE;
    }

    private IEnumerator WaitTillDistanceWalked(int id, double distanceToWalk,
        bool startBattle, bool startSet = false)
    {
        if (startBattle)
            yield return new WaitUntil(() => battleManager.BattleOver);
        // do some "walking"

        if (!turnOffWalkingRequirement)
        {
            textWhenWalking.gameObject.SetActive(true);
            decisionsPanel.gameObject.SetActive(false);
            currentSituation.gameObject.SetActive(false);

            if(!startSet && manager != null)
                textWhenWalking.SetStart(manager.profile.getDistanceTraveled(), distanceToWalk);

            while (distanceToWalk - textWhenWalking.Distance <= 0 && !turnOffWalkingRequirement)
                yield return null;

            textWhenWalking.gameObject.SetActive(false);
            decisionsPanel.gameObject.SetActive(true);
            currentSituation.gameObject.SetActive(true);
        }
        // flush current options
        decisionsPanel.Flush();
        // set new id for current situation
        currentSituationID = id;
        Situation current = currentChapter.situations[currentSituationID];
        currentSituation.text = current.description;
      
        if (!characterChosen && !(current is CharacterSelection))
        {
            for (int i = 0; i < starterModels.Length; i++)
                if (starterModels[i].isActiveAndEnabled)
                    player = starterClasses[i].Init(transform.parent, Vector3.zero, Quaternion.identity);
            Destroy(starterModels[0].transform.parent.gameObject);
            playerLifebar.SetFighter(player);
            player.SetLifebar(playerLifebar);
            player.gameObject.SetActive(false);
            characterChosen = true;
        }
        if (!(current is CharacterSelection) && current is Navigation)
            foreach (NextPoint info in (current as Navigation).options)
                InstantiateDecision(info);
        if (current is Dialogue)
        {
            Dialogue dialogue = current as Dialogue;
            foreach (DialogueOption info in dialogue.options)
                InstantiateDecision(info);

            for(int i = 0; i < dialogue.enemies.Length; i++)
            {
                Enemy enemy = Instantiate(dialogue.enemies[i], dialogue.enemyPosition[i],
                    Quaternion.Euler(dialogue.enemyRotation[i]));
                battleManager.AddEnemy(enemy);
            }
        }
        if(current is CharacterSelection)
        {
            CharacterSelection charSel = current as CharacterSelection;
            for(int i = 0; i < starterModels.Length; i++)
                starterModels[i].gameObject.SetActive(i == charSel.classID);
            foreach (NextPoint info in (current as Navigation).options)
                InstantiateDecision(info);
        }
        // change to new situation
        Debug.Log($"change to situation [{current.name}] with id: [{currentSituationID}]");
        cam.UpdateCamPositionAndRotation(current.camPosition, Quaternion.Euler(current.camRotation));
        cam.SetSpotlightRange(current.spotlightRange);

        if (current.flushDeadEnemies)
        {
            foreach (GameObject gameObject in destroyOnSituationChange)
                Destroy(gameObject);
            destroyOnSituationChange = new List<GameObject>();
        }
    }

    public DCPlayer GetPlayerCharacter()
    {
        return player;
    }

    public void ResetDecisions()
    {
        decisionsPanel.Flush();
        Situation current = currentChapter.situations[currentSituationID];
        if (!(current is CharacterSelection) && current is Navigation)
            foreach (NextPoint info in (current as Navigation).options)
                InstantiateDecision(info);
        if (current is Dialogue)
        {
            Dialogue dialogue = current as Dialogue;
            foreach (DialogueOption info in dialogue.options)
                InstantiateDecision(info);
        }
    }

    public void DestroyOnSituationChange(GameObject gameObject)
    {
        destroyOnSituationChange.Add(gameObject);
    }

    private void InstantiateDecision(NextPoint info)
    {
        if (player == null || player.StatsCheckOut(info.strRequirement, info.dexRequirement,
                    info.intRequirement, info.fthRequirement, info.lckRequirement))
            if (info.changeChapter)
                Instantiate(decisionPrefab, decisionsPanel.transform).
                    Init(info.description, info.nextSituationID, info.conditionDistance,
                    info.nextChapter, info.startSituation);
            else
                Instantiate(decisionPrefab, decisionsPanel.transform).
                    Init(info.description, info.nextSituationID, info.conditionDistance);
    }

    private void InstantiateDecision(DialogueOption info)
    {
        if (player.StatsCheckOut(info.strRequirement, info.dexRequirement,
                    info.intRequirement, info.fthRequirement, info.lckRequirement))
            Instantiate(decisionPrefab, decisionsPanel.transform).
                Init(info.description, info.nextSituationID, info.startBattle);
    }
}
