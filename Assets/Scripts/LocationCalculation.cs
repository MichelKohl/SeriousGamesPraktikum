using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Location;
using Mapbox.CheapRulerCs;
using Mapbox.Unity;
using System.Text;

/// <summary>
/// This class handles all calculations, that are bounded to the players location.
/// </summary>
public class LocationCalculation : MonoBehaviour
{   
    /// <summary>
    /// A bool field which holds the information, if the gps location was recently updated or not.
    /// </summary>
    private bool locationUpdated = false;

    /// <summary>
    /// A Vector2d, which holds the second newest lat,long position of the player provided by the locationProvider.
    /// </summary>
    public Mapbox.Utils.Vector2d old_loc;

    /// <summary>
    /// A double array with the length of 2, which holds the second newest lat,long position of the player converted from the Vector2d old_loc.
    /// </summary>
    double[] old_loc_array;

    /// <summary>
    /// The geocoder used for reverse geocoding.
    /// </summary>
    Mapbox.Geocoding.Geocoder geocoder;

    private void Start()
    {
        MapboxConfiguration mc = new MapboxConfiguration();
        Mapbox.Platform.FileSource fs = new Mapbox.Platform.FileSource(mc.GetMapsSkuToken, "pk.eyJ1IjoibWljaGVsa29obCIsImEiOiJjazlrNWFnZ3gwOTFnM2Vuc3ZjMnR6OW41In0.-7gMg1G2xOMe46pw_b7qqA");
        geocoder = new Mapbox.Geocoding.Geocoder(fs);
        
        //Update rate of reverse geocoding should be modified
        //InvokeRepeating("requestCity", 1f, 600f);
    }

    private void Update()
    {
        if (!locationUpdated && GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude.x != 0
               && GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude.y != 0)
        {
            old_loc = GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            old_loc_array = old_loc.ToArray();
            locationUpdated = true;
        }

        if (locationUpdated && GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.IsLocationUpdated)
        {
            Mapbox.Utils.Vector2d new_loc = GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            double[] new_loc_array = new_loc.ToArray();
            CheapRuler cr = new CheapRuler(old_loc_array[1], CheapRulerUnits.Kilometers);
            GameManager.INSTANCE.profile.setDistanceTraveled(GameManager.INSTANCE.profile.getDistanceTraveled() + cr.Distance(old_loc_array, new_loc_array));

            locationUpdated = false;
        }
    }

    /// <summary>
    /// This method executes a reverse geocoding by the second newest location od the player. After getting the result, a callback method
    /// will be called together with the data s.
    /// </summary>
    private void requestCity()
    {
        Mapbox.Geocoding.ReverseGeocodeResource rgr = new Mapbox.Geocoding.ReverseGeocodeResource(old_loc);
        geocoder.Geocode(rgr, s => requestCityCallback(s));
    }

    /// <summary>
    /// This callback method formats the received data.
    /// </summary>
    /// <param name="data">the data received from the reverse geocoding</param>
    /// <returns>The reverse geocode response of the reverse geocode request</returns>
    private Mapbox.Geocoding.ReverseGeocodeResponse requestCityCallback(Mapbox.Geocoding.ReverseGeocodeResponse data) {
        string address = data.Features[2].PlaceName;

        //Format Address from {City, Region, Country} in {City, Country}
        char[] temp = address.ToCharArray();
        StringBuilder citysb = new StringBuilder();
        int i = 0;
        while (temp[i] != ',') {
            citysb.Append(temp[i]);
            i++;
        }
        string city = citysb.ToString();

        bool firstCommaDetected = false;
        bool secondCommaDetected = false;
        StringBuilder countrysb = new StringBuilder(); 

        i = 0;
        while (!secondCommaDetected) {
            if (temp[i] == ',' && firstCommaDetected)
            {
                secondCommaDetected = true;
                i++;
            }

            else if (temp[i] == ',' && !firstCommaDetected)
            {
                firstCommaDetected = true;
                i++;
            }
            else i++;
        }
        for (int j = i + 1; j < temp.Length; j++) {
            countrysb.Append(temp[j]);
        }
        string country = countrysb.ToString();

        GameObject.Find("Canvas").GetComponent<GeneralGUI>().locationTagText.SetText(city + ", " + country);
        return data;
    }
}
