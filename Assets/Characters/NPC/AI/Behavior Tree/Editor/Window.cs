// Source: https://www.youtube.com/watch?v=491TSNwXTIg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Window : EditorWindow
    {
        string myString = "Hello, World!";
        Color color = Color.white;

        [MenuItem("Tools/Ranchy Rats/Brackeys Editor Window Example")]
        public static void ShowWindow()
        {
            GetWindow<Window>("Brackeys Editor Window Example");
        }

        private void OnGUI()
        {
            GUILayout.Label("Ayylmao", EditorStyles.boldLabel);

            myString = EditorGUILayout.TextField("Name", myString);

            if (GUILayout.Button("Press me"))
            {
                UnityEngine.Debug.Log("Button was pressed");
            }

            EditorGUILayout.Space();

            color = EditorGUILayout.ColorField("Color", color);
            if (GUILayout.Button("COLORIZE!"))
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.sharedMaterial.color = color;
                    }
                }
            }
        }
    }
}