using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChargingStationSpawnPoint : NetworkBehaviour
{
    [SerializeField] private GameObject chargingStationPrefab;

    private void Start()
    {
        //GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
        
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.part2PlayerCoOpStartServer.AddListener(Part2Start);
            PlayerStateManager.Singleton.part2PlayerVsPlayerStartServer.AddListener(Part2Start);
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }
    }

    private void Part2Start()
    {
        GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
        station.GetComponent<NetworkObject>().Spawn(true);
    }
}
