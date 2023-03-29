using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class ChargingStationSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject chargingStationPrefab;

    private void Start()
    {
        GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);

    }

    private void Part2Start()
    {
        GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
        station.GetComponent<NetworkObject>().Spawn(true);
    }
}
