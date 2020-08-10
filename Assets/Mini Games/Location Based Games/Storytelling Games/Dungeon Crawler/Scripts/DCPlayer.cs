using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCPlayer : Fighter
{
    [SerializeField] private float xpForNextLvlUp = 100f;
    [SerializeField] private float nextXpMultiplier = 1.5f;

    private float currentXP = 0f;

    // TODO equipment


    protected override void Start()
    {
        base.Start();
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
}
