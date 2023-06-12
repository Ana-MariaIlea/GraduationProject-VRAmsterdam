using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsVisuals : MonoBehaviour
{
    [SerializeField] Color customColor = new Color(1, 0, 0, 0.5f);

    void OnDrawGizmos()
    {
        Gizmos.color = customColor;
        Gizmos.DrawCube(transform.position, new Vector3(1, 0, 1));
    }
}
