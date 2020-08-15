using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mapbox.Unity.MeshGeneration.Data;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The singleton variable.
    /// </summary>
    public static GameManager INSTANCE;

    /// <summary>
    /// The profile attached to the game.
    /// </summary>
    public Profile profile;

    /// <summary>
    /// GamePopUp is a reference to the game pop up screen
    /// </summary>
    public GameObject GamePopUp;

    /// <summary>
    /// List of mini games that we have playable
    /// </summary>
    public List<MiniGame> miniGames = new List<MiniGame>();

    public static Dictionary<string, double[]> poiLocaitonList = new Dictionary<string, double[]>();
    public static Dictionary<string, Dictionary<string, object>> poiTypeList = new Dictionary<string, Dictionary<string, object>>();

    private void Awake()
    {
        if (INSTANCE != null && INSTANCE != this)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
            INSTANCE.profile = null;
            DontDestroyOnLoad(gameObject);
        }

        Debug.Log(Application.persistentDataPath);
        if (File.Exists(Application.persistentDataPath + "/profileInfo.dat"))
        {
            LoadProfile();

            //After the profile has been loaded, set up the latest selected character from the loaded profile
            LoadCharacter();
        }
        else SceneManager.LoadScene("InitialProfileScreen");

        LoadMiniGames();

    }

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void LoadCharacter()
    {
        //first disable all character models
        for (int i = 2; i < GameObject.Find("Player").transform.childCount; i++)
        {
            GameObject.Find("Player").transform.GetChild(i).gameObject.SetActive(false);
        }

        //then activate the selected character model by getting the id
        GameObject.Find("Player").transform.GetChild(INSTANCE.profile.selectedCharacterID + 2).gameObject.SetActive(true);
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
    public void LoadProfile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/profileInfo.dat", FileMode.Open);

        Profile profile = (Profile)bf.Deserialize(file);
        file.Close();

        INSTANCE.profile = profile;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && INSTANCE.profile != null)
        {
            INSTANCE.profile.setPlayTime(INSTANCE.profile.getPlayTime() + (Time.time / 60));
            SaveProfile(INSTANCE.profile);
        }
    }

    private void OnApplicationQuit()
    {
        if (INSTANCE.profile != null)
        {
            INSTANCE.profile.setPlayTime(INSTANCE.profile.getPlayTime() + (Time.time / 60));
            SaveProfile(INSTANCE.profile);
        }
    }
}
