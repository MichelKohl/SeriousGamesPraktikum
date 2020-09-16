using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

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

    [SerializeField] private GameObject[] toggleableObjects;

    private DCPlayer player;
    private int classID = -1;
    private int currentSituationID = 0;
    private int currentChapterID = 0;
    private GameManager manager;
    private BattleManager battleManager;
    private bool characterChosen = false;// must be set to true when loading a saved game state.
    private List<GameObject> destroyOnSituationChange = new List<GameObject>();
    private List<PlotPoint> pathPlayerTook = new List<PlotPoint>();

    public void ChangeSituation(int toID = 0, double distanceToWalk = 0, bool startBattle = false)
    {
        if (startBattle) battleManager.StartBattle();

        StartCoroutine(WaitTillDistanceWalked(toID, distanceToWalk, startBattle));
    }

    public void ChangeChapter(int toChapter, int startSituation = 0)
    {
        currentChapterID = toChapter;
        ChangeSituation(startSituation);
    }

    protected virtual void Start()
    {
        manager = GameManager.INSTANCE;
        battleManager = GetComponent<BattleManager>();
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
                textWhenWalking.SetStart(manager.profile.GetDistanceTraveled(), distanceToWalk);

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
        Situation current = chapters[currentChapterID].situations[currentSituationID];
        currentSituation.text = current.description;

        CharacterSelection charSel = current as CharacterSelection;
        Navigation navi = current as Navigation;
        Dialogue dialogue = current as Dialogue;
        GameOverSituation gameOver = current as GameOverSituation;
      
        if (!characterChosen && !(current is CharacterSelection))
        {
            for (int i = 0; i < starterModels.Length; i++)
                if (starterModels[i].isActiveAndEnabled)
                {
                    player = starterClasses[i].Init(transform.parent, Vector3.zero, Quaternion.identity);
                    classID = i;
                }
            Destroy(starterModels[0].transform.parent.gameObject);
            playerLifebar.SetFighter(player);
            player.SetLifebar(playerLifebar);
            player.gameObject.SetActive(false);
            characterChosen = true;
        }
        if (charSel == null && navi != null)
            foreach (NextPoint info in (current as Navigation).options)
                InstantiateDecision(info);
        if (dialogue != null)
        {
            foreach (DialogueOption info in dialogue.options)
                InstantiateDecision(info);

            for(int i = 0; i < dialogue.enemies.Length; i++)
            {
                Enemy enemy = Instantiate(dialogue.enemies[i], dialogue.enemyPosition[i],
                    Quaternion.Euler(dialogue.enemyRotation[i]));
                battleManager.AddEnemy(enemy);
            }
        }
        if(charSel != null)
        {
            for(int i = 0; i < starterModels.Length; i++)
                starterModels[i].gameObject.SetActive(i == charSel.classID);
            foreach (NextPoint info in (current as Navigation).options)
                InstantiateDecision(info);
        }
        if (current.flushDeadEnemies)
        {
            foreach (GameObject gameObject in destroyOnSituationChange)
                Destroy(gameObject);
            destroyOnSituationChange = new List<GameObject>();
        }
        if (current.important)
        {
            PlotPoint plotPoint = new PlotPoint();
            plotPoint.chapter = currentChapterID;
            plotPoint.situation = currentSituationID;
            pathPlayerTook.Add(plotPoint);
        }
        if(gameOver != null)
        {
            if(gameOver.vfx != null)
            {
                ParticleSystem gameOverVFX = Instantiate(gameOver.vfx, player.transform.position,
                    player.transform.rotation, transform);
                gameOverVFX.Play();
            }
            //TODO: handle game over
        }
        if(dialogue == null || dialogue.dialogueStart)
        {
            cam.UpdateCamPositionAndRotation(current.camPosition, Quaternion.Euler(current.camRotation));
            cam.SetSpotlight(current.spotlightRange);
        }
        try
        {
            foreach (int i in current.activateGameObject)
                toggleableObjects[i].gameObject.SetActive(true);
            foreach (int i in current.deactivateGameObject)
                toggleableObjects[i].gameObject.SetActive(false);
        }
        catch (NullReferenceException) { }
        
    }

    public DCPlayer GetPlayerCharacter()
    {
        return player;
    }

    public void ResetDecisions()
    {
        decisionsPanel.Flush();
        Situation current = chapters[currentChapterID].situations[currentSituationID];
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

    public void ContinueGame()
    {
        StoryGameSave save = GameManager.INSTANCE.profile.GetStoryGameSave();

       // if (save.newGame) return;

        Debug.Log($"Loading current chapter: {save.currentChapter}, {save.currentSituation}");

        pathPlayerTook = save.playerPath;

        classID = save.classID;

        player = starterClasses[classID].Init(transform.parent, Vector3.zero, Quaternion.identity);
        Destroy(starterModels[0].transform.parent.gameObject);

        playerLifebar.SetFighter(player);
        player.SetLifebar(playerLifebar);
        player.gameObject.SetActive(false);
        characterChosen = true;

        player.SetLevel(save.level);
        player.SetAttributes(save.playerAttributes);
        /*
        foreach (Perk perk in save.perks)
            player.AddPerk(perk);
        */
        player.SetUnlockedAttacks(save.unlockedAttacks);
        player.SetStat(Stat.STR, save.strength);
        player.SetStat(Stat.DEX, save.dexterity);
        player.SetStat(Stat.INT, save.intelligence);
        player.SetStat(Stat.FTH, save.faith);
        player.SetStat(Stat.LCK, save.luck);
        player.SetSkillPoints(save.skillPoints);

        startChapter = save.currentChapter;
        startSituationID = save.currentSituation;

        ChangeChapter(save.currentChapter, save.currentSituation);
    }

    public void StartNewGame()
    {
        ChangeChapter(startChapter);
    }

    public void SaveGame()
    {
        GameManager.INSTANCE.profile.SaveStoryGame(classID, currentChapterID, currentSituationID, pathPlayerTook,
            player.GetAttributes(), player.GetUnlockedAttacks(), player.GetLevel(), player.GetSkillPoints(),
            player.GetStat(Stat.STR).Item2, player.GetStat(Stat.DEX).Item2, player.GetStat(Stat.INT).Item2,
            player.GetStat(Stat.FTH).Item2, player.GetStat(Stat.LCK).Item2, player.GetUnlockedPerks()
            );
        GameManager.INSTANCE.SaveProfile(GameManager.INSTANCE.profile);
    }

    public void ExitGame()
    {
        SaveGame();
        SceneManager.LoadScene(0);
    }

    public void ExitGameFromStartMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void InstantiateDecision(NextPoint info)
    {
        if ((player == null || player.StatsCheckOut(info.strRequirement, info.dexRequirement,
                    info.intRequirement, info.fthRequirement, info.lckRequirement)) &&
                    !info.IsBlocked(pathPlayerTook))
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
                    info.intRequirement, info.fthRequirement, info.lckRequirement) &&
                    !info.IsBlocked(pathPlayerTook))
            Instantiate(decisionPrefab, decisionsPanel.transform).
                Init(info.description, info.nextSituationID, info.startBattle);
    }
}

[Serializable]
public class StoryGameSave
{
    public bool newGame = true;
    public int classID;
    public int currentChapter;
    public int currentSituation;
    public List<PlotPoint> playerPath = new List<PlotPoint>();
    public int level;
    public int xp;
    public int skillPoints;
    public int strength;
    public int dexterity;
    public int intelligence;
    public int faith;
    public int luck;
    public Attributes playerAttributes = new Attributes();
    public bool[] unlockedAttacks;
    public bool[] unlockedPerks;
}
