﻿namespace M5x.EventStore
{
    // public class PrivateSetterContractResolver : DefaultContractResolver
    // {
    //     protected override JsonProperty CreateProperty(
    //         MemberInfo member,
    //         MemberSerialization memberSerialization)
    //     {
    //         var prop = base.CreateProperty(member, memberSerialization);
    //
    //         if (!prop.Writable)
    //         {
    //             var property = member as PropertyInfo;
    //             if (property != null)
    //             {
    //                 var hasPrivateSetter = property.GetSetMethod(true) != null;
    //                 prop.Writable = hasPrivateSetter;
    //             }
    //         }
    //
    //         return prop;
    //     }
    // }
}