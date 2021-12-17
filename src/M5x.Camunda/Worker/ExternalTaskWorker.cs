using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using M5x.Camunda.Service;
using M5x.Camunda.Transfer;
using Serilog;

namespace M5x.Camunda.Worker;

public class ExternalTaskWorker : IDisposable
{
    private readonly ExternalTaskService2 _externalTaskService2;
    private readonly long lockDurationInMilliseconds = 1 * 60 * 1000; // 1 minute
    private readonly int maxDegreeOfParallelism = 2;
    private readonly int maxTasksToFetchAtOnce = 10;
    private readonly long pollingIntervalInMilliseconds = 50; // every 50 milliseconds
    private readonly ExternalTaskWorkerInfo taskWorkerInfo;
    private readonly string workerId = Guid.NewGuid().ToString(); // TODO: Make configurable

    private Timer taskQueryTimer;

    public ExternalTaskWorker(ExternalTaskService2 externalTaskService2, ExternalTaskWorkerInfo taskWorkerInfo)
    {
        _externalTaskService2 = externalTaskService2;
        this.taskWorkerInfo = taskWorkerInfo;
    }

    public void Dispose()
    {
        if (taskQueryTimer != null) taskQueryTimer.Dispose();
    }

    public void DoPolling()
    {
        // Query External Tasks
        try
        {
            var tasks = _externalTaskService2.FetchAndLockTasks(workerId, maxTasksToFetchAtOnce,
                taskWorkerInfo.TopicName, lockDurationInMilliseconds, taskWorkerInfo.VariablesToFetch);

            // run them in parallel with a max degree of parallelism
            Parallel.ForEach(
                tasks,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                externalTask => Execute(externalTask)
            );
        }
        catch (EngineException ex)
        {
            // Most probably server is not running or request is invalid
            Log.Fatal(ex, ex.Message);
        }
        finally
        {
            Log.CloseAndFlush();
        }

        // schedule next run (if not stopped in between)
        taskQueryTimer?
            .Change(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(Timeout.Infinite));
    }

    private void Execute(ExternalTask externalTask)
    {
        var resultVariables = new Dictionary<string, object>();

        Log.Information($"Execute External Task from topic '{taskWorkerInfo.TopicName}': {externalTask}...");
        try
        {
            taskWorkerInfo.TaskAdapter.Execute(externalTask, ref resultVariables);
            Log.Information($"Finished External Task {externalTask.Id}");
            _externalTaskService2.Complete(workerId, externalTask.Id, resultVariables);
        }
        catch (UnrecoverableBusinessErrorException ex)
        {
            Log.Error($"Failed with business error code from External Task  {externalTask.Id}");
            _externalTaskService2.Error(workerId, externalTask.Id, ex.BusinessErrorCode);
        }
        catch (Exception ex)
        {
            Log.Error($"...failed External Task  {externalTask.Id}");
            Log.Fatal(ex, ex.Message);
            var retriesLeft = taskWorkerInfo.Retries; // start with default
            if (externalTask.Retries.HasValue) // or decrement if retries are already set
                retriesLeft = externalTask.Retries.Value - 1;
            _externalTaskService2.Failure(workerId, externalTask.Id, ex.Message, retriesLeft,
                taskWorkerInfo.RetryTimeout);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public void StartWork()
    {
        taskQueryTimer = new Timer(_ => DoPolling(), null, pollingIntervalInMilliseconds, Timeout.Infinite);
    }

    public void StopWork()
    {
        taskQueryTimer.Dispose();
        taskQueryTimer = null;
    }
}