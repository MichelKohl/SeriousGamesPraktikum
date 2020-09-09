using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

/// <summary>
/// class to draw circle for the visual radius of POI radius
/// </summary>
public class DrawCircle : MonoBehaviour
{
    [Range(0,50)]
    public int segments = 50;
    [Range(0,50)]
    public float xradius = 5;
    [Range(0,50)]
    public float yradius = 5;
    LineRenderer line;
    public float xOffset = -14;
    public float yOffset = 0;

    void Start ()
    {
        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        CreatePoints ();
    }

    /// <summary>
    /// creates the points of the circle
    /// </summary>
    void CreatePoints ()
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius + xOffset;
            y = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius + yOffset;

            line.SetPosition (i,new Vector3(x,y,0) );

            angle += (360f / segments);
        }
    }
}
