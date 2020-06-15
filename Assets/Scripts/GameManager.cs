using System.Collections;
using System.Collections.Generic;
using System;
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

    private void Awake()
    {
        if (INSTANCE == null) {
            INSTANCE = this;
        }
        DontDestroyOnLoad(this);
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
    }

    // Update is called once per frame
    void Update()
    {

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

    private void OnApplicationPause(bool pause)
    {
        if (pause && this.profile != null) {
            this.profile.setPlayTime(this.profile.getPlayTime() + (Time.time / 60));
            SaveProfile(this.profile);
        }
    }

    private void OnApplicationQuit()
    {
        if (this.profile != null) {
            this.profile.setPlayTime(this.profile.getPlayTime() + (Time.time / 60));
            SaveProfile(this.profile);
        }
    }
}
