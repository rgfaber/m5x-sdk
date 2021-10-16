using System;

namespace M5x.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbNameAttribute : Attribute
    {
        public DbNameAttribute(string dbName)
        {
            DbName = dbName;
        }

        public string DbName { get; set; }
    }
}