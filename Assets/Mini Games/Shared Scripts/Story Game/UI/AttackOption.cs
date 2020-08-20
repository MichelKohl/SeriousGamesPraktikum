using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackOption : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonDescription;

    private DCPlayer player;
    private Move attack;
    private TextMeshProUGUI description;

    public void Init(Move attack, TextMeshProUGUI description, DCPlayer player)
    {
        this.player = player;
        this.attack = attack;
        this.description = description;
        buttonDescription.text = attack.name;
    }

    public void OnClick()
    {
        player.SetCurrentAttack(attack);
        description.text = $"Use {attack.name} on:";
    }
}
