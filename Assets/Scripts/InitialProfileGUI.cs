using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// This class handles all GUI interactions within the inital profile creation screen.
/// </summary>
public class InitialProfileGUI : MonoBehaviour
{
    /// <summary>
    /// All kinds of GUI Elements
    /// </summary>
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

    /// <summary>
    /// The profiletype of the profile: local resident or tourist
    /// </summary>
    private Profiletype type = Profiletype.NONE;
    
    /// <summary>
    /// Different status settings with no functionality yet
    /// </summary>
    private bool notificationStatus;
    private bool vibrationsStatus;

    /// <summary>
    /// The id of the selected character, intially set to -1
    /// </summary>
    private int selectedCharacter = -1;

    /// <summary>
    /// The keyboard of the touchscreen
    /// </summary>
    private TouchScreenKeyboard keyboard;

    /// <summary>
    /// This Method will be invoked, if the player presses the "I'm a local Resident" Button. It will set up a profile for
    /// a local resident.
    /// </summary>
    public void LocalResidentButtonPressed()
    {
        localResidentButton.GetComponent<Image>().color = pressedColor;
        touristButton.GetComponent<Image>().color = Color.white;
        type = Profiletype.LOCALRESIDENT;
    }

    /// <summary>
    /// This Method will be invoked, if the player presses the "I'm a tourist" Button. It will set up a profile or a
    /// tourist.
    /// </summary>
    public void TouristButtonPressed()
    {
        touristButton.GetComponent<Image>().color = pressedColor;
        localResidentButton.GetComponent<Image>().color = Color.white;
        type = Profiletype.TOURIST;
    }

    /// <summary>
    /// This Method will be invoked, if the player presses the ok Button to confirm his profile setup.
    /// </summary>
    public void OkButtonPressed()
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
    public void DialogButtonPressed()
    {
        dialogPanel.SetActive(false);
        okButton.interactable = true;
    }

    /// <summary>
    /// If the player clicks on the OK button, but has not selected a character properly, this method will be invoked.
    /// </summary>
    public void CharacterDialogButtonPressed()
    {
        characterDialogPanel.SetActive(false);
        okButton.interactable = true;
    }

    /// <summary>
    /// This method shows the keyboard for typing the name of the player.
    /// </summary>
    public void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    /// <summary>
    /// If the player selects the boy as a character, this method will be invoked.
    /// </summary>
    public void SonSelected()
    {
        selectedCharacter = 8;
        sonButton.GetComponent<Image>().color = Color.grey;
        daughterButton.GetComponent<Image>().color = Color.white;
    }

    /// <summary>
    /// If the player selects the girl as a character, this method will be invoked.
    /// </summary>
    public void DaughterSelected()
    {
        selectedCharacter = 0;
        sonButton.GetComponent<Image>().color = Color.white;
        daughterButton.GetComponent<Image>().color = Color.grey;
    }

}
