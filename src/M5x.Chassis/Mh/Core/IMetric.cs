using System.Text;

namespace M5x.Chassis.Mh.Core;

public interface IMetric : ICopyable<IMetric>
{
    void LogJson(StringBuilder sb);
}