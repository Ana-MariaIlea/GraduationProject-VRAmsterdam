using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbableItem : NetworkBehaviour
{
    [SerializeField] private ItemID itemID;
    private NetworkVariable<int> objectID = new NetworkVariable<int>(-1);

    public int ObjectID
    {
        get
        {
            //Some other code
            return objectID.Value;
        }
        set
        {
            objectID.Value = value;
        }
    }
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
