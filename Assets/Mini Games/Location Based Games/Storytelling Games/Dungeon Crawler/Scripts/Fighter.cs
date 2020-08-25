using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEditor;

public class Fighter : MonoBehaviour
{
    // fighter settings
    [SerializeField] protected int level = 1;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegen = 0f;
    [SerializeField] private float initiative = 7f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegen = 0f;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float manaRegen = 0f;
    [SerializeField] private Lifebar lifebar;
    [SerializeField] private TextMeshProUGUI damageLabel;
    [SerializeField] private float damageLabelVerticalMove = 2f;
    // for battle
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private Transform attackPosition;
    [SerializeField] protected float attackPositionOffset = 0f;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Move[] moves;
    [SerializeField] protected Collider[] hitboxes;
    [SerializeField] protected Transform[] spellSpawnTransforms;
    // max caps for values during fight (can be increased when fighting)
    private float currentMaxHealth;
    private float currentMaxStamina;
    private float currentMaxMana;
    private float currentInitiative;
    public List<Status> currentStatus;
    // current figher values
    protected float health = 1f;
    protected float stamina;
    protected float mana;
    protected float currentHealthRegen;
    protected float currentStaminaRegen;
    protected float currentManaRegen;
    protected float accuracy;
    private float timer;
    private float poisonTimer;
    private float damageLabelTimer;
    private Vector3 damageLabelTargetPosition;
    private Vector3 damageLabelStartPosition;

    protected Action<float, int> IncreaseBy = (value, percent) => value *= 1f + (percent / 100);
    protected Action<float, int> DecreaseBy = (value, percent) => value *= 1f - (percent / 100);

    protected bool fighting = false;
    public bool IsFighting { get => fighting; set => fighting = value; }
    protected bool isAttacking = false;
    private bool addedToAttackQueue = false;
    protected BattleManager battleManager;
    protected bool walkingForward = true;
    protected Move currentMove;
    

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ResetFighterValues();
        currentStatus = new List<Status>();
        battleManager = GameObject.Find("Story Manager").GetComponent<BattleManager>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!fighting)
        {
            agent.enabled = false;
            animator.SetFloat("movementSpeed", 0f);
            return;
        }
        else agent.enabled = true;

        animator.SetFloat("movementSpeed", (walkingForward ? 1 : -1) * Vector3.Distance(agent.velocity, Vector3.zero));
        animator.SetFloat("health", health);

        timer += Time.deltaTime;
        damageLabelTimer += Time.deltaTime;

        if (damageLabel.gameObject.activeSelf)
            damageLabel.rectTransform.position = Vector3.MoveTowards(damageLabel.rectTransform.position,
                damageLabelTargetPosition, 0.1f);
        if (damageLabelTimer > 1f)
            damageLabel.gameObject.SetActive(false);

        if (!addedToAttackQueue)
        {
            health = Mathf.Min(currentMaxHealth, health + currentHealthRegen * Time.deltaTime);
            stamina = Mathf.Min(currentMaxStamina, stamina + currentStaminaRegen * Time.deltaTime);
            mana = Mathf.Min(currentMaxMana, mana + currentManaRegen * Time.deltaTime);

            // apply status effects depending on current status
            foreach (Status status in currentStatus)
                switch (status)
                {
                    case Status.Poison:
                        if (poisonTimer >= 5f)
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
        }

        if (!addedToAttackQueue && timer >= 10f - initiative)
        {
            addedToAttackQueue = true;
            battleManager.AddToAttackQueue(this);
        }


    }

    public virtual void ResetFighterValues()
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
        damageLabelTimer = 0f;
        currentHealthRegen = healthRegen;
        currentStaminaRegen = staminaRegen;
        currentManaRegen = manaRegen;
        damageLabelStartPosition = damageLabel.rectTransform.position;
        damageLabelTargetPosition = damageLabel.rectTransform.position + new Vector3(0, damageLabelVerticalMove, 0);
        foreach (Collider hitbox in hitboxes)
            hitbox.enabled = false;
    }

    public int GetLevel()
    {
        return level;
    }

    public float GetHealthRatio()
    {
        return health / currentMaxHealth;
    }

    public float GetStaminaRatio()
    {
        return stamina / currentMaxStamina;
    }

    public float GetManaRatio()
    {
        return mana / currentMaxMana;
    }

    public float GetInitiativeRatio()
    {
        return timer / (10f - initiative);
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

    public Transform GetAttackPosition()
    {
        return attackPosition;
    }

    public void ShowBattleUI(bool show)
    {
        lifebar.gameObject.SetActive(show);
        damageLabel.gameObject.SetActive(show);
    }

    public void SetLifebar(Lifebar lifebar)
    {
        this.lifebar = lifebar;
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public virtual (float healthDamage, float staminaDamage, float manaDamage, List<Status> status) CalculateDamage()
    {
        return (0f, 0f, 0f, new List<Status>());
    }

    public void Attack()
    {
        //Debug.Log($"{name} is attacking...");
        StartCoroutine(Attacking());
    }

    protected virtual IEnumerator Attacking()
    {
        // end attack
        battleManager.SendAttackDone();
        addedToAttackQueue = false;
        timer = 0;
        isAttacking = false;
        yield return null;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(transform.tag) && !isAttacking && (!battleManager.SomeoneGotHit || battleManager.CurrentMoveDoesMultipleHits()))
        {
            animator.SetTrigger("hit");
            battleManager.SomeoneGotHit = true;
            (float healthDamage, float staminaDamage, float manaDamage, List<Status> status) = battleManager.GetDamage();
            damageLabel.gameObject.SetActive(true);
            damageLabel.rectTransform.position = damageLabelStartPosition;
            damageLabel.text = $"{Mathf.RoundToInt(healthDamage)}";
            damageLabelTimer = 0;
            health -= healthDamage;
            stamina -= staminaDamage;
            mana -= manaDamage;
            foreach (Status s in status) status.Add(s);

            if (battleManager.CurrentMove is PlayerMelee)
                DoOnHit();

            SpellProjectile projectile = other.gameObject.GetComponent<SpellProjectile>();
            if (projectile != null)
                projectile.DoExplosion();
        }
            
    }

    protected virtual void DoOnHit()
    {

    }

    protected void PayForAttack()
    {
        health -= currentMove.healthCost;
        stamina -= currentMove.staminaCost;
        mana -= currentMove.manaCost;
    }

    public Collider GetCollider(int withID)
    {
        return hitboxes[withID];
    }
}