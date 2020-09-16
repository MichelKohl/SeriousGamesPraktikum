using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static readonly int maxSkillLevel = 20;
    [SerializeField] private StoryManager manager;
    [SerializeField] private Cam cam;
    [SerializeField] private bool pause;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private ScrollPanel attackOptionsPanel;
    [SerializeField] private AttackOption attackOptionPrefab;
    [SerializeField] private ParticleSystem[] particlesOnHit;
    [SerializeField] private GameObject mainMenuButton;

    private DCPlayer player;

    public bool BattleOver { get; private set; }
    private List<Enemy> enemies;
    private Queue<Fighter> attackQueue;
    private bool someoneIsAttacking = false;
    private Fighter currentAttacker;
    private int numberOfDeadEnemies = 0;

    public bool SomeoneGotHit { get; set; } = false;
    public Move CurrentMove { get; set; }
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
        }
        else
        {
            player.IsFighting = true;
            foreach (Enemy enemy in enemies)
                enemy.IsFighting = true;
        }

        if (AllEnemiesDead() && !someoneIsAttacking)
        {
            cam.ChangeToFirstPerson();
            pause = true;
        }
        if (AllEnemiesDead() && !someoneIsAttacking && cam.PositionSet)
        {
            HandleBattleOver();
            pause = true;
        }
        if (player != null && player.IsDead())
        {
            StartCoroutine(DoDeathCam(1.5f));
        }

        if(!someoneIsAttacking && attackQueue.Count > 0 && !BattleOver)
        {
            currentAttacker = attackQueue.Dequeue();
            if (!currentAttacker.IsDead())
            {
                SomeoneGotHit = false;
                someoneIsAttacking = true;

                foreach (Enemy enemy in enemies)
                    enemy.PauseInitiativeTimer(true);
                player.PauseInitiativeTimer(true);

                if (currentAttacker == player)
                {
                    attackOptionsPanel.Flush();
                    foreach (PlayerMove attack in player.GetAvailableMoves())
                        Instantiate(attackOptionPrefab, attackOptionsPanel.transform).
                            Init(attack, description, player);
                    player.Attack();
                }
                else currentAttacker.Attack();
               
            }
        }
    }

    public void StartBattle()
    {
        BattleOver = false;
        pause = false;
        enemies.RemoveAll(enemy => enemy == null);
        foreach(Enemy enemy in enemies)
        {
            enemy.SetPlayerPosition(player);
            enemy.ShowBattleUI(true);
            if (enemy.name.Contains("Evil Wizard"))
                enemy.GetComponent<Animator>().SetTrigger("start battle");
        }
        cam.ChangeToThirdPerson();
        player.ResetFighterValues();
        player.ShowBattleUI(true);
        player.DrawWeapon();
        description.text = "";
        attackOptionsPanel.Flush();
        SomeoneGotHit = false;
        numberOfDeadEnemies = 0;
        mainMenuButton.SetActive(false);
    }

    public void AddEnemy(Enemy enemy)
    {
        if (enemies == null) enemies = new List<Enemy>();
        enemies.Add(enemy);
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
        description.text = "";
    }

    public void SendEnemyDeadSignal(Fighter fighter)
    {
        Enemy enemy = fighter as Enemy;
        if (enemy == null) return;
        numberOfDeadEnemies++;
        manager.DestroyOnSituationChange(fighter.gameObject);
        player.GiveXP(enemy.GetXP());
    }

    public bool CurrentMoveDoesMultipleHits()
    {
        PlayerAttack pa = CurrentMove as PlayerAttack;
        return pa != null && pa.multipleHits;
    }

    private bool AllEnemiesDead()
    {
        return enemies.Count == numberOfDeadEnemies;
    }

    public (float healthDamage, float staminaDamage, float manaDamage, List<Status> status, float statusProbability, float luck)
        GetDamage()
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
        mainMenuButton.SetActive(true);
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
        description.text = "you dead.";
        HandleBattleOver();
    }

    public bool SomeoneAttacking()
    {
        return someoneIsAttacking;
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }

    public void WriteInDescription()
    {
        description.text = (currentAttacker is DCPlayer ? "You" : currentAttacker.name.Split('(')[0]) + $" used {CurrentMove.name}";
    }
}
