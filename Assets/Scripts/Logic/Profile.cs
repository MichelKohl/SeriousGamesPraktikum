using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Profile
{
    private string _name;
    private Profiletype _type = Profiletype.NONE;
    private bool _notifications = true;
    private bool _vibrations = true;
    private int _coins;

    public Profile(string name, Profiletype type, bool notifications, bool vibrations, int coins) {
        this._name = name;
        this._type = type;
        this._notifications = notifications;
        this._vibrations = vibrations;
        this._coins = coins;
    }

    public void setProfileName(string name) {
        this._name = name;
    }

    public string getProfileName() {
        return this._name;
    }

    public void setProfileType(Profiletype type) {
        this._type = type;
    }

    public Profiletype getProfileType() {
        return this._type;
    }

    public void setNotificationStatus(bool status) {
        this._notifications = status;
    }

    public bool getNotificationStatus() {
        return this._notifications;
    }

    public void setVibrationStatus(bool status) {
        this._vibrations = status;
    }

    public bool getVibrationsStatus() {
        return this._vibrations;
    }

    public void setCoins(int amount) {
        this._coins = amount;
    }

    public int getCoins() {
        return this._coins;
    }

}

public enum Profiletype {
    NONE = -1,
    LOCALRESIDENT = 0,
    TOURIST = 1
}
