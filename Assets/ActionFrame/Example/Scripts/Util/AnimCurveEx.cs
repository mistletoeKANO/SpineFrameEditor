using UnityEngine;

namespace ActionFrame.Runtime
{
    public static class AnimCurveEx
    {
        private static GameCurveCtr _curveCtr;

        static AnimCurveEx()
        {
            if (_curveCtr == null)
            {
                GameObject obj = new GameObject("AnimCurve");
                _curveCtr = obj.AddComponent<GameCurveCtr>();
            }
        }
        public static void MoveSin(this UnityEngine.Transform self, float value, float time)
        {
            _curveCtr.DoJumpCurve(self, value, time);
        } 
    }
}