using UnityEngine;

namespace ActionFrame.Runtime
{
    public class ESkeletonState : MonoBehaviour
    {
        private void Start()
        {
            CheckBoxColliderScene.AddESkeleton(this.GetComponent<ESkeletonAnimation>());
        }

        private void OnDestroy()
        {
            CheckBoxColliderScene.RemoveESkeleton(this.GetComponent<ESkeletonAnimation>());
        }
    }
}