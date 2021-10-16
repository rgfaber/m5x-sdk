using System;
using M5x.DEC.Schema;

namespace Robby.Robot.Schema
{
    public record Robot
    {
        public record ID : Identity<ID>
        {
            public ID(string value) : base(value)
            {
            }
        }

        [Flags]
        public enum Flags
        {
            Unknown = 1,
            
        }
        
        
        public Robot() {}
        
        public string Id { get; set; }
        public string Name { get; set; }
        
    }
}
