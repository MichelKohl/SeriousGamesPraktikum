using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WalkingText : MonoBehaviour
{
    private double startDistance;
    private double distanceToWalk;
    [SerializeField] private TextMeshProUGUI text;

    public double Distance { get => GameManager.INSTANCE == null ? double.PositiveInfinity : GameManager.INSTANCE.profile.GetDistanceTraveled() - startDistance; }

    // Start is called before the first frame update
    void Start()
    {
        startDistance = 0;
        distanceToWalk = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.INSTANCE != null)
            text.text = $"You need to walk {(distanceToWalk - Distance) * 1000}m to reach next part of the story.";
    }

    public void SetStart(double startDistance, double distanceToWalk)
    {
        this.startDistance = startDistance;
        this.distanceToWalk = distanceToWalk;
    }
}
