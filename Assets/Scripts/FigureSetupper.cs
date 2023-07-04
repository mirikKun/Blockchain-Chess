using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BzKovSoft.ObjectSlicer;
using UnityEngine;

#if UNITY_EDITOR

public class FigureSetupper : MonoBehaviour
{
    [SerializeField] private Transform readyFigure;
    [SerializeField] private Transform figureToSetup;

    [SerializeField] private GameObject[] readeObjectsToRemove;
    [SerializeField] private bool sameModel;
    [SerializeField] private bool notCopyDetails;


    public void SetupFigure()
    {

        foreach (var collider in readyFigure.GetComponentsInChildren<Collider>())
        {
            Transform curTransform = collider.transform;            
            
        
            string path = "";
            path = FindPath(curTransform);
            //Debug.Log(path);
            Transform colliderHolder = figureToSetup.Find(path);
            var newComponents = collider.transform.GetComponents<Component>();
            foreach (var newComponent in newComponents)
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(newComponent);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(colliderHolder.gameObject);
            }

            if (collider.GetComponent<CharacterJoint>())
            {
                Debug.Log("collider");

                Debug.Log(colliderHolder);
                colliderHolder.GetComponent<CharacterJoint>().connectedBody = colliderHolder.transform.parent.GetComponentInParent<Rigidbody>();
            }
        }

        foreach (var skinnedMesh in figureToSetup.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            skinnedMesh.gameObject.AddComponent<BzSliceConfiguration>();
        }

        BzSliceConfiguration[][] keepOneFirst =
            new BzSliceConfiguration[readyFigure.GetComponentsInChildren<FigureCuttingFatality>().Length][];
        BzSliceConfiguration[][] keepOneSecond=            new BzSliceConfiguration[readyFigure.GetComponentsInChildren<FigureCuttingFatality>().Length][];

        if(sameModel)
        {
            int i=0;
            foreach (var figureCuttingFatality in readyFigure.GetComponentsInChildren<FigureCuttingFatality>())
            {
                keepOneFirst[i] = figureCuttingFatality.GetKeepOneFirst();
                keepOneSecond[i] = figureCuttingFatality.GetKeepOneSecond();
                i++;
            }
        }

        Transform[][] fatalitisDetails =
            new Transform[readyFigure.GetComponentsInChildren<FigureCuttingFatality>().Length][];
        if (!notCopyDetails)
        {
            int i=0;
            foreach (var figureCuttingFatality in readyFigure.GetComponentsInChildren<FigureCuttingFatality>())
            {
                fatalitisDetails[i] = figureCuttingFatality.GetCuttingParameters();
                i++;
            }
        }
        
        figureToSetup.parent = readyFigure;
        figureToSetup.localPosition=Vector3.zero;
        figureToSetup.localEulerAngles=Vector3.zero;        
        EffectInRig[] innerComponents = readyFigure.GetComponentsInChildren<EffectInRig>();

        foreach (var innerComponent in innerComponents)
        {
            Transform curTransform = innerComponent.transform;
            string path = "";
            path = FindPath(curTransform.parent);
            Debug.Log(innerComponent);
            Transform innerComponentHolder = figureToSetup.Find(path);
            innerComponent.transform.SetParent(innerComponentHolder,false);
        }

        

        if(sameModel)
        {
            int i=0;
            foreach (var figureCuttingFatality in readyFigure.GetComponentsInChildren<FigureCuttingFatality>())
            {
                List<BzSliceConfiguration> curBzSliceConfigurationsFirst = new List<BzSliceConfiguration>();
                foreach (var newKeepOneFirst in keepOneFirst[i])
                {
                    string path = FindPath(newKeepOneFirst.transform);
                    if (readyFigure.Find(path).GetComponent<BzSliceConfiguration>())
                    {

                        curBzSliceConfigurationsFirst.Add(readyFigure.Find(path).GetComponent<BzSliceConfiguration>());
                    }
                } 

                List<BzSliceConfiguration> curBzSliceConfigurationsSecond = new List<BzSliceConfiguration>();
                foreach (var newKeepOneSecond in keepOneSecond[i])
                {

                    string path = FindPath(newKeepOneSecond.transform);
                    if (readyFigure.Find(path).GetComponent<BzSliceConfiguration>())
                    {

                        curBzSliceConfigurationsSecond.Add(readyFigure.Find(path).GetComponent<BzSliceConfiguration>());
                    }
                }

                figureCuttingFatality.SetupMeshesToKeep(curBzSliceConfigurationsFirst.ToArray(),curBzSliceConfigurationsSecond.ToArray());
               i++;
            }
        }
        
        
        foreach (var readeObjectToRemove in readeObjectsToRemove)
        {
            DestroyImmediate(readeObjectToRemove);
        }
        
        for (int i=figureToSetup.childCount-1; i >= 0; --i) {
            Transform child = figureToSetup.GetChild(i);
            child.SetParent(figureToSetup.parent.transform, false);
        }
        DestroyImmediate(figureToSetup.gameObject);
        
        if (!notCopyDetails)
        {
            int i=0;
            foreach (var figureCuttingFatality in readyFigure.GetComponentsInChildren<FigureCuttingFatality>())
            {
                figureCuttingFatality.SetCuttingParameters(fatalitisDetails[i]);
                i++;
            }
        }
    }


    private string FindPath(Transform objectTransform)
    {
        string path = "";
        while(!objectTransform.parent.Find("Root_M"))
        {            
            //Debug.Log("1 "+objectTransform.gameObject.name);
            //Debug.Log("2 "+objectTransform.parent.gameObject.name);
            path = "/"+objectTransform.gameObject.name + path;
            objectTransform = objectTransform.parent;
        }
        path = objectTransform.gameObject.name + path;
        
        return path;
    }
}
#endif
