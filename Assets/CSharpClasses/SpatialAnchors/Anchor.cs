// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;

/// <summary>
/// Specific functionality for spawned anchors
/// </summary>
[RequireComponent(typeof(OVRSpatialAnchor))]
public class Anchor : MonoBehaviour
{
    public const string NumUuidsPlayerPref = "numUuids";

    [SerializeField, FormerlySerializedAs("canvas_")]
    private Canvas _canvas;

    [SerializeField, FormerlySerializedAs("pivot_")]
    private Transform _pivot;

    [Space(10)]
    [Header("Anchor info:")]
    [SerializeField, FormerlySerializedAs("anchorName_")]
    private TextMeshProUGUI _anchorName;

    [SerializeField, FormerlySerializedAs("assignedObjectId_")]
    private TextMeshProUGUI _assignedObjectIdLabel;

    [SerializeField, FormerlySerializedAs("saveIcon_")]
    private GameObject _saveIcon;

    [SerializeField, FormerlySerializedAs("labelImage_")]
    private Image _labelImage;

    [SerializeField, FormerlySerializedAs("labelBaseColor_")]
    private Color _labelBaseColor;

    [SerializeField, FormerlySerializedAs("labelHighlightColor_")]
    private Color _labelHighlightColor;

    [SerializeField, FormerlySerializedAs("labelSelectedColor_")]
    private Color _labelSelectedColor;


    [Space(10)]
    [Header("Anchor menu:")]
    [SerializeField, FormerlySerializedAs("anchorMenu_")]
    private GameObject _anchorMenu;

    private bool _isAnchorSelected;

    private bool _isHovered;

    [SerializeField, FormerlySerializedAs("uiManager_")]
    private AnchorUIManager _uiManager;

    [SerializeField, FormerlySerializedAs("renderers_")]
    private MeshRenderer[] _renderers;

    private int _anchorMenuIndex = 0;
    private bool _isAnchorMenuSelected = false;

    [SerializeField, FormerlySerializedAs("buttonList_")]
    private List<Button> _buttonList;

    private Button _selectedButton;

    private OVRSpatialAnchor _spatialAnchor;

    private GameObject _icon;

    [Space(10)]
    [Header("Object Menu:")]
    [SerializeField, FormerlySerializedAs("objectMenu_")]
    private GameObject _objectMenu;
    private int _objectMenuIndex = 0;

    [SerializeField, FormerlySerializedAs("anchorSpawnObjects_")]
    [Tooltip("List of objects that can be spawned on an anchor through selecting options in the Object Menu.")]
    private List<GameObject> _availableObjects;

    private GameObject _assignedObject = null;
    private bool _hasObject = false;

    [SerializeField, FormerlySerializedAs("objectMenuButtonList_")]
    private List<Button> _objectMenuButtonList;

    private const int defaultAssignedObjectId = -1;

    #region Monobehaviour Methods

    private void Awake()
    {
        _anchorMenu.SetActive(false);
        _objectMenu.SetActive(false);
        _renderers = GetComponentsInChildren<MeshRenderer>();
        _canvas.worldCamera = Camera.main;
        _selectedButton = _buttonList[0];
        _selectedButton.OnSelect(null);
        _spatialAnchor = GetComponent<OVRSpatialAnchor>();
        _icon = GetComponent<Transform>().FindChildRecursive("Sphere").gameObject;

        AnchorUIManager.OnLoadAnchorObjects += SpawnAnchorObject;
    }

    private IEnumerator Start()
    {
        while (_spatialAnchor && !_spatialAnchor.Created)
        {
            yield return null;
        }

        if (_spatialAnchor)
        {
            _anchorName.text = _spatialAnchor.Uuid.ToString("D");
            UpdateLabelAssignedObjectId();

            if (_spatialAnchor.AssignedObjectId > defaultAssignedObjectId)
                InstantiateObjectOnAnchor(_spatialAnchor.AssignedObjectId);
        }
        else
        {
            // Creation must have failed
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Billboard the boundary
        BillboardPanel(_canvas.transform);

        // Billboard the menu
        BillboardPanel(_pivot);

        HandleMenuNavigation();
        HandleAssignMenuNavigation();

        //Billboard the icon
        BillboardPanel(_icon.transform);
    }

    #endregion // MonoBehaviour Methods

    #region UI Event Listeners Anchor Menu

    /// <summary>
    /// UI callback for the anchor menu's Save button
    /// </summary>
    public void OnSaveLocalButtonPressed()
    {
        if (!_spatialAnchor) return;

        DisableAllUIMenus();

        _spatialAnchor.Save((anchor, success) =>
        {
            if (!success) return;

            // Enables save icon on the anchor menu
            ShowSaveIcon = true;

            // Write uuid of saved anchor to file
            if (!PlayerPrefs.HasKey(NumUuidsPlayerPref))
            {
                //Set generic value for this key
                PlayerPrefs.SetInt(NumUuidsPlayerPref, 0);
            }

            int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);//returns 0
            PlayerPrefs.SetString("uuid" + playerNumUuids, anchor.Uuid.ToString());//"anchor number", actual uuid of this anchor
            PlayerPrefs.SetInt(NumUuidsPlayerPref, ++playerNumUuids);//increase the total number of Uuids that are saved in the Player Preferences

            PlayerPrefs.SetInt(anchor.Uuid.ToString(), _spatialAnchor.AssignedObjectId);
        });
    }

    /// <summary>
    /// UI callback for the anchor menu's Hide button
    /// </summary>
    public void OnHideButtonPressed()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// UI callback for the anchor menu's Erase button
    /// </summary>
    public void OnEraseButtonPressed()
    {
        if (!_spatialAnchor) return;

        DisableAllUIMenus();

        _spatialAnchor.Erase((anchor, success) =>
        {
            if (success)
            {
                _saveIcon.SetActive(false);
            }
        });
    }

    /// <summary>
    /// UI callback for the anchor menu's Assign Button
    /// </summary>
    public void OnAssignObjectButtonPressed()
    {
        if (!_spatialAnchor) return;

        _objectMenuIndex = -1;
        _objectMenu.SetActive(true);
        _isAnchorMenuSelected = false;//navigate only in the "Assign Anchor object" menu
    }

    #endregion // UI Event Listeners Anchor Menu

    #region UI Event Listeners Assign Anchor Menu

    public void OnReturnBackButtonPressed()
    {
        if (!_spatialAnchor) return;

        _objectMenu.SetActive(false);
        _isAnchorMenuSelected = true;//navigate in the "Anchor" menu
    }
    public void OnPlayerSpawnPointButtonPressed()
    {
        InstantiateObjectOnAnchor(0);
    }
    public void OnCreatureSpawnPointButtonPressed()
    {
        InstantiateObjectOnAnchor(1);
    }
    public void OnCreatureStationButtonPressed()
    {
        InstantiateObjectOnAnchor(2);
    }
    public void OnCreatureActionPointButtonPressed()
    {
        InstantiateObjectOnAnchor(3);
    }
    public void OnBossSpawnPointButtonPressed()
    {
        InstantiateObjectOnAnchor(4);
    }
    public void OnEnvironmentObjectButtonPressed()
    {
        InstantiateObjectOnAnchor(5);
    }
    public void OnDeleteObjectButtonPressed()
    {
        DeleteAnchorObject();
    }
    #endregion // UI Event Listeners Assign Anchor Menu

    #region Event Listeners
    /// <summary>
    /// This method is executed once a C# event from the UIManager is registered.
    /// </summary>
    private void SpawnAnchorObject()
    {
        if (_spatialAnchor.AssignedObjectId > defaultAssignedObjectId)
            InstantiateObjectOnAnchor(_spatialAnchor.AssignedObjectId);
    }
    #endregion //Event Listeners

    #region Public Methods

    public bool ShowSaveIcon
    {
        set => _saveIcon.SetActive(value);
    }

    /// <summary>
    /// Handles interaction when anchor is hovered
    /// </summary>
    public void OnHoverStart()
    {
        if (_isHovered)
        {
            return;
        }
        _isHovered = true;

        // Yellow highlight
        foreach (MeshRenderer renderer in _renderers)
        {
            renderer.material.SetColor("_EmissionColor", Color.yellow);
        }
        _labelImage.color = _labelHighlightColor;
    }

    /// <summary>
    /// Handles interaction when anchor is no longer hovered
    /// </summary>
    public void OnHoverEnd()
    {
        if (!_isHovered)
        {
            return;
        }
        _isHovered = false;

        // Go back to normal
        foreach (MeshRenderer renderer in _renderers)
        {
            renderer.material.SetColor("_EmissionColor", Color.clear);
        }

        if (_isAnchorSelected)
        {
            _labelImage.color = _labelSelectedColor;
        }
        else
        {
            _labelImage.color = _labelBaseColor;
        }
    }

    /// <summary>
    /// Handles interaction when anchor is selected
    /// </summary>
    public void OnSelect()
    {
        if (_isAnchorSelected)
        {
            // Hide Anchor menu on deselect
            DisableAllUIMenus();
            _isAnchorSelected = false;
            _isAnchorMenuSelected = false;
            _selectedButton = null;
            if (_isHovered)
            {
                _labelImage.color = _labelHighlightColor;
            }
            else
            {
                _labelImage.color = _labelBaseColor;
            }
        }
        else
        {
            // Show Anchor Menu on select
            _anchorMenu.SetActive(true);
            _objectMenu.SetActive(false);

            _isAnchorSelected = true;
            _isAnchorMenuSelected = true;

            _anchorMenuIndex = -1;
            NavigateToIndexInMenu(true);
            if (_isHovered)
            {
                _labelImage.color = _labelHighlightColor;
            }
            else
            {
                _labelImage.color = _labelSelectedColor;
            }
        }
    }

    #endregion // Public Methods

    #region Private Methods

    private void BillboardPanel(Transform panel)
    {
        // The z axis of the panel faces away from the side that is rendered, therefore this code is actually looking away from the camera
        panel.LookAt(new Vector3(panel.position.x * 2 - Camera.main.transform.position.x, panel.position.y * 2 - Camera.main.transform.position.y, panel.position.z * 2 - Camera.main.transform.position.z), Vector3.up);
    }

    // Anchor Menu
    private void HandleMenuNavigation()
    {
        if (!_isAnchorSelected || !_isAnchorMenuSelected)
        {
            return;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp))
        {
            NavigateToIndexInMenu(false);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickDown))
        {
            NavigateToIndexInMenu(true);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            _selectedButton.OnSubmit(null);
        }
    }
    private void NavigateToIndexInMenu(bool moveNext)
    {
        if (moveNext)
        {
            _anchorMenuIndex++;
            if (_anchorMenuIndex > _buttonList.Count - 1)
            {
                _anchorMenuIndex = 0;
            }
        }
        else
        {
            _anchorMenuIndex--;
            if (_anchorMenuIndex < 0)
            {
                _anchorMenuIndex = _buttonList.Count - 1;
            }
        }
        if (_selectedButton)
        {
            _selectedButton.OnDeselect(null);
        }
        _selectedButton = _buttonList[_anchorMenuIndex];
        _selectedButton.OnSelect(null);
    }

    // Assign Anchor Menu
    private void HandleAssignMenuNavigation()
    {
        if (!_isAnchorSelected || _isAnchorMenuSelected)
        {
            return;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp))
        {
            NavigateToIndexInAssignMenu(false);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickDown))
        {
            NavigateToIndexInAssignMenu(true);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            _selectedButton.OnSubmit(null);
        }
    }
    private void NavigateToIndexInAssignMenu(bool moveNext)
    {
        if (moveNext)
        {
            _objectMenuIndex++;
            if (_objectMenuIndex > _objectMenuButtonList.Count - 1)
            {
                _objectMenuIndex = 0;
            }
        }
        else
        {
            _objectMenuIndex--;
            if (_objectMenuIndex < 0)
            {
                _objectMenuIndex = _objectMenuButtonList.Count - 1;
            }
        }
        if (_selectedButton)
        {
            _selectedButton.OnDeselect(null);
        }
        _selectedButton = _objectMenuButtonList[_objectMenuIndex];
        _selectedButton.OnSelect(null);
    }
    private void InstantiateObjectOnAnchor(int listIndex)
    {
        if (listIndex <= -1 && listIndex > (_availableObjects.Count - 1))
        {
            Debug.LogError($"The index for _availableObjects list was out of bounds and no object for the anchor could be created!");
            return;
        }

        DeleteAnchorObject();

        //Assign object to the anchor
        if (!_hasObject && _availableObjects != null)
        {
            _hasObject = true;
            _spatialAnchor.AssignedObjectId = listIndex;
            var objectRot = Quaternion.Euler(0, _spatialAnchor.transform.eulerAngles.y, 0);

            _assignedObject = Instantiate(_availableObjects[listIndex], _spatialAnchor.transform.position, objectRot, _spatialAnchor.transform);
            UpdateLabelAssignedObjectId();
        }
    }

    private void UpdateLabelAssignedObjectId()
    {
        _assignedObjectIdLabel.text = "Assigned Object ID: " + _spatialAnchor.AssignedObjectId.ToString();
    }

    private void DeleteAnchorObject()
    {
        if (_assignedObject != null && _hasObject)
        {
            Destroy(_assignedObject);
            _assignedObject = null;
            _hasObject = false;
            _spatialAnchor.AssignedObjectId = defaultAssignedObjectId;
            UpdateLabelAssignedObjectId();
        }
    }

    private void DisableAllUIMenus()
    {
        _anchorMenu.SetActive(false);
        _objectMenu.SetActive(false);
    }

    #endregion // Private Methods
}
