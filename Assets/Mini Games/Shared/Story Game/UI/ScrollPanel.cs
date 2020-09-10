using UnityEngine;

public class ScrollPanel : MonoBehaviour
{
    public void Flush()
    {
        foreach (Transform child in transform)
            GameObject.Destroy(child.gameObject);
    }
}
