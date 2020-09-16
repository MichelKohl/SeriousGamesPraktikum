using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

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
    [SerializeField] protected DamageNumber damageNumbers;
    [SerializeField] private float damageLabelVerticalMove = 2f;
    // for battle
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private Transform attackPosition;
    [SerializeField] protected float attackPositionOffset = 0f;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Move[] moves;
    [SerializeField] protected Collider[] hitboxes;
    [SerializeField] protected Collider hurtbox;
    [SerializeField] protected Transform[] spellSpawnTransforms;
    [SerializeField] protected float poisonDamageRecieved = 4f;
    [SerializeField] protected float bleedDamageRecieved = 4f;
    [SerializeField] protected float burnDamageRecieved = 4f;
    [SerializeField] private ParticleSystem bleedFX;
    [SerializeField] private ParticleSystem poisonFX;
    [SerializeField] private ParticleSystem burnFX;
    [SerializeField] private Transform projectileTarget;
    [SerializeField] private float baseProbabilityOfBlock;
    [SerializeField] private bool testAttacks = false;

    // attributes
    protected Attributes attributes = new Attributes();
    protected List<Perk> perks = new List<Perk>();
    private List<Status> currentStatus;
    private bool timerOnPause = false;
    private float timer;
    private float statusTimer;

    private float poisonResistance = 0f;
    private float bleedResistance = 0.2f;
    private float stunResistance = 0f;
    private float burnResistance = 0f;

    protected Action<float, int> IncreaseBy = (value, percent) => value *= 1f + (percent / 100);
    protected Action<float, int> DecreaseBy = (value, percent) => value *= 1f - (percent / 100);

    protected bool fighting = false;
    public bool IsFighting { get => fighting; set => fighting = value; }
    protected bool isAttacking = false;
    private bool addedToAttackQueue = false;
    private bool deathNotficationSent = false;
    protected BattleManager battleManager;
    protected bool walkingForward = true;
    protected Move currentMove;
    protected Dictionary<Perk, int> perkRoundsPassed = new Dictionary<Perk, int>();
    private List<GameObject> removeOnInit = new List<GameObject>();
    

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ResetFighterValues();
        currentStatus = new List<Status>();
        battleManager = GameObject.Find("Story Manager").GetComponent<BattleManager>();
        if(this is Enemy) GetComponent<Rigidbody>().useGravity = false;
    }

    // Update is called once per frame
    protected void Update()
    {
        if(IsDead())
        {
            hurtbox.enabled = false;
        }
        if (IsDead() && !deathNotficationSent)
        {
            if (poisonFX.isPlaying) poisonFX.Stop();
            damageNumbers.gameObject.SetActive(false);
            lifebar.gameObject.SetActive(false);
            battleManager.SendEnemyDeadSignal(this as Enemy);
            deathNotficationSent = true;
        }
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
                    
            if (timer >= 10f || testAttacks)
            {
                addedToAttackQueue = true;
                battleManager.AddToAttackQueue(this);
                foreach (Perk perk in perks)
                    if (perk.temporary)
                        perkRoundsPassed[perk]++;
            }

            if (!battleManager.SomeoneAttacking() && !IsDead())
            {
                // apply status effects depending on current status
                float poisonDamage = 0f;
                float bleedDamage = 0f;
                float burnDamage = 0f;

                foreach (Status status in currentStatus)
                    switch (status)
                    {
                        case Status.Poison:
                            if (poisonResistance >= 1) continue;
                            if (statusTimer >= 2f && poisonResistance < UnityEngine.Random.Range(0, 1f))
                            {
                                poisonDamage += poisonDamageRecieved;
                                poisonResistance -= 0.05f;
                            }
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
                            if(bleedResistance < UnityEngine.Random.Range(0, 1f))
                            {
                                bleedDamage += bleedDamageRecieved;
                                bleedResistance -= 0.1f;
                            }
                            break;
                        case Status.Burn:
                            if (burnResistance >= 1) continue;
                            if (statusTimer >= 2f && burnResistance < UnityEngine.Random.Range(0, 1f))
                            {
                                burnDamage += burnDamageRecieved;
                                burnResistance += 0.05f;
                            }
                            break;
                        case Status.HealPoison:
                            poisonDamage = 0;
                            currentStatus.RemoveAll(s => s.Equals(Status.Poison));
                            poisonResistance = 0;
                            break;
                    }
                currentStatus.RemoveAll(status => status.Equals(Status.Bleed)|| status.Equals(Status.Stun));
                if (poisonResistance >= 1f) currentStatus.RemoveAll(status => status.Equals(Status.Poison));

                statusTimer = statusTimer >= 2f ? 0 : statusTimer + Time.deltaTime;

                if (currentStatus.Contains(Status.Poison) && !IsDead())
                {
                    if (!poisonFX.isPlaying) poisonFX.Play();
                }
                else if (poisonFX.isPlaying) poisonFX.Stop();

                if (currentStatus.Contains(Status.Burn) && !IsDead())
                {
                    if (!burnFX.isPlaying) burnFX.Play();
                }
                else if (burnFX.isPlaying) burnFX.Stop();

                if (poisonDamage > 0) damageNumbers.PoisonBy(poisonDamage);
                if (burnDamage > 0)   damageNumbers.BurnBy(burnDamage);
                if (bleedDamage > 0)
                {
                    damageNumbers.BleedBy(bleedDamage);
                    bleedFX.Play();
                }
                attributes.AddToAttributes(-(poisonDamage + bleedDamage + burnDamage), -poisonDamage, -poisonDamage);
            }
        }
    }

    public virtual void ResetFighterValues()
    {
        attributes.Init(maxHealth, maxStamina, maxMana, healthRegen, staminaRegen, manaRegen,
            maxHealth, maxStamina, maxMana, initiative, baseProbabilityOfBlock);
        timer = 0f;
        foreach (Collider hitbox in hitboxes)
            hitbox.enabled = false;
        foreach (GameObject gameObject in removeOnInit)
            if(gameObject != null)
            {
                Debug.Log($"Destroy {gameObject.name}");
                Destroy(gameObject);
            }
        removeOnInit = new List<GameObject>();
        InitializePerks();
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
        if(lifebar != null)         lifebar.gameObject.SetActive(show);
        if(damageNumbers != null)   damageNumbers.gameObject.SetActive(show);
    }

    public void SetLifebar(Lifebar lifebar)
    {
        this.lifebar = lifebar;
    }

    public bool IsDead()
    {
        return  Mathf.Round(attributes.Health * 100f) / 100f <= 0;
    }

    public virtual (float healthDamage, float staminaDamage, float manaDamage, List<Status> status, float statusProbability, float luck)
        CalculateDamage()
    {
        return (0f, 0f, 0f, new List<Status>(), 0f, 0f);
    }

    public void Attack()// returns whether someone is attacking or not
    {
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

    public IEnumerator PlayParticles(float delay, ParticleSystem particles, Transform origin, float duration = 0, bool releaseOnInit = false)
    {
        yield return new WaitForSeconds(delay);
        ParticleSystem vfx = Instantiate(particles, origin.position, Quaternion.Euler(particles.transform.eulerAngles.x,
            transform.eulerAngles.y, particles.transform.eulerAngles.z), releaseOnInit ? transform : origin);
        vfx.Play();
        removeOnInit.Add(vfx.gameObject);
        yield return new WaitForSeconds(duration == 0 ? vfx.main.duration : duration);
        Destroy(vfx.gameObject);
    }

    private IEnumerator PlayBuffParticles(Perk perk, bool preview = false)
    {
        yield return new WaitForSeconds(perk.particleSpawnDelay);
        ParticleSystem fx = Instantiate(perk.buffParticles, spellSpawnTransforms[perk.particleSpawnPositionID].
            transform.position, Quaternion.identity, transform);
        fx.Play();
        removeOnInit.Add(fx.gameObject);
        if (perk.temporary)
        {
            if (preview)
            {
                removeOnInit.Add(fx.gameObject);
                yield return new WaitForSeconds(perk.duration);
            }
            else yield return new WaitUntil(() => perk.duration < perkRoundsPassed[perk]);
            Destroy(fx.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!fighting) return;
        if (!other.CompareTag(transform.tag) && !isAttacking && (!battleManager.SomeoneGotHit || battleManager.CurrentMoveDoesMultipleHits()))
        {
            _ = DoOnHit();

            SpellProjectile projectile = other.gameObject.GetComponent<SpellProjectile>();
            if (projectile != null)
                projectile.DoExplosion();
        }
            
    }

    protected virtual float DoOnHit()
    {
        animator.SetTrigger("hit");
        battleManager.SomeoneGotHit = true;
        (float healthDamage, float staminaDamage, float manaDamage, List<Status> status, float _, float _) = battleManager.GetDamage();
        damageNumbers.DamageBy(Mathf.RoundToInt(healthDamage));
        attributes.AddToAttributes(-healthDamage, -staminaDamage, -manaDamage);
        foreach (Status s in status) currentStatus.Add(s);

        return healthDamage;
    }

    protected void PayForAttack()
    {
        attributes.AddToAttributes(-currentMove.healthCost, -currentMove.staminaCost, -currentMove.manaCost);
        battleManager.WriteInDescription();
    }

    protected void InitializePerks()
    {
        perks = new List<Perk>();
        foreach (Perk perk in startPerks)
            perks.Add(perk);
    }

    protected void AddPerk(Perk perk, int durationEnhancer = 0, bool preview = false)
    {
        perks.Add(perk);
        perkRoundsPassed[perk] = -durationEnhancer;

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
                case Status.Burn:
                    burnResistance += statusPerk.resistance;
                    burnDamageRecieved *= statusPerk.damageMultiplier;
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
            attributes.StaminaConsumption = attributePerk.staminaConsumption;
            attributes.ManaConsumption = attributePerk.manaConsumption;
            attributes.probabilityOfBlock *= attributePerk.probOfBlockMultiplier;
        }
        if (perk.temporary)
            StartCoroutine(DeleteTemporaryPerk(perk));

        if(perk.buffParticles != null)
            StartCoroutine(PlayBuffParticles(perk, preview));
    }

    public List<Perk> GetActivePerks()
    {
        return perks;
    }

    public Collider GetCollider(int withID)
    {
        return hitboxes[withID];
    }

    public void PauseInitiativeTimer(bool pause)
    {
        timerOnPause = pause;
    }

    public Move[] GetAllMoves()
    {
        return moves;
    }

    public Transform GetProjectileTarget()
    {
        return projectileTarget;
    }

    private IEnumerator DeleteTemporaryPerk(Perk perk)
    {
        yield return new WaitUntil(() => perk.duration < perkRoundsPassed[perk]);

        StatusPerk statusPerk = perk as StatusPerk;
        AttributePerk attributePerk = perk as AttributePerk;

        if (statusPerk != null)
        {
            switch (statusPerk.status)
            {
                case Status.Bleed:
                    bleedResistance -= statusPerk.resistance;
                    bleedDamageRecieved /= statusPerk.damageMultiplier;
                    break;
                case Status.Poison:
                    poisonResistance -= statusPerk.resistance;
                    poisonDamageRecieved /= statusPerk.damageMultiplier;
                    break;
                case Status.Stun:
                    stunResistance -= statusPerk.resistance;
                    break;
                default:
                    break;
            }
        }
        if (attributePerk != null)
        {
            attributes.AddToRegen(-attributePerk.addHealthRegen, -attributePerk.addStaminaRegen,
                -attributePerk.addManaRegen);
            attributes.MultiplyMaxAttributes(1/attributePerk.maxHealthMultiplier,
                1/attributePerk.maxStaminaMultiplier, 1/attributePerk.maxManaMultiplier);
            attributes.Accuracy = 1/attributePerk.accuracyMultiplier;
            attributes.Initiative = 1/attributePerk.initiativeMultiplier;
            attributes.StaminaConsumption = 1/attributePerk.staminaConsumption;
            attributes.ManaConsumption = 1/attributePerk.manaConsumption;
            attributes.probabilityOfBlock /= attributePerk.probOfBlockMultiplier;
        }

        perks.Remove(perk);
    }
}


//
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

    private float staminaConsumption;
    private float manaConsumption;

    public float StaminaConsumption { get => staminaConsumption; set => staminaConsumption *= value; }
    public float ManaConsumption { get => manaConsumption; set => manaConsumption *= value; }

    public float probabilityOfBlock = 0f;

    public float Health { get; private set; }
    public float Initiative { get => initiative; set => initiative *= value; }
    public float Accuracy { get => accuracy; set => accuracy *= value; }

    public Attributes()
    {
        Health = 100f;
    }

    public void Init(float health, float stamina, float mana, float healthRegen,
        float staminaRegen, float manaRegen, float maxHealth, float maxStamina,
        float maxMana, float initiative, float probabilityOfBlock)
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
        this.probabilityOfBlock = probabilityOfBlock;
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