using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GrabbableItemManager : NetworkBehaviour
{
    public static GrabbableItemManager Singleton;
    private List<GrabbableItem> grabbableItems;

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
    // Start is called before the first frame update
    void Start()
    {
        int indexID = 0;
        grabbableItems = FindObjectsOfType<GrabbableItem>().ToList();
        foreach (GrabbableItem grabbableItem in grabbableItems)
        {
            grabbableItem.ObjectID = indexID;
            indexID++;
        }
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
}
