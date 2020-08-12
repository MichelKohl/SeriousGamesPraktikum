using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private DCPlayer player;
    [SerializeField] private bool pause;

    public bool BattleOver { get; private set; }
    private List<Fighter> enemies;
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

        if (pause)
        {
            player.fighting = false;
            foreach (Fighter enemy in enemies)
                enemy.fighting = false;
        } else
        {
            player.fighting = true;
            foreach (Fighter enemy in enemies)
                enemy.fighting = true;
        }
        if(!someoneIsAttacking && attackQueue.Count > 0)
        {
            Fighter fighter = attackQueue.Dequeue();
            Debug.Log($"battlemanager: {fighter.GetName()} will be attacking.");
            if (!fighter.IsDead())
            {
                someoneIsAttacking = true;
                fighter.Attack();
            }
        }
        if (AllEnemiesDead())
        {
            pause = true;
            BattleOver = true;
            player.ActivateSpotlight(false);
        }
    }

    public void StartBattle()
    {
        Debug.Log("battle starts...");
        Debug.Log($"enemy count: {enemies.Count}");
        BattleOver = false;
        pause = false;
        player.ActivateSpotlight(true);
        foreach(Fighter enemy in enemies)
        {
            enemy.SetEnemyAttackPosition(player.GetAttackPosition());
        }
    }

    public void AddEnemy(Fighter enemy)
    {
        if (enemies == null) enemies = new List<Fighter>();
        Debug.Log($"adding {enemy.GetName()} to the list of enemies.");
        enemies.Add(enemy);
        Debug.Log($"enemy count: {enemies.Count}");
    }

    public void AddEnemies(Fighter[] enemies)
    {
        foreach (Fighter enemy in enemies)
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
