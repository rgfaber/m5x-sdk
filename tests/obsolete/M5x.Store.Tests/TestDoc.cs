using System;
using System.Collections.Generic;
using M5x.Networking;

namespace M5x.Store.Tests
{
    public class TestDoc
    {
        public string Id { get; set; }
        public string Rev { get; set; }
        public string Name { get; set; }
    }


    public static class DocFactory
    {
        public static TestDoc LocalDoc => new()
        {
            Id = "testdoc:local:latest"
//            Items = DataFactory.GenerateDemoItems()
        };

        public static TestDoc ClusterDoc => new()
        {
            Id = $"testdoc:cluster:{DateTime.UtcNow}"
//            Items = DataFactory.GenerateDemoItems()
        };


        public static IEnumerable<TestDoc> GenerateLocalTestDocs(int count)
        {
            var res = new List<TestDoc>();
            var i = 0;
            while (i < count)
            {
                i++;
                res.Add(new TestDoc
                {
                    Id = $"testdoc:{NetUtils.GetHostName()}:{i}"
//                    Items = DataFactory.GenerateDemoItems()
                });
            }

            return res;
        }


        public static IEnumerable<TestDoc> GenerateClusterTestDocs(int count)
        {
            var res = new List<TestDoc>();
            var i = 0;
            while (i < count)
            {
                i++;
                res.Add(new TestDoc
                {
                    Id = $"testdoc:cluster:{i}"
                    //                 Items = DataFactory.GenerateDemoItems()
                });
            }

            return res;
        }
    }
}