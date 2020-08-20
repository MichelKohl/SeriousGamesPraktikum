using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Fighter
{
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

        isAttacking = true;
        List<Move> availableAttacks = GetAvailableMoves();
        if(availableAttacks.Count > 0)
        {
            //choose random attack
            currentMove = availableAttacks[Random.Range(0, availableAttacks.Count)];
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            bool isMelee = currentMove is EnemyAttack && !(currentMove is EnemySpell);

            if(isMelee)
            {
                agent.SetDestination(playerTransform.position);
                agent.stoppingDistance = attackPositionOffset;
                yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= attackPositionOffset);
            }

            animator.SetTrigger(currentMove.animationName);
            if (isMelee)
            {
                if ((currentMove as EnemyAttack).hitboxDelay > 0)
                    StartCoroutine(HandleHitbox(currentMove as EnemyAttack));
                else hitboxes[(currentMove as EnemyAttack).hitboxID].enabled = true;
            }
            yield return new WaitUntil(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentMove.animationName));
            yield return new WaitUntil(() => !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentMove.animationName));

            if(isMelee)
            {
                agent.SetDestination(startPos);
                agent.stoppingDistance = 0f;
                hitboxes[(currentMove as EnemyAttack).hitboxID].enabled = false;
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
        hitboxes[attack.hitboxID].enabled = true;
    }
}
