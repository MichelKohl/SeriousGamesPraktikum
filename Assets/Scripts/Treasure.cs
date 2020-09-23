using UnityEngine;

/// <summary>
/// This class handles the behaviour of treasures.
/// </summary>
public class Treasure : MonoBehaviour
{
    /// <summary>
    /// The rotation speed of the treasures
    /// </summary>
    [SerializeField]
    private float rotationSpeed = 50f;
    
    /// <summary>
    /// The amount of coins that are inside of the treasure
    /// </summary>
    public int coins = 0;

    /// <summary>
    /// The instance of the generalGUI script
    /// </summary>
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

        //Use a random value between 1 and 5 for the coins inside of the treasure
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
        if (GeneralGUI.profileViewActive) {
            //do nothing
            return;
        }

        GameObject ring = this.transform.GetChild(0).gameObject;
        CanClickOnTreasure ccot = ring.GetComponent<CanClickOnTreasure>();
        if (ccot.inRange)
        {
            GameManager.INSTANCE.profile.SetCoins(GameManager.INSTANCE.profile.GetCoins() + this.coins);
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
