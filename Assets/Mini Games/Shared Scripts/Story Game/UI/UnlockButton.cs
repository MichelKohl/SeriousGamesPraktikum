using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI info;

    private DCPlayer player;
    private Perk currentPerk = null;
    private PlayerMove currentMove = null;

    public void SetPlayer(DCPlayer player)
    {
        this.player = player;
    }

    public void SetCurrentPreview(Perk perk)
    {
        currentMove = null;
        currentPerk = perk;
        info.text = $"Unlock [{perk.skillPointCost}]";
        button.interactable = perk.skillPointCost <= player.GetSkillPoints();
    }

    public void SetCurrentPreview(PlayerMove move)
    {
        currentPerk = null;
        currentMove = move;
        info.text = $"Unlock [{move.skillPointCost}]";
        button.interactable = move.skillPointCost <= player.GetSkillPoints();
    }

    public void Unlock()
    {
        if (currentMove == null && currentPerk == null) return;
        if (currentMove != null)
            player.UnlockMove(player.GetMoveID(currentMove));
        else
            player.UnlockPerk(currentPerk);
    }
}
