using System.Collections.Generic;
using System.IO;
using System.Linq;
using Docker.DotNet.Models;
using M5x.Docker.Models;
using Serilog;

namespace M5x.Docker
{
    public static class DockerExtensions
    {
        public static IDictionary<string, EmptyStruct> ToEmptyStructDict(this IList<string> items)
        {
            return items?
                .ToDictionary(item => item, item => new EmptyStruct());
        }

        public static IDictionary<string, IList<PortBinding>> ToPortBindings(this IDictionary<string, string> source)
        {
            var res = new Dictionary<string, IList<PortBinding>>();
            foreach (var it in source)
            {
                var pbl = new List<PortBinding>
                {
                    new()
                    {
                        HostIP = "localhost",
                        HostPort = it.Value
                    }
                };
                res.Add(it.Key, pbl);
            }

            return res;
        }


        public static NetworkCreateInfo ToNetworkCreateInfo(this NetworksCreateResponse src)
        {
            return new NetworkCreateInfo
            {
                Id = src.ID,
                Warnings = src.Warning
            };
        }

        public static IPAM ToIPAM(this IPAMInfo info)
        {
            return new IPAM
            {
                Driver = info.Driver,
                Options = info.Options,
                Config = info.Config.ToIPAMConfigs()
            };
        }


        public static IList<IPAMConfig> ToIPAMConfigs(this IList<IPAMConfigInfo> src)
        {
            return src
                .Select(item => item.ToIPAMConfig())
                .ToList();
        }


        public static IPAMConfig ToIPAMConfig(this IPAMConfigInfo config)
        {
            return new IPAMConfig
            {
                AuxAddress = config.AuxAddress,
                Gateway = config.Gateway,
                IPRange = config.IPRange,
                Subnet = config.Subnet
            };
        }


        public static void WriteStartResult(this ContainerListResponse res)
        {
            Log.Debug($"Started Service: {res.Image}");
            foreach (var it in res.Names)
                Log.Debug($" - {it}");
        }


        public static string AsString(this Stream sIn)
        {
            if (sIn.CanSeek)
                sIn.Position = 0;
            var sr = new StreamReader(sIn);
            var s = sr.ReadToEnd();
            return s;
        }
    }
}