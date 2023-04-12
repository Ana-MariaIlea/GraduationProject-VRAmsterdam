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
        //GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
        PlayerCreatureHandler.Singleton.part2StartServer.AddListener(Part2Start);
    }

    private void Part2Start()
    {
        GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
        station.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Part2StartServerRPC()
    {
        GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
        station.GetComponent<NetworkObject>().Spawn(true);
    }
}
