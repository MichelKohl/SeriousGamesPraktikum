using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using System.Collections.Generic;

public class Fighter : MonoBehaviour
{
    // fighter settings
    [SerializeField] private string fighterName;
    [SerializeField] protected int level = 1;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegen = 1f;
    [SerializeField] private float initiative = 7f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegen = 2f;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float manaRegen = 2f;
    // character stats
    public int strength = 1;    // -> one and two handed weapons & stamina regen
    public int dexterity = 1;   // -> one handed weapons (especially daggers) & initiative
    public int intelligence = 1;// -> effectiveness of magic spells & mana regen
    public int faith = 1;       // -> effectiveness of faith spells & mana regen
    public int luck = 1;        // -> chance of critical hits & effects (stun, bleed, poison etc.) and chance of evading an attack
    // UI
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image initiativeBar;
    // for battle
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform attackPosition;
    [SerializeField] private Animator animator;
    // max caps for values during fight (can be increased when fighting)
    private float currentMaxHealth;
    private float currentMaxStamina;
    private float currentMaxMana;
    private float currentInitiative;
    public List<Status> currentStatus;
    // current figher values
    private float health;
    private float stamina;
    private float mana;
    private float currentHealthRegen;
    private float currentStaminaRegen;
    private float currentManaRegen;
    private float accuracy;
    private float timer;
    private float poisonTimer;

    protected Action<float, int> IncreaseBy = (value, percent) => value *= 1f + (percent / 100);
    protected Action<float, int> DecreaseBy = (value, percent) => value *= 1f - (percent / 100);

    public bool fighting = false;
    private bool addedToAttackQueue = false;
    private BattleManager battleManager;
    private Vector3 enemyAttackPosition;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ResetFighterValues();
        nameLabel.text = fighterName;
        currentStatus = new List<Status>();
        battleManager = GameObject.Find("Story Manager").GetComponent<BattleManager>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!fighting) return;

        timer += Time.deltaTime;

        health = Mathf.Min(currentMaxHealth, health * currentHealthRegen);
        stamina = Mathf.Min(currentMaxStamina, stamina * currentStaminaRegen);
        mana = Mathf.Min(currentMaxMana, mana * currentManaRegen);

        healthBar.fillAmount = health / currentMaxHealth;
        staminaBar.fillAmount = stamina / currentMaxStamina;
        manaBar.fillAmount = mana / currentMaxMana;
        initiativeBar.fillAmount = timer / (10f - initiative);

        // apply status effects depending on current status
        foreach (Status status in currentStatus)
            switch (status)
            {
                case Status.Poison:
                    if(poisonTimer >= 5f)
                    {
                        DecreaseBy(health, 2);
                        DecreaseBy(stamina, 2);
                        DecreaseBy(mana, 2);
                    }
                    break;
                case Status.Stun:
                    timer = 0;
                    DecreaseBy(mana, 50);
                    DecreaseBy(stamina, 50);
                    break;
                case Status.Bleed:
                    DecreaseBy(health, 5);
                    DecreaseBy(mana, 5);
                    DecreaseBy(stamina, 5);
                    break;
            }
        poisonTimer = poisonTimer >= 5f ? 0 : poisonTimer + Time.deltaTime;
        currentStatus.RemoveAll(status => status == Status.Bleed);

        if (!addedToAttackQueue && timer >= 10f - initiative)
        {
            addedToAttackQueue = true;
            battleManager.AddToAttackQueue(this);
        }
    }

    public void ResetFighterValues()
    {
        health = maxHealth;
        currentMaxHealth = maxHealth;
        stamina = maxStamina;
        currentMaxStamina = maxStamina;
        mana = maxMana;
        currentMaxMana = maxMana;
        currentInitiative = initiative;
        accuracy = 1f;
        timer = 0f;
        currentHealthRegen = healthRegen;
        currentStaminaRegen = staminaRegen;
        currentManaRegen = manaRegen;
    }

    public string GetName()
    {
        return fighterName;
    }

    public void IncreaseMaxHealthBy(int percent, bool persistent = false)
    {
        IncreaseBy(persistent ? maxHealth : currentMaxHealth, percent);
    }

    public void IncreaseHealthBy(int percent)
    {
        IncreaseBy(health, percent);
    }

    public void IncreaseHealthRegenBy(int percent)
    {
        IncreaseBy(currentHealthRegen, percent);
    }

    public void IncreaseMaxStaminaBy(int percent, bool persistent = false)
    {
        IncreaseBy(persistent ? maxStamina : currentMaxStamina, percent);
    }

    public void IncreaseStaminaBy(int percent)
    {
        IncreaseBy(stamina, percent);
    }

    public void IncreaseStaminaRegenBy(int percent)
    {
        IncreaseBy(currentStaminaRegen, percent);
    }

    public void IncreaseMaxManaBy(int percent, bool persistent = false)
    {
        IncreaseBy(persistent ? maxMana : currentMaxMana, percent);
    }

    public void IncreaseManaBy(int percent)
    {
        IncreaseBy(mana, percent);
    }

    public void IncreaseManaRegenBy(int percent)
    {
        IncreaseBy(currentManaRegen, percent);
    }

    public void IncreaseInitiativeBy(int percent, bool persistent = false)
    {
        IncreaseBy(persistent ? initiative : currentInitiative, percent);
    }

    public void IncreaseAccuracyBy(int percent)
    {
        IncreaseBy(accuracy, percent);
    }

    public void DecreaseMaxHealthBy(int percent)
    {
        DecreaseBy(currentMaxHealth, percent);
    }

    public void DecreaseHealthBy(float value)
    {
        health -= value;
    }

    public void DecreaseHealthRegenBy(int percent)
    {
        DecreaseBy(currentHealthRegen, percent);
    }

    public void DecreaseMaxStaminaBy(int percent)
    {
        DecreaseBy(currentMaxStamina, percent);
    }

    public void DecreaseStaminaBy(float value)
    {
        stamina -= value;
    }

    public void DecreaseStaminaRegenBy(int percent)
    {
        DecreaseBy(currentStaminaRegen, percent);
    }

    public void DecreaseMaxManaBy(int percent)
    {
        DecreaseBy(currentMaxMana, percent);
    }

    public void DecreaseManaBy(float value)
    {
        mana -= value;
    }

    public void DecreaseManaRegenBy(int percent)
    {
        DecreaseBy(currentManaRegen, percent);
    }

    public void DecreaseInitiativeBy(int percent)
    {
        DecreaseBy(currentInitiative, percent);
    }

    public void DecreaseAccuracyBy(int percent)
    {
        DecreaseBy(accuracy, percent);
    }

    public void SetEnemyAttackPosition(Vector3 position)
    {
        enemyAttackPosition = position;
    }

    public Vector3 GetAttackPosition()
    {
        return attackPosition.position;
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public void Attack()
    {
        Debug.Log($"{fighterName} is attacking...");
        if (enemyAttackPosition == null)
        {
            Debug.Log($"ERROR: enemy attack position of {fighterName}");
            return;
        }
        animator.SetBool("run", true);
        agent.SetDestination(enemyAttackPosition);
        agent.enabled = true;
    }

    public bool StatsCheckOut(int strength, int dexterity, int intelligence, int faith, int luck)
    {
        return this.strength >= strength && this.dexterity >= dexterity && this.intelligence >= intelligence && this.faith >= faith && this.luck >= luck;
    }

    protected virtual IEnumerator Attacking()
    {
        yield return null;

        battleManager.SendAttackDone();
        addedToAttackQueue = false;
        timer = 0;
    }
}
