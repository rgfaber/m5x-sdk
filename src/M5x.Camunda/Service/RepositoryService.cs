using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using M5x.Camunda.Requests;
using M5x.Camunda.Transfer;
using Newtonsoft.Json;
using Serilog;

namespace M5x.Camunda.Service;

public class RepositoryService
{
    private readonly CamundaClientHelper helper;

    public RepositoryService(CamundaClientHelper helper)
    {
        this.helper = helper;
    }


    public List<ProcessDefinition> LoadProcessDefinitions(bool onlyLatest)
    {
        var http = helper.HttpClient();
        var response = http.GetAsync("process-definition/?latestVersion=" + (onlyLatest ? "true" : "false")).Result;
        if (response.IsSuccessStatusCode)
        {
            var result =
                JsonConvert.DeserializeObject<IEnumerable<ProcessDefinition>>(response.Content.ReadAsStringAsync()
                    .Result);

            // Could be extracted into separate method call if you run a lot of process definitions and want to optimize performance
            foreach (var pd in result)
            {
                http = helper.HttpClient();
                var response2 = http.GetAsync("process-definition/" + pd.Id + "/startForm").Result;
                var startForm =
                    JsonConvert.DeserializeObject<StartForm>(response2.Content.ReadAsStringAsync().Result);

                pd.StartFormKey = startForm.Key;
            }

            return new List<ProcessDefinition>(result);
        }

        return new List<ProcessDefinition>();
    }


    public string LoadProcessDefinitionXml(string processDefinitionId)
    {
        var http = helper.HttpClient();
        var response = http.GetAsync("process-definition/" + processDefinitionId + "/xml").Result;
        if (response.IsSuccessStatusCode)
        {
            var processDefinitionXml =
                JsonConvert.DeserializeObject<ProcessDefinitionXml>(response.Content.ReadAsStringAsync().Result);
            return processDefinitionXml.Bpmn20Xml;
        }

        return null;
    }

    public void DeleteDeployment(string deploymentId)
    {
        var http = helper.HttpClient();
        var response = http.DeleteAsync("deployment/" + deploymentId + "?cascade=true").Result;
        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = response.Content.ReadAsStringAsync();
            throw new EngineException(response.ReasonPhrase);
        }
    }

    public string Deploy(string deploymentName, List<object> files)
    {
        var postParameters = new Dictionary<string, object>();
        postParameters.Add("deployment-name", deploymentName);
        postParameters.Add("deployment-source", "C# Process Application");
        postParameters.Add("enable-duplicate-filtering", "true");
        postParameters.Add("data", files);

        // Create request and receive response
        var postURL = helper.RestUrl + "deployment/create";
        var webResponse =
            FormUpload.MultipartFormDataPost(postURL, helper.RestUsername, helper.RestPassword, postParameters);

        using (var reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
        {
            var deployment = JsonConvert.DeserializeObject<Deployment>(reader.ReadToEnd());
            return deployment.Id;
        }
    }

    public void AutoDeploy()
    {
        var thisExe = Assembly.GetEntryAssembly();
        var resources = thisExe.GetManifestResourceNames();

        if (resources.Length == 0) return;

        var files = new List<object>();
        foreach (var resource in resources)
        {
            // TODO Check if Camunda relevant (BPMN, DMN, HTML Forms)

            // Read and add to Form for Deployment                
            files.Add(FileParameter.FromManifestResource(thisExe, resource));

            Log.Information("Adding resource to deployment: " + resource);
        }

        Deploy(thisExe.GetName().Name, files);

        Log.Information("Deployment to Camunda BPM succeeded.");
    }
}