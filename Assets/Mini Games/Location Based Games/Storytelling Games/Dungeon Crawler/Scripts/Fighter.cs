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
    [SerializeField] private DamageNumber damageNumbers;
    [SerializeField] private float damageLabelVerticalMove = 2f;
    // for battle
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private Transform attackPosition;
    [SerializeField] protected float attackPositionOffset = 0f;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Move[] moves;
    [SerializeField] protected Collider[] hitboxes;
    [SerializeField] protected Transform[] spellSpawnTransforms;
    [SerializeField] protected float poisonDamageRecieved = 3f;
    [SerializeField] protected float bleedDamageRecieved = 6f;
    // max caps for values during fight (can be increased when fighting)
    private float currentMaxHealth;
    private float currentMaxStamina;
    private float currentMaxMana;
    protected float currentInitiative;
    private List<Status> currentStatus;
    // current figher values
    protected float health = 1f;
    protected float stamina;
    protected float mana;
    protected float currentHealthRegen;
    protected float currentStaminaRegen;
    protected float currentManaRegen;
    protected float accuracy;
    private bool timerOnPause = false;
    private float timer;
    private float poisonTimer;

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

        if(!timerOnPause) timer += Time.deltaTime * currentInitiative;

        if (!addedToAttackQueue)
        {
            health = Mathf.Min(currentMaxHealth, health + currentHealthRegen * Time.deltaTime);
            stamina = Mathf.Min(currentMaxStamina, stamina + currentStaminaRegen * Time.deltaTime);
            mana = Mathf.Min(currentMaxMana, mana + currentManaRegen * Time.deltaTime);

            if (timer >= 10f)
            {
                addedToAttackQueue = true;
                battleManager.AddToAttackQueue(this);
            }

            if (!battleManager.SomeoneAttacking())
            {
                // apply status effects depending on current status
                float poisonDamage = 0f;
                float bleedDamage = 0f;

                foreach (Status status in currentStatus)
                    switch (status)
                    {
                        case Status.Poison:
                            if (poisonTimer >= 2f)
                                poisonDamage += poisonDamageRecieved;
                            break;
                        case Status.Stun:
                            timer = 0;
                            mana -= 50f;
                            stamina -= 50f;
                            mana = Mathf.Max(0f, mana);
                            stamina = Mathf.Max(0f, stamina);
                            damageNumbers.Stunned();
                            break;
                        case Status.Bleed:
                            bleedDamage += bleedDamageRecieved;
                            break;
                    }
                currentStatus.RemoveAll(status => status == Status.Bleed || status == Status.Stun);
                poisonTimer = poisonTimer >= 2f ? 0 : poisonTimer + Time.deltaTime;
                if (poisonDamage > 0)   damageNumbers.PoisonBy(poisonDamage);
                if (bleedDamage > 0)    damageNumbers.BleedBy(bleedDamage);
                health -= poisonDamage + bleedDamage;
            }
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
        currentHealthRegen = healthRegen;
        currentStaminaRegen = staminaRegen;
        currentManaRegen = manaRegen;
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
        return Mathf.Min(timer / 10f, 1f);
    }

    public Transform GetAttackPosition()
    {
        return attackPosition;
    }

    public void ShowBattleUI(bool show)
    {
        lifebar.gameObject.SetActive(show);
        damageNumbers.gameObject.SetActive(show);
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
            damageNumbers.DamageBy(Mathf.RoundToInt(healthDamage));
            health -= healthDamage;
            stamina -= staminaDamage;
            mana -= manaDamage;
            foreach (Status s in status) currentStatus.Add(s);

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

    public void PauseInitiativeTimer(bool pause)
    {
        timerOnPause = pause;
    }
}