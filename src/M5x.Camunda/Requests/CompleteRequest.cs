using System.Collections.Generic;
using M5x.Camunda.Transfer;

namespace M5x.Camunda.Requests
{
    internal class CompleteRequest
    {
        public string BusinessKey { get; set; }
        public Dictionary<string, Variable> Variables { get; set; }
        public string WorkerId { get; set; }
    }
}