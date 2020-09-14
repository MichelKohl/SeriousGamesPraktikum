using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// class to create indicators for point of interests which are in proximity to player but are offscreen
/// </summary>
public class CreatePOIIndicator : MonoBehaviour
{
    private Dictionary<string, double[]> poiList;
    public GameObject foodIndicator;
    public GameObject parkIndicator;
    public GameObject transportIndicator;
    public GameObject artsIndicator;
    public GameObject defaultIndicator;
    public GameObject screenCollider;

    private void FixedUpdate()
    {
        // Calculate the planes from the main camera's view frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        Vector3 point1 = PlanePlaneIntersection(planes[0], planes[2]);
        Vector3 point2 = PlanePlaneIntersection(planes[0], planes[3]);
        Vector3 point3 = PlanePlaneIntersection(planes[1], planes[3]);
        Vector3 point4 = PlanePlaneIntersection(planes[1], planes[2]);
        Vector3[] points = new Vector3[] { point1, point2, point3, point4, point1 };

        // Create colliders at the edge of screen
        poiList = GameManager.poiLocaitonList;
        for (int i = 0; i < 4; i++)
        {
            Vector3 direction = (points[i + 1] - points[i]).normalized;
            BoxCollider edge;
            if (GameObject.Find("Edge" + i) == null)
            {
                edge = new GameObject("Edge" + i).AddComponent<BoxCollider>();
                GameObject.Find("Edge" + i).layer = 9;
                GameObject.Find("Edge" + i).transform.parent = screenCollider.transform;
            }
            else
            {
                edge = GameObject.Find("Edge" + i).GetComponent<BoxCollider>();
            }

            edge.transform.position = (points[i + 1] + points[i]) / 2f;
            edge.transform.LookAt(points[i]);
            edge.size = new Vector3(1f, 50f, Vector3.Distance(points[i], points[i + 1]));
        }

        // create indicators for offscreen pois using collision points of raycasts from pois to edge of screen
        foreach (string poiName in poiList.Keys)
        {
            string[] splitArray = poiName.Split(char.Parse("-"));
            string category = splitArray[0].Trim();
            string name = splitArray[1].Trim();
            if (GameObject.Find(name) && !category.Contains("Treasures"))
            {
                GameObject go = GameObject.Find(name);
                int layerMask = 1 << 9;
                RaycastHit hit;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Vector3 direction = (player.transform.position - go.transform.position).normalized;
                if (Physics.Raycast(go.transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
                {
                    
                    if (!ContainsPoint(points, go.transform.position))
                    {
                        Debug.DrawRay(go.transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
                        name = "ind" + go.name;
                        GameObject instantiatedIndicator;
                        if (GameObject.Find(name) == null)
                        {
                            switch (category)
                            {
                                case "FoodPOI":
                                    instantiatedIndicator = (GameObject)Instantiate(foodIndicator, hit.point, Quaternion.identity);
                                    break;
                                case "ParkPOI":
                                    instantiatedIndicator = (GameObject)Instantiate(parkIndicator, hit.point, Quaternion.identity);
                                    break;
                                case "TransitPOI":
                                    instantiatedIndicator = (GameObject)Instantiate(transportIndicator, hit.point, Quaternion.identity);
                                    break;
                                case "ArtsPOI":
                                    instantiatedIndicator = (GameObject)Instantiate(artsIndicator, hit.point, Quaternion.identity);
                                    break;
                                default:
                                    instantiatedIndicator = (GameObject)Instantiate(defaultIndicator, hit.point, Quaternion.identity);
                                    break;

                            }
                            instantiatedIndicator.name = "ind" + go.name;
                            instantiatedIndicator.transform.parent = screenCollider.transform;
                        }
                        else
                        {
                            instantiatedIndicator = GameObject.Find(name);
                            Vector3 pos = Camera.main.WorldToViewportPoint(new Vector3(hit.point.x, hit.point.y + 10, hit.point.z));
                            pos.x = Mathf.Clamp01(pos.x);
                            pos.y = Mathf.Clamp01(pos.y);
                            instantiatedIndicator.transform.position = Camera.main.ViewportToWorldPoint(pos);
                        }
                    }
                    // Destroy indicator if not needed
                    else
                    {
                        if (GameObject.Find("ind" + name))
                        {
                            Destroy(GameObject.Find("ind" + name));
                        }
                    }
                }
                else
                {
                    if (GameObject.Find("ind" + go.name))
                    {
                        Destroy(GameObject.Find("ind" + go.name));
                    }
                }
            }
            else
            {
                if (GameObject.Find("ind" + name))
                {
                    Destroy(GameObject.Find("ind" + name));
                }
            }

        }
    }

    /// <summary>
    /// Checks if a point lies within a polygon
    /// </summary>
    /// <param name="polyPoints"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static bool ContainsPoint(Vector3[] polyPoints, Vector3 p)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length-1; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            if (((pi.z <= p.z && p.z < pj.z) || (pj.z <= p.z && p.z < pi.z)) &&
                (p.x < (pj.x - pi.x) * (p.z - pi.z) / (pj.z - pi.z) + pi.x))
                inside = !inside;
        }
        return inside;
    }

    /// <summary>
    /// calculates intersection between two planes
    /// based on : https://forum.unity.com/threads/how-to-find-line-of-intersecting-planes.109458/
    /// </summary>
    /// <param name="plane1"></param>
    /// <param name="plane2"></param>
    /// <returns></returns>
    Vector3 PlanePlaneIntersection(Plane plane1, Plane plane2)
    {
        Vector3 intersection = Vector3.zero;
        Vector3 linePoint = Vector3.zero;
        Vector3 lineVec = Vector3.zero;

        //Get the normals of the planes.
        Vector3 plane1Normal = plane1.normal;
        Vector3 plane2Normal = plane2.normal;
        Vector3 plane1Pos = -plane1.normal * plane1.distance;
        Vector3 plane2Pos = -plane2.normal * plane2.distance;

        //We can get the direction of the line of intersection of the two planes by calculating the
        //cross product of the normals of the two planes. Note that this is just a direction and the line
        //is not fixed in space yet.
        lineVec = Vector3.Cross(plane1Normal, plane2Normal);

        //Next is to calculate a point on the line to fix it's position. This is done by finding a vector from
        //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
        //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
        //the cross product of the normal of plane2 and the lineDirection.      
        Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

        float numerator = Vector3.Dot(plane1Normal, ldir);

        //Prevent divide by zero.
        if (Mathf.Abs(numerator) > 0.000001f)
        {

            Vector3 plane1ToPlane2 = plane1Pos - plane2Pos;
            float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / numerator;
            linePoint = plane2Pos + t * ldir;

            float dotNumerator = Vector3.Dot((Vector3.zero - linePoint), Vector3.up);
            float dotDenominator = Vector3.Dot(lineVec, Vector3.up);

            if (dotDenominator != 0.0f)
            {
                float length = dotNumerator / dotDenominator;

                Vector3 vector = lineVec.normalized * length;

                intersection = linePoint + vector;
            }
        }
        return intersection;
    }
}

