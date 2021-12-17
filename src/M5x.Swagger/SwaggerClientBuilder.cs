using System.Net;
using System.Threading.Tasks;
using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace M5x.Swagger;

public class SwaggerClientBuilder
{
    public async Task<string> BuildPoco(string url, string nameSpace, string className)
    {
        var wclient = new WebClient();

        var document = await OpenApiDocument.FromJsonAsync(wclient.DownloadString(url));

        wclient.Dispose();

        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = className,
            CSharpGeneratorSettings =
            {
                Namespace = nameSpace
            }
        };

        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();
        return code;
    }
}