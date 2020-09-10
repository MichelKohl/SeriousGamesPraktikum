using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackOption : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonDescription;

    private DCPlayer player;
    private PlayerMove attack;
    private TextMeshProUGUI description;

    public void Init(PlayerMove attack, TextMeshProUGUI description, DCPlayer player)
    {
        this.player = player;
        this.attack = attack;
        this.description = description;
        buttonDescription.text = attack.name;
    }

    public void OnClick()
    {
        player.SetCurrentAttack(attack);
        if (attack is PlayerMelee || (attack is PlayerSpell && (attack as PlayerSpell).type != SpellType.Areal))
            description.text = $"Use {attack.name} on:";
        else
            description.text = $"You used {attack.name}";
    }
}
