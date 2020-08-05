using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Location;
using Mapbox.CheapRulerCs;
using UnityEngine.SceneManagement;
using TMPro;

public class CrawlerManager : MonoBehaviour
{
    public double distanceInKm = 5;

    public double distanceTraveled = 0;

    double[] old_loc_array;

    public bool trackLocation = false;

    public bool locationUpdated = false; 

    public Mapbox.Utils.Vector2d old_loc;

    public int nextStoryIndex;


    public List<string> stories = new List<string>()
    {
        @"Sei gegrüßt Champion! 

        Seit Tagen ist unsere Stadt vom Handel abgeschnitten. Unsere Vorräte reichen kaum noch aus, um uns zu versorgen. 

        Finde heraus, warum unsere Karawanen verschwinden. 

        Als Belohnung versprechen wir dir dein Gewicht in Gold aufgewogen!

        Nun Los!",
        @"Im Dorf sind aufgebracht Menschen .... 
        
        Das Crawler Game endet hier. Dank für das Spielen der Beta!!
        ",
        @"Das Banditen Dorft ist leer...
        
        Du suchst nach Hinweisen für Raubzüge und Waren, die der Stadt gehören könnten.
        
        Das Crawler Game endet hier. Dank für das Spielen der Beta!!
        "
    };

    void Awake() {
        DontDestroyOnLoad(this.gameObject);    
    }

    void Update() {
        if (trackLocation) {
            CalculateDistance();
        }

        if (distanceTraveled >= distanceInKm) {
            ShowNewStory();
        }
    }

    public void SetStoryIndex(int i) {
        this.nextStoryIndex = i;

        StartStoryAndWalking();
    }

    void StartStoryAndWalking() {
        // Scene transition
        SceneManager.LoadScene("Scenes/DefaultScreen");

        this.trackLocation = true;
    }

    void ShowNewStory() {
        SceneManager.LoadScene("MiniGames/DungeonCrawler/Gameplay");

        GameObject.Find("Story").GetComponent<TextMeshProUGUI>().text = stories[nextStoryIndex];
        GameObject.Find("Choice1").GetComponent<TextMeshProUGUI>().text = "In die Stadt.";
    }

    void CalculateDistance() {
        if (!locationUpdated && GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude.x != 0
               && GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude.y != 0)
        {
            old_loc = GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            double[] old_loc_array = old_loc.ToArray();
            locationUpdated = true;
        }

        if (GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.IsLocationUpdated)
        {
            Mapbox.Utils.Vector2d new_loc = GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            double[] new_loc_array = new_loc.ToArray();
            CheapRuler cr = new CheapRuler(old_loc_array[1], CheapRulerUnits.Kilometers);
            this.distanceTraveled += cr.Distance(old_loc_array, new_loc_array);

            locationUpdated = false;
        }
    }
}
