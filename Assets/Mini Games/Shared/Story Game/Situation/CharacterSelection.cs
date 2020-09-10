using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Situation/Character Selection")]
[Serializable]
public class CharacterSelection : Navigation
{
    public int classID = 0; // index of starter class in storymanager
}
