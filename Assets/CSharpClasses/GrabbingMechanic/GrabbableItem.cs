using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbableItem : MonoBehaviour
{
    [SerializeField] private ItemID itemID;

    public ItemID IItemID
    {
        get
        {
            //Some other code
            return itemID;
        }
    }
}

public enum ItemID
{
    [HideInInspector]
    None,
    Food,
    Tool,
    Water
}
