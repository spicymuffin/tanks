using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEffects : MonoBehaviour
{
    public TrailEffect[] trails;

    public void StartTrails()
    {
        foreach (TrailEffect trail in trails)
        {
            trail.emit = true;
        }
    }

    public void StopTrails()
    {
        foreach (TrailEffect trail in trails)
        {
            trail.emit = false;
        }
    }

    private void Awake()
    {
        StartTrails();
    }
}
