using UnityEngine;

public class DecisionsPanel : MonoBehaviour
{
    public void Flush()
    {
        foreach (Transform child in transform)
            GameObject.Destroy(child.gameObject);
    }
}
