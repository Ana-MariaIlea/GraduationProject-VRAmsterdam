using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionLaserPointer : MonoBehaviour
{
    public float raycastLength = 100.0f;

    private GameObject _currentObject;
    private int _currentObjId;

    void Start()
    {
        _currentObject = null;
        _currentObjId = 0;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, raycastLength);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            int id = hit.collider.gameObject.GetInstanceID();

            if(_currentObjId != id)
            {
                //_currentObject.GetComponent<Button>().OnPointerEnter

                _currentObjId = id;
                _currentObject = hit.collider.gameObject;

                string tag = _currentObject.gameObject.tag;
                switch (tag)
                {
                    case "VRButton3D":
                        if(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) >= 1.0f)
                            _currentObject.GetComponent<Button>().onClick.Invoke();
                        break;
                }
            }
        }
    }
}
