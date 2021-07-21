using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public class GameCurveCtr : MonoBehaviour
    {
        private Queue<CurveInfo> m_CurveCache = new Queue<CurveInfo>();
        private List<CurveInfo> m_RunTimeCurve = new List<CurveInfo>();

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (m_RunTimeCurve.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < m_RunTimeCurve.Count; i++)
            {
                CurveInfo curve = m_RunTimeCurve[i];
                Vector3 startPos = curve.m_Transform.position;
                if (curve.m_CurRunTime < curve.m_Runtime)
                {
                    curve.m_CurRunTime += Time.deltaTime;
                    float value = Mathf.Sin((curve.m_CurRunTime / curve.m_Runtime) * 180f * Mathf.Deg2Rad) * curve.m_YMax;
                    float endValue = Mathf.Abs(value - curve.m_LastValue);
                    curve.m_LastValue = value;
                    if (curve.m_CurRunTime < curve.m_Runtime / 2f)
                    {
                        curve.m_Transform.position = Vector3.Lerp(startPos, startPos + new Vector3(0, endValue, 0), 0.5f);
                    }
                    else if(curve.m_CurRunTime > curve.m_Runtime / 2f)
                    {
                        curve.m_Transform.position = Vector3.Lerp(startPos, startPos + new Vector3(0, -endValue, 0), 0.5f);
                    }
                }
                else
                {
                    curve.m_Transform.position = Vector3.Lerp(startPos, new Vector3(startPos.x, curve.m_YDefault), 1);
                    m_CurveCache.Enqueue(m_RunTimeCurve[i]);
                    m_RunTimeCurve.RemoveAt(i);
                }
            }
        }

        public void DoJumpCurve(Transform transform, float jumpValue, float time)
        {
            CurveInfo newCurve;
            if (m_CurveCache.Count > 0)
            {
                newCurve = m_CurveCache.Dequeue();
            }
            else
            {
                newCurve = new CurveInfo();
            }

            newCurve.m_Transform = transform;
            newCurve.m_Runtime = time;
            newCurve.m_CurRunTime = 0f;
            newCurve.m_LastValue = 0f;
            newCurve.m_YMax = jumpValue;
            newCurve.m_YDefault = transform.position.y;
            m_RunTimeCurve.Add(newCurve);
        }
    }

    public class CurveInfo
    {
        public Transform m_Transform;
        public float m_YMax;
        public float m_YDefault;
        public float m_Runtime;
        public float m_CurRunTime;
        public float m_LastValue;
    }
}