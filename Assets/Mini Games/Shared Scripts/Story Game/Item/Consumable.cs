using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Item/Consumable")]
[Serializable]
public class Consumable : Item
{
    public GameObject prefab;
    private bool throwable = false;
    private float duration = 0f;
    private float damage = 0f;
    private float initiativeModifier = 0f;
    private float healthRegenModifier = 0f;
    private float staminaModifier = 0f;
    private float staminaRegenModifier = 0f;
    private float manaModifier = 0f;
    private float manaRegenModifier = 0f;
    private Status status = Status.None;

    public IEnumerator ApplyEffect(Fighter target, int strength, int dexterity, int intelligence, int faith, int luck)
    {
        //TODO instant effects

        //TODO long term effects

        yield return null;
    }
}
