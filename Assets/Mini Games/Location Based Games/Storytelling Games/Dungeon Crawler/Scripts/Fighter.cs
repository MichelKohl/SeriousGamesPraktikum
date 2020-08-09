using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fighter : MonoBehaviour
{
    // fighter settings
    [SerializeField] private string fighterName;
    [SerializeField] protected int level = 1;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegen = 1f;
    [SerializeField] private float initiative = 7f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegen = 2f;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float manaRegen = 2f;
    // UI
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image initiativeBar;
    // max caps for values during fight (can be increased when fighting)
    private float currentMaxHealth;
    private float currentMaxStamina;
    private float currentMaxMana;
    private float currentInitiative;
    // current figher values
    private float health;
    private float stamina;
    private float mana;
    private float accuracy;
    private float timer;
    // returns whether fighter can attack now (depending on initiative)
    protected bool CanAttack { get {
            timer += Time.deltaTime;
            if (timer >= (10f - initiative)) { timer = 0; return true; }
            else return false; } }
    // returns whether figher is attacking right now
    public bool IsAttacking { get; private set; }
    // 
    private Action<float, int> IncreaseBy = (value, percent) => value *= 1f + (percent / 100);
    private Action<float, int> DecreaseBy = (value, percent) => value *= 1f - (percent / 100);

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ResetFighterValues();
        nameLabel.text = fighterName;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        health = Mathf.Min(currentMaxHealth, health * healthRegen);
        stamina = Mathf.Min(currentMaxStamina, stamina * staminaRegen);
        mana = Mathf.Min(currentMaxMana, mana * manaRegen);

        healthBar.fillAmount = health / currentMaxHealth;
        staminaBar.fillAmount = stamina / currentMaxStamina;
        manaBar.fillAmount = mana / currentMaxMana;
        initiativeBar.fillAmount = timer / (10f - initiative);

        if (CanAttack) { }// StartCoroutine(Attacking()); 
    }

    public void ResetFighterValues()
    {
        health = maxHealth;
        currentMaxHealth = maxHealth;
        stamina = maxStamina;
        currentMaxStamina = maxStamina;
        mana = maxMana;
        currentMaxMana = maxMana;
        currentInitiative = initiative;
        accuracy = 1f;
        IsAttacking = false;
    }

    public void IncreaseMaxHealthBy(int percent, bool persistent = false)
    {
        IncreaseBy(persistent ? maxHealth : currentMaxHealth, percent);
    }

    public void IncreaseHealthBy(int percent)
    {
        IncreaseBy(health, percent);
    }

    public void IncreaseMaxStaminaBy(int percent, bool persistent = false)
    {
        IncreaseBy(persistent ? maxStamina : currentMaxStamina, percent);
    }

    public void IncreaseStaminaBy(int percent)
    {
        IncreaseBy(stamina, percent);
    }

    public void IncreaseMaxManaBy(int percent, bool persistent = false)
    {
        IncreaseBy(persistent ? maxMana : currentMaxMana, percent);
    }

    public void IncreaseManaBy(int percent)
    {
        IncreaseBy(mana, percent);
    }

    public void IncreaseInitiativeBy(int percent, bool persistent = false)
    {
        IncreaseBy(persistent ? initiative : currentInitiative, percent);
    }

    public void IncreaseAccuracyBy(int percent)
    {
        IncreaseBy(accuracy, percent);
    }

    public void DecreaseMaxHealthBy(int percent)
    {
        DecreaseBy(currentMaxHealth, percent);
    }

    public void DecreaseHealthBy(float value)
    {
        health -= value;
    }

    public void DecreaseMaxStaminaBy(int percent)
    {
        DecreaseBy(currentMaxStamina, percent);
    }

    public void DecreaseStaminaBy(float value)
    {
        stamina -= value;
    }

    public void DecreaseMaxManaBy(int percent)
    {
        DecreaseBy(currentMaxMana, percent);
    }

    public void DecreaseManaBy(float value)
    {
        mana -= value;
    }

    public void DecreaseInitiativeBy(int percent)
    {
        DecreaseBy(currentInitiative, percent);
    }

    public void DecreaseAccuracyBy(int percent)
    {
        DecreaseBy(accuracy, percent);
    }

    protected virtual IEnumerator Attacking()
    {
        // wait until no one is attacking...
        // while(othersAttacking) yield return null;

        // start attack
        IsAttacking = true;

        yield return null;

        IsAttacking = false;
        // end attack
    }
}
