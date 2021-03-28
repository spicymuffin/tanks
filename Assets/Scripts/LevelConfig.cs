using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelConfig : MonoBehaviour
{
    public List<GameObject> staticSP = new List<GameObject>();
    [HideInInspector]
    public List<Vector3> spawnpoints = new List<Vector3>();
    public CameraController cameraController;
    public Transform trails;
    public Transform rockets;
    public Transform effects;

    public void RandomizeSpawnPoints()
    {
        int index = 0;
        int iterations = staticSP.Count;
        List<GameObject> tempSP = staticSP;
        var random = new System.Random();
        for (int i = 0; i < iterations; i++)
        {
            index = random.Next(tempSP.Count);
            //Debug.Log(index);
            //Debug.Log(tempSP.Count);
            foreach(GameObject obj in tempSP)
            {
                //Debug.Log(obj.transform.position);
            }
            //Debug.Log(tempSP[index].transform.position);
            spawnpoints.Add(tempSP[index].transform.position);
            tempSP.Remove(tempSP[index]);
        }
    }

    public void DeleteTrails()
    {
        foreach (Transform child in trails.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
