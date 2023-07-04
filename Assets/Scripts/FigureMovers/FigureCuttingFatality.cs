
using BzKovSoft.CharacterSlicer.Samples;
using BzKovSoft.ObjectSlicer;
using BzKovSoft.ObjectSlicer.Samples.Scripts;
using UnityEngine;

public abstract class FigureCuttingFatality : FigureFatality
{
    protected CharacterSlicerSampleFast characterSlicerSampleFast;
    protected AnimSliceConfiguration animSliceConfiguration;
    [SerializeField] protected BzSliceConfiguration[] keepOneFirst;
    [SerializeField] protected BzSliceConfiguration[] keepOneSecond;

    public Transform rootBone;
    public Transform childBone;
    public Transform slicePlane;


    public BzSliceConfiguration[] GetKeepOneFirst()
    {
        return keepOneFirst;
    }
    public BzSliceConfiguration[] GetKeepOneSecond()
    {
        return keepOneSecond;
    }
    public void SetupMeshesToKeep(BzSliceConfiguration[] newKeepOneFirst,BzSliceConfiguration[] newKeepOneSecond)
    {
        keepOneFirst = newKeepOneFirst;
        keepOneSecond = newKeepOneSecond;
    }
    public abstract void SetCuttingParameters(Transform[] cuttingParameters);
    public abstract Transform[] GetCuttingParameters();

    private void Awake()
    {
        base.Awake();
        characterSlicerSampleFast = GetComponent<CharacterSlicerSampleFast>();
        animSliceConfiguration = GetComponent<AnimSliceConfiguration>();
    }
}
