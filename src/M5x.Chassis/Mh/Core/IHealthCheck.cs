namespace M5x.Chassis.Mh.Core
{
    public interface IHealthCheck
    {
        string Name { get; }
        IMetric Execute();
    }
}