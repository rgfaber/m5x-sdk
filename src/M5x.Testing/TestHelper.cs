using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using AutoFixture;
using M5x.Testing.Interfaces;

namespace M5x.Testing;

internal class TestHelper : ITestHelper
{
    private readonly Fixture _fix;

    public TestHelper()
    {
        _fix = CreateFixture();
    }

    public string GetXml(byte[] fileData)
    {
        var doc1 = new XmlDocument();
        var ms1 = new MemoryStream(fileData);
        doc1.Load(ms1);
        return doc1.InnerXml;
    }

    public byte[] GetBytes(string filepath)
    {
        var doc = new XmlDocument();
        doc.Load(Path.Combine(GetDirPath(), filepath));

        var ms = new MemoryStream();
        doc.Save(ms);
        var data = ms.ToArray();
        return data;
    }

    public string GetEmbeddedResourceLocation(string filename)
    {
        var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
        var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
        var dirPath = Path.GetDirectoryName(codeBasePath);
        return Path.Combine(dirPath, filename);
    }

    public string GetEmbeddedResourceText(string resourceName)
    {
        var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        using var sIn = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        var s = StreamToString(sIn);
        return s;
    }

    public Fixture CreateFixture()
    {
        var fix = new Fixture();
        fix.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => fix.Behaviors.Remove(b));
        fix.Behaviors.Add(new OmitOnRecursionBehavior());
        return fix;
    }


    public IEnumerable<T> CreateMany<T>(int count = 3)
    {
        return _fix.CreateMany<T>(count);
    }


    public T Create<T>()
    {
        return _fix.Create<T>();
    }

    private static string StreamToString(Stream stream)
    {
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static string GetDirPath()
    {
        var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
        var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
        var dirPath = Path.GetDirectoryName(codeBasePath);
        return dirPath;
    }
}