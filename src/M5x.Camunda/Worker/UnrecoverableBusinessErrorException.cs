using System;

namespace M5x.Camunda.Worker
{
    public class UnrecoverableBusinessErrorException : Exception
    {
        public UnrecoverableBusinessErrorException(string businessErrorCode, string message)
            : base(message)
        {
            BusinessErrorCode = businessErrorCode;
        }

        public string BusinessErrorCode { get; set; }
    }
}