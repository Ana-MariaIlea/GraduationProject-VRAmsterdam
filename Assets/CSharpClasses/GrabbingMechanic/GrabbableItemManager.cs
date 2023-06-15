//Made by Ana-Maria Ilea

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Grabbable item manager
//      Handles the items between the server and the client
// </summary>
//------------------------------------------------------------------------------
public class GrabbableItemManager : NetworkBehaviour
{
    public static GrabbableItemManager Singleton { get; private set; }
    private List<GrabbableItem> grabbableItems = new List<GrabbableItem>();

    int indexID = -1;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartServer.AddListener(Part2Start);
                PlayerStateManager.Singleton.part2PlayerCoOpStartServer.AddListener(Part2Start);
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
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartServer.RemoveListener(Part2Start);
                PlayerStateManager.Singleton.part2PlayerCoOpStartServer.RemoveListener(Part2Start);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }
    public void AddGrabbableItem(GrabbableItem item)
    {
        grabbableItems.Add(item);

        item.ObjectID = indexID;
        indexID++;
    }

    public GrabbableItem FindGivenObject(int ID)
    {
        for (int i = 0; i < grabbableItems.Count; i++)
        {
            if (grabbableItems[i].ObjectID == ID)
            {
                return grabbableItems[i];
            }
        }

        return null;
    }

    public void RemoveGivenObject(GrabbableItem item)
    {
        grabbableItems.Remove(item);
    }

    public void Part2Start()
    {
        for (int i = grabbableItems.Count-1; i >= 0; i--)
        {
            GrabbableItem item = grabbableItems[i];
            grabbableItems.Remove(item);
            item.GetComponent<NetworkObject>().Despawn();
            Destroy(item.gameObject);
        }
    }
}
