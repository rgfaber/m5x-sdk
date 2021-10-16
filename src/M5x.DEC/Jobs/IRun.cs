namespace M5x.DEC.Jobs
{
    public interface IRun<in TJob>
    {
        bool Run(TJob job);
    }
}