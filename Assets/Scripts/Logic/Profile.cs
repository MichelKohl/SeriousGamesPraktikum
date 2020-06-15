﻿using System;
using System.Collections;
using System.Collections.Generic;
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

    public Profile(string name, Profiletype type, bool notifications, bool vibrations) {
        this.name = name;
        this.type = type;
        this.notifications = notifications;
        this.vibrations = vibrations;
        this.coins = 0;
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

}

public enum Profiletype {
    NONE = -1,
    LOCALRESIDENT = 0,
    TOURIST = 1
}