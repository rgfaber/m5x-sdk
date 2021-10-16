using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace M5x.Swagger
{
    public interface IApiSettings
    {
        int Port { get; set; }
        IPAddress Address { get; set; }

        string Id { get; set; }

        string Version { get; }

        string Key { get; }
        string DisplayName { get; set; }
        string Description { get; set; }
        IEnumerable<string> Tags { get; set; }
        OpenApiContact Contact { get; set; }
        Dictionary<string, object> Extensions { get; set; }
        OpenApiLicense License { get; set; }
        Uri TermsOfService { get; set; }

        string TenantKey { get; set; }
    }


    public class ApiSettings : IApiSettings
    {
        public ApiSettings(string id, string version, string tenantKey, string displayName, string[] tags)
        {
            Id = id;
            TenantKey = tenantKey;
            DisplayName = displayName;
            tags.ToList().AddRange(tags);
        }


        public ApiSettings(string id,
            string version,
            string tenantKey,
            string displayName,
            string[] tags,
            string termsOfServiceUrl)
        {
            Id = id;
            TenantKey = tenantKey;
            DisplayName = displayName;
            tags.ToList().AddRange(tags);
            TermsOfService = new Uri(termsOfServiceUrl);
        }


        public int Port { get; set; }
        public IPAddress Address { get; set; }
        public string Id { get; set; }

        public string Version => $"{Assembly.GetEntryAssembly()?.GetName().Version}";

        public string Key => $"{TenantKey}-{Id}-{Version}";
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public IEnumerable<string> Tags { get; set; } = new[] { "api" };

        public OpenApiContact Contact { get; set; }
        public Dictionary<string, object> Extensions { get; set; }
        public OpenApiLicense License { get; set; }
        public Uri TermsOfService { get; set; }
        public string TenantKey { get; set; }
    }
}