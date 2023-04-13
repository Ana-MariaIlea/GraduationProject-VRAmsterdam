// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Demonstrates loading existing spatial anchors from storage.
/// </summary>
/// <remarks>
/// Loading existing anchors involves two asynchronous methods:
/// 1. Call <see cref="OVRSpatialAnchor.LoadUnboundAnchors"/>
/// 2. For each unbound anchor you wish to localize, invoke <see cref="OVRSpatialAnchor.UnboundAnchor.Localize"/>.
/// 3. Once localized, your callback will receive an <see cref="OVRSpatialAnchor.UnboundAnchor"/>. Instantiate an
/// <see cref="OVRSpatialAnchor"/> component and bind it to the `UnboundAnchor` by calling
/// <see cref="OVRSpatialAnchor.UnboundAnchor.BindTo"/>.
/// </remarks>
public class SpatialAnchorLoader : MonoBehaviour
{
    public TextMeshPro OutputTextPanel;

    [SerializeField]
    OVRSpatialAnchor _anchorPrefab;

    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;//delegate for a function with parameters

    public void LoadAnchorsByUuid()
    {
        // Get number of saved anchor uuids
        if (!PlayerPrefs.HasKey(Anchor.NumUuidsPlayerPref))
            PlayerPrefs.SetInt(Anchor.NumUuidsPlayerPref, 0);

        var playerUuidCount = PlayerPrefs.GetInt("numUuids");
            Log($"Attempting to load {playerUuidCount} saved anchors.");
        if (playerUuidCount == 0)
            return;


        var uuids = new Guid[playerUuidCount];
        //Query found anchors in the Player preferences
        for (int i = 0; i < playerUuidCount; ++i)
        {
            var uuidKey = "uuid" + i;
            var currentUuid = PlayerPrefs.GetString(uuidKey);//get the "anchor.Uuid.ToString()" value
            Log("QueryAnchorByUuid: " + currentUuid);

            uuids[i] = new Guid(currentUuid);//put the anchor in "queue" so that all of them can be loaded later
        }

        //Load all found anchors
        Load(new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = uuids /*list of uuids representing all anchors to load*/
        });
    }

    private void Awake()
    {
        _onLoadAnchor = OnLocalized;
        Debug.Assert( _anchorPrefab = null, $"{this.name}: _anchorPrefab reference to an OVRSpatialAnchor is not defined!");
    }

    private void Load(OVRSpatialAnchor.LoadOptions options) => OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
    {
        if (anchors == null)
        {
            Log("Query failed.");
            return;
        }

        foreach (var anchor in anchors)
        {
            if (anchor.Localized)
            {
                _onLoadAnchor(anchor, true);
            }
            else if (!anchor.Localizing)
            {
                anchor.Localize(_onLoadAnchor);//delegate, OnLocalized function gets called
            }
        }
    });

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        if (!success)
        {
            Log($"{unboundAnchor} Localization failed!");
            return;
        }

        var pose = unboundAnchor.Pose;
        
        var spatialAnchor = Instantiate(_anchorPrefab, pose.position, pose.rotation);
        spatialAnchor.AssignedObjectId = PlayerPrefs.GetInt(unboundAnchor.Uuid.ToString());
        unboundAnchor.BindTo(spatialAnchor);

        //Check if anchor was loaded & exists in persistent storage.
        if (spatialAnchor.TryGetComponent<Anchor>(out var anchor))
            anchor.ShowSaveIcon = true;
        
    }

    private static void Log(string message) => Debug.Log($"[SpatialAnchorsUnity]: {message}");
}
