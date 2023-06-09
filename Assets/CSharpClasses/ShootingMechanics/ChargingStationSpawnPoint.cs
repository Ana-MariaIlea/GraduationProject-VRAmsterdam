using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChargingStationSpawnPoint : NetworkBehaviour
{
    [SerializeField] private GameObject chargingStationPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
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
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            base.OnNetworkDespawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerCoOpStartServer.RemoveListener(Part2Start);
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartServer.RemoveListener(Part2Start);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    private void Part2Start()
    {
        GameObject station = Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
        station.GetComponent<NetworkObject>().Spawn(true);
    }
}
