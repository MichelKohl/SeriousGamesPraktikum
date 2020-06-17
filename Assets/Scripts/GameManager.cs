using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The singleton variable.
    /// </summary>
    public static GameManager INSTANCE;

    /// <summary>
    /// The profile attached to the game.
    /// </summary>
    public Profile profile = null;

    /// <summary>
    /// GamePopUp is a reference to the game pop up screen
    /// </summary>
    public GameObject GamePopUp;

    /// <summary>
    /// List of mini games that we have playable
    /// </summary>
    public List<MiniGame> miniGames = new List<MiniGame>();

    private void Awake()
    {
        if (INSTANCE != null && INSTANCE != this)
        {
            Destroy(gameObject);
        }
        else INSTANCE = this;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        if (File.Exists(Application.persistentDataPath + "/profileInfo.dat"))
        {
            LoadProfile();
        }
        else SceneManager.LoadScene("InitialProfileScreen");

        LoadMiniGames();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// LoadMiniGames populates a list of the available mini games
    /// which is later used for game pop ups and scene transtitions
    /// </summary>
    public void LoadMiniGames() 
    {
        miniGames.Add(
            new MiniGame(
                "SpaceInvaders",
                "Mini Games/Space Invaders/Gameplay",
                Resources.Load<Sprite>("Space Invaders")
            )
        );

        miniGames.Add(
            new MiniGame(
                "Snake",
                "Mini Games/Snake/Gameplay",
                Resources.Load<Sprite>("Snake")
            )
        );
    }

    /// <summary>
    /// GetRandomMiniGame selects a mini game from the 
    /// mini game list at random and returns it
    /// </summary>
    /// <returns>A MiniGame</returns>
    public MiniGame GetRandomMiniGame() 
    {
        if (miniGames.Count < 1) return null;

        var index = Random.Range(0, this.miniGames.Count);
        return miniGames[index];
    }

    /// <summary>
    /// This method saves the profile from the initial creation screen view to a file on the device.
    /// </summary>
    /// <param name="profile"></param>
    public void SaveProfile(Profile profile) 
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/profileInfo.dat", FileMode.Create);

        bf.Serialize(file, profile);
        file.Close();
    }

    /// <summary>
    /// This method loads the profile from a file on the device.
    /// </summary>
    public void LoadProfile() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/profileInfo.dat", FileMode.Open);

        Profile profile = (Profile)bf.Deserialize(file);
        file.Close();

        this.profile = profile;
    }
}
