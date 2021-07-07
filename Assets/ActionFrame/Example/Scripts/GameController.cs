using UnityEngine;

namespace ActionFrame.Runtime
{
    public class GameController : MonoBehaviour
    {
        public GameObject PrefabHero;
        private GameObject m_HeroObj;
        private ESkeletonAnimation m_Hero;

        private void Start()
        {
            this.m_HeroObj = Instantiate(PrefabHero);
            this.m_Hero = this.m_HeroObj.GetComponent<ESkeletonAnimation>();
        }

        private void Update()
        {
            
        }
    }
}