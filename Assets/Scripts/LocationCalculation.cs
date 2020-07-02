using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Location;
using Mapbox.CheapRulerCs;
using System.Net.Http;
using System.IO;
using Mapbox.Unity;

/// <summary>
/// This class handles all calculations, that are bounded by the players location.
/// </summary>
public class LocationCalculation : MonoBehaviour
{
    private bool locationUpdated = false;
    double[] old_loc_array;

    HttpClient client = new HttpClient();


    private void Update()
    {
        //    requestCityAsync();
        grofAsync();

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

    private async System.Threading.Tasks.Task requestCityAsync()
    {
        FormUrlEncodedContent requestContent = new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>("types", "place"), new KeyValuePair<string, string>("access_token", "eyJ1IjoiZmxlZGVybWF1c2xvY2hlciIsImEiOiJjazlrNWh4b3owMjZpM2lteHhoaDRvcm1iIn0")});

        HttpResponseMessage response = await client.PostAsync("https://api.mapbox.com/geocoding/v5/mapbox.places/-73.989,40.733.json", requestContent);

        HttpContent responseContent = response.Content;

        using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
        {
            Debug.Log(await reader.ReadToEndAsync());
        }
    }

    private async System.Threading.Tasks.Task grofAsync() {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.mapbox.com/geocoding/v5/mapbox.places/-73.989,40.733.json?types=place&access_token=pk.eyJ1IjoiZmxlZGVybWF1c2xvY2hlciIsImEiOiJjazlrNWh4b3owMjZpM2lteHhoaDRvcm1iIn0.zBKAX3s9ia8a6IgYsVU2EQ"))
            {
                var response = await httpClient.SendAsync(request);
                HttpContent responseContent = response.Content;

                Mapbox.Utils.Vector2d location = GameObject.Find("LocationProvider").GetComponent<LocationProviderFactory>().DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
                Mapbox.Geocoding.ReverseGeocodeResource hallo = new Mapbox.Geocoding.ReverseGeocodeResource(location);
                Mapbox.Geocoding.ReverseGeocodeResponse response2 = new Mapbox.Geocoding.ReverseGeocodeResponse();
                Mapbox.Unity.MapboxConfiguration mc = new MapboxConfiguration();
                Mapbox.Platform.FileSource fs = new Mapbox.Platform.FileSource(mc.GetMapsSkuToken, "pk.eyJ1IjoiZmxlZGVybWF1c2xvY2hlciIsImEiOiJjazlrNWh4b3owMjZpM2lteHhoaDRvcm1iIn0.zBKAX3s9ia8a6IgYsVU2EQ");
                var geocoder = new Mapbox.Geocoding.Geocoder(fs);
                



                using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    // Write the output.
                    Mapbox.Geocoding.GeocodeResponse res = geocoder.Deserialize<Mapbox.Geocoding.ReverseGeocodeResponse>(await reader.ReadToEndAsync());
                    Debug.Log(res.Features[0].PlaceName);
                  
               
                    
                }
            }
        }
    }
}
