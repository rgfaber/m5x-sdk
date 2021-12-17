using System.IO;
using System.Reflection;

namespace M5x.Camunda.Transfer;

public class FileParameter
{
    public FileParameter(byte[] file) : this(file, null)
    {
    }

    public FileParameter(byte[] file, string filename) : this(file, filename, null)
    {
    }

    public FileParameter(byte[] file, string filename, string contenttype)
    {
        File = file;
        FileName = filename;
        ContentType = contenttype;
    }

    public byte[] File { get; }
    public string FileName { get; }
    public string ContentType { get; }

    public static FileParameter FromManifestResource(Assembly assembly, string resourcePath)
    {
        var resourceAsStream = assembly.GetManifestResourceStream(resourcePath);
        byte[] resourceAsBytearray;
        using (var ms = new MemoryStream())
        {
            resourceAsStream.CopyTo(ms);
            resourceAsBytearray = ms.ToArray();
        }

        // TODO: Verify if this is the correct way of doing it:
        var assemblyBaseName = assembly.GetName().Name;
        var fileLocalName = resourcePath.Replace(assemblyBaseName + ".", "");

        return new FileParameter(resourceAsBytearray, fileLocalName);
    }
}