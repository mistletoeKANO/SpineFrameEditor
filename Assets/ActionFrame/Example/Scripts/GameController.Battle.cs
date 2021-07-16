using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public partial class GameController
    {
        private void OnAddListener()
        {
            EventManager.Instance.AddEventListener<Event_HitAnim>(this.HitAnimHandle);
        }

        private void OnRemoveListener()
        {
            EventManager.Instance.RemoveEventListener<Event_HitAnim>(this.HitAnimHandle);
        }

        private void HitAnimHandle(Event_HitAnim e)
        {
            if (!this.m_Hero.CurrentState.FrameDic.ContainsKey(e.FrameIndex))
            {
                return;
            }

            List<ESkeletonAnimation> beHitAnim = this.CheckBoxOverlap(this.m_Hero, this.m_Monster, e.FrameIndex);
            if (beHitAnim.Count == 0) return;
            foreach (var monster in beHitAnim)
            {
                monster.ChangeStateNoCtr("hit", false, 0);
                monster.transform.parent.GetComponent<Rigidbody2D>().AddForce(e.HitForce * this.m_Hero.skeleton.ScaleX);
            }
        }

        private List<ESkeletonAnimation> CheckBoxOverlap(ESkeletonAnimation self, List<ESkeletonAnimation> animList, int frameIndex)
        {
            List<ESkeletonAnimation> beHitAnim = new List<ESkeletonAnimation>();
            FrameData frameData = self.CurrentState.FrameDic[frameIndex];
            Queue<Rect> beHitBoxList = new Queue<Rect>();
            foreach (var monster in animList)
            {
                FrameData monsterHitBox = monster.DefaultState.FrameDic[0];
                Vector2 maxHeight = new Vector2();
                foreach (var box in monsterHitBox.BeHitRangeList)
                {
                    Vector3 pos = monster.transform.position + (Vector3) box.Offset;
                    Rect newRect = new Rect(pos, box.Size);
                    beHitBoxList.Enqueue(newRect);
                    float newBottom = box.Offset.y - box.Size.y / 2f;
                    maxHeight.x = maxHeight.x > newBottom ? newBottom : maxHeight.x;
                    float newTop = box.Offset.y + box.Size.y / 2f;
                    maxHeight.y = maxHeight.y < newTop ? newTop : maxHeight.y;
                }
                float height = Mathf.Abs(maxHeight.y - maxHeight.x);
                // z 轴 攻击距离 暂定 为 受击框长度 的 1/8
                if (Mathf.Abs(self.transform.position.y - monster.transform.position.y) > height / 6f)
                {
                    continue;
                }
                foreach (var boxItem in frameData.AttackedRangeList)
                {
                    bool isHit = false;
                    Vector3 pos = self.transform.position +
                                  new Vector3(boxItem.Offset.x * self.skeleton.ScaleX, boxItem.Offset.y);
                    Rect newRect = new Rect(pos, boxItem.Size);
                    while (beHitBoxList.Count > 0)
                    {
                        Rect popRect = beHitBoxList.Dequeue();
                        if (popRect.Overlaps(newRect))
                        {
                            beHitAnim.Add(monster);
                            isHit = true;
                            break;
                        }
                    }
                    if (isHit)
                    {
                        break;
                    }
                }
                beHitBoxList.Clear();
            }
            return beHitAnim;
        }
    }
}