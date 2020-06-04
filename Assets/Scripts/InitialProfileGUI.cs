using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InitialProfileGUI : MonoBehaviour
{
    public TMP_InputField _nameInput;
    public Button _localResidentButton;
    public Button _touristButton;
    public Toggle _notificationToggle;
    public Toggle _vibrationToggle;
    public Button _profileImageButton;
    public Button _okButton;
    public Color _pressedColor;
    public GameObject _dialogPanel;
    public Button _dialogPanelButton;

    private Profiletype _type = Profiletype.NONE;
    private bool _notifications = false;
    private bool _vibration = false;

    /// <summary>
    /// This Method will be invoked, if the player presses the "I'm a local Resident" Button. It will set up a profile for
    /// a local resident.
    /// </summary>
    public void localResidentButtonPressed()
    {
        _localResidentButton.GetComponent<Image>().color = _pressedColor;
        _touristButton.GetComponent<Image>().color = Color.white;
        _type = Profiletype.LOCALRESIDENT;
    }

    /// <summary>
    /// This Method will be invoked, if the player presses the "I'm a tourist" Button. It will set up a profile or a
    /// tourist.
    /// </summary>
    public void touristButtonPressed()
    {
        _touristButton.GetComponent<Image>().color = _pressedColor;
        _localResidentButton.GetComponent<Image>().color = Color.white;
        _type = Profiletype.TOURIST;
    }

    /// <summary>
    /// This Method will be invoked, if the player presses the ok Button to confirm his profile setup.
    /// </summary>
    public void okButtonPressed()
    {
        if (_nameInput.text == "Enter your name..." || (_type == Profiletype.NONE) || _nameInput.text.Length == 0 ||
                _nameInput.text.StartsWith(" "))
        {
            _dialogPanel.SetActive(true);
            _okButton.interactable = false;
        }
        else
        {
            Debug.Log("Profile creation successfull");

            Profile profile = new Profile(_nameInput.text, _type, _notifications, _vibration, 0);
            
        }

    }

    /// <summary>
    /// This Method will be invoked, if the dialog panel appears and the player presses the ok button.
    /// </summary>
    public void dialogButtonPressed()
    {
        _dialogPanel.SetActive(false);
        _okButton.interactable = true;
    }
}
