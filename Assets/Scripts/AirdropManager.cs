using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirdropManager : MonoBehaviour
{
    public Vector2 height;
    public Vector3 torque;
    public Vector2 waitTime;
    public Transform SpawnPoint;
    public AudioSource airdropSound;

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
        float height = Random.Range(this.height.x, this.height.y);
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
        float height = Random.Range(this.height.x, this.height.y);
        bound activeBound = AirdropSpawnRegions[index];
        //active bound coords
        float[] ACB = activeBound.bounds;
        float x = Random.Range(ACB[0], ACB[1]);
        float y = Random.Range(ACB[2], ACB[3]);
        float z = SpawnPoint.position.y;
        Vector3 landingPosition = new Vector3(x, z, y);

        //GameObject marker = new GameObject();
        //marker.transform.position = landingPosition;
        //marker.name = "landing marker";

        GameObject drop = Instantiate(ChooseAirdrop(airdrops), SpawnPoint.position, Quaternion.identity);
        Rigidbody dropRb = drop.GetComponent<Rigidbody>();
        //Debug.Log("computing forces...");

        Vector3 flightHorVector = landingPosition - SpawnPoint.position;
        Quaternion rotation = Quaternion.LookRotation(flightHorVector, transform.up);

        //gravity is -9.81f
        //physics time

        //throw angle
        float alpha = Mathf.Atan2(4 * height, flightHorVector.magnitude);
        float velocity = Mathf.Sqrt((2 * 9.81f * height) / (Mathf.Sin(alpha) * Mathf.Sin(alpha)));

        float throwx = Mathf.Cos(alpha) * velocity;
        float throwy = Mathf.Sin(alpha) * velocity;

        Vector3 throwVector = new Vector3(throwx, throwy, 0);
        dropRb.AddForce(rotation * Quaternion.Euler(0, -90, 0) * throwVector, ForceMode.VelocityChange);

        Vector3 rndTorque = new Vector3(Random.Range(-torque.x, torque.x), Random.Range(-torque.y, torque.y), Random.Range(-torque.z, torque.z));
        dropRb.AddTorque(rndTorque);
        airdropSound.Play();

        //Debug.Log("launching...");
    }

    IEnumerator WaitToSpawn()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(waitTime.x, waitTime.y));
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
