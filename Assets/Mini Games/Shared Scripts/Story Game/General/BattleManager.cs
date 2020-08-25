using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BattleManager : MonoBehaviour
{
    public static readonly int maxSkillLevel = 20;
    [SerializeField] private StoryManager manager;
    [SerializeField] private Cam cam;
    [SerializeField] private bool pause;
    [SerializeField] private TextMeshProUGUI descripition;
    [SerializeField] private DecisionsPanel attackOptionsPanel;
    [SerializeField] private AttackOption attackOptionPrefab;
    [SerializeField] private ParticleSystem[] particlesOnHit;

    private DCPlayer player;

    public bool BattleOver { get; private set; }
    private List<Enemy> enemies;
    private Queue<Fighter> attackQueue;
    private Move currentMove;
    private bool someoneIsAttacking = false;
    private bool someoneGotHit = false;
    private Fighter currentAttacker;

    public bool SomeoneGotHit { get => someoneGotHit; set => someoneGotHit = value; }
    public Move CurrentMove { get => currentMove; set => currentMove = value; }
    // Start is called before the first frame update
    void Start()
    {
        BattleOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackQueue == null) attackQueue = new Queue<Fighter>();
        if (player == null) player = manager.GetPlayerCharacter();
        if (player == null) return;

        if (pause)
        {
            player.IsFighting = false;
            if (enemies == null) return;
            foreach (Enemy enemy in enemies)
                enemy.IsFighting = false;
        } else
        {
            player.IsFighting = true;
            foreach (Enemy enemy in enemies)
                enemy.IsFighting = true;
        }

        if(!someoneIsAttacking && attackQueue.Count > 0 && !BattleOver)
        {
            currentAttacker = attackQueue.Dequeue();
            //Debug.Log($"battlemanager: {currentAttacker.name} will be attacking.");
            if (!currentAttacker.IsDead())
            {
                someoneIsAttacking = true;
                someoneGotHit = false;

                if (currentAttacker == player)
                {
                    attackOptionsPanel.Flush();
                    foreach (Move attack in player.GetAvailableMoves())
                        Instantiate(attackOptionPrefab, attackOptionsPanel.transform).
                            Init(attack, descripition, player);
                    foreach (Enemy enemy in enemies)
                        enemy.PauseInitiativeTimer(true);
                    player.Attack();
                }
                else
                {
                    player.PauseInitiativeTimer(true);
                    currentAttacker.Attack();
                }
            }
        }

        if (AllEnemiesDead())
        {
            cam.ChangeToFirstPerson();
            HandleBattleOver();
            pause = true;
        }
        if (player != null && player.IsDead())
        {
            StartCoroutine(DoDeathCam(1.5f));
        }
    }

    public void StartBattle()
    {
        //Debug.Log("battle starts...");
        //Debug.Log($"enemy count: {enemies.Count}");
        BattleOver = false;
        pause = false;
        foreach(Enemy enemy in enemies)
        {
            enemy.SetPlayerPosition(player.GetAttackPosition());
            enemy.ShowBattleUI(true);
        }
        cam.ChangeToThirdPerson();
        player.ShowBattleUI(true);
        player.DrawWeapon();
        descripition.text = "";
        attackOptionsPanel.Flush();
        someoneGotHit = false;
    }

    public void AddEnemy(Enemy enemy)
    {
        if (enemies == null) enemies = new List<Enemy>();
        //Debug.Log($"adding {enemy.name} to the list of enemies.");
        enemies.Add(enemy);
        //Debug.Log($"enemy count: {enemies.Count}");
    }

    public void AddEnemies(Enemy[] enemies)
    {
        foreach (Enemy enemy in enemies)
            this.enemies.Add(enemy);
    }

    public void FlushEnemies()
    {
        enemies.RemoveAll(enemy => true);
    }

    public void AddToAttackQueue(Fighter fighter)
    {
        attackQueue.Enqueue(fighter);
    }

    public void SendAttackDone()
    {
        someoneIsAttacking = false;
        player.PauseInitiativeTimer(false);
        foreach (Enemy enemy in enemies)
            enemy.PauseInitiativeTimer(false);
    }

    public bool CurrentMoveDoesMultipleHits()
    {
        PlayerAttack pa = currentMove as PlayerAttack;
        return pa != null && pa.multipleHits;
    }

    private bool AllEnemiesDead()
    {
        foreach (Fighter enemy in enemies)
            if (!enemy.IsDead()) return false;
        return true;
    }

    public (float healthDamage, float staminaDamage, float manaDamage, List<Status> status) GetDamage()
    {
        return currentAttacker.CalculateDamage();
    }

    public Transform GetPlayerHitboxTransform(int id)
    {
        return player.GetCollider(id).transform;
    }

    private void HandleBattleOver()
    {
        BattleOver = true;
        foreach (Enemy enemy in enemies)
        {
            enemy.ShowBattleUI(false);
            //enemy.IsFighting = false;
        }
        player.ShowBattleUI(false);
    }

    private IEnumerator DoDeathCam(float delay)
    {
        yield return new WaitForSeconds(delay);
        cam.ChangeToDeathCam();
        descripition.text = "you dead.";
        HandleBattleOver();
    }
}
