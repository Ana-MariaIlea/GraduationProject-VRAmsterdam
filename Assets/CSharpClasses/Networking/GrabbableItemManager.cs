using System.Collections.Generic;
using Unity.Netcode;

public class GrabbableItemManager : NetworkBehaviour
{
    public static GrabbableItemManager Singleton;
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
}
