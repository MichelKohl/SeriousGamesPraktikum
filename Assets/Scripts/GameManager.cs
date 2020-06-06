using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public Profile _profile = null;

    private void Awake()
    {
        if (_instance == null) {
            _instance = this;
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


    public void SaveProfile(Profile profile) 
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/profileInfo.dat", FileMode.Create);

        bf.Serialize(file, profile);
        file.Close();
    }

    public void LoadProfile() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/profileInfo.dat", FileMode.Open);

        Profile profile = (Profile)bf.Deserialize(file);
        file.Close();

        this._profile = profile;
    }
}
