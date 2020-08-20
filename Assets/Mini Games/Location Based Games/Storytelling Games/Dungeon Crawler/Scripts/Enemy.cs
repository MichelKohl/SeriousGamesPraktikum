using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Fighter
{
    [SerializeField] protected GameObject hitboxes;

    private Transform playerTransform;

    public void SetPlayerPosition(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }

    private List<Move> GetAvailableMoves()
    {
        List<Move> attacks = new List<Move>();
        foreach (Move attack in this.moves)
            if (attack.staminaCost < stamina && attack.manaCost < mana)
                attacks.Add(attack);
        return attacks;
    }

    protected override IEnumerator Attacking()
    {
        
        List<Move> availableAttacks = GetAvailableMoves();
        if(availableAttacks.Count > 0)
        {
            //choose random attack
            Move currentAttack = availableAttacks[Random.Range(0, availableAttacks.Count)];
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            if(!(currentAttack is EnemySpell))
            {
                agent.SetDestination(playerTransform.position);
                yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0);
            }

            animator.SetTrigger(currentAttack.animationName);
            if (currentAttack is EnemyAttack)
            {
                if ((currentAttack as EnemyAttack).hitboxDelay > 0)
                    StartCoroutine(HandleHitbox(currentAttack as EnemyAttack));
            } else hitboxes.SetActive(true);

            yield return new WaitUntil(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentAttack.animationName));
            yield return new WaitUntil(() => !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentAttack.animationName));
            hitboxes.SetActive(false);

            if(!(currentAttack is EnemySpell))
            {
                agent.SetDestination(startPos);
                yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0);

                while (transform.rotation != startRot)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, startRot, 10f);
                    yield return null;
                }
            }
        }
        yield return base.Attacking();
    }

    private IEnumerator HandleHitbox(EnemyAttack attack)
    {
        yield return new WaitForSeconds(attack.hitboxDelay);
        hitboxes.SetActive(true);
    }
}
