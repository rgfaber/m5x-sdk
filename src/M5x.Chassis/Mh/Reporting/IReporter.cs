using System;

namespace M5x.Chassis.Mh.Reporting;

internal interface IReporter : IDisposable
{
    void Run();
    void Start(long period, TimeUnit unit);
    void Stop();

    event EventHandler<EventArgs> Started;
    event EventHandler<EventArgs> Stopped;
}