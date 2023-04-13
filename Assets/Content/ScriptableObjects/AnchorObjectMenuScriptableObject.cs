using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "AnchorObjectMenu", menuName = "ScriptableObjects/AnchorObjectMenuScriptableObject", order = 1)]
public class AnchorObjectMenuScriptableObject : ScriptableObject
{
    #region Anchor assigned objects limitations
    [Header("NOTE: Give the value of -1 to allow infinite number of objects of that type.")]
    [Space(20)]
    [Header("Boss fight objects")]
    public int bossSpawnPointsMaxNum = 3;
    public int minionsMaxNum = 6;

    [Space(20)]
    [Header("Creature charging stations")]
    public int fireCreatureStationMaxNum = 1;
    public int waterCreatureStationMaxNum = 1;
    public int earthCreatureStationMaxNum = 1;
    [Space(5)]
    [Header("Creature spawn points")]
    public int FireCreatureMaxNum = 2;
    public int waterCreatureMaxNum = 2;
    public int earthCreatureMaxNum = 2;
    [Space(5)]
    [Header("Creature interaction items")]
    public int fireItemMaxNum = 2;
    public int waterItemMaxNum = 2;
    public int earthItemMaxNum = 2;

    [Space(20)]
    [Header("Vegetation spawn points")]
    public int singleTreeMaxNum = -1;
    public int singleBushMaxNum = -1;
    public int singleGroundGrassMaxNum = -1;
    public int vegetationGroupMaxNum = -1;
    public int pillarVegetationMaxNum = -1;

    #endregion

    //public enum AnchorObjectTypes
    //{
    //    BossSpawnPoint = 0,
    //    MinionObject = 1,
    //
    //    FireCreatureStation = 2,
    //    WaterCreatureStation = 3,
    //    EarthCreatureStation = 4,
    //
    //    FireCreature = 5,
    //    WaterCreature = 6,
    //    EarthCreature = 7,
    //
    //    FireItem = 8,
    //    WaterItem = 9,
    //    EarthItem = 10,
    //
    //    SingleTree = 11,
    //    SingleBush = 12,
    //    SingleGroundGrass = 13,
    //    SinglePillarVegetation = 14,
    //    VegetationGroup = 15
    //};
}
