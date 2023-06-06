using Meta.WitAi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelpersManager : MonoBehaviour
{
    public GameObject eventSystemObj;
    public void EnableEventSystem(bool isEnabled)
    {
        eventSystemObj.SetActive(isEnabled);
    }
}
