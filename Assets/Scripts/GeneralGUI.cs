using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneralGUI : MonoBehaviour
{
    public Button _profileButton;
    public GameObject _profileView;
    public Button _returnFromProfileViewButton;
    public TextMeshProUGUI _profiletypeText;
    public TextMeshProUGUI _profilenameText;
    public TextMeshProUGUI _coinsText;
    public Toggle _notificationsToggle;
    public Toggle _vibrationsToggle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showProfileView()
    {
        //update Profileinformations
        updateProfileInfo();

        _profileView.SetActive(true);
        _profileButton.gameObject.SetActive(false);
    }

    public void backToDefaultView()
    {
        _profileView.SetActive(false);
        _profileButton.gameObject.SetActive(true);
    }

    private void updateProfileInfo()
    {
    //    _profilenameText.SetText(GameManager._instance._profile.getProfileName());

        if (GameManager._instance._profile.getProfileType() == Profiletype.LOCALRESIDENT)
        {
            _profiletypeText.SetText("Local Resident");
        }
        if (GameManager._instance._profile.getProfileType() == Profiletype.TOURIST)
        {
            _profiletypeText.SetText("Tourist");
        }

        _coinsText.SetText(GameManager._instance._profile.getCoins().ToString());

        if (GameManager._instance._profile.getNotificationStatus())
        {
            _notificationsToggle.isOn = true;
        }
        else _notificationsToggle.isOn = false;

        if (GameManager._instance._profile.getVibrationsStatus())
        {
            _vibrationsToggle.isOn = true;
        }
        else _vibrationsToggle.isOn = false;
    }
}
