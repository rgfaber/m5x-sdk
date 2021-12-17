﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using M5x.Camunda.Requests;
using M5x.Camunda.Transfer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace M5x.Camunda.Service;

public class BpmnWorkflowService
{
    private readonly CamundaClientHelper helper;

    public BpmnWorkflowService(CamundaClientHelper client)
    {
        helper = client;
    }

    public string StartProcessInstance(string processDefinitionKey, Dictionary<string, object> variables)
    {
        return StartProcessInstance(processDefinitionKey, null, variables);
    }

    public string StartProcessInstance(string processDefinitionKey, string businessKey,
        Dictionary<string, object> variables)
    {
        var http = helper.HttpClient();

        var request = new CompleteRequest();
        request.Variables = CamundaClientHelper.ConvertVariables(variables);
        request.BusinessKey = businessKey;

        var requestContent =
            new StringContent(
                JsonConvert.SerializeObject(request,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
        var response = http.PostAsync("process-definition/key/" + processDefinitionKey + "/start", requestContent)
            .Result;
        if (response.IsSuccessStatusCode)
        {
            var processInstance =
                JsonConvert.DeserializeObject<ProcessInstance>(response.Content.ReadAsStringAsync().Result);
            return processInstance.Id;
        }

        //var errorMsg = response.Content.ReadAsStringAsync();
        throw new EngineException(response.ReasonPhrase);
    }

    public Dictionary<string, object> LoadVariables(string taskId)
    {
        var http = helper.HttpClient();

        var response = http.GetAsync("task/" + taskId + "/variables").Result;
        if (response.IsSuccessStatusCode)
        {
            // Successful - parse the response body
            var variableResponse =
                JsonConvert.DeserializeObject<Dictionary<string, Variable>>(response.Content.ReadAsStringAsync()
                    .Result);

            var variables = new Dictionary<string, object>();
            foreach (var variable in variableResponse) variables.Add(variable.Key, variable.Value.Value);
            return variables;
        }

        throw new EngineException("Could not load variable: " + response.ReasonPhrase);
    }

    public IList<ProcessInstance> LoadProcessInstances(IDictionary<string, string> queryParameters)
    {
        var queryString = string.Join("&", queryParameters.Select(x => x.Key + "=" + x.Value));
        var http = helper.HttpClient();

        var response = http.GetAsync("process-instance/?" + queryString).Result;
        if (response.IsSuccessStatusCode)
        {
            // Successful - parse the response body
            var instances =
                JsonConvert.DeserializeObject<IEnumerable<ProcessInstance>>(response.Content.ReadAsStringAsync()
                    .Result);
            return new List<ProcessInstance>(instances);
        }

        //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
        throw new EngineException("Could not load process instances: " + response.ReasonPhrase);
    }
}