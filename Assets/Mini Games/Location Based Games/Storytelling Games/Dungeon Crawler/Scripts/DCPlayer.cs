﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DCPlayer : Fighter
{
    protected bool[] unlocked; // which attacks are unlocked for usage
    // character stats
    [SerializeField] protected int strength = 1;    // -> one and two handed weapons & stamina regen
    [SerializeField] protected int dexterity = 1;   // -> one handed weapons (especially daggers) & initiative
    [SerializeField] protected int intelligence = 1;// -> effectiveness of magic spells & mana regen
    [SerializeField] protected int faith = 1;       // -> effectiveness of faith spells & mana regen
    [SerializeField] protected int luck = 1;        // -> chance of critical hits & effects (stun, bleed, poison etc.) and chance of evading an attack

    [SerializeField] private float xpForNextLvlUp = 100f;
    [SerializeField] private float nextXpMultiplier = 1.5f;

    private string className = "";
    private string playerName = "";
    private float currentXP = 0f;

    // TODO equipment
    private List<Consumable> consumables;
    [SerializeField] private Light spotlight;

    private Fighter currentTarget;

    private bool attackChosen = false;


    public DCPlayer Init(StarterClass starter)
    {
        level =             starter.level;
        className =         starter.name;
        strength =          starter.strength;
        dexterity =         starter.dexterity;
        intelligence =      starter.intelligence;
        faith =             starter.faith;
        luck =              starter.luck;
        moves =           starter.attacks;
        unlocked =          starter.unlocked;

        foreach (Consumable consumable in starter.consumables)
            consumables.Add(consumable);

        return this;
    }

    protected override void Start()
    {
        base.Start();

        consumables = new List<Consumable>();
    }

    protected override void Update()
    {
        base.Update();

        if(currentXP >= xpForNextLvlUp)
        {
            xpForNextLvlUp *= nextXpMultiplier;
            // TODO handle lvl up
        }
    }

    protected override IEnumerator Attacking()
    {
        isAttacking = true;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        yield return new WaitUntil(() => attackChosen);
        yield return new WaitUntil(() => TargetChosen());

        bool isMelee = currentMove is PlayerAttack && !(currentMove is PlayerSpell);

        if (isMelee) 
        {
            walkingForward = true;
            agent.SetDestination(currentTarget.GetAttackPosition().position);
            yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= attackPositionOffset);
            hitboxes[(currentMove as PlayerAttack).hitboxID].enabled = true;
        }
        animator.SetTrigger(name: currentMove.animationName);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentMove.animationName));
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains(currentMove.animationName));

        if (isMelee)
        {
            walkingForward = false;
            agent.updateRotation = false;
            agent.SetDestination(startPos);
            agent.stoppingDistance = 0f;
            hitboxes[(currentMove as PlayerAttack).hitboxID].enabled = false;
            yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0);
            agent.updateRotation = true;
            transform.rotation = startRot;
        }

        yield return base.Attacking();
    }

    public void SetCurrentAttack(Move attack)
    {
        currentMove = attack;
        attackChosen = true;
    }

    private bool TargetChosen()
    {
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.transform.tag);
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        currentTarget = hit.transform.gameObject.GetComponent<Fighter>();
                        return true;
                    }
                }
              
            }
        }
        else
        {
            Debug.Log($"touch count: {Input.touchCount}");
            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log(hit.transform.tag);
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
            if (unlocked[i] && attack.staminaCost <= stamina &&
                attack.manaCost <= mana)
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
        }
        catch (Exception) { }
    }
}
