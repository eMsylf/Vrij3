// GENERATED AUTOMATICALLY FROM 'Assets/Input/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Game"",
            ""id"": ""2680061e-e6f7-4b4d-9223-848700219278"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""f2783a58-7c5d-4bc8-b28f-38d5c1d7d943"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dodge"",
                    ""type"": ""Button"",
                    ""id"": ""503c1796-6ccd-43b4-b8f6-44962e5b74d5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""1cdbec42-dc3e-4159-a2c3-0e65065d78af"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""94838e5b-987c-4f15-b9e7-a578708197b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LockOn"",
                    ""type"": ""Button"",
                    ""id"": ""d551f09d-2cd2-43b8-931a-4620664e2085"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""2cfbc086-9528-485d-9832-11d032c8f4c0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraZoom"",
                    ""type"": ""Value"",
                    ""id"": ""4e655597-5473-4ebf-be25-2cac26900e52"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""b429cfaa-5bda-482e-8ce2-4633662e7b1f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""466f4fd8-223d-4945-a69a-6a4c3b9a82f9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""dc494ec4-8df4-493a-998d-7c21622c1000"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0d0b4d93-2648-4e4e-ab15-b75c543b9cc9"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5cb06d24-4dab-4ff0-b0b8-142c6df23a53"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09cf282f-dac4-4f85-9474-6f2d692449fa"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1efd78ac-a3ec-4ea8-a285-56e4c3af9928"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""5a0474d7-ec02-4109-98a6-b123719bd48f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""848d6481-2067-496f-9dd5-585c46aed327"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5413f05b-42f3-4133-a1ea-97e5fa733759"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2b392467-0ca5-4853-9fb9-7fb6e7a6bbac"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""1784f7ee-c574-44a9-8c07-8dcb7cd9b178"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""01276f86-19ac-4730-88e7-7d3b1cdba835"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""976c3b49-c772-4bab-8c9b-0f1f2e4e9e19"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""501d227f-c12a-4227-8431-0173b84186e1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""16bbd119-6ed2-4471-8dab-78d1a03caff8"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""96a95579-8139-4f51-9ad3-6087c3320807"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Thumbstick"",
                    ""id"": ""ffe7dff3-ef3e-42a7-84c3-0c89544cbb64"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9a9c3298-8181-4224-a00b-2d3ab406a0a3"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7d814f44-a8da-4498-af54-fa51a8a992dc"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7887295e-1bb9-4027-a81d-95ae6806c250"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3b9dbddb-20c8-40ff-9442-14066e1bef05"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3e0da132-4b0e-428f-b20c-e1fcdeeecc78"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8fcbe01a-5aba-4509-ba4d-0bd07fb85132"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e924b8d0-1446-4d27-b560-37cb8105c27c"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""LockOn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fdfd46a0-2ebd-4310-a707-f2a1bdf53e3e"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""LockOn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""67040d35-6c80-48f0-bac8-5831146bb725"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0a6d488-768f-47a5-bd18-98e6016999cf"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5c77726b-cd6c-49fa-b099-998a57e83c7b"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.3,y=0.1)"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a13c7bc8-555b-49e2-9a03-dadefbd3ec20"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": ""Hold(duration=0.01,pressPoint=0.5)"",
                    ""processors"": ""StickDeadzone,ScaleVector2(x=4,y=2)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3d1218eb-56ef-47f3-b244-ad2498323b84"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c38d52c0-603d-4cf8-9012-1bb73d25a46b"",
                    ""path"": ""<Gamepad>/rightStick/y"",
                    ""interactions"": ""Hold(duration=0.01)"",
                    ""processors"": ""Scale(factor=7),Invert"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""875f966c-bab8-4dc3-b57c-11f465df7bab"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""89a88136-fe8f-4e12-adfc-6d0a02721417"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""44a75ce6-4199-4a20-ba9f-5f123ca747d8"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea2204e9-d1db-45b3-aaa2-570d5e79ba39"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Hold(duration=Infinity)"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6d407da2-fadb-4acb-ae93-3243cb55ab6c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Hold(duration=Infinity)"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""7c06a6b8-e2a2-4948-98a0-39e5c73e013e"",
            ""actions"": [
                {
                    ""name"": ""Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""30231658-998f-4896-b2b5-798d0778b93d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""2f4d73e8-63f6-4408-890b-9009e4fffb3e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0ba71961-7116-4ff8-b838-ac52ffa43a80"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b26b6a83-64c3-41ac-b55e-5daa2d2a8949"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9d3024fa-140b-42a3-b886-aa96d5f00673"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""SlideShow"",
            ""id"": ""096270c7-ab08-49ad-b24d-34ad8256d1ea"",
            ""actions"": [
                {
                    ""name"": ""Continue"",
                    ""type"": ""Button"",
                    ""id"": ""d0e12201-0b81-4017-bd9f-bba8ad833ff6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skip"",
                    ""type"": ""Button"",
                    ""id"": ""a3b8ba70-8acc-41c8-a792-b9dbe2f93e92"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bf142aa4-833c-429f-b287-7996ec3b63d7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Continue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""adb8b2cc-db9c-475d-88de-09a9bb6e3f9d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Continue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""83cc5933-1317-42e1-9a40-e18e36f02e7c"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Skip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9404e946-95be-41b6-bd22-61efd1ee0c2e"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""CameraController"",
            ""id"": ""dbee8ffa-257f-4e38-bd44-c18e5b248c5c"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""1401aa23-7958-49fa-8a36-30f15920f9b8"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Altitude"",
                    ""type"": ""Button"",
                    ""id"": ""b2b7ed13-a712-49a8-a39d-73e464065bbe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""bda90d1d-297d-4bda-931d-4796a0217386"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Boost"",
                    ""type"": ""Button"",
                    ""id"": ""25c6c4ab-025b-4e2c-86a5-7a1982f9af51"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""9eb8007b-f94c-4349-a0a6-f6cd95a2d003"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1196e129-1a23-497d-9810-b045e885db1d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""245d5706-19da-4846-ade8-396e5e4520a3"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0e684365-9a6c-4fd7-88b7-a1329ee96983"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3073af09-2f03-4a62-944e-406699ccba2d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7308d95a-4c01-40b8-b804-2fbd94a41d50"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fe3e4edd-73df-485b-b5c6-94dc8321598f"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false,invertY=false)"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""QE"",
                    ""id"": ""ce9ebae8-7a28-4b48-81e6-3d96b8965cda"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Altitude"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""17c0a1d8-e95f-4bd6-ba8f-4c5db0f094b1"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Altitude"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""255fdedd-6b12-49aa-987a-253aac9b0f5b"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Altitude"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""TitleScreen"",
            ""id"": ""c5c63700-aa9b-4a55-a963-ff12f6cb8cf2"",
            ""actions"": [
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""f313042b-4d45-400f-95e8-52be02005b8e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3a7b8899-6a02-4ac0-9488-0bf747f44b6b"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff653b07-2a6e-4025-bd99-8d61451f97ff"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Game
        m_Game = asset.FindActionMap("Game", throwIfNotFound: true);
        m_Game_Movement = m_Game.FindAction("Movement", throwIfNotFound: true);
        m_Game_Dodge = m_Game.FindAction("Dodge", throwIfNotFound: true);
        m_Game_Interact = m_Game.FindAction("Interact", throwIfNotFound: true);
        m_Game_Attack = m_Game.FindAction("Attack", throwIfNotFound: true);
        m_Game_LockOn = m_Game.FindAction("LockOn", throwIfNotFound: true);
        m_Game_Look = m_Game.FindAction("Look", throwIfNotFound: true);
        m_Game_CameraZoom = m_Game.FindAction("CameraZoom", throwIfNotFound: true);
        m_Game_Reload = m_Game.FindAction("Reload", throwIfNotFound: true);
        m_Game_Pause = m_Game.FindAction("Pause", throwIfNotFound: true);
        m_Game_Run = m_Game.FindAction("Run", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Toggle = m_Menu.FindAction("Toggle", throwIfNotFound: true);
        m_Menu_Back = m_Menu.FindAction("Back", throwIfNotFound: true);
        // SlideShow
        m_SlideShow = asset.FindActionMap("SlideShow", throwIfNotFound: true);
        m_SlideShow_Continue = m_SlideShow.FindAction("Continue", throwIfNotFound: true);
        m_SlideShow_Skip = m_SlideShow.FindAction("Skip", throwIfNotFound: true);
        // CameraController
        m_CameraController = asset.FindActionMap("CameraController", throwIfNotFound: true);
        m_CameraController_Movement = m_CameraController.FindAction("Movement", throwIfNotFound: true);
        m_CameraController_Altitude = m_CameraController.FindAction("Altitude", throwIfNotFound: true);
        m_CameraController_Look = m_CameraController.FindAction("Look", throwIfNotFound: true);
        m_CameraController_Boost = m_CameraController.FindAction("Boost", throwIfNotFound: true);
        // TitleScreen
        m_TitleScreen = asset.FindActionMap("TitleScreen", throwIfNotFound: true);
        m_TitleScreen_Start = m_TitleScreen.FindAction("Start", throwIfNotFound: true);
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

    // Game
    private readonly InputActionMap m_Game;
    private IGameActions m_GameActionsCallbackInterface;
    private readonly InputAction m_Game_Movement;
    private readonly InputAction m_Game_Dodge;
    private readonly InputAction m_Game_Interact;
    private readonly InputAction m_Game_Attack;
    private readonly InputAction m_Game_LockOn;
    private readonly InputAction m_Game_Look;
    private readonly InputAction m_Game_CameraZoom;
    private readonly InputAction m_Game_Reload;
    private readonly InputAction m_Game_Pause;
    private readonly InputAction m_Game_Run;
    public struct GameActions
    {
        private @Controls m_Wrapper;
        public GameActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Game_Movement;
        public InputAction @Dodge => m_Wrapper.m_Game_Dodge;
        public InputAction @Interact => m_Wrapper.m_Game_Interact;
        public InputAction @Attack => m_Wrapper.m_Game_Attack;
        public InputAction @LockOn => m_Wrapper.m_Game_LockOn;
        public InputAction @Look => m_Wrapper.m_Game_Look;
        public InputAction @CameraZoom => m_Wrapper.m_Game_CameraZoom;
        public InputAction @Reload => m_Wrapper.m_Game_Reload;
        public InputAction @Pause => m_Wrapper.m_Game_Pause;
        public InputAction @Run => m_Wrapper.m_Game_Run;
        public InputActionMap Get() { return m_Wrapper.m_Game; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
        public void SetCallbacks(IGameActions instance)
        {
            if (m_Wrapper.m_GameActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_GameActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnMovement;
                @Dodge.started -= m_Wrapper.m_GameActionsCallbackInterface.OnDodge;
                @Dodge.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnDodge;
                @Dodge.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnDodge;
                @Interact.started -= m_Wrapper.m_GameActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnInteract;
                @Attack.started -= m_Wrapper.m_GameActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnAttack;
                @LockOn.started -= m_Wrapper.m_GameActionsCallbackInterface.OnLockOn;
                @LockOn.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnLockOn;
                @LockOn.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnLockOn;
                @Look.started -= m_Wrapper.m_GameActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnLook;
                @CameraZoom.started -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoom;
                @CameraZoom.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoom;
                @CameraZoom.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoom;
                @Reload.started -= m_Wrapper.m_GameActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnReload;
                @Pause.started -= m_Wrapper.m_GameActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnPause;
                @Run.started -= m_Wrapper.m_GameActionsCallbackInterface.OnRun;
                @Run.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnRun;
                @Run.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnRun;
            }
            m_Wrapper.m_GameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Dodge.started += instance.OnDodge;
                @Dodge.performed += instance.OnDodge;
                @Dodge.canceled += instance.OnDodge;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @LockOn.started += instance.OnLockOn;
                @LockOn.performed += instance.OnLockOn;
                @LockOn.canceled += instance.OnLockOn;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @CameraZoom.started += instance.OnCameraZoom;
                @CameraZoom.performed += instance.OnCameraZoom;
                @CameraZoom.canceled += instance.OnCameraZoom;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
            }
        }
    }
    public GameActions @Game => new GameActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Toggle;
    private readonly InputAction m_Menu_Back;
    public struct MenuActions
    {
        private @Controls m_Wrapper;
        public MenuActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Toggle => m_Wrapper.m_Menu_Toggle;
        public InputAction @Back => m_Wrapper.m_Menu_Back;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Toggle.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnToggle;
                @Toggle.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnToggle;
                @Toggle.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnToggle;
                @Back.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Toggle.started += instance.OnToggle;
                @Toggle.performed += instance.OnToggle;
                @Toggle.canceled += instance.OnToggle;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);

    // SlideShow
    private readonly InputActionMap m_SlideShow;
    private ISlideShowActions m_SlideShowActionsCallbackInterface;
    private readonly InputAction m_SlideShow_Continue;
    private readonly InputAction m_SlideShow_Skip;
    public struct SlideShowActions
    {
        private @Controls m_Wrapper;
        public SlideShowActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Continue => m_Wrapper.m_SlideShow_Continue;
        public InputAction @Skip => m_Wrapper.m_SlideShow_Skip;
        public InputActionMap Get() { return m_Wrapper.m_SlideShow; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SlideShowActions set) { return set.Get(); }
        public void SetCallbacks(ISlideShowActions instance)
        {
            if (m_Wrapper.m_SlideShowActionsCallbackInterface != null)
            {
                @Continue.started -= m_Wrapper.m_SlideShowActionsCallbackInterface.OnContinue;
                @Continue.performed -= m_Wrapper.m_SlideShowActionsCallbackInterface.OnContinue;
                @Continue.canceled -= m_Wrapper.m_SlideShowActionsCallbackInterface.OnContinue;
                @Skip.started -= m_Wrapper.m_SlideShowActionsCallbackInterface.OnSkip;
                @Skip.performed -= m_Wrapper.m_SlideShowActionsCallbackInterface.OnSkip;
                @Skip.canceled -= m_Wrapper.m_SlideShowActionsCallbackInterface.OnSkip;
            }
            m_Wrapper.m_SlideShowActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Continue.started += instance.OnContinue;
                @Continue.performed += instance.OnContinue;
                @Continue.canceled += instance.OnContinue;
                @Skip.started += instance.OnSkip;
                @Skip.performed += instance.OnSkip;
                @Skip.canceled += instance.OnSkip;
            }
        }
    }
    public SlideShowActions @SlideShow => new SlideShowActions(this);

    // CameraController
    private readonly InputActionMap m_CameraController;
    private ICameraControllerActions m_CameraControllerActionsCallbackInterface;
    private readonly InputAction m_CameraController_Movement;
    private readonly InputAction m_CameraController_Altitude;
    private readonly InputAction m_CameraController_Look;
    private readonly InputAction m_CameraController_Boost;
    public struct CameraControllerActions
    {
        private @Controls m_Wrapper;
        public CameraControllerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_CameraController_Movement;
        public InputAction @Altitude => m_Wrapper.m_CameraController_Altitude;
        public InputAction @Look => m_Wrapper.m_CameraController_Look;
        public InputAction @Boost => m_Wrapper.m_CameraController_Boost;
        public InputActionMap Get() { return m_Wrapper.m_CameraController; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraControllerActions set) { return set.Get(); }
        public void SetCallbacks(ICameraControllerActions instance)
        {
            if (m_Wrapper.m_CameraControllerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnMovement;
                @Altitude.started -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnAltitude;
                @Altitude.performed -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnAltitude;
                @Altitude.canceled -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnAltitude;
                @Look.started -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnLook;
                @Boost.started -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnBoost;
                @Boost.performed -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnBoost;
                @Boost.canceled -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnBoost;
            }
            m_Wrapper.m_CameraControllerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Altitude.started += instance.OnAltitude;
                @Altitude.performed += instance.OnAltitude;
                @Altitude.canceled += instance.OnAltitude;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Boost.started += instance.OnBoost;
                @Boost.performed += instance.OnBoost;
                @Boost.canceled += instance.OnBoost;
            }
        }
    }
    public CameraControllerActions @CameraController => new CameraControllerActions(this);

    // TitleScreen
    private readonly InputActionMap m_TitleScreen;
    private ITitleScreenActions m_TitleScreenActionsCallbackInterface;
    private readonly InputAction m_TitleScreen_Start;
    public struct TitleScreenActions
    {
        private @Controls m_Wrapper;
        public TitleScreenActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Start => m_Wrapper.m_TitleScreen_Start;
        public InputActionMap Get() { return m_Wrapper.m_TitleScreen; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TitleScreenActions set) { return set.Get(); }
        public void SetCallbacks(ITitleScreenActions instance)
        {
            if (m_Wrapper.m_TitleScreenActionsCallbackInterface != null)
            {
                @Start.started -= m_Wrapper.m_TitleScreenActionsCallbackInterface.OnStart;
                @Start.performed -= m_Wrapper.m_TitleScreenActionsCallbackInterface.OnStart;
                @Start.canceled -= m_Wrapper.m_TitleScreenActionsCallbackInterface.OnStart;
            }
            m_Wrapper.m_TitleScreenActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Start.started += instance.OnStart;
                @Start.performed += instance.OnStart;
                @Start.canceled += instance.OnStart;
            }
        }
    }
    public TitleScreenActions @TitleScreen => new TitleScreenActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IGameActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnDodge(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnLockOn(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnCameraZoom(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnToggle(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
    }
    public interface ISlideShowActions
    {
        void OnContinue(InputAction.CallbackContext context);
        void OnSkip(InputAction.CallbackContext context);
    }
    public interface ICameraControllerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnAltitude(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnBoost(InputAction.CallbackContext context);
    }
    public interface ITitleScreenActions
    {
        void OnStart(InputAction.CallbackContext context);
    }
}
