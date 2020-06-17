using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mapbox.CheapRulerCs;
using Mapbox.Unity.Location;

/// <summary>
/// This class handles all GUI interactions within the default screen.
/// </summary>
public class GeneralGUI : MonoBehaviour
{
    [SerializeField]
    private Button profileButton;
    [SerializeField]
    private GameObject profileView;
    [SerializeField]
    private Button returnFromProfileViewButton;
    [SerializeField]
    private TextMeshProUGUI profiletypeText;
    [SerializeField]
    private TextMeshProUGUI profilenameText;
    [SerializeField]
    private TextMeshProUGUI coinsText;
    [SerializeField]
    private TextMeshProUGUI totalPlayTimeText;
    [SerializeField]
    private TextMeshProUGUI distanceTravelledText;
    [SerializeField]
    private GameObject notificationsToggle;
    [SerializeField]
    private GameObject vibrationsToggle;
    [SerializeField]
    private Button changeTypeButton;
    [SerializeField]
    private Button changeNameButton;

    private TouchScreenKeyboard keyboard;
    private bool changeName = false;

    bool locationUpdated = false;
    double[] old_loc_array;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (changeName)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                changeName = false;
                GameManager.INSTANCE.profile.setProfileName(keyboard.text);
                setProfileInfo();
            }
        }

        if (!locationUpdated && GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude.x != 0
                && GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude.y != 0)
        {
            Mapbox.Utils.Vector2d old_loc = GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            old_loc_array = old_loc.ToArray();
            locationUpdated = true;
        }

        if (GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.IsLocationUpdated)
        {
            Mapbox.Utils.Vector2d new_loc = GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            double[] new_loc_array = new_loc.ToArray();
            CheapRuler cr = new CheapRuler(old_loc_array[1], CheapRulerUnits.Kilometers);
            GameManager.INSTANCE.profile.setDistanceTraveled(GameManager.INSTANCE.profile.getDistanceTraveled() + cr.Distance(old_loc_array, new_loc_array));

            locationUpdated = false;
        }
    }

    /// <summary>
    /// This method will make the profile view visible and will set up all necessary informations.
    /// </summary>
    public void showProfileView()
    {
        setProfileInfo();

        profileView.SetActive(true);
        profileButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// This method will make the profile view invisible by return to default screen view. It will also save setting changes that were made.
    /// </summary>
    public void backToDefaultView()
    {
        if (notificationsToggle.GetComponent<Toggle>().isOn)
        {
            GameManager.INSTANCE.profile.setNotificationStatus(true);
        }
        else GameManager.INSTANCE.profile.setNotificationStatus(false);

        if (vibrationsToggle.GetComponent<Toggle>().isOn)
        {
            GameManager.INSTANCE.profile.setVibrationStatus(true);
        }
        else GameManager.INSTANCE.profile.setVibrationStatus(false);

        profileView.SetActive(false);
        profileButton.gameObject.SetActive(true);

        GameManager.INSTANCE.SaveProfile(GameManager.INSTANCE.profile);
    }

    /// <summary>
    /// This method sets up all profile informations by requesting them from the saved profile.
    /// </summary>
    private void setProfileInfo()
    {
        profilenameText.SetText(GameManager.INSTANCE.profile.getProfileName());

        if (GameManager.INSTANCE.profile.getProfileType() == Profiletype.LOCALRESIDENT)
        {
            profiletypeText.SetText("Local Resident");
        }
        if (GameManager.INSTANCE.profile.getProfileType() == Profiletype.TOURIST)
        {
            profiletypeText.SetText("Tourist");
        }

        coinsText.SetText(GameManager.INSTANCE.profile.getCoins().ToString());

        if (GameManager.INSTANCE.profile.getNotificationStatus())
        {
            notificationsToggle.GetComponent<Toggle>().isOn = true;
        }
        else notificationsToggle.GetComponent<Toggle>().isOn = false;

        if (GameManager.INSTANCE.profile.getVibrationsStatus())
        {
            vibrationsToggle.GetComponent<Toggle>().isOn = true;
        }
        else vibrationsToggle.GetComponent<Toggle>().isOn = false;

        int hours = (int)(GameManager.INSTANCE.profile.getPlayTime() + (Time.time / 60)) / 60;
        int minutes = (int)(GameManager.INSTANCE.profile.getPlayTime() + (Time.time / 60)) % 60;
        totalPlayTimeText.SetText(hours + " h " + minutes + " m");

        distanceTravelledText.SetText(GameManager.INSTANCE.profile.getDistanceTraveled().ToString("0.000") + " km");
    }

    /// <summary>
    /// This method will be invoked, if the player changes the current profile type.
    /// </summary>
    public void changeProfiletype()
    {
        if (GameManager.INSTANCE.profile.getProfileType() == Profiletype.TOURIST)
        {
            GameManager.INSTANCE.profile.setProfileType(Profiletype.LOCALRESIDENT);
            profiletypeText.SetText("Local Resident");
        }
        else if (GameManager.INSTANCE.profile.getProfileType() == Profiletype.LOCALRESIDENT)
        {
            GameManager.INSTANCE.profile.setProfileType(Profiletype.TOURIST);
            profiletypeText.SetText("Tourist");
        }
    }

    /// <summary>
    /// This method will be invoked, if the player presses the button for changing the profile name.
    /// </summary>
    public void changeProfilename()
    {
        keyboard = TouchScreenKeyboard.Open(GameManager.INSTANCE.profile.getProfileName(), TouchScreenKeyboardType.Default, false);
        changeName = true;
    }
}
