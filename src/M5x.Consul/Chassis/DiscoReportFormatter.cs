using Newtonsoft.Json;

namespace M5x.Consul.Chassis;

public class DiscoReportFormatter
{
    private readonly IDiscoInspector _inspector;

    public DiscoReportFormatter(IDiscoInspector insp)
    {
        _inspector = insp;
    }

    public string GetOutput()
    {
        return JsonConvert.SerializeObject(_inspector);
    }
}