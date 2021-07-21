﻿using System.Collections.Generic;
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

            List<ESkeletonAnimation> beHitAnim = this.m_Hero.RectColliderEnter(this.m_Monster, e.FrameIndex);
            if (beHitAnim.Count == 0) return;
            foreach (var monster in beHitAnim)
            {
                monster.ChangeStateNoCtr("hit", false, 0);
                monster.transform.parent.GetComponent<Rigidbody2D>().AddForce(e.HitForce * this.m_Hero.skeleton.ScaleX);
                monster.DelayFrame += 6;
            }
            this.m_Hero.DelayFrame += 6;
        }
    }
}