using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public static class CheckBoxColliderScene
    {
        private static List<ESkeletonAnimation> m_CurSceneSkeleton = new List<ESkeletonAnimation>();
        public static void AddESkeleton(ESkeletonAnimation anim)
        {
            if (!m_CurSceneSkeleton.Contains(anim))
            {
                m_CurSceneSkeleton.Add(anim);
            }
        }
        public static void RemoveESkeleton(ESkeletonAnimation anim)
        {
            if (!m_CurSceneSkeleton.Contains(anim)) return;
            m_CurSceneSkeleton.Remove(anim);
        }

        private static List<ESkeletonAnimation> GetESkeletonSelect(ESkeletonAnimation anim)
        {
            List<ESkeletonAnimation> result = new List<ESkeletonAnimation>();
            for (int i = 0; i < m_CurSceneSkeleton.Count; i++)
            {
                if (m_CurSceneSkeleton[i] == anim)
                {
                    continue;
                }
                result.Add(m_CurSceneSkeleton[i]);
            }
            return result;
        }
        
        public static List<ESkeletonAnimation> RectColliderOverlap(
            this ESkeletonAnimation self,
            int frameIndex)
        {
            List<ESkeletonAnimation> beHitAnim = new List<ESkeletonAnimation>();
            List<ESkeletonAnimation> animList = GetESkeletonSelect(self);
            foreach (var monster in animList)
            {
                if (CheckSkeletonCollider(self, monster, frameIndex))
                {
                    beHitAnim.Add(monster);
                }
            }
            return beHitAnim;
        }

        public static List<ESkeletonAnimation> EBox2DColliderOverlap(EBox2DCollider self)
        {
            List<ESkeletonAnimation> beHitAnim = new List<ESkeletonAnimation>();
            List<ESkeletonAnimation> animList = GetESkeletonSelect(self.Owner);
            foreach (var monster in animList)
            {
                if (CheckEBox2D(self, monster))
                {
                    beHitAnim.Add(monster);
                }
            }
            return beHitAnim;
        }

        private static bool CheckEBox2D(EBox2DCollider self, ESkeletonAnimation anim)
        {
            if (self.BoxItem == null)
            {
                return false;
            }
            Dictionary<int, FrameData> animFrameData = anim.CurrentState.FrameDic;
            List<BoxItem> beHitBoxList = null;
            //是否使用 当前状态受击框
            if (anim.DefaultState.UseCurSBeHitBox)
            {
                if (animFrameData.ContainsKey(0) && animFrameData[0].ApplyBeHitBoxToAllFrame)
                {
                    beHitBoxList = animFrameData[0].BeHitRangeList;
                }
                else if (animFrameData.ContainsKey(anim.CurrentFrameCount))
                {
                    beHitBoxList = animFrameData[anim.CurrentFrameCount].BeHitRangeList;
                }
            }
            else
            {
                //如果不使用 当前 状态默认受击框 或者 当前状态当前帧 受击框，则 使用 角色默认 状态 受击框
                Dictionary<int, FrameData> animDefaultFrameData = anim.DefaultState.FrameDic;
                if (animDefaultFrameData.ContainsKey(0))
                {
                    beHitBoxList = animDefaultFrameData[0].BeHitRangeList;
                }
            }
            if (beHitBoxList == null)
            {
                return false;
            }
            return CheckEBox2DOverlap(self, anim, self.BoxItem, beHitBoxList);
        }

        private static bool CheckEBox2DOverlap(EBox2DCollider self, ESkeletonAnimation anim, BoxItem attackBox,
            List<BoxItem> beHitRangeList)
        {
            float height = 0f;
            Queue<Rect> beHitBoxList = GetAnimBeHitBoxInternal(anim, beHitRangeList, out height);
            // z 轴 攻击距离 暂定 为 受击框长度 的 1/6, 后续 可 针对单个技能可配
            if (Mathf.Abs(self.RootPosY - anim.transform.parent.position.y) > height / 6f)
            {
                return false;
            }
            //检查攻击框 与 所有 受击框 是否有交集
            Vector3 pos = self.transform.position + (Vector3) attackBox.Offset;
            pos = pos - (Vector3) attackBox.Size / 2f;
            Rect attackRect = new Rect(pos, attackBox.Size);
            while (beHitBoxList.Count > 0)
            {
                Rect beHitRect = beHitBoxList.Dequeue();
                if (beHitRect.Overlaps(attackRect)) return true;
            }
            return false;
        }

        private static bool CheckSkeletonCollider(ESkeletonAnimation self, ESkeletonAnimation anim, int frameIndex)
        {
            if (self.CurrentState.FrameDic.Count <= 0)
            {
                return false;
            }
            Dictionary<int, FrameData> selfFrameData = self.CurrentState.FrameDic;
            Dictionary<int, FrameData> animFrameData = anim.CurrentState.FrameDic;
            List<BoxItem> beHitBoxList = null;
            //是否使用 当前状态受击框
            if (anim.DefaultState.UseCurSBeHitBox)
            {
                if (animFrameData.ContainsKey(0) && animFrameData[0].ApplyBeHitBoxToAllFrame)
                {
                    beHitBoxList = animFrameData[0].BeHitRangeList;
                }
                else if (animFrameData.ContainsKey(anim.CurrentFrameCount))
                {
                    beHitBoxList = animFrameData[anim.CurrentFrameCount].BeHitRangeList;
                }
            }
            else
            {
                //如果不使用 当前 状态默认受击框 或者 当前状态当前帧 受击框，则 使用 角色默认 状态 受击框
                Dictionary<int, FrameData> animDefaultFrameData = anim.DefaultState.FrameDic;
                if (animDefaultFrameData.ContainsKey(0))
                {
                    beHitBoxList = animDefaultFrameData[0].BeHitRangeList;
                }
            }
            if (beHitBoxList == null)
            {
                return false;
            }
            List<BoxItem> attackBoxList = selfFrameData[frameIndex].AttackedRangeList;
            return CheckBoxOverlap(self, anim, attackBoxList, beHitBoxList);
        }

        private static bool CheckBoxOverlap(ESkeletonAnimation self, ESkeletonAnimation anim, List<BoxItem> attackBoxList, List<BoxItem> beHitRangeList)
        {
            float height = 0f;
            Queue<Rect> beHitBoxList = GetAnimBeHitBoxInternal(anim, beHitRangeList, out height);
            // z 轴 攻击距离 暂定 为 受击框长度 的 1/6, 后续 可 针对单个技能可配
            if (Mathf.Abs(self.transform.parent.position.y - anim.transform.parent.position.y) > height / 6f)
            {
                return false;
            }
            //检查所有攻击框 与 所有 受击框 两两 是否有交集
            foreach (var boxItem in attackBoxList)
            {
                Vector3 pos = self.transform.position +
                              new Vector3(boxItem.Offset.x * self.skeleton.ScaleX, boxItem.Offset.y);
                pos = pos - (Vector3) boxItem.Size / 2f;
                Rect newRect = new Rect(pos, boxItem.Size);
                while (beHitBoxList.Count > 0)
                {
                    Rect popRect = beHitBoxList.Dequeue();
                    if (popRect.Overlaps(newRect)) return true;
                }
            }
            return false;
        }

        private static Queue<Rect> GetAnimBeHitBoxInternal(ESkeletonAnimation anim, List<BoxItem> beHitRangeList, out float height)
        {
            Queue<Rect> beHitBoxList = new Queue<Rect>();
            Vector2 maxHeight = new Vector2();
            foreach (var box in beHitRangeList)
            {
                Vector3 pos = anim.transform.position + new Vector3(box.Offset.x * anim.skeleton.ScaleX, box.Offset.y);
                pos = pos - (Vector3) box.Size / 2f;
                Rect newRect = new Rect(pos, box.Size);
                beHitBoxList.Enqueue(newRect);
                float newBottom = box.Offset.y - box.Size.y / 2f;
                maxHeight.x = maxHeight.x > newBottom ? newBottom : maxHeight.x;
                float newTop = box.Offset.y + box.Size.y / 2f;
                maxHeight.y = maxHeight.y < newTop ? newTop : maxHeight.y;
            }
            height = Mathf.Abs(maxHeight.y - maxHeight.x);
            return beHitBoxList;
        }
    }
}