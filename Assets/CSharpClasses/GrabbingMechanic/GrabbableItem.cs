//Made by Ana-Maria Ilea

using Unity.Netcode;
using UnityEngine;

public class GrabbableItem : NetworkBehaviour
{
    [SerializeField] private ItemID itemID;
    private NetworkVariable<int> objectID = new NetworkVariable<int>(-1);

    public int ObjectID
    {
        get
        {
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
            return itemID;
        }
    }
}

//Item ID enum
//Can be replaced by the creature type
public enum ItemID
{
    [HideInInspector]
    None,
    Food,
    Tool,
    Water
}
