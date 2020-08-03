using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Costs : MonoBehaviour
{
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
