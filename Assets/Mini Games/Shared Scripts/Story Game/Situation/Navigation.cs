using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Situation/Navigation")]
[Serializable]
public class Navigation : Situation
{
    public Vector3 camPosition;
    public Vector3 camRotation;
    public NextPoint[] decisions;
}
