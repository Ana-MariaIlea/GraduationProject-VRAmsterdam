using Unity.Netcode;
using UnityEngine;

public class ChargingStationSpawnPoint : NetworkBehaviour
{
    [SerializeField] private GameObject chargingStationPrefab;

    private void Start()
    {
        //GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
        PlayerStateManager.Singleton.part2StartServer.AddListener(Part2Start);
    }

    private void Part2Start()
    {
        GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
        station.GetComponent<NetworkObject>().Spawn(true);
    }
}
