using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Location;
using Mapbox.CheapRulerCs;
using System.Net.Http;
using System.IO;
using Mapbox.Unity;
using TMPro;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// This class handles all calculations, that are bounded to the players location.
/// </summary>
public class LocationCalculation : MonoBehaviour
{
    private bool locationUpdated = false;
    double[] old_loc_array;
    public Mapbox.Utils.Vector2d old_loc;

    MapboxConfiguration mcTest;
    Mapbox.Platform.FileSource fsTest;
    Mapbox.Geocoding.Geocoder geocoderTest;

    private void Start()
    {
        mcTest = new MapboxConfiguration();
        fsTest = new Mapbox.Platform.FileSource(mcTest.GetMapsSkuToken, "pk.eyJ1IjoiZmxlZGVybWF1c2xvY2hlciIsImEiOiJjazlrNWh4b3owMjZpM2lteHhoaDRvcm1iIn0.zBKAX3s9ia8a6IgYsVU2EQ");
        geocoderTest = new Mapbox.Geocoding.Geocoder(fsTest);
        
        InvokeRepeating("requestCity", 1f, 10f);
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

        if (GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.IsLocationUpdated)
        {
            Mapbox.Utils.Vector2d new_loc = GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
            double[] new_loc_array = new_loc.ToArray();
            CheapRuler cr = new CheapRuler(old_loc_array[1], CheapRulerUnits.Kilometers);
            GameManager.INSTANCE.profile.setDistanceTraveled(GameManager.INSTANCE.profile.getDistanceTraveled() + cr.Distance(old_loc_array, new_loc_array));

            locationUpdated = false;
        }
    }

    private void requestCity()
    {
        Mapbox.Geocoding.ReverseGeocodeResource rgr = new Mapbox.Geocoding.ReverseGeocodeResource(old_loc);
        geocoderTest.Geocode(rgr, s => MyCallback(s));
    }

    private Mapbox.Geocoding.ReverseGeocodeResponse MyCallback(Mapbox.Geocoding.ReverseGeocodeResponse data) {
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
