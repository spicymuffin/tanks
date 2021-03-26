using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirdropManager : MonoBehaviour
{
    public float height;
    public Vector3 torque;
    public Vector2 WaitTime;

    public class bound
    {
        public GameObject box;
        public float[] bounds;

        public bound(GameObject _box, float[] _bounds)
        {
            box = _box;
            bounds = _bounds;
        }
    }

    public List<bound> AirdropSpawnRegions = new List<bound>();
    public List<GameObject> regions = new List<GameObject>();
    public List<GameObject> airdrops = new List<GameObject>();
    private bool isWaiting = false;

    public void SpawnAirdrop(GameObject _drop)
    {
        GameObject drop = Instantiate(_drop, new Vector3(0, height, 0), Quaternion.identity);
        Rigidbody dropRb = drop.GetComponent<Rigidbody>();
        Vector3 rndTorque = new Vector3(Random.Range(-torque.x, torque.x), Random.Range(-torque.y, torque.y), Random.Range(-torque.z, torque.z));
        dropRb.AddTorque(rndTorque);
    }

    private void StartUpASR()
    {
        for (int i = 0; i < regions.Count; i++)
        {
            AirdropSpawnRegions.Add(new bound(regions[i], GetBounds(regions[i])));
        }
    }

    private float[] GetBounds(GameObject _cube)
    {
        float[] bounds = new float[4];
        bounds[0] = _cube.transform.position.x + 0.5f * _cube.transform.transform.localScale.x;
        bounds[1] = _cube.transform.position.x - 0.5f * _cube.transform.transform.localScale.x;
        bounds[2] = _cube.transform.position.z + 0.5f * _cube.transform.transform.localScale.z;
        bounds[3] = _cube.transform.position.z - 0.5f * _cube.transform.transform.localScale.z;
        return bounds;
    }

    private void Awake()
    {
        StartUpASR();
    }

    public void SpawnRndAirdrop()
    {
        int index = Random.Range(0, AirdropSpawnRegions.Count);
        bound activeBound = AirdropSpawnRegions[index];
        //active bound coords
        float[] ACB = activeBound.bounds;
        GameObject drop = Instantiate(ChooseAirdrop(airdrops), new Vector3(Random.Range(ACB[0], ACB[1]), height, Random.Range(ACB[2], ACB[3])), Quaternion.identity);
        Rigidbody dropRb = drop.GetComponent<Rigidbody>();
        Vector3 rndTorque = new Vector3(Random.Range(-torque.x, torque.x), Random.Range(-torque.y, torque.y), Random.Range(-torque.z, torque.z));
        dropRb.AddTorque(rndTorque);
    }

    IEnumerator WaitToSpawn()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(WaitTime.x, WaitTime.y));
        SpawnRndAirdrop();
        isWaiting = false;
    }

    private void Update()
    {
        if (!isWaiting) StartCoroutine(WaitToSpawn());
    }

    private GameObject ChooseAirdrop(List<GameObject> _list)
    {
        return _list[Random.Range(0, _list.Count)];
    }
}
