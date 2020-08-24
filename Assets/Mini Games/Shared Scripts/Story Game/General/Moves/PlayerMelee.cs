using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Player/Attack")]
[Serializable]
public class PlayerMelee : PlayerAttack
{
    public int hitboxID = 0;
    public ParticleSystem hitParticles;
}
