//Made by Ana-Maria Ilea

using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     This class is used to store all the visual assets for the friendly creatures.
//     This is done because all the creatures will be spawned using spawn points
// </summary>
//------------------------------------------------------------------------------
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
