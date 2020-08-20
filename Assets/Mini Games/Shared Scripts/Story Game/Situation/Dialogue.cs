using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Situation/Dialogue")]
[Serializable]
public class Dialogue : Situation
{
    public DialogueOption[] options;
    public Fighter[] talker;
    public string[] dialogue;
    public Enemy[] enemies;
    public string[] enemyDialogue;
    public Vector3[] enemyPosition;
    public Vector3[] enemyRotation;
}
