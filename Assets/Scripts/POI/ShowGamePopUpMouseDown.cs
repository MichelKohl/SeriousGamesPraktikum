using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to show game pop if the player clicks on a poi label
/// </summary>
public class ShowGamePopUpMouseDown : MonoBehaviour
{
    public ShowGamePopUp script;

    /// <summary>
    /// Triggers game pop up if the player clicks on the poi label
    /// </summary>
    private void OnMouseDown()
    {
        if (script.inRange)
        {
            var gamePopUp = GameManager.INSTANCE.GamePopUp;
            var miniGameStarter = gamePopUp.GetComponent<MiniGameStarter>();

            miniGameStarter.miniGame = GameManager.INSTANCE.GetRandomMiniGame();

            // Activate gamePopUp in hirarchy after setting it up
            gamePopUp.SetActive(true);
        }

    }

}
