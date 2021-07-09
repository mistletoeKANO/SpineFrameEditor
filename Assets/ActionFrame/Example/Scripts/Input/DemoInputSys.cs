// GENERATED AUTOMATICALLY FROM 'Assets/ActionFrame/Example/Editor/DemoInputSys.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace ActionFrame.Runtime
{
    public class @HeroInputSys : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @HeroInputSys()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""DemoInputSys"",
    ""maps"": [
        {
            ""name"": ""Hero"",
            ""id"": ""1548d98b-bbbe-412d-992b-dbd67e62dcb4"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""ee201fec-d197-4cef-91e0-f388e17b8017"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""dc925c0c-1229-49c5-bf47-502c47b96246"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""1f6d5caf-d665-4827-8bd3-e5adab603726"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""518c0687-3603-4d02-b9a4-f5127e8cf0a1"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b6c3f11d-3a56-46f7-a087-9bcb2e39ee78"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DemoScheme"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e808429d-8c08-42a2-b93e-eaf5e5cc4f5f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DemoScheme"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""3f79855d-9299-4dd0-a2f5-048e8e11a7a0"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DemoScheme"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b102ec48-912d-48c6-b65f-cc4a7e815574"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DemoScheme"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""87cd3b6b-f635-4f60-b333-b39ebc518b95"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DemoScheme"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1646f513-021c-495d-aff7-52679683f63b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DemoScheme"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""DemoScheme"",
            ""bindingGroup"": ""DemoScheme"",
            ""devices"": []
        }
    ]
}");
            // Hero
            m_Hero = asset.FindActionMap("Hero", throwIfNotFound: true);
            m_Hero_Move = m_Hero.FindAction("Move", throwIfNotFound: true);
            m_Hero_Jump = m_Hero.FindAction("Jump", throwIfNotFound: true);
            m_Hero_Attack = m_Hero.FindAction("Attack", throwIfNotFound: true);
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

        // Hero
        private readonly InputActionMap m_Hero;
        private IHeroActions m_HeroActionsCallbackInterface;
        private readonly InputAction m_Hero_Move;
        private readonly InputAction m_Hero_Jump;
        private readonly InputAction m_Hero_Attack;
        public struct HeroActions
        {
            private @HeroInputSys m_Wrapper;
            public HeroActions(@HeroInputSys wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Hero_Move;
            public InputAction @Jump => m_Wrapper.m_Hero_Jump;
            public InputAction @Attack => m_Wrapper.m_Hero_Attack;
            public InputActionMap Get() { return m_Wrapper.m_Hero; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(HeroActions set) { return set.Get(); }
            public void SetCallbacks(IHeroActions instance)
            {
                if (m_Wrapper.m_HeroActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_HeroActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_HeroActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_HeroActionsCallbackInterface.OnMove;
                    @Jump.started -= m_Wrapper.m_HeroActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_HeroActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_HeroActionsCallbackInterface.OnJump;
                    @Attack.started -= m_Wrapper.m_HeroActionsCallbackInterface.OnAttack;
                    @Attack.performed -= m_Wrapper.m_HeroActionsCallbackInterface.OnAttack;
                    @Attack.canceled -= m_Wrapper.m_HeroActionsCallbackInterface.OnAttack;
                }
                m_Wrapper.m_HeroActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Attack.started += instance.OnAttack;
                    @Attack.performed += instance.OnAttack;
                    @Attack.canceled += instance.OnAttack;
                }
            }
        }
        public HeroActions @Hero => new HeroActions(this);
        private int m_DemoSchemeSchemeIndex = -1;
        public InputControlScheme DemoSchemeScheme
        {
            get
            {
                if (m_DemoSchemeSchemeIndex == -1) m_DemoSchemeSchemeIndex = asset.FindControlSchemeIndex("DemoScheme");
                return asset.controlSchemes[m_DemoSchemeSchemeIndex];
            }
        }
        public interface IHeroActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnAttack(InputAction.CallbackContext context);
        }
    }
}
