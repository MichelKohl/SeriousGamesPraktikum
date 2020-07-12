using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class represents a valid profile of the game.
/// </summary>
[Serializable]
public class Profile
{
    /// <summary>
    /// The name of the profile.
    /// </summary>
    private string name;

    /// <summary>
    /// The type of the profile (tourist or local resident).
    /// </summary>
    private Profiletype type = Profiletype.NONE;

    /// <summary>
    /// The setting, whether notifications are allowed or not.
    /// </summary>
    private bool notifications;

    /// <summary>
    /// The setting, whether vibrations are allowed or not.
    /// </summary>
    private bool vibrations;

    /// <summary>
    /// The amount of coins the profile has.
    /// </summary>
    private int coins;

    /// <summary>
    /// The amount of km the player traveled.
    /// </summary>
    private double distanceTraveled;

    /// <summary>
    /// The amount of time the player played the game in minutes.
    /// </summary>
    private float totalPlaytime;

    private HighscoreStorage highscoreStorage;

    private AchievementStorage achievementStorage;

    public Profile(string name, Profiletype type, bool notifications, bool vibrations)
    {
        this.name = name;
        this.type = type;
        this.notifications = notifications;
        this.vibrations = vibrations;
        this.coins = 0;
        this.distanceTraveled = 0;
        this.totalPlaytime = 0;
    }

    public void setProfileName(string name) {
        this.name = name;
    }

    public string getProfileName() {
        return this.name;
    }

    public void setProfileType(Profiletype type) {
        this.type = type;
    }

    public Profiletype getProfileType() {
        return this.type;
    }

    public void setNotificationStatus(bool status) {
        this.notifications = status;
    }

    public bool getNotificationStatus() {
        return this.notifications;
    }

    public void setVibrationStatus(bool status) {
        this.vibrations = status;
    }

    public bool getVibrationsStatus() {
        return this.vibrations;
    }

    public void setCoins(int amount) {
        this.coins = amount;
    }

    public int getCoins() {
        return this.coins;
    }

    public void setDistanceTraveled(double distance) {
        this.distanceTraveled = distance; 
    }

    public double getDistanceTraveled() {
        return this.distanceTraveled;
    }

    public void setPlayTime(float playTime) {
        this.totalPlaytime = playTime;
    }

    public float getPlayTime() {
        return this.totalPlaytime;
    }

    public void SetHighscore(string gameName, int score)
    {
        if (highscoreStorage == null) highscoreStorage = new HighscoreStorage();
        highscoreStorage.highscores[gameName] = score;
    }

    public int GetHighscore(string gameName)
    {
        return highscoreStorage.highscores[gameName];
    }

    public void AddAchievement(string gameName, string achievementName)
    {
        if(achievementStorage == null) achievementStorage = new AchievementStorage();
        achievementStorage.isAchieved[gameName][achievementName] = false;
    }

    public void SetAchievements(string gameName, Dictionary<string, bool> isAchieved)
    {
        if (achievementStorage == null) achievementStorage = new AchievementStorage();
        achievementStorage.isAchieved[gameName] = isAchieved;
           
    }

    public void SetAchievementsInfo(string gameName, Dictionary<string, (TrophyType trophyType, string desription, int reward)> info)
    {
        achievementStorage.info[gameName] = info;
    }

    public void SetAchieved(string gameName, string achievementName)
    {
        if (achievementStorage == null) achievementStorage = new AchievementStorage();
        achievementStorage.isAchieved[gameName][achievementName] = true;
    }

    public Dictionary<string, bool> GetAchieved(string gameName)
    {
        if (achievementStorage.isAchieved.ContainsKey(gameName))
            return achievementStorage.isAchieved[gameName];
        else
            return null;
    }

    public AchievementStorage GetAchievements()
    {
        return achievementStorage;
    }
}

public enum Profiletype {
    NONE = -1,
    LOCALRESIDENT = 0,
    TOURIST = 1
}

[Serializable]
public class HighscoreStorage{
    public Dictionary<string, int> highscores = new Dictionary<string, int>();
}

[Serializable]
public class AchievementStorage
{
    public Dictionary<string, Dictionary<string, bool>> isAchieved = new Dictionary<string, Dictionary<string, bool>>();

    public Dictionary<string, Dictionary<string, (TrophyType trophyType, string description, int reward)>> info =
        new Dictionary<string, Dictionary<string, (TrophyType trophyType, string description, int reward)>>();
}