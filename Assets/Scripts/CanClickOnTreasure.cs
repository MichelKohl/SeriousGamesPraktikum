using UnityEngine;

/// <summary>
/// This class sets a boolean to true, if players location is within the visual radius of the treasure.
/// </summary>
public class CanClickOnTreasure : MonoBehaviour
{
    /// <summary>
    /// The boolean which will be set to true, if the player is within the visual radius of the treasure
    /// </summary>
    public bool inRange = false;

    /// <summary>
    /// Sets a boolean if the player enters the visual radius of the treasure
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
    /// Sets a boolean if the player exits the visual radius of the treasure
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
