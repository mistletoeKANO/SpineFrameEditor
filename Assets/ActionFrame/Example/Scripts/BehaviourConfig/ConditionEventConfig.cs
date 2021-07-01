using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    [BehaviourConfig(typeof(object))]
    public class ConditionEventConfig : BehaviourData
    {
        [LabelName("施加的力")]
        public Vector2 AddForce;
        
        [LabelName("测试List")]
        public List<DemoClass> Demo;
    }

    public class DemoClass
    {
        [LabelName("测试")]
        public int IntDemo;
        
        [LabelName("测试B")]
        [SerializeReference]
        public DemoB StrDemo;
    }

    public class DemoB
    {
        [LabelName("嵌套测试")]
        public string TestC;

        [LabelName("嵌套Vector")]
        public Vector2 VecDemo;

        [LabelName("枚举测试")]
        public EnumTest EnumTest;

        [LabelName("LayerMask测试")]
        [SerializeReference]
        public LayerMask Mask;
    }

    public enum EnumTest
    {
        A,
        B,
        C,
        D
    }
}