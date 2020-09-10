using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Situation/Navigation")]
[Serializable]
public class Navigation : Situation
{
    public NextPoint[] options;
}
