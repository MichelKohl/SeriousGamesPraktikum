using System.Collections;
using UnityEngine;
using TMPro;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private Cam cam;
    [SerializeField] private StarterClass starter;
    [SerializeField] private Lifebar playerLifebar;
    [SerializeField] private int startSituationID = 0;
    [SerializeField] private TextMeshProUGUI currentSituation;
    [SerializeField] private DecisionsPanel decisionsPanel;
    [SerializeField] private Situation[] situations;
    [SerializeField] private Decision decisionPrefab;
    [SerializeField] private bool turnOffWalkingRequirement = false;
    [SerializeField] private WalkingText textWhenWalking;
    [SerializeField] private float skyboxRotationSpeed = 0.2f;

    private DCPlayer player;
    private int currentSituationID = 0;
    private GameManager manager;
    private BattleManager battleManager;

    public void ChangeSituation(int toID = 0, double distanceToWalk = 0, bool startBattle = false)
    {
        if (startBattle) battleManager.StartBattle();

        IEnumerator coroutine = WaitTillDistanceWalked(toID, distanceToWalk, startBattle);
        // TODO save coroutine in profile
        StartCoroutine(coroutine);
    }

    protected virtual void Start()
    {
        manager = GameManager.INSTANCE;
        battleManager = GetComponent<BattleManager>();
        player = starter.Init(transform.parent, transform.position, transform.rotation);
        playerLifebar.SetFighter(player);
        player.SetLifebar(playerLifebar);
        player.gameObject.SetActive(false);

        ChangeSituation(startSituationID);
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
        Situation current = situations[currentSituationID];
        currentSituation.text = current.description;
        // create new options;
        if (current is Navigation)
            foreach (NextPoint info in (current as Navigation).options)
                if (player.StatsCheckOut(info.strRequirement, info.dexRequirement,
                    info.intRequirement, info.fthRequirement, info.lckRequirement))
                    Instantiate(decisionPrefab, decisionsPanel.transform).
                        Init(info.description, info.nextSituationID, info.conditionDistance);
        if (current is Dialogue)
        {
            Dialogue dialogue = current as Dialogue;
            foreach (DialogueOption info in dialogue.options)
                if (player.StatsCheckOut(info.strRequirement, info.dexRequirement,
                    info.intRequirement, info.fthRequirement, info.lckRequirement))
                    Instantiate(decisionPrefab, decisionsPanel.transform).
                        Init(info.description, info.nextSituationID, info.startBattle);

            for(int i = 0; i < dialogue.enemies.Length; i++)
                battleManager.AddEnemy(Instantiate(dialogue.enemies[i], dialogue.enemyPosition[i],
                    Quaternion.Euler(dialogue.enemyRotation[i])));
        }
                
            
        //TODO: disable and enable assets to save on computation
        // change to new situation
        Debug.Log($"change to situation with id: [{currentSituationID}]");
        cam.transform.position = current.camPosition;
        cam.transform.rotation = Quaternion.Euler(current.camRotation);
    }

    public DCPlayer GetPlayerCharacter()
    {
        return player;
    }
}
