using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DCPlayer : Fighter
{
    private bool[] unlocked; // which attacks are unlocked for usage
    // character stats
    [SerializeField] private int strength = 1;      // -> one and two handed weapons & stamina regen
    [SerializeField] private int dexterity = 1;     // -> one handed weapons (especially daggers) & initiative
    [SerializeField] private int intelligence = 1;  // -> effectiveness of magic spells & mana regen
    [SerializeField] private int faith = 1;         // -> effectiveness of faith spells & mana regen
    [SerializeField] private int luck = 1;          // -> chance of critical hits & effects (stun, bleed, poison etc.) and chance of evading an attack
    [SerializeField] private float damageReduction; // -> reduces incoming damage, while player is blocking

    [SerializeField] private int xpForNextLvlUp = 100;
    [SerializeField] private float nextXpMultiplier = 1.5f;

    [SerializeField] private PerkCollection allPerks;

    private string playerName = "";
    private string className;
    private int currentXP = 0;
    public int skillPoints = 0; 

    private Fighter currentTarget;

    private bool attackChosen = false;

    public DCPlayer Init(StarterClass starter)
    {
        level =             starter.level;
        strength =          starter.strength;
        dexterity =         starter.dexterity;
        intelligence =      starter.intelligence;
        faith =             starter.faith;
        luck =              starter.luck;
        unlocked =          starter.unlocked;
        className =         starter.name;

        return this;
    }

    protected override IEnumerator Attacking()
    {
        isAttacking = true;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        animator.SetBool("blocking", false);

        hurtbox.enabled = false;

        yield return new WaitUntil(() => attackChosen);
        yield return new WaitUntil(() => TargetChosen());

        battleManager.CurrentMove = currentMove;

        try
        {
            for (int i = 0; i < currentMove.particleEffects.Length; i++)
                StartCoroutine(PlayParticles(currentMove.particleDelay[i],
                    currentMove.particleEffects[i], spellSpawnTransforms[currentMove.particleSpawnPositionID[i]],
                    currentMove.particleDuration[i], currentMove.releaseOnInit[i]));
        }
        catch (NullReferenceException) {
            Debug.Log("particles are null");
        }

        PlayerMelee melee =         currentMove as PlayerMelee;
        PlayerSpell spell =         currentMove as PlayerSpell;
        PlayerSelfBuff selfBuff =   currentMove as PlayerSelfBuff;
        SpellProjectile projectile = null;

        if (melee != null) 
        {
            walkingForward = true;
            agent.SetDestination(currentTarget.GetAttackPosition().position);
            agent.stoppingDistance = attackPositionOffset;
            yield return new WaitUntil(() => !agent.pathPending);
            yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= attackPositionOffset);
            hitboxes[melee.hitboxID].enabled = true;
        }

        animator.SetTrigger(name: currentMove.animationName);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentMove.animationName));
        PayForAttack();

        if (selfBuff != null)
        {
            Debug.Log($"{selfBuff.EnhanceDuration(this, selfBuff.buff.duration)}");
            AddPerk(selfBuff.buff, selfBuff.EnhanceDuration(this, selfBuff.buff.duration));
        }

        if (spell != null)
        {
            if(spell.type == SpellType.Areal)
            {
                yield return new WaitForSeconds(spell.delay[0]);
                foreach (Enemy enemy in battleManager.GetEnemies())
                    for (int i = 0; i < spell.projectile.Length; i++)
                    {
                        //yield return new WaitForSeconds(spell.delay[i]);
                        projectile = Instantiate(spell.projectile[i], spellSpawnTransforms[0].position,
                        transform.rotation, transform.parent);
                        projectile.LockOnTarget(enemy.GetProjectileTarget());
                    }
            }
            else
            {
                bool isMultiple = spell.type == SpellType.Multiple;
                if(!isMultiple)
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                     Quaternion.LookRotation(currentTarget.transform.position - transform.position),
                            Time.deltaTime);
                Enemy[] aliveEnemies = battleManager.GetEnemies().FindAll(enemy => !enemy.IsDead()).ToArray();
                for (int i = 0; i < spell.projectile.Length; i++)
                {
                    yield return new WaitForSeconds(spell.delay[i]);
                    projectile = Instantiate(spell.projectile[i], spellSpawnTransforms[spell.spawnTransformID].position,
                    transform.rotation, transform);
                    projectile.LockOnTarget(isMultiple ? aliveEnemies[Mathf.Min(aliveEnemies.Length - 1, i)].GetProjectileTarget() :
                        currentTarget.GetProjectileTarget());
                }
            }
        }
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentMove.animationName));

        if (melee != null)
        {
            walkingForward = false;
            agent.updateRotation = false;
            agent.SetDestination(startPos);
            agent.stoppingDistance = 0f;
            hitboxes[melee.hitboxID].enabled = false;
            yield return new WaitUntil(() => !agent.pathPending);
            yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0);
            agent.updateRotation = true;
            transform.rotation = startRot;
        }

        yield return new WaitUntil(() => projectile == null);
        attackChosen = false;
        bool doBlock = attributes.probabilityOfBlock +
            (currentMove as PlayerMove).probOfBlock > UnityEngine.Random.Range(0, 1f);

        animator.SetBool("blocking", doBlock);

        hurtbox.enabled = true;
        yield return base.Attacking();
    }

    protected override float DoOnHit()
    {
        if (animator.GetBool("blocking"))
        {
            float damage = base.DoOnHit();
            float blockDmg = damage * damageReduction;
            attributes.AddToAttributes(blockDmg, 0, 0);
            damageNumbers.DamageBy(Mathf.RoundToInt(blockDmg), false);
        }
        else return base.DoOnHit();
            
        return 0;
    }

    public void SetCurrentAttack(Move attack)
    {
        currentMove = attack;
        attackChosen = true;
    }

    private bool TargetChosen()
    {
        if (currentMove is PlayerSelfBuff) return true;
        if (currentMove is PlayerSpell)
            if ((currentMove as PlayerSpell).type != SpellType.Single)
                return true;
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        currentTarget = hit.transform.gameObject.GetComponent<Fighter>();
                        return true;
                    }
            }
        }
        else
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.transform.CompareTag("Enemy"))
                        {
                            currentTarget = hit.transform.gameObject.GetComponent<Fighter>();
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool StatsCheckOut(int strength, int dexterity, int intelligence, int faith, int luck)
    {
        return this.strength >= strength && this.dexterity >= dexterity && this.intelligence >= intelligence && this.faith >= faith && this.luck >= luck;
    }

    public List<Move> GetAvailableMoves()
    {
        List<Move> available = new List<Move>();
        for (int i = 0; i < moves.Length; i++)
        {
            Move attack = moves[i];
            if (unlocked[i] && attributes.CanPay(attack.healthCost, attack.staminaCost, attack.manaCost))
                available.Add(attack);
        }
        return available;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void DrawWeapon()
    {
        try
        {
            animator.SetTrigger("draw weapon");
            bool doBlock = attributes.probabilityOfBlock > UnityEngine.Random.Range(0, 1f);
            animator.SetBool("blocking", doBlock);
        }
        catch (Exception) { }
    }

    public void SheathWeapon()
    {
        try
        {
            animator.SetTrigger("sheath weapon");
        }
        catch (Exception) { }
    }

    public override (float healthDamage, float staminaDamage, float manaDamage, List<Status> status, float statusProbability, float luck) CalculateDamage()
    {
        return currentMove is PlayerAttack && attributes.Accuracy >= UnityEngine.Random.Range(0, 1f) ?
            (currentMove as PlayerAttack).GetAttackInfo(this) :
            base.CalculateDamage();
    }

    public override void ResetFighterValues()
    {
        base.ResetFighterValues();
        attributes.Init((float)(strength + dexterity) / 2,
            (float)(intelligence + faith) / 2, dexterity);
    }

    public void GiveXP(int xp)
    {
        currentXP += xp;
        while (currentXP >= xpForNextLvlUp)
        {
            xpForNextLvlUp = Mathf.RoundToInt(xpForNextLvlUp * nextXpMultiplier);
            skillPoints += level;
            level++;
        }
    }

    public void IncreaseStat(Stat stat)
    {
        if (skillPoints <= 0) return;
        skillPoints--;
        switch (stat)
        {
            case Stat.STR:
                strength++;
                return;
            case Stat.DEX:
                dexterity++;
                return;
            case Stat.INT:
                intelligence++;
                return;
            case Stat.FTH:
                faith++;
                return;
            case Stat.LCK:
                luck++;
                return;
            default:
                return;
        }
    }

    public (string, int) GetStat(Stat stat)
    {
        switch (stat)
        {
            case Stat.STR:
                return ("STR", strength);
            case Stat.DEX:
                return ("DEX", dexterity);
            case Stat.INT:
                return ("INT", intelligence);
            case Stat.FTH:
                return ("FTH", faith);
            case Stat.LCK:
                return ("LCK", luck);
            default:
                return ("ERR", -1);
        }
    }

    public void DisableAgent(bool disable)
    {
        agent.enabled = !disable;
    }

    public string GetClass()
    {
        return className;
    }

    public int GetSkillPoints()
    {
        return skillPoints;
    }

    public Move GetMove(int at)
    {
        return moves[at];
    }

    public List<Perk> GetAvailablePerks()
    {
        return allPerks.GetAvailablePerks(level);
    }

    public Perk[] GetAllPerks()
    {
        return allPerks.GetAllPerks();
    }

    public void UnlockMove(int at)
    {
        PlayerMove move = moves[at] as PlayerMove;
        if (skillPoints < move.skillPointCost || unlocked[at]) return;
        unlocked[at] = true;
        skillPoints -= move.skillPointCost;
    }

    public void UnlockPerk(Perk perk)
    {
        if (skillPoints < perk.skillPointCost || perks.Contains(perk)) return;
        skillPoints -= perk.skillPointCost;
        perks.Add(perk);
    }

    public int GetMoveID(PlayerMove move)
    {
        return Array.IndexOf(moves, move);
    }

    public void Preview(PlayerMove move)
    {
        animator.SetTrigger(move.animationName);

        PlayerSpell spell = move as PlayerSpell;
        if (spell != null) StartCoroutine(SpellPreview(spell));

        PlayerSelfBuff buff = move as PlayerSelfBuff;
        if (buff != null) AddPerk(buff.buff, 0, true);
    }

    public IEnumerator SpellPreview(PlayerSpell spell)
    {
        SpellProjectile projectile = null;
        for(int i = 0; i < spell.projectile.Length; i++)
        {
            yield return new WaitForSeconds(spell.delay[i]);
            projectile = Instantiate(spell.projectile[i], spellSpawnTransforms[spell.spawnTransformID].position,
                Quaternion.identity, transform);
            projectile.LockOnTarget(Camera.main.transform);
            projectile.DoExplosion(0.5f);
        }
        yield return new WaitUntil(() => projectile == null);
    }

    public Attributes GetAttributes()
    {
        return attributes;
    }

    public bool[] GetUnlockedAttacks()
    {
        return unlocked;
    }
}

public enum Stat
{
    STR, DEX, INT, FTH, LCK
}
