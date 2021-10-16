using System;
using System.Collections.Generic;

namespace M5x.Camunda.Worker
{
    [AttributeUsage(AttributeTargets.Class |
                    AttributeTargets.Struct)
    ]
    public sealed class ExternalTaskVariableRequirementsAttribute : Attribute
    {
        public ExternalTaskVariableRequirementsAttribute()
        {
            VariablesToFetch = new List<string>();
        }

        public ExternalTaskVariableRequirementsAttribute(params string[] variablesToFetch)
        {
            VariablesToFetch = new List<string>(variablesToFetch);
        }

        public List<string> VariablesToFetch { get; }
    }
}