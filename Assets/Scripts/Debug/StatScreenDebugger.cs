using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankStatistics;

public class StatScreenDebugger : MonoBehaviour
{
    List<Client> clients = new List<Client>() {new Client(1, "luigi", 16), new Client(2, "sasha", 69), new Client(3, "alen", 0), new Client(4, "matvei", 6)};
    public void Run()
    {
        StatScreen.instance.PassStats(new List<Stats>(), new Stats(), clients);
        Debug.Log("starting debug...");
    }
}
