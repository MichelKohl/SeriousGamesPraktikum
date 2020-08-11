using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCPlayer : Fighter
{
    [SerializeField] private float xpForNextLvlUp = 100f;
    [SerializeField] private float nextXpMultiplier = 1.5f;

    private string className;
    private float currentXP = 0f;

    // TODO equipment
    private List<Item> stash;
    private List<Consumable> consumables;
    [SerializeField] private Armor headArmor;
    [SerializeField] private Armor armsArmor;
    [SerializeField] private Armor bodyArmor;
    [SerializeField] private Armor legsArmor;
    [SerializeField] private Armor ring;
    [SerializeField] private Weapon leftWeapon;
    [SerializeField] private Weapon rightWeapon;
    [SerializeField] private Light spotlight;

 
    protected override void Start()
    {
        base.Start();

        stash = new List<Item>();
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
        yield return null;
    }

    public void ActivateSpotlight(bool turnOn)
    {
        //spotlight.gameObject.SetActive(turnOn);
    }
}
