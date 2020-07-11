using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowGamePopUp : MonoBehaviour
{
    private bool inRange = false;
    public Transform rangeCollider;
    private void OnMouseDown() {
        if (inRange)
        {
            var gamePopUp = GameManager.INSTANCE.GamePopUp;
            var miniGameStarter = gamePopUp.GetComponent<MiniGameStarter>();

            miniGameStarter.miniGame = GameManager.INSTANCE.GetRandomMiniGame();

            // Activate gamePopUp in hirarchy after setting it up
            gamePopUp.SetActive(true);
        }   
        
    }

    /// <summary>
    /// Sets a boolean if the player enters the range/collider of the POI
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inRange = true;
        }
    }

    /// <summary>
    /// Sets a boolean if the player exits the range/collider of the POI
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inRange = false;
        }
    }
}
