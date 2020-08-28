using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0414 // disable warnings caused by line 66

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
    [SerializeField] private Perk[] startPerks;
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
    // attributes
    protected Attributes attributes = new Attributes();
    protected List<Perk> perks = new List<Perk>();
    private List<Status> currentStatus;
    private bool timerOnPause = false;
    private float timer;
    private float poisonTimer;

    private float poisonResistance = 0f;
    private float bleedResistance = 0f;
    private float stunResistance = 0f;


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
        InitializePerks();
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
        animator.SetFloat("health", attributes.Health);

        if(!timerOnPause) timer += Time.deltaTime * attributes.Initiative;

        if (!addedToAttackQueue)
        {
            // regenerates health, stamina and mana according to the regen variables
            attributes.Regenerate();
            
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
                            if (poisonResistance >= 1) continue;
                            if (poisonTimer >= 2f && poisonResistance < UnityEngine.Random.Range(0, 1f))
                                poisonDamage += poisonDamageRecieved;
                            break;
                        case Status.Stun:
                            if (stunResistance >= 1) continue;
                            if(stunResistance < UnityEngine.Random.Range(0, 1f))
                            {
                                timer = 0;
                                attributes.AddToAttributes(0f, -50f, -50f);
                                damageNumbers.Stunned();
                            }
                            break;
                        case Status.Bleed:
                            if (bleedResistance >= 1) continue;
                            if(bleedResistance < UnityEngine.Random.Range(0, 1f)) { }
                                bleedDamage += bleedDamageRecieved;
                            break;
                        case Status.HealPoison:
                            poisonDamage = 0;
                            currentStatus.RemoveAll(s => s.Equals(Status.Poison));
                            break;
                    }
                currentStatus.RemoveAll(status => status.Equals(Status.Bleed)|| status.Equals(Status.Stun));
                if (poisonResistance >= 1f) currentStatus.RemoveAll(status => status.Equals(Status.Poison));

                poisonTimer = poisonTimer >= 2f ? 0 : poisonTimer + Time.deltaTime;

                if (poisonDamage > 0)   damageNumbers.PoisonBy(poisonDamage);
                if (bleedDamage > 0)    damageNumbers.BleedBy(bleedDamage);
                attributes.AddToAttributes(-(poisonDamage + bleedDamage), 0f, 0f);
            }
        }
    }

    public virtual void ResetFighterValues()
    {
        attributes.Init(maxHealth, maxStamina, maxMana, healthRegen, staminaRegen, manaRegen,
            maxHealth, maxStamina, maxMana, initiative);
        timer = 0f;
        foreach (Collider hitbox in hitboxes)
            hitbox.enabled = false;
    }

    public int GetLevel()
    {
        return level;
    }

    public float GetHealthRatio()
    {
        return attributes.GetHealthRatio();
    }

    public float GetStaminaRatio()
    {
        return attributes.GetStaminaRatio(); ;
    }

    public float GetManaRatio()
    {
        return attributes.GetManaRatio();
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
        return  Mathf.Round(attributes.Health * 100f) / 100f <= 0;
    }

    public virtual (float healthDamage, float staminaDamage, float manaDamage, List<Status> status) CalculateDamage()
    {
        return (0f, 0f, 0f, new List<Status>());
    }

    public void Attack()
    {
        /*
        if(currentMove.particleEffects != null && currentMove.particleDelay != null
            && currentMove.particleSpawnPositions != null)
        {
            for (int i = 0; i < currentMove.particleEffects.Length; i++)
                StartCoroutine(PlayParticles(currentMove.particleDelay[i],
                    currentMove.particleEffects[i], currentMove.particleSpawnPositions[i]));
        }
        */
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

    private IEnumerator PlayParticles(float delay, ParticleSystem particles, Transform origin)
    {
        yield return new WaitForSeconds(delay);
        ParticleSystem vfx = Instantiate(particles, origin.position, origin.rotation, transform.parent);
        vfx.Play();
        yield return new WaitForSeconds(vfx.main.duration);
        Destroy(vfx.gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(transform.tag) && !isAttacking && (!battleManager.SomeoneGotHit || battleManager.CurrentMoveDoesMultipleHits()))
        {
            animator.SetTrigger("hit");
            battleManager.SomeoneGotHit = true;
            (float healthDamage, float staminaDamage, float manaDamage, List<Status> status) = battleManager.GetDamage();
            damageNumbers.DamageBy(Mathf.RoundToInt(healthDamage));
            attributes.AddToAttributes(-healthDamage, -staminaDamage, -manaDamage);
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
        attributes.AddToAttributes(-currentMove.healthCost, -currentMove.staminaCost, -currentMove.manaCost);
    }

    protected void InitializePerks()
    {
        foreach (Perk perk in startPerks)
            perks.Add(perk);
    }

    protected void AddPerk(Perk perk)
    {
        perks.Add(perk);

        StatusPerk statusPerk = perk as StatusPerk;
        AttributePerk attributePerk = perk as AttributePerk;

        if(statusPerk != null)
        {
            switch (statusPerk.status)
            {
                case Status.Bleed:
                    bleedResistance += statusPerk.resistance;
                    bleedDamageRecieved *= statusPerk.damageMultiplier;
                    break;
                case Status.Poison:
                    poisonResistance += statusPerk.resistance;
                    poisonDamageRecieved *= statusPerk.damageMultiplier;
                    break;
                case Status.Stun:
                    stunResistance += statusPerk.resistance;
                    break;
                default:
                    break;
            }
        }
        if(attributePerk != null)
        {
            attributes.AddToRegen(attributePerk.addHealthRegen, attributePerk.addStaminaRegen,
                attributePerk.addManaRegen);
            attributes.MultiplyMaxAttributes(attributePerk.maxHealthMultiplier,
                attributePerk.maxStaminaMultiplier, attributePerk.maxManaMultiplier);
            attributes.Accuracy = attributePerk.accuracyMultiplier;
            attributes.Initiative = attributePerk.initiativeMultiplier;
        }
        if (perk.temporary)
            StartCoroutine(DeleteTemporaryPerk(perk, perk.duration));
    }

    public Collider GetCollider(int withID)
    {
        return hitboxes[withID];
    }

    public void PauseInitiativeTimer(bool pause)
    {
        timerOnPause = pause;
    }

    private IEnumerator DeleteTemporaryPerk(Perk perk, float duration)
    {
        yield return new WaitForSeconds(duration);
        perks.Remove(perk);
    }
}



public class Attributes
{
    private float stamina = 100f;
    private float mana = 100f;

    private float healthRegen = 0;
    private float staminaRegen = 0;
    private float manaRegen = 0;

    private float maxHealth = 100f;
    private float maxStamina = 100f;
    private float maxMana = 100f;

    private float accuracy = 1f;
    private float initiative = 5;

    public float Health { get; private set; }
    public float Initiative { get => initiative; set => initiative *= value; }
    public float Accuracy { get => accuracy; set => accuracy *= value; }

    public Attributes()
    {
        Health = 100f;
    }

    public void Init(float health, float stamina, float mana, float healthRegen,
        float staminaRegen, float manaRegen, float maxHealth, float maxStamina,
        float maxMana, float initiative)
    {
        Health = health;
        this.stamina = stamina;
        this.mana = mana;
        this.healthRegen = healthRegen;
        this.staminaRegen = staminaRegen;
        this.manaRegen = manaRegen;
        this.maxHealth = maxHealth;
        this.maxStamina = maxStamina;
        this.maxMana = maxMana;
        this.initiative = initiative;
    }

    public void Init(float staminaRegen, float manaRegen, float initiative)
    {
        this.staminaRegen = staminaRegen;
        this.manaRegen = manaRegen;
        this.initiative = initiative;
    }

    public void Regenerate()
    {
        if(healthRegen != 0)
            Health = Mathf.Min(maxHealth, Health + healthRegen * Time.deltaTime);
        stamina = Mathf.Min(maxStamina, stamina + staminaRegen * Time.deltaTime);
        mana = Mathf.Min(maxMana, mana + manaRegen * Time.deltaTime);
    }

    public void AddToAttributes(float health = 0, float stamina = 0, float mana = 0)
    {
        this.Health = Mathf.Min(this.Health + health, maxHealth);
        this.stamina = Mathf.Min(this.stamina + stamina, maxStamina);
        this.mana = Mathf.Min(this.mana + mana, maxMana);

    }

    public void AddToRegen(float healthRegen = 0, float staminaRegen = 0, float manaRegen = 0)
    {
        this.healthRegen += healthRegen;
        this.staminaRegen += staminaRegen;
        this.manaRegen += manaRegen;
    }

    public void MultiplyMaxAttributes(float healthMultiplier = 1f, float staminaMultiplier = 1f, float manaMultiplier = 1f)
    {
        maxHealth *= healthMultiplier;
        maxStamina *= staminaMultiplier;
        maxMana *= manaMultiplier;
    }

    public void MultiplyAttributes(float healthMultiplier = 1f, float staminaMultiplier = 1f, float manaMultiplier = 1f)
    {
        Health = Mathf.Min(Health * healthMultiplier, maxHealth);
        stamina = Mathf.Min(stamina * staminaMultiplier, maxStamina);
        mana = Mathf.Min(mana * manaMultiplier, maxMana);
    }

    public float GetHealthRatio()
    {
        return Health / maxHealth;
    }

    public float GetStaminaRatio()
    {
        return stamina / maxStamina;
    }

    public float GetManaRatio()
    {
        return mana / maxMana;
    }

    public bool CanPay(float healthCost, float staminaCost, float manaCost)
    {
        return healthCost <= Health && staminaCost <= stamina && manaCost <= mana;
    }
}