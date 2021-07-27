using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace ActionFrame.Runtime
{
    public partial class GameController : MonoBehaviour
    {
        public GameObject PrefabHero;
        public GameObject PrefabBG;
        public Transform BGRoot;
        public Vector2 CameraFollowRange;
        public float CameraFollowSpeed;

        private GameObject m_HeroObj;
        private GameObject[] m_BGArray;
        private ESkeletonAnimation m_Hero;
        private List<ESkeletonAnimation> m_Monster;
        private Camera m_CurCamera;
        private float width = 0f;

        private HeroInputSys m_Input;

        private readonly float m_LogicFrameTime = 1 / 30f;
        private float m_CurLogicTime = 0f;

        private void Start()
        {
            this.m_CurCamera = Camera.main;
            this.m_Input = new HeroInputSys();
            this.m_Input.Enable();
            Physics.autoSimulation = false;
            this.OnAddListener();
            this.InitMap();
            this.InitData();
            this.gameObject.AddComponent<GUIManager>();
            
        }

        private void InitMap()
        {
            RectTransform rect = this.PrefabBG.transform.GetChild(0).GetComponent<RectTransform>();
            this.width = rect.sizeDelta.x * rect.localScale.x;
            this.m_BGArray = new GameObject[3];

            for (int i = 0; i < 3; i++)
            {
                var obj = Instantiate(PrefabBG, BGRoot);
                float widthCur =  i == 0 ? -this.width : i == 1 ? 0 : this.width;
                obj.transform.position = new Vector3(-widthCur, 0, 0);
                obj.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = (i & 1) == 0;
                
                //初始化 精灵图的 层级
                Transform spriteRoot = obj.transform.GetChild(1);
                for (int j = 0; j < spriteRoot.childCount; j++)
                {
                    GameObject child = spriteRoot.GetChild(j).gameObject;
                    child.GetComponent<SpriteRenderer>().sortingOrder = (int) (-child.transform.position.y * 1000);
                }
                this.m_BGArray[i] = obj;
            }
        }

        private void InitData()
        {
            this.m_HeroObj = Instantiate(PrefabHero);
            this.m_Hero = this.m_HeroObj.GetComponentInChildren<ESkeletonAnimation>();
            this.m_Hero.GetComponent<Renderer>().sortingOrder = (int) (-this.m_HeroObj.transform.position.y * 1000);

            this.m_Monster = new List<ESkeletonAnimation>();
            for (int i = 0; i < 2; i++)
            {
                var monster = Instantiate(PrefabHero);
                monster.transform.position = this.m_Hero.transform.position +
                                             new Vector3(Random.Range(-2, 2), Random.Range(-1, 1));
                ESkeletonAnimation esk = monster.GetComponentInChildren<ESkeletonAnimation>();
                esk.GetComponent<Renderer>().sortingOrder = (int) (-monster.transform.position.y * 1000);
                this.m_Monster.Add(esk);
            }
        }

        private void Update()
        {
            this.UpdateCameraPos();
            this.UpdateInput();

            this.m_CurLogicTime += Time.deltaTime;
            if (this.m_CurLogicTime >= this.m_LogicFrameTime)
            {
                this.m_CurLogicTime -= this.m_LogicFrameTime;
                //this.m_Hero.UpdateLogic(this.m_LogicFrameTime);
                //InputEventCache.Clear();
                foreach (var monster in this.m_Monster)
                {
                    //monster.UpdateLogic(this.m_LogicFrameTime);
                }
                Physics.Simulate(this.m_LogicFrameTime);
            }
        }

        private void UpdateInput()
        {
            var hero = this.m_Input.Hero;
            if (hero.Move.phase == InputActionPhase.Started && hero.RunBtn.phase == InputActionPhase.Waiting)
            {
                var move = hero.Move.ReadValue<Vector2>();
                InputEventCache.EventType |= InputEventType.Walk;
                InputEventCache.InputAxis = move;
            }
            
            if (hero.RunBtn.phase == InputActionPhase.Started && hero.Move.phase == InputActionPhase.Started)
            {
                var move = hero.Move.ReadValue<Vector2>();
                InputEventCache.EventType |= InputEventType.Run;
                InputEventCache.InputAxis = move;
            }

            if (hero.Move.phase == InputActionPhase.Waiting)
            {
                InputEventCache.EventType |= InputEventType.Idle;
                this.m_Hero.AttachMoveSpeed(Vector2.zero);
            }

            if (hero.Attack.triggered)
            {
                InputEventCache.EventType |= InputEventType.Attack;
                this.m_Hero.AttachMoveSpeed(Vector2.zero);
            }

            if (hero.Jump.triggered)
            {
                InputEventCache.EventType |= InputEventType.Jump;
                this.m_Hero.AttachMoveSpeed(Vector2.zero);
            }

            if (hero.Jump.phase == InputActionPhase.Started)
            {
                InputEventCache.EventType |= InputEventType.Jumping;
            }
        }

        private void UpdateCameraPos()
        {
            Vector3 startPos = this.m_CurCamera.transform.position;
            Vector3 targetPos = this.m_HeroObj.transform.position;
            float moveValue = 0f;
            if (startPos.x - targetPos.x > CameraFollowRange.y)
            {
                moveValue = startPos.x - targetPos.x - CameraFollowRange.y;
            }
            if (startPos.x - targetPos.x < -CameraFollowRange.x)
            {
                moveValue = startPos.x - targetPos.x + CameraFollowRange.x;
            }
            if (Mathf.Abs(moveValue) > 0)
            {
                this.m_CurCamera.transform.position = Vector3.Lerp(startPos, 
                    startPos - new Vector3(moveValue, 0, 0), Time.deltaTime * CameraFollowSpeed);
            }
            this.CheckBgPos(this.m_CurCamera.transform.position);
        }

        private void CheckBgPos(Vector3 camePos)
        {
            float posX = this.m_BGArray[1].transform.position.x;
            if (camePos.x > posX + this.width / 2f)
            {
                GameObject temp = this.m_BGArray[0];
                temp.transform.position = this.m_BGArray[2].transform.position + new Vector3(this.width, 0, 0);
                this.m_BGArray[0] = this.m_BGArray[1];
                this.m_BGArray[1] = this.m_BGArray[2];
                this.m_BGArray[2] = temp;
                SpriteRenderer two = this.m_BGArray[2].transform.GetChild(0).GetComponent<SpriteRenderer>();
                two.flipX = !two.flipX;
            }

            if (camePos.x < posX - this.width / 2f)
            {
                GameObject temp = this.m_BGArray[2];
                temp.transform.position = this.m_BGArray[0].transform.position - new Vector3(this.width, 0, 0);
                this.m_BGArray[2] = this.m_BGArray[1];
                this.m_BGArray[1] = this.m_BGArray[0];
                this.m_BGArray[0] = temp;
                SpriteRenderer zero = this.m_BGArray[0].transform.GetChild(0).GetComponent<SpriteRenderer>();
                zero.flipX = !zero.flipX;
            }
        }

        private void OnDestroy()
        {
            this.OnRemoveListener();
        }
    }
}