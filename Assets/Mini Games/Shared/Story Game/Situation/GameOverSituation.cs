using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Situation/Game Over")]
[Serializable]
public class GameOverSituation : Situation
{
    public ParticleSystem vfx;
}
