using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private StoryManager storyManager;
    [SerializeField] private Cam cam;
    [SerializeField] private Transform playerTransformInMainMenu;
    [SerializeField] private Transform camTransformInMainMenu;
    [SerializeField] private ScrollPanel scrollPanel;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject startMenuUI;
    [SerializeField] private GameObject pedestal;
    [SerializeField] private SkillButton skillPrefab;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI classLabel;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TextMeshProUGUI skillLabel;
    [SerializeField] private UnlockButton unlockButton;

    [SerializeField] private TextMeshProUGUI perkDescription;
    [SerializeField] private GameObject moveDescription;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI stamina;
    [SerializeField] private TextMeshProUGUI mana;
    [SerializeField] private TextMeshProUGUI status;
    [SerializeField] private TextMeshProUGUI statusProbability;
    [SerializeField] private TextMeshProUGUI STR;
    [SerializeField] private TextMeshProUGUI AGL;
    [SerializeField] private TextMeshProUGUI INT;
    [SerializeField] private TextMeshProUGUI FTH;
    [SerializeField] private TextMeshProUGUI LCK;
    [SerializeField] private TextMeshProUGUI crit;
    [SerializeField] private TextMeshProUGUI moveInfo;
    [SerializeField] private TextMeshProUGUI blockEnable;

    [SerializeField] private Button continueButton;

    private DCPlayer player;
    private Vector3 currentPlayerPosition;
    private Vector3 currentCamPosition;
    private Quaternion currentPlayerRotation;
    private Quaternion currentCamRotation;
    private bool perksAndMovesSet = false;

    private void ResetUI()
    {
        type.transform.parent.gameObject.SetActive(false);
        skillName.gameObject.SetActive(false);
        perkDescription.gameObject.SetActive(false);
        moveDescription.SetActive(false);
    }

    private void Start()
    {
        gameplayUI.SetActive(true);
        mainMenuUI.SetActive(false);
        unlockButton.gameObject.SetActive(false);
        mainMenuButton.SetActive(false);

        ChangeToStartMenu();
    }

    private void Update()
    {
        if(player == null)
        {
            player = storyManager.GetPlayerCharacter();
            if (player == null) return;
            else
            {
                unlockButton.SetPlayer(player);
                mainMenuButton.SetActive(true);
            }
        }
        classLabel.text = player.GetClass();
        levelLabel.text = $"{player.GetLevel()}";
        skillLabel.text = $"{player.GetSkillPoints()}";
    }

    public void ChangeToStartMenu()
    {
        Profile profile = GameManager.INSTANCE.profile;
        StoryGameSave save = profile.GetStoryGameSave();
        if (!save.newGame) continueButton.gameObject.SetActive(true);
    }

    public void ChangeToMainMenu()
    {
        if (player == null)
            player = storyManager.GetPlayerCharacter();
        if (player == null)
            return;
        player.DisableAgent(true);

        cam.SetSpotlight(50, 1.5f);

        currentPlayerPosition = player.transform.position;
        currentCamPosition = cam.transform.position;
        currentPlayerRotation = player.transform.rotation;
        currentCamRotation = cam.transform.rotation;

        player.transform.position = playerTransformInMainMenu.position;
        player.transform.rotation = playerTransformInMainMenu.rotation;
        cam.transform.position = camTransformInMainMenu.position;
        cam.transform.rotation = camTransformInMainMenu.rotation;

        player.gameObject.SetActive(true);
        player.ResetFighterValues();
        pedestal.SetActive(true);

        mainMenuUI.SetActive(true);
        gameplayUI.SetActive(false);

        if (!perksAndMovesSet)// needed for first time in main menu
        {
            
            foreach (PlayerMove move in player.GetAllMoves())
                Instantiate(skillPrefab, scrollPanel.transform).
                    Init(this, unlockButton, move).gameObject.SetActive(true);
             
            foreach (Perk perk in player.GetAllPerks())
                Instantiate(skillPrefab, scrollPanel.transform).
                    Init(this, unlockButton, perk).gameObject.SetActive(true);
               
            perksAndMovesSet = true;
        }
    }

    public void DisableStartMenu()
    {
        startMenuUI.gameObject.SetActive(false);
    }

    public void EnableStartMenu()
    {
        startMenuUI.gameObject.SetActive(true);
    }

    public void ChangeBackToGameplay()
    {
        if (player == null)
            player = storyManager.GetPlayerCharacter();
        try
        {
            player.transform.position = currentPlayerPosition;
            player.transform.rotation = currentPlayerRotation;
            player.gameObject.SetActive(false);
            player.ResetFighterValues();
        }
        catch (Exception) { }


        cam.transform.position = currentCamPosition;
        cam.transform.rotation = currentCamRotation;

        pedestal.SetActive(false);

        gameplayUI.SetActive(true);
        mainMenuUI.SetActive(false);

        
        storyManager.ResetDecisions();

        cam.ResetSpotlight();

        ResetUI();

    }

    public void PreviewAnimation(PlayerMove move)
    {
        player.Preview(move);
    }

    public void PreviewInfo(Perk perk)
    {
        perkDescription.gameObject.SetActive(true);
        moveDescription.SetActive(false);

        skillName.text = perk.name;
        skillName.gameObject.SetActive(true);

        type.text = "Perk";
        perkDescription.text = perk.descritption;
    }

    public void PreviewInfo(PlayerMove move)
    {
        perkDescription.gameObject.SetActive(false);
        moveDescription.SetActive(true);

        skillName.text = move.name;
        skillName.gameObject.SetActive(true);

        PlayerSelfBuff selfBuff = move as PlayerSelfBuff;

        blockEnable.text = $"({Mathf.RoundToInt(move.probOfBlock * 100)}%)";

        if(selfBuff != null)
        {
            type.text = "Self Buff";
            type.gameObject.SetActive(true);
            type.transform.parent.gameObject.SetActive(true);
            moveDescription.SetActive(false);
            perkDescription.text = selfBuff.buff.descritption +
                $"\nCurrent duration of buff: {selfBuff.buff.duration + selfBuff.EnhanceDuration(player, selfBuff.buff.duration)} turns";
            perkDescription.gameObject.SetActive(true);
        }
        else
        {
            PlayerAttack attack = move as PlayerAttack;
            type.text = attack is PlayerMelee ? "Melee" : "Spell";
            type.gameObject.SetActive(true);
            type.transform.parent.gameObject.SetActive(true);

            (string str, string dex, string inte, string fth, string lck, float critMultiplier) = attack.GetScalingInfo(player);
            STR.text = $"STR\n" + str;
            AGL.text = $"AGL\n" + dex;
            INT.text = $"INT\n" + inte;
            FTH.text = $"FTH\n" + fth;
            LCK.text = $"LCK\n" + lck;

            (float healthDamage, float _, float _, List<Status> statuses, float statusProbability, float luck) = attack.GetAttackInfo(player, true);

            damage.text =   $"{Mathf.RoundToInt(healthDamage)}";
            stamina.text =  $"{Mathf.RoundToInt(move.staminaCost)}";
            mana.text =     $"{Mathf.RoundToInt(move.manaCost)}";

            int poisonCounter = 0, bleedCounter = 0, burnCounter = 0;
            bool doesStun = false;

            foreach(Status status in statuses)
            {
                switch (status)
                {
                    case Status.Poison:
                        poisonCounter++;
                        break;
                    case Status.Bleed:
                        bleedCounter++;
                        break;
                    case Status.Stun:
                        doesStun = true;
                        break;
                    case Status.Burn:
                        burnCounter++;
                        break;
                    default:
                        break;
                }
            }
            string statusInfo = "";
            if (poisonCounter == 0 && bleedCounter == 0 && doesStun == false)
                this.statusProbability.text = "None";
            else
            {
                if (poisonCounter > 0)  statusInfo += $"Poison ({poisonCounter}x) ";
                if (bleedCounter > 0)   statusInfo += $"Bleed ({bleedCounter}x) ";
                if (burnCounter > 0)    statusInfo += $"Burn ({burnCounter}x)";
                if (doesStun)           statusInfo += "Stun";
                this.statusProbability.text = $"({Mathf.Min(100, Mathf.RoundToInt(statusProbability * 100))}%)";
            }
            status.text = statusInfo;
            crit.text = $"{Mathf.Min(100, Mathf.RoundToInt(luck * 100))}%/{critMultiplier}";
        }
    }
}
