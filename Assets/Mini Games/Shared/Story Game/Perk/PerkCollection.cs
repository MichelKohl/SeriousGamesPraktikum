using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Perk/Collection")]
[Serializable]
public class PerkCollection : ScriptableObject
{
    public Perk[] perks;

    public List<Perk> GetAvailablePerks(int playerLevel)
    {
        List<Perk> available = new List<Perk>();
        foreach (Perk perk in perks)
            if (perk.levelRequirement <= playerLevel)
                available.Add(perk);
        return available;
    }

    public Perk[] GetAllPerks()
    {
        return perks;
    }
}


