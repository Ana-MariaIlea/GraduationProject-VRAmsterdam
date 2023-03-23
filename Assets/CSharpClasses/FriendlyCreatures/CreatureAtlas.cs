using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAtlas : MonoBehaviour
{

    [System.Serializable]

    public struct CreatureVisualData
    {
        public CreatureType creatureType;
        public List<GameObject> Mesh;
    }

    public List<CreatureVisualData> creatureVisualDatas;
}
