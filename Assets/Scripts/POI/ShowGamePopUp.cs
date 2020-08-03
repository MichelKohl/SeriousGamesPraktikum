using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowGamePopUp : MonoBehaviour
{
    public bool inRange = false;
    public Transform rangeCollider;

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
            var gamePopUp = GameManager.INSTANCE.GamePopUp;
            gamePopUp.SetActive(false);
        }
    }
}
