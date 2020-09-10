using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Fighter
{
    [SerializeField] private int XP;
    private DCPlayer player;

    public void SetPlayerPosition(DCPlayer player)
    {
        this.player = player;
    }

    private List<Move> GetAvailableMoves()
    {
        List<Move> attacks = new List<Move>();
        foreach (Move attack in this.moves)
            if (attributes.CanPay(attack.healthCost, attack.staminaCost, attack.manaCost))
                attacks.Add(attack);
        return attacks;
    }

    protected override IEnumerator Attacking()
    {

        isAttacking = true;
        List<Move> availableAttacks = GetAvailableMoves();
        if(availableAttacks.Count > 0)
        {
            //choose random attack
            currentMove = availableAttacks[Random.Range(0, availableAttacks.Count)];
            battleManager.CurrentMove = currentMove;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            EnemyMelee melee = currentMove as EnemyMelee;
            EnemySelfBuff selfBuff = currentMove as EnemySelfBuff;
            EnemySpell spell = currentMove as EnemySpell;
            SpellProjectile projectile = null;

            if(melee)
            {
                agent.SetDestination(player.GetAttackPosition().position);
                agent.stoppingDistance = attackPositionOffset;
                yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= attackPositionOffset);
            }

            animator.SetTrigger(currentMove.animationName);
            if (melee)
            {
                if (melee.hitboxDelay > 0)
                    StartCoroutine(HandleHitbox(melee));
                else hitboxes[melee.hitboxID].enabled = true;
            }
            yield return new WaitUntil(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentMove.animationName));
            PayForAttack();

            if(spell != null)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(player.transform.position - transform.position),
                        Time.deltaTime);
                for (int i = 0; i < spell.projectile.Length; i++)
                {
                    yield return new WaitForSeconds(spell.delay[i]);
                    projectile = Instantiate(spell.projectile[i], spellSpawnTransforms[spell.spawnTransformID].position,
                    transform.rotation, transform);
                    projectile.LockOnTarget(player.GetProjectileTarget());
                }
            }

            if (selfBuff != null)
                perks.Add(selfBuff.buff);

            yield return new WaitUntil(() => !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentMove.animationName));

            if(melee != null)
            {
                agent.SetDestination(startPos);
                agent.stoppingDistance = 0f;
                hitboxes[melee.hitboxID].enabled = false;
                yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0);

                while (transform.rotation != startRot)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, startRot, 10f);
                    yield return null;
                }
            }
            yield return new WaitUntil(() => projectile == null);
        }
        yield return base.Attacking();
    }

    private IEnumerator HandleHitbox(EnemyMelee melee)
    {
        yield return new WaitForSeconds(melee.hitboxDelay);
        hitboxes[melee.hitboxID].enabled = true;
    }

    protected override float DoOnHit()
    {
        _ = base.DoOnHit();

        PlayerMelee currentPlayerMove = battleManager.CurrentMove as PlayerMelee;
        if (currentPlayerMove != null)
            StartCoroutine(PlayHitParticles(battleManager.GetPlayerHitboxTransform(currentPlayerMove.hitboxID).position,
                currentPlayerMove.hitParticles));
        return 0;
    }

    public override (float healthDamage, float staminaDamage, float manaDamage, List<Status> status, float statusProbability, float luck)
        CalculateDamage()
    {
        return currentMove is EnemyAttack && attributes.Accuracy >= Random.Range(0, 1f) ?
            (currentMove as EnemyAttack).GetAttackInfo(level, perks) :
            base.CalculateDamage();
    }

    private IEnumerator PlayHitParticles(Vector3 location, ParticleSystem particles)
    {
        ParticleSystem hitParticles = Instantiate(particles, location, transform.rotation, transform);
        hitParticles.gameObject.SetActive(true);
        hitParticles.Play();
        yield return new WaitForSeconds(hitParticles.main.duration);
        Destroy(hitParticles.gameObject);
    }

    public int GetXP()
    {
        return XP;
    }
}
