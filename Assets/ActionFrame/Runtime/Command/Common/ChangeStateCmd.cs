using Spine;

namespace ActionFrame.Runtime
{
    public class ChangeStateCmd : BaseCmd
    {
        public TrackEntry ChangeState(ESkeletonAnimation self, StateData data)
        {
            if (self.CurrentTrack != null && self.CurrentTrack.Animation.Name == data.NextStateName)
            {
                return self.CurrentTrack;
            }
            if (self.CurrentTrack == null)
            {
                self.CurrentTrack = self.AnimationState.SetAnimation(0, data.StateName, data.IsLoop);
            }
            else
            {
                self.ForwardTrack = self.CurrentTrack;
                self.AnimationState.Data.SetMix(self.ForwardTrack?.Animation.Name, data.NextStateName, data.TransitionTime);
                self.CurrentTrack = self.ForwardTrack.MixingTo;
            }
            return self.CurrentTrack;
        }
    }
}