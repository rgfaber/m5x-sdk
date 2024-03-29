﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using M5x.Camunda.Requests;
using M5x.Camunda.Transfer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace M5x.Camunda.Service;

public class ExternalTaskService2
{
    private readonly CamundaClientHelper helper;

    public ExternalTaskService2(CamundaClientHelper client)
    {
        helper = client;
    }

    public IList<ExternalTask> FetchAndLockTasks(string workerId, int maxTasks, string topicName,
        long lockDurationInMilliseconds, IEnumerable<string> variablesToFetch = null)
    {
        return FetchAndLockTasks(workerId, maxTasks, new List<string> { topicName }, lockDurationInMilliseconds,
            variablesToFetch);
    }

    public IList<ExternalTask> FetchAndLockTasks(string workerId, int maxTasks, IEnumerable<string> topicNames,
        long lockDurationInMilliseconds, IEnumerable<string> variablesToFetch = null)
    {
        var lockRequest = new FetchAndLockRequest
        {
            WorkerId = workerId,
            MaxTasks = maxTasks
        };
        //if (longPolling)
        //{
        //    lockRequest.AsyncResponseTimeout = 1 * 60 * 1000; // 1 minute
        //}
        foreach (var topicName in topicNames)
        {
            var lockTopic = new FetchAndLockTopic
            {
                TopicName = topicName,
                LockDuration = lockDurationInMilliseconds,
                Variables = variablesToFetch
            };
            lockRequest.Topics.Add(lockTopic);
        }

        return FetchAndLockTasks(lockRequest);
    }

    public IList<ExternalTask> FetchAndLockTasks(FetchAndLockRequest fetchAndLockRequest)
    {
        var http = helper.HttpClient();
        try
        {
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(fetchAndLockRequest,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
            var response = http.PostAsync("external-task/fetchAndLock", requestContent).Result;
            if (response.IsSuccessStatusCode)
            {
                var tasks = JsonConvert.DeserializeObject<IEnumerable<ExternalTask>>(response.Content
                    .ReadAsStringAsync().Result);
                return new List<ExternalTask>(tasks);
            }

            throw new EngineException("Could not fetch and lock tasks: " + response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, ex.Message);
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public void Complete(string workerId, string externalTaskId,
        Dictionary<string, object> variablesToPassToProcess = null)
    {
        var http = helper.HttpClient();

        var request = new CompleteRequest();
        request.WorkerId = workerId;
        request.Variables = CamundaClientHelper.ConvertVariables(variablesToPassToProcess);

        var requestContent =
            new StringContent(
                JsonConvert.SerializeObject(request,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
        var response = http.PostAsync("external-task/" + externalTaskId + "/complete", requestContent).Result;
        if (!response.IsSuccessStatusCode)
            throw new EngineException("Could not complete external Task: " + response.ReasonPhrase);
    }

    public void Error(string workerId, string externalTaskId, string errorCode)
    {
        var http = helper.HttpClient();

        var request = new BpmnErrorRequest();
        request.WorkerId = workerId;
        request.ErrorCode = errorCode;

        var requestContent =
            new StringContent(
                JsonConvert.SerializeObject(request,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
        var response = http.PostAsync("external-task/" + externalTaskId + "/bpmnError", requestContent).Result;
        if (!response.IsSuccessStatusCode)
            throw new EngineException("Could not report BPMN error for external Task: " + response.ReasonPhrase);
    }

    public void Failure(string workerId, string externalTaskId, string errorMessage, int retries, long retryTimeout)
    {
        var http = helper.HttpClient();

        var request = new FailureRequest();
        request.WorkerId = workerId;
        request.ErrorMessage = errorMessage;
        request.Retries = retries;
        request.RetryTimeout = retryTimeout;

        var requestContent =
            new StringContent(
                JsonConvert.SerializeObject(request,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
        var response = http.PostAsync("external-task/" + externalTaskId + "/failure", requestContent).Result;
        if (!response.IsSuccessStatusCode)
            throw new EngineException("Could not report failure for external Task: " + response.ReasonPhrase);
    }
}