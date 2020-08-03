using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// This class handles all GUI interactions within the inital profile creation screen.
/// </summary>
public class InitialProfileGUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField nameInput;
    [SerializeField]
    private Button localResidentButton;
    [SerializeField]
    private Button touristButton;
    [SerializeField]
    private GameObject notificationToggle;
    [SerializeField]
    private GameObject vibrationToggle;
    [SerializeField]
    private GameObject sonButton;
    [SerializeField]
    private GameObject daughterButton;
    [SerializeField]
    private Button okButton;
    [SerializeField]
    private Color pressedColor;
    [SerializeField]
    private GameObject dialogPanel;
    [SerializeField]
    private Button dialogPanelButton;
    [SerializeField]
    private GameObject characterDialogPanel;
    [SerializeField]
    private GameObject characterDialogPanelButton;

    private Profiletype type = Profiletype.NONE;
    private bool notificationStatus;
    private bool vibrationsStatus;

    private int selectedCharacter = -1;

    private TouchScreenKeyboard keyboard;

    /// <summary>
    /// This Method will be invoked, if the player presses the "I'm a local Resident" Button. It will set up a profile for
    /// a local resident.
    /// </summary>
    public void localResidentButtonPressed()
    {
        localResidentButton.GetComponent<Image>().color = pressedColor;
        touristButton.GetComponent<Image>().color = Color.white;
        type = Profiletype.LOCALRESIDENT;
    }

    /// <summary>
    /// This Method will be invoked, if the player presses the "I'm a tourist" Button. It will set up a profile or a
    /// tourist.
    /// </summary>
    public void touristButtonPressed()
    {
        touristButton.GetComponent<Image>().color = pressedColor;
        localResidentButton.GetComponent<Image>().color = Color.white;
        type = Profiletype.TOURIST;
    }

    /// <summary>
    /// This Method will be invoked, if the player presses the ok Button to confirm his profile setup.
    /// </summary>
    public void okButtonPressed()
    {
        if (nameInput.text == "Enter your name..." || (type == Profiletype.NONE) || nameInput.text.Length == 0 ||
                nameInput.text.StartsWith(" "))
        {
            dialogPanel.SetActive(true);
            okButton.interactable = false;
        }
        else if (selectedCharacter < 0)
        {
            characterDialogPanel.SetActive(true);
            okButton.interactable = false;
        }
        else
        {
            Debug.Log("Profile creation successfull");

            notificationStatus = notificationToggle.GetComponent<Toggle>().isOn;
            vibrationsStatus = vibrationToggle.GetComponent<Toggle>().isOn;

            Profile profile = new Profile(nameInput.text, type, notificationStatus, vibrationsStatus, selectedCharacter);
            GameManager.INSTANCE.SaveProfile(profile);
            GameManager.INSTANCE.LoadProfile();
            SceneManager.LoadScene("DefaultScreen");
        }

    }

    /// <summary>
    /// This Method will be invoked, if the dialog panel appears and the player presses the ok button.
    /// </summary>
    public void dialogButtonPressed()
    {
        dialogPanel.SetActive(false);
        okButton.interactable = true;
    }

    public void characterDialogButtonPressed()
    {
        characterDialogPanel.SetActive(false);
        okButton.interactable = true;
    }

    public void openKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    public void sonSelected()
    {
        selectedCharacter = 8;
        sonButton.GetComponent<Image>().color = Color.grey;
        daughterButton.GetComponent<Image>().color = Color.white;
    }

    public void daughterSelected()
    {
        selectedCharacter = 0;
        sonButton.GetComponent<Image>().color = Color.white;
        daughterButton.GetComponent<Image>().color = Color.grey;
    }

}
