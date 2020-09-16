using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// MiniGameStarter is responsible for starting mini games from
/// a game pop up
/// </summary>
public class MiniGameStarter : MonoBehaviour
{
    /// <summary>
    /// Associated mini game to the current pop up
    /// </summary>
    public MiniGame miniGame { get; set; }

 /*   private void Awake() {

        var gameName = this.transform.Find("GameName");
        var gameThumbnail = transform.Find("GameThumbnail");

        gameName.GetComponent<TextMeshProUGUI>().text = miniGame.name;
        gameThumbnail.GetComponent<Image>().sprite = miniGame.thumbnail;
    } */

    private void Start()
    {
        var gameName = this.transform.Find("GameName");
        var gameThumbnail = transform.Find("GameThumbnail");

        gameName.GetComponent<TextMeshProUGUI>().text = miniGame.name;
        gameThumbnail.GetComponent<Image>().sprite = miniGame.thumbnail;
    }

    /// <summary>
    /// OnPlay is executed when play button is pressed.
    /// Performs a scene transition to the minigame
    /// </summary>
    public void OnPlay() {
        // deactivate this pop up
        gameObject.SetActive(false);

        // scene transtition to game
        SceneManager.LoadScene(miniGame.scene, LoadSceneMode.Single);
    }

    /// <summary>
    /// OnClose is executed when close button is pressed.
    /// Deactivates pop up
    /// </summary>
    public void OnClose() {
        // no scene transition
        // just deactivate pop up
        // gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

}