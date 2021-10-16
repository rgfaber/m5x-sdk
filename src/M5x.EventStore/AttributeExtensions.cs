using System;
using Ardalis.GuardClauses;

namespace M5x.EventStore
{
       
        public static class AttributeExtensions
        {
            public static void AttributeNotDefined(this IGuardClause clause, string attributeName, Attribute[] attributes,
                string className)
            {
                Guard.Against.Null(attributes, nameof(attributes));
                if (attributes.Length == 0)
                    throw new Exception($"Attribute [{attributeName}] is not defined on class {className}");
            }
            
            
            public static string GroupName<TEsSubscriber>(this TEsSubscriber subscriber) where TEsSubscriber:class
            {
                var atts = (GroupNameAttribute[])typeof(TEsSubscriber).GetCustomAttributes(typeof(GroupNameAttribute), true);
                Guard.Against.AttributeNotDefined("GroupName", atts, typeof(TEsSubscriber).Name);
                return atts[0].GroupName;
            }
            
            public static string StreamName<TEsSubscriber>(this TEsSubscriber subscriber) where TEsSubscriber:class
            {
                var atts = (StreamNameAttribute[])typeof(TEsSubscriber).GetCustomAttributes(typeof(StreamNameAttribute), true);
                Guard.Against.AttributeNotDefined("StreamName", atts, typeof(TEsSubscriber).Name);
                return atts[0].StreamName;
            }
            
        }
        

}