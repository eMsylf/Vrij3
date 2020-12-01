using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BobJeltes.Menu
{
    [RequireComponent(typeof(Button))]
    public class NavigationButton : MonoBehaviour
    {
        public Transform TargetMenuScreen;
        public bool IsBackButton;

        private MenuManager menuManager;
        public MenuManager GetMenuManager()
        {
            if (menuManager == null)
            {
                menuManager = GetComponentInParent<MenuManager>();
            }
            return menuManager;
        }

        public void GoToTarget()
        {
            MenuManager menu = GetMenuManager();
            if (menu == null) return;
            if (IsBackButton)
            {
                menu.GoToPreviousScreen();
            }
            else
            {
                if (TargetMenuScreen == null)
                    Debug.LogError("Target of button has not been set", gameObject);
                else
                {
                    Debug.Log("GO TO THIS SCREEN: " + TargetMenuScreen);
                    menu.GoToScreen(TargetMenuScreen);
                }
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (TargetMenuScreen != null)
            {
                Gizmos.DrawLine(transform.position, TargetMenuScreen.transform.position);
            }
        }
#endif
    }
}
