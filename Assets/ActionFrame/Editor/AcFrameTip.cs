using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace ActionFrame.Editor
{
    public class AcFrameTip
    {
        private VisualElement m_Root;
        private Dictionary<int, TipFormInfo> TipDic;

        public void Init(VisualElement root)
        {
            this.TipDic = new Dictionary<int, TipFormInfo>();
            this.m_Root = root;
        }

        public void UpdateTipTime(float dealtTime)
        {
            if (TipDic.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < TipDic.Count; i++)
            {
                var tip = TipDic.ElementAt(i);
                tip.Value.RunTime += dealtTime;
                if (tip.Value.RunTime >= tip.Value.Duration)
                {
                    this.m_Root.Remove(tip.Value.TipForm);
                    TipDic.Remove(tip.Key);
                }
            }
        }

        public void ShowTip(string tipMessage, float duration)
        {
            int tipId = GenerateTipId();
            TipFormInfo newForm = new TipFormInfo();
            newForm.TipForm = new Label(tipMessage);
            newForm.TipForm.name = "TipForm";
            this.m_Root.Add(newForm.TipForm);
            newForm.Duration = duration;
            newForm.RunTime = 0f;
            TipDic.Add(tipId, newForm);
            
            float leftOffset = this.m_Root.layout.width / 2.4f;
            float topOffset = (this.m_Root.layout.height - newForm.TipForm.style.height.value.value) / 3f;
            newForm.TipForm.style.left = leftOffset;
            newForm.TipForm.style.top = topOffset;
        }

        private int GenerateTipId()
        {
            int id = 11010;
            while (TipDic.Keys.Contains(id))
            {
                id++;
            }
            return id;
        }
        
        private class TipFormInfo
        {
            public Label TipForm;
            public float Duration;
            public float RunTime;
        }
    }
}