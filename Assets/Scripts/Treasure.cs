using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Treasure : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 50f;
    public float forceStrength = 10f;
    public int coins = 0;
    private GeneralGUI generalGUI;

    // Start is called before the first frame update
    void Start()
    {
        //Delete 50% of all appearing Treasures
        float randomNumber = Random.Range(0f, 100f);
        if (randomNumber < 50f)
        {
            Destroy(this.gameObject);
        }
        coins = Random.Range(1, 6);
        generalGUI = GameObject.Find("Canvas").GetComponent<GeneralGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void OnMouseDown()
    {
        GameObject ring = this.transform.GetChild(0).gameObject;
        CanClickOnTreasure ccot = ring.GetComponent<CanClickOnTreasure>();
        if (ccot.inRange)
        {
            GameManager.INSTANCE.profile.setCoins(GameManager.INSTANCE.profile.getCoins() + this.coins);
            string message = "+" + this.coins + " Coins";
            generalGUI.ShowTreasureMessageDialog(message);
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Delete all multiple-times appearing treasures
        if (collision.gameObject.tag == "Treasure")
        {
            Destroy(collision.gameObject);
        }
    }
}
