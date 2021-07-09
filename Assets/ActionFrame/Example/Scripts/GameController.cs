﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionFrame.Runtime
{
    public class GameController : MonoBehaviour
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

        private void Start()
        {
            this.m_CurCamera = Camera.main;
            this.m_Input = new HeroInputSys();
            this.m_Input.Enable();

            this.InitMap();
            this.InitData();
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
            this.m_HeroObj.GetComponent<Renderer>().sortingOrder = (int) (-this.m_HeroObj.transform.position.y * 1000);
            this.m_Hero = this.m_HeroObj.GetComponent<ESkeletonAnimation>();
            
            this.m_Monster = new List<ESkeletonAnimation>();
            for (int i = 0; i < 2; i++)
            {
                var monster = Instantiate(PrefabHero);
                monster.transform.position = this.m_Hero.transform.position +
                                             new Vector3(Random.Range(-2, 2), Random.Range(-1, 1));
                monster.GetComponent<Renderer>().sortingOrder = (int) (-monster.transform.position.y * 1000);
                ESkeletonAnimation esk = monster.GetComponent<ESkeletonAnimation>();
                this.m_Monster.Add(esk);
            }
        }

        private void Update()
        {
            InputEventCache.Clear();
            this.UpdateCameraPos();
            this.UpdateInput();
            this.m_Hero.UpdateLogic(Time.deltaTime);

            InputEventCache.Clear();
            foreach (var monster in this.m_Monster)
            {
                monster.UpdateLogic(Time.deltaTime);
            }
        }

        private void UpdateInput()
        {
            var hero = this.m_Input.Hero;
            if (hero.Move.phase == InputActionPhase.Started)
            {
                var move = hero.Move.ReadValue<Vector2>();
                InputEventCache.EventType |= InputEventType.Walk;
                InputEventCache.InputAxis = move;
                this.m_Hero.skeleton.ScaleX = (int) (move.x * 100) == 0
                    ? this.m_Hero.skeleton.ScaleX : move.x > 0 ? 1 : -1;
            }

            if (hero.Move.phase == InputActionPhase.Waiting)
            {
                InputEventCache.EventType |= InputEventType.Idle;
            }

            if (hero.Attack.triggered)
            {
                InputEventCache.EventType |= InputEventType.Attack;
            }

            if (hero.Jump.triggered)
            {
                InputEventCache.EventType |= InputEventType.Jump;
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
                SpriteRenderer two = this.m_BGArray[2].GetComponent<SpriteRenderer>();
                two.flipX = !two.flipX;
            }

            if (camePos.x < posX - this.width / 2f)
            {
                GameObject temp = this.m_BGArray[2];
                temp.transform.position = this.m_BGArray[0].transform.position - new Vector3(this.width, 0, 0);
                this.m_BGArray[2] = this.m_BGArray[1];
                this.m_BGArray[1] = this.m_BGArray[0];
                this.m_BGArray[0] = temp;
                SpriteRenderer zero = this.m_BGArray[0].GetComponent<SpriteRenderer>();
                zero.flipX = !zero.flipX;
            }
        }
    }
}