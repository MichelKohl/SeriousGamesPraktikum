using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;

    private Perk perk;
    private PlayerMove move;
    private MainMenu menu;
    private UnlockButton unlockButton;

    public SkillButton Init(MainMenu menu,  UnlockButton unlockButton, Perk perk)
    {
        this.perk = perk;
        this.menu = menu;
        this.unlockButton = unlockButton;
        buttonText.text = perk.name;
        return this;
    }

    public SkillButton Init(MainMenu menu, UnlockButton unlockButton, PlayerMove move)
    {
        this.move = move;
        this.menu = menu;
        this.unlockButton = unlockButton;
        buttonText.text = move.name;
        return this;
    }

    public void OnClick()
    {
        if (perk == null && move == null) return;
        unlockButton.gameObject.SetActive(true);
        if(perk != null)
        {
            unlockButton.SetCurrentPreview(perk);
            menu.PreviewInfo(perk);
        }
        else
        {
            unlockButton.SetCurrentPreview(move);
            menu.PreviewInfo(move);
            menu.PreviewAnimation(move);
        }
    }
}
