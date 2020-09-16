using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Situation/Dialogue")]
[Serializable]
public class Dialogue : Situation
{
    public bool dialogueStart = false;
    public DialogueOption[] options;
    public Enemy[] enemies;
    public Vector3[] enemyPosition;
    public Vector3[] enemyRotation;
}
