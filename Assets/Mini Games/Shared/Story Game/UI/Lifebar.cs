using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Lifebar : MonoBehaviour
{
    [SerializeField] private Fighter fighter;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image initiative;

    private void Start()
    {
        nameLabel.text = fighter is Enemy ? fighter.name.Split('(')[0] : (fighter as DCPlayer).GetPlayerName(); 
        UpdateLevel();
    }
    private void Update()
    {
        try
        {
            healthBar.fillAmount = fighter.GetHealthRatio();
            staminaBar.fillAmount = fighter.GetStaminaRatio();
            manaBar.fillAmount = fighter.GetManaRatio();
            initiative.fillAmount = fighter.GetInitiativeRatio();
        }
        catch (Exception) { }
        
    }

    public void UpdateLevel()
    {
        try
        {
            levelLabel.text = fighter.GetLevel().ToString();
        } catch (Exception) { }
    }

    public void SetFighter(Fighter fighter)
    {
        this.fighter = fighter;
    }
}
