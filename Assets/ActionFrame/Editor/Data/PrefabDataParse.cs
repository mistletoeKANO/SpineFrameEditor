using ActionFrame.Runtime;
using UnityEngine;

namespace ActionFrame.Editor
{
    /// <summary>
    /// 场景预制体 数据解析类
    /// </summary>
    public class PrefabDataParse
    {
        private GameObject m_CurPrefab;
        private ESkeletonAnimation m_ESkeletonAnim;

        public ESkeletonAnimation ESkeletonAnim
        {
            get => this.m_ESkeletonAnim;
        }

        public Transform PrefabTransform
        {
            get => this.m_CurPrefab.GetComponent<Transform>();
        }

        public PrefabDataParse(GameObject obj)
        {
            this.m_CurPrefab = obj;
            this.m_ESkeletonAnim = obj.GetComponent<ESkeletonAnimation>();
            this.m_ESkeletonAnim.Init();
        }
    }
}