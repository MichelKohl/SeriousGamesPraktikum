using UnityEngine;
using TMPro;

/// <summary>
/// This class sets up the marketplace texts of the buttons for buying new characters.
/// </summary>
public class Costs : MonoBehaviour
{
    /// <summary>
    /// The costs for buying the new character
    /// </summary>
    [SerializeField]
    public int costs;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("Buy " + costs);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
