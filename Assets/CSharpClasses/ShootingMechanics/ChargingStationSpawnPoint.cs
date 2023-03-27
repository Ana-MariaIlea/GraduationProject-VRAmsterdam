using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingStationSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject chargingStationPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(chargingStationPrefab, transform.position, Quaternion.identity);
    }
}
