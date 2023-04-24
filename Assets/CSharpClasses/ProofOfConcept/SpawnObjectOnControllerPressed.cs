using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnControllerPressed : MonoBehaviour
{
    public GameObject testObjectPrefab;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            SpawnObjectAtControllerPosition();
        }
    }

    private void SpawnObjectAtControllerPosition()
    {
        // TODO: TELL SERVER TO SPAWN OBJECT AT LOCATION
        Instantiate(testObjectPrefab, this.transform.position, this.transform.rotation);
    }
}
