namespace M5x.Camunda.Requests;

internal class BpmnErrorRequest
{
    public string WorkerId { get; set; }
    public string ErrorCode { get; set; }
}