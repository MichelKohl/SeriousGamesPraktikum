using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles all GUI interactions within the default screen.
/// </summary>
public class GeneralGUI : MonoBehaviour
{
    /// <summary>
    /// All different kinds of GUI Elements of the profile view
    /// </summary>
    [SerializeField]
    private Button profileButton;
    [SerializeField]
    private GameObject profileView;
    [SerializeField]
    private Button returnFromProfileViewButton;
    [SerializeField]
    private TextMeshProUGUI profiletypeText;
    [SerializeField]
    private GameObject ProfileCharacterImageButton;
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
    [SerializeField]
    public TextMeshProUGUI locationTagText;
    [SerializeField]
    private GameObject CharacterSelectionPanel;
    [SerializeField]
    private GameObject treasureMessagePanel;
    [SerializeField]
    private TextMeshProUGUI treasureMessageText;

    /// <summary>
    /// The characters, which can be chosen by pressing the button 
    /// </summary>
    [SerializeField]
    private GameObject daughter01Button;
    [SerializeField]
    private GameObject father01Button;
    [SerializeField]
    private GameObject father02Button;
    [SerializeField]
    private GameObject mother01Button;
    [SerializeField]
    private GameObject mother02Button;
    [SerializeField]
    private GameObject schoolboy01Button;
    [SerializeField]
    private GameObject schoolgirl01Button;
    [SerializeField]
    private GameObject shopkeeper01Button;
    [SerializeField]
    private GameObject son01Button;

    private List<GameObject> characterButtons = new List<GameObject>();

    [SerializeField] private GameObject achievementsView;
    [SerializeField] private AchievementsView achievementScrollView;
    [SerializeField] private GameObject highscoreView;
    [SerializeField] private HighscoreView highscoreScrollView;

    private TouchScreenKeyboard keyboard;
    private bool changeName = false;

    [SerializeField]
    private GameObject insufficientCoinsTextbox;

    // Start is called before the first frame update
    void Start()
    {
        //fill list with all available character buttons
        characterButtons.Add(daughter01Button);
        characterButtons.Add(father01Button);
        characterButtons.Add(father02Button);
        characterButtons.Add(mother01Button);
        characterButtons.Add(mother02Button);
        characterButtons.Add(schoolboy01Button);
        characterButtons.Add(schoolgirl01Button);
        characterButtons.Add(shopkeeper01Button);
        characterButtons.Add(son01Button);

        GameManager.INSTANCE.LoadCharacter();

        InitCharacterSelection();
        profileButton.image.sprite = ProfileCharacterImageButton.GetComponent<Image>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (changeName)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                changeName = false;
                GameManager.INSTANCE.profile.SetProfileName(keyboard.text);
                SetProfileInfo();
            }
        }
    }

    /// <summary>
    /// This method will make the profile view visible and will set up all necessary informations.
    /// </summary>
    public void ShowProfileView()
    {
        InitCharacterSelection();
        SetProfileInfo();

        CharacterSelectionPanel.SetActive(false);
        profileView.SetActive(true);
        profileButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// This method will make the profile view invisible by return to default screen view. It will also save setting changes that were made.
    /// </summary>
    public void BackToDefaultView()
    {
        if (notificationsToggle.GetComponent<Toggle>().isOn)
        {
            GameManager.INSTANCE.profile.SetNotificationStatus(true);
        }
        else GameManager.INSTANCE.profile.SetNotificationStatus(false);

        if (vibrationsToggle.GetComponent<Toggle>().isOn)
        {
            GameManager.INSTANCE.profile.SetVibrationStatus(true);
        }
        else GameManager.INSTANCE.profile.SetVibrationStatus(false);

        CharacterSelectionPanel.SetActive(false);
        profileView.SetActive(false);
        profileButton.gameObject.SetActive(true);

        GameManager.INSTANCE.SaveProfile(GameManager.INSTANCE.profile);
    }

    /// <summary>
    /// This method sets up all profile informations by requesting them from the saved profile.
    /// </summary>
    private void SetProfileInfo()
    {
        profilenameText.SetText(GameManager.INSTANCE.profile.GetProfileName());

        if (GameManager.INSTANCE.profile.GetProfileType() == Profiletype.LOCALRESIDENT)
        {
            profiletypeText.SetText("Local Resident");
        }
        if (GameManager.INSTANCE.profile.GetProfileType() == Profiletype.TOURIST)
        {
            profiletypeText.SetText("Tourist");
        }

        coinsText.SetText(GameManager.INSTANCE.profile.GetCoins().ToString());

        if (GameManager.INSTANCE.profile.GetNotificationStatus())
        {
            notificationsToggle.GetComponent<Toggle>().isOn = true;
        }
        else notificationsToggle.GetComponent<Toggle>().isOn = false;

        if (GameManager.INSTANCE.profile.GetVibrationsStatus())
        {
            vibrationsToggle.GetComponent<Toggle>().isOn = true;
        }
        else vibrationsToggle.GetComponent<Toggle>().isOn = false;

        int hours = (int)(GameManager.INSTANCE.profile.GetPlayTime() + (Time.time / 60)) / 60;
        int minutes = (int)(GameManager.INSTANCE.profile.GetPlayTime() + (Time.time / 60)) % 60;
        totalPlayTimeText.SetText(hours + " h " + minutes + " m");

        distanceTravelledText.SetText(GameManager.INSTANCE.profile.GetDistanceTraveled().ToString("0.000") + " km");
    }

    /// <summary>
    /// This method will be invoked, if the player changes the current profile type.
    /// </summary>
    public void ChangeProfiletype()
    {
        if (GameManager.INSTANCE.profile.GetProfileType() == Profiletype.TOURIST)
        {
            GameManager.INSTANCE.profile.SetProfileType(Profiletype.LOCALRESIDENT);
            profiletypeText.SetText("Local Resident");
        }
        else if (GameManager.INSTANCE.profile.GetProfileType() == Profiletype.LOCALRESIDENT)
        {
            GameManager.INSTANCE.profile.SetProfileType(Profiletype.TOURIST);
            profiletypeText.SetText("Tourist");
        }
    }

    /// <summary>
    /// This method will be invoked, if the player presses the button for changing the profile name.
    /// </summary>
    public void ChangeProfilename()
    {
        keyboard = TouchScreenKeyboard.Open(GameManager.INSTANCE.profile.GetProfileName(), TouchScreenKeyboardType.Default, false);
        changeName = true;
    }

    /// <summary>
    /// This method displays the panel, where the player can see all available and buyable characters.
    /// </summary>
    public void ShowCharacterSelectionPanel()
    {
        if (!CharacterSelectionPanel.activeSelf)
            CharacterSelectionPanel.SetActive(true);
        else CharacterSelectionPanel.SetActive(false);
    }

    /// <summary>
    /// This method will be invoked, if the player selects a different character.
    /// </summary>
    /// <param name="pressedButton">The character button on which the player pressed</param>
    public void SelectCharacter(GameObject pressedButton)
    {
        ProfileCharacterImageButton.GetComponent<Image>().sprite = pressedButton.GetComponent<Image>().sprite;
        CharacterSelectionPanel.SetActive(false);

        //disable all character models
        for (int i = 2; i < GameObject.Find("Player").transform.childCount; i++)
        {
            GameObject.Find("Player").transform.GetChild(i).gameObject.SetActive(false);
        }

        GameManager.INSTANCE.profile.selectedCharacterID = pressedButton.GetComponent<CharacterID>().id;

        //activate the selected character model by getting the id
        GameObject.Find("Player").transform.GetChild(GameManager.INSTANCE.profile.selectedCharacterID + 2).gameObject.SetActive(true);

        //update player button
        profileButton.image.sprite = pressedButton.GetComponent<Image>().sprite;

        GameManager.INSTANCE.SaveProfile(GameManager.INSTANCE.profile);
    }

    /// <summary>
    /// This method initaliazes the characters.
    /// </summary>
    public void InitCharacterSelection()
    {
        foreach (GameObject go in characterButtons)
        {
            if (GameManager.INSTANCE.profile.charactersBought == null)
            {
                GameManager.INSTANCE.profile.charactersBought = new List<int>();
                GameManager.INSTANCE.profile.charactersBought.Add(8); //son is free at the beginning
                GameManager.INSTANCE.profile.charactersBought.Add(0); //daughter is free at the beginning
                GameManager.INSTANCE.profile.selectedCharacterID = 0;
            }

            foreach (int id in GameManager.INSTANCE.profile.charactersBought)
            {
                if (go.GetComponent<CharacterID>().id == id)
                {
                    go.GetComponent<Button>().interactable = true;

                    if (go.transform.childCount > 0)
                    {
                        go.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }

            if (go.GetComponent<CharacterID>().id == GameManager.INSTANCE.profile.selectedCharacterID)
            {
                ProfileCharacterImageButton.GetComponent<Image>().sprite = go.GetComponent<Image>().sprite;
            }
        }
    }

    /// <summary>
    /// This method handles all character buyings.
    /// </summary>
    /// <param name="button">The buy button of a character, which was pressed by the player</param>
    public void Buy(GameObject button)
    {
        int costs = button.transform.GetChild(0).gameObject.GetComponent<Costs>().costs;
        if (GameManager.INSTANCE.profile.GetCoins() - costs < 0)
        {
            StartCoroutine(ShowTextboxForSeconds(insufficientCoinsTextbox, 3f));
            return;
        }
        GameManager.INSTANCE.profile.SetCoins(GameManager.INSTANCE.profile.GetCoins() - costs);
        coinsText.SetText(GameManager.INSTANCE.profile.GetCoins().ToString());
        button.GetComponent<Button>().interactable = true;

        GameManager.INSTANCE.profile.charactersBought.Add(button.GetComponent<CharacterID>().id);

        GameManager.INSTANCE.SaveProfile(GameManager.INSTANCE.profile);

        InitCharacterSelection();
    }

    /// <summary>
    /// This Coroutine displays a textbox panel for a certain amount of seconds.
    /// </summary>
    /// <param name="textbox">the panel that should be displayed</param>
    /// <param name="sec">the amount of seconds, the panel should be displayed</param>
    /// <returns></returns>
    IEnumerator ShowTextboxForSeconds(GameObject textbox, float sec)
    {
        textbox.SetActive(true);
        yield return new WaitForSeconds(sec);
        textbox.SetActive(false);
    }

    /// <summary>
    /// This method shows the achievements view
    /// </summary>
    public void ChangeToAchievementsView()
    {
        achievementsView.gameObject.SetActive(true);
        achievementScrollView.ChangeToAchievementView();
    }

    /// <summary>
    /// This method returns from the achievements view
    /// </summary>
    public void AchievementViewBack()
    {
        achievementsView.gameObject.SetActive(false);
    }

    /// <summary>
    /// This method shows the highscores view
    /// </summary>
    public void ChangeToHighscoresView()
    {
        highscoreView.SetActive(true);
        highscoreScrollView.ChangeToHighscoreView();
    }

    /// <summary>
    /// This method returns from the highscore views
    /// </summary>
    public void HighscoreViewBack()
    {
        highscoreView.gameObject.SetActive(false);
    }

    /// <summary>
    /// Scene switches of different scenes
    /// </summary>
    public void PlaySpaceInvaders()
    {
        SceneManager.LoadScene(2);
    }

    public void PlaySnakes()
    {
        SceneManager.LoadScene(3);
    }

    public void PlayStoryGame()
    {
        SceneManager.LoadScene(4);
    }

    /// <summary>
    /// This method will be invoked, if the player collects a treasure. It shows how many coins the player collected
    /// from the treasure he clicked on.
    /// </summary>
    /// <param name="message">The amount of coins in the treasure</param>
    public void ShowTreasureMessageDialog(string message)
    {
        treasureMessageText.SetText(message);
        StartCoroutine(ShowTextboxForSeconds(treasureMessagePanel, 3));
    }
}
