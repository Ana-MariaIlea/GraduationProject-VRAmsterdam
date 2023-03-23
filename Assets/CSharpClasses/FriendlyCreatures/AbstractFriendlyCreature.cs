using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureType
{
    Fire,
    Water,
    Earth
}
public abstract class AbstractFriendlyCreature : MonoBehaviour
{
    enum CreatureState
    {
        Unfriended,
        Befriended,
        Helping
    }

    private CreatureState State = CreatureState.Unfriended;
    private CreatureType Type;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InitializeCreatureVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case CreatureState.Unfriended:
                UnfriendedBehaviour();
                break;
            case CreatureState.Befriended:
                BefriendedBehaviour();
                break;
            case CreatureState.Helping:
                HelpingBehaviour();
                break;
        }
    }

    public void InitializeCreatureData(CreatureType type)
    {
        Type = type;
    }

    void InitializeCreatureVisuals()
    {
        CreatureAtlas atlas = GetComponent<CreatureAtlas>();
        for (int i = 0; i < atlas.creatureVisualDatas.Count; i++)
        {
            if (atlas.creatureVisualDatas[i].creatureType == Type)
            {
                int randomIndex = Random.Range(0, atlas.creatureVisualDatas[i].Mesh.Count - 1);
                Instantiate(atlas.creatureVisualDatas[i].Mesh[randomIndex], transform.position, Quaternion.identity, gameObject.transform);
                break;
            }
        }
    }

    protected virtual void UnfriendedBehaviour()
    {

    }
    protected virtual void BefriendedBehaviour()
    {

    }
    protected virtual void HelpingBehaviour()
    {

    }
}
