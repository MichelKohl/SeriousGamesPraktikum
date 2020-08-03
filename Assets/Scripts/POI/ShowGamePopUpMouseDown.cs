using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGamePopUpMouseDown : MonoBehaviour
{
    public ShowGamePopUp script;
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
