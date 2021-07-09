namespace ActionFrame.Runtime
{
    public abstract class BaseHandle
    {
        public object config;
        public void InitConfig(object data)
        {
            this.config = data;
        }
        public abstract void StartHandle(ESkeletonAnimation hero);
        public abstract void UpdateHandle(ESkeletonAnimation hero, float dealtTime);
        public abstract void ExitHandle(ESkeletonAnimation hero);
    }
}