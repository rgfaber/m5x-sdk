// ***********************************************************************
// <copyright file="TypeExtensions.cs" company="Flint Group">
//     Copyright (c) Flint Group. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using M5x.Utils;

namespace M5x.Extensions
{
    /// <summary>
    ///     Class TypeExtensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Knowns the types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;Type&gt;.</returns>
        public static IEnumerable<Type> KnownTypes(this Type type)
        {
            return Attribute
                .GetCustomAttributes(type)
                .OfType<KnownTypeAttribute>()
                .Select(attr => attr.Type);
        }


        /// <summary>
        ///     Gets the embedded file.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Stream.</returns>
        public static Stream GetEmbeddedFile(this Type type, string fileName)
        {
            return type.Assembly.GetEmbeddedFile(fileName);
        }


        /// <summary>
        ///     Gets the embedded XML.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>XmlDocument.</returns>
        public static XmlDocument GetEmbeddedXml(this Type type, string fileName)
        {
            var str = GetEmbeddedFile(type, fileName);
            var tr = new XmlTextReader(str);
            var xml = new XmlDocument();
            xml.Load(tr);
            return xml;
        }
    }
}