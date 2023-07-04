using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class EditorManager : EditorWindow
    {

        public Character[] Characters;
        [SerializeField]private int number3;
        
        private MeshRenderer[] allObjects;
 
        [MenuItem("Tools/SS")]
 
        void Start()
        {
            allObjects = GameObject.FindObjectsOfType<MeshRenderer>();
        }
        [MenuItem("Window/Explorer Window")]
        public static void ShowWindow() // Gets called
        {
            GetWindow<EditorManager>().Show();
        }
        void OnGUI()
        {

            if (GUILayout.Button("Create scriptable objects "))
            {
                
                
                
            }            
            
            // if (GUILayout.Button("SetupModel"))
            // {
            //    FindObjectOfType<FigureSetupper>().SetupFigure();
            // }
        }
    }
#endif
