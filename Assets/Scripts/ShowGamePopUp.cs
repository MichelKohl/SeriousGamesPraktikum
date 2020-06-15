using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowGamePopUp : MonoBehaviour
{
    private void OnMouseDown() {
        var gamePopUp = GameManager.INSTANCE.GamePopUp;
        var miniGameStarter = gamePopUp.GetComponent<MiniGameStarter>();
        
        miniGameStarter.miniGame = GameManager.INSTANCE.GetRandomMiniGame();

        // Activate gamePopUp in hirarchy after setting it up
        gamePopUp.SetActive(true);
    }
}
