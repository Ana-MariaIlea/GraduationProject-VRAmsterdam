using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteCollidingObjectOnControllerPressed : MonoBehaviour
{
    public Material normalMat;
    public Material collisionMat;

    private GameObject _currentCollider;

    void Start()
    {
        this.GetComponent<MeshRenderer>().material = normalMat;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {
            if (_currentCollider != null)
            {
                Destroy(_currentCollider);
                this.GetComponent<Renderer>().material = normalMat;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        this.GetComponent<Renderer>().material = collisionMat;
        _currentCollider = other.gameObject;
    }
    private void OnTriggerExit(Collider other)
    {
        this.GetComponent<Renderer>().material = normalMat;
        _currentCollider = null;
    }
}
