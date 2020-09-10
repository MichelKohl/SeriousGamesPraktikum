using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Item/Consumable")]
[Serializable]
public class Consumable : Item
{
    public GameObject prefab;
    public bool throwable = false;
    private float duration = 0f;
    private int damage = 0;
    private int initiativeModifier = 0;
    private int healthRegenModifier = 0;
    private int staminaModifier = 0;
    private int staminaRegenModifier = 0;
    private int manaModifier = 0;
    private int manaRegenModifier = 0;
    private int accuracyModifier;
    private Status status = Status.None;

    /*
    public IEnumerator ApplyEffect(Fighter target, int strength, int dexterity, int intelligence, int faith, int luck)
    {
        // apply instant effects
        int nrOfStatusApplied = 1 + (luck * (int) luckScaling / 100);
        switch (status)
        {
            case Status.Bleed:
                for (int i = 0; i < nrOfStatusApplied; i++)
                    target.currentStatus.Add(Status.Bleed);
                break;
            case Status.Poison:
                for (int i = 0; i < nrOfStatusApplied; i++)
                    target.currentStatus.Add(Status.Poison);
                break;
            case Status.Stun:
                target.currentStatus.Add(Status.Stun);
                break;
            case Status.HealPoison:
                target.currentStatus.RemoveAll(status => status == Status.Poison);
                break;
        }
        int scalingFactor = 0;
        scalingFactor += strength * (int)strengthScaling;
        scalingFactor += dexterity * (int)dexterityScaling;
        scalingFactor += intelligence * (int)intelligenceScaling;
        scalingFactor += faith * (int)faithScaling;
        scalingFactor += luck * (int)luckScaling;
        scalingFactor /= 50;

        target.IncreaseAccuracyBy(accuracyModifier * scalingFactor);
        target.IncreaseHealthBy(damage * scalingFactor);
        target.IncreaseHealthRegenBy(healthRegenModifier * scalingFactor);
        target.IncreaseInitiativeBy(initiativeModifier * scalingFactor);
        target.IncreaseManaBy(manaModifier * scalingFactor);
        target.IncreaseManaRegenBy(manaRegenModifier * scalingFactor);
        target.IncreaseStaminaBy(staminaModifier * scalingFactor);
        target.IncreaseStaminaRegenBy(staminaRegenModifier * scalingFactor);

        float timer = 0f;
        while(timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        target.DecreaseAccuracyBy(accuracyModifier * scalingFactor);
        target.DecreaseHealthBy(damage * scalingFactor);
        target.DecreaseHealthRegenBy(healthRegenModifier * scalingFactor);
        target.DecreaseInitiativeBy(initiativeModifier * scalingFactor);
        target.DecreaseManaBy(manaModifier * scalingFactor);
        target.DecreaseManaRegenBy(manaRegenModifier * scalingFactor);
        target.DecreaseStaminaBy(staminaModifier * scalingFactor);
        target.DecreaseStaminaRegenBy(staminaRegenModifier * scalingFactor);
    }*/
}
