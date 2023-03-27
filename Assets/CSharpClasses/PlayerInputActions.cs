//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.0
//     from Assets/CSharpClasses/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""PlayerPart1"",
            ""id"": ""94d6cd4b-464d-4e01-8c83-5f683323c852"",
            ""actions"": [
                {
                    ""name"": ""GrabbingLeft"",
                    ""type"": ""Button"",
                    ""id"": ""fb0521ce-475f-4101-9c13-1a2ae6d4888e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""GrabbingRight"",
                    ""type"": ""Button"",
                    ""id"": ""38c8c0e9-8847-4706-9226-e98f7c24d31b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""23b0135e-355f-4169-b81c-0681c4bd848e"",
                    ""path"": ""<OculusTouchController>{LeftHand}/gripPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GrabbingLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aeef98b3-845f-4994-a579-8367a95762af"",
                    ""path"": ""<OculusTouchController>{RightHand}/gripPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GrabbingRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerPart2"",
            ""id"": ""b1745e1e-68ac-4b76-b866-450880f37e27"",
            ""actions"": [
                {
                    ""name"": ""ShootingLeft"",
                    ""type"": ""Button"",
                    ""id"": ""5a34b7ff-0eba-4d65-98b1-6075562964a3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ShootingRight"",
                    ""type"": ""Button"",
                    ""id"": ""dd0c7880-01ff-43e9-b763-e41b80ec6b05"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""411bd8f9-45de-47e4-95c9-f3130af93d11"",
                    ""path"": ""<XRController>{LeftHand}/triggerPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShootingLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bfdfc9e7-b2d7-424b-9c15-e168d894dc80"",
                    ""path"": ""<XRController>{RightHand}/triggerPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShootingRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerPart1
        m_PlayerPart1 = asset.FindActionMap("PlayerPart1", throwIfNotFound: true);
        m_PlayerPart1_GrabbingLeft = m_PlayerPart1.FindAction("GrabbingLeft", throwIfNotFound: true);
        m_PlayerPart1_GrabbingRight = m_PlayerPart1.FindAction("GrabbingRight", throwIfNotFound: true);
        // PlayerPart2
        m_PlayerPart2 = asset.FindActionMap("PlayerPart2", throwIfNotFound: true);
        m_PlayerPart2_ShootingLeft = m_PlayerPart2.FindAction("ShootingLeft", throwIfNotFound: true);
        m_PlayerPart2_ShootingRight = m_PlayerPart2.FindAction("ShootingRight", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerPart1
    private readonly InputActionMap m_PlayerPart1;
    private List<IPlayerPart1Actions> m_PlayerPart1ActionsCallbackInterfaces = new List<IPlayerPart1Actions>();
    private readonly InputAction m_PlayerPart1_GrabbingLeft;
    private readonly InputAction m_PlayerPart1_GrabbingRight;
    public struct PlayerPart1Actions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerPart1Actions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @GrabbingLeft => m_Wrapper.m_PlayerPart1_GrabbingLeft;
        public InputAction @GrabbingRight => m_Wrapper.m_PlayerPart1_GrabbingRight;
        public InputActionMap Get() { return m_Wrapper.m_PlayerPart1; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerPart1Actions set) { return set.Get(); }
        public void AddCallbacks(IPlayerPart1Actions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerPart1ActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerPart1ActionsCallbackInterfaces.Add(instance);
            @GrabbingLeft.started += instance.OnGrabbingLeft;
            @GrabbingLeft.performed += instance.OnGrabbingLeft;
            @GrabbingLeft.canceled += instance.OnGrabbingLeft;
            @GrabbingRight.started += instance.OnGrabbingRight;
            @GrabbingRight.performed += instance.OnGrabbingRight;
            @GrabbingRight.canceled += instance.OnGrabbingRight;
        }

        private void UnregisterCallbacks(IPlayerPart1Actions instance)
        {
            @GrabbingLeft.started -= instance.OnGrabbingLeft;
            @GrabbingLeft.performed -= instance.OnGrabbingLeft;
            @GrabbingLeft.canceled -= instance.OnGrabbingLeft;
            @GrabbingRight.started -= instance.OnGrabbingRight;
            @GrabbingRight.performed -= instance.OnGrabbingRight;
            @GrabbingRight.canceled -= instance.OnGrabbingRight;
        }

        public void RemoveCallbacks(IPlayerPart1Actions instance)
        {
            if (m_Wrapper.m_PlayerPart1ActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerPart1Actions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerPart1ActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerPart1ActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerPart1Actions @PlayerPart1 => new PlayerPart1Actions(this);

    // PlayerPart2
    private readonly InputActionMap m_PlayerPart2;
    private List<IPlayerPart2Actions> m_PlayerPart2ActionsCallbackInterfaces = new List<IPlayerPart2Actions>();
    private readonly InputAction m_PlayerPart2_ShootingLeft;
    private readonly InputAction m_PlayerPart2_ShootingRight;
    public struct PlayerPart2Actions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerPart2Actions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ShootingLeft => m_Wrapper.m_PlayerPart2_ShootingLeft;
        public InputAction @ShootingRight => m_Wrapper.m_PlayerPart2_ShootingRight;
        public InputActionMap Get() { return m_Wrapper.m_PlayerPart2; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerPart2Actions set) { return set.Get(); }
        public void AddCallbacks(IPlayerPart2Actions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerPart2ActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerPart2ActionsCallbackInterfaces.Add(instance);
            @ShootingLeft.started += instance.OnShootingLeft;
            @ShootingLeft.performed += instance.OnShootingLeft;
            @ShootingLeft.canceled += instance.OnShootingLeft;
            @ShootingRight.started += instance.OnShootingRight;
            @ShootingRight.performed += instance.OnShootingRight;
            @ShootingRight.canceled += instance.OnShootingRight;
        }

        private void UnregisterCallbacks(IPlayerPart2Actions instance)
        {
            @ShootingLeft.started -= instance.OnShootingLeft;
            @ShootingLeft.performed -= instance.OnShootingLeft;
            @ShootingLeft.canceled -= instance.OnShootingLeft;
            @ShootingRight.started -= instance.OnShootingRight;
            @ShootingRight.performed -= instance.OnShootingRight;
            @ShootingRight.canceled -= instance.OnShootingRight;
        }

        public void RemoveCallbacks(IPlayerPart2Actions instance)
        {
            if (m_Wrapper.m_PlayerPart2ActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerPart2Actions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerPart2ActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerPart2ActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerPart2Actions @PlayerPart2 => new PlayerPart2Actions(this);
    public interface IPlayerPart1Actions
    {
        void OnGrabbingLeft(InputAction.CallbackContext context);
        void OnGrabbingRight(InputAction.CallbackContext context);
    }
    public interface IPlayerPart2Actions
    {
        void OnShootingLeft(InputAction.CallbackContext context);
        void OnShootingRight(InputAction.CallbackContext context);
    }
}
