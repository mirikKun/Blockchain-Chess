using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatesTester : MonoBehaviour
{
    //[SerializeField] [Range(0, 1)] private float meshTrianglesMultiplayer = 1;
    public float meshTrianglesMultiplayer = 0.5f;

    //private MeshFilter[] allMeshFilters;

    private void Start()
    {
        // allMeshFilters = FindObjectsOfType<MeshFilter>();
        // foreach (var meshFilter in allMeshFilters)
        // {


        //}
    }

    private void OnGUI()
    {
        if (GUI.Button(Rect.MinMaxRect(0, 0, 200, 200), "Simply Mesh"))
        {
            var meshFilter = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
            meshSimplifier.Initialize(meshFilter);
            meshSimplifier.SimplifyMesh(meshTrianglesMultiplayer);
            var destMesh = meshSimplifier.ToMesh();
            GetComponent<SkinnedMeshRenderer>().sharedMesh = destMesh;
        }
    }
}