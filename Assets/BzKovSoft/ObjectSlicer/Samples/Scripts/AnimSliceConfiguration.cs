using UnityEngine;

namespace BzKovSoft.ObjectSlicer.Samples.Scripts
{
    public class AnimSliceConfiguration : MonoBehaviour
    {
        public AnimSliceType mainSliceType;
        public AnimSliceType cuttedSliceType;
        [SerializeField] private GameObject[] severedBones;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public Transform rootBone;
        public Transform childBone;
        public bool dawnPartMain;

        public void NewAnimSliceConfiguration(AnimSliceType newMainSliceType,AnimSliceType newCuttedSliceType , Vector3 newVelocity, Vector3 newAngularVelocity,Transform newRootBone,Transform newChildBone,bool newDawnPartMain)
        {
            mainSliceType = newMainSliceType;
            cuttedSliceType = newCuttedSliceType;
            velocity = newVelocity;
            angularVelocity = newAngularVelocity;
            rootBone = newRootBone;
            childBone = newChildBone;
            dawnPartMain= newDawnPartMain;
        }
    }
}
