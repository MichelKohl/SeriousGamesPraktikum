using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private StoryManager manager;
    [SerializeField] private Cam cam;
    [SerializeField] private bool pause;
    [SerializeField] private TextMeshProUGUI descripition;
    [SerializeField] private DecisionsPanel attackOptionsPanel;
    [SerializeField] private AttackOption attackOptionPrefab;

    private DCPlayer player;

    public bool BattleOver { get; private set; }
    private List<Enemy> enemies;
    private Queue<Fighter> attackQueue;
    private bool someoneIsAttacking = false;
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
        if(!someoneIsAttacking && attackQueue.Count > 0)
        {
            Fighter fighter = attackQueue.Dequeue();
            Debug.Log($"battlemanager: {fighter.name} will be attacking.");
            if (!fighter.IsDead())
            {
                someoneIsAttacking = true;
                if (fighter == player)
                {
                    attackOptionsPanel.Flush();
                    foreach (Move attack in player.GetAvailableMoves())
                        Instantiate(attackOptionPrefab, attackOptionsPanel.transform).
                            Init(attack, descripition, player);
                    player.Attack();
                }
                else fighter.Attack();
            }
        }
        if (AllEnemiesDead())
        {
            pause = true;
            BattleOver = true;
            cam.ChangeToFirstPerson();

            foreach (Enemy enemy in enemies)
                enemy.ShowLifebar(false);
            player.ShowLifebar(false);
        }
        // TODO: if player dead -> game over
    }

    public void StartBattle()
    {
        Debug.Log("battle starts...");
        Debug.Log($"enemy count: {enemies.Count}");
        BattleOver = false;
        pause = false;
        foreach(Enemy enemy in enemies)
        {
            enemy.SetPlayerPosition(player.GetAttackPosition());
            enemy.ShowLifebar(true);
        }
        cam.ChangeToThirdPerson();
        player.ShowLifebar(true);
        player.DrawWeapon();
        descripition.text = "";
        attackOptionsPanel.Flush();
    }

    public void AddEnemy(Enemy enemy)
    {
        if (enemies == null) enemies = new List<Enemy>();
        Debug.Log($"adding {enemy.name} to the list of enemies.");
        enemies.Add(enemy);
        Debug.Log($"enemy count: {enemies.Count}");
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
    }

    private bool AllEnemiesDead()
    {
        foreach (Fighter enemy in enemies)
            if (!enemy.IsDead()) return false;
        return true;
    }
}
