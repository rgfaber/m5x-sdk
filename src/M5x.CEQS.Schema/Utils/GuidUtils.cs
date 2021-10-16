// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-20-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-20-2014
// ***********************************************************************
// <copyright file="GuidUtils.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace M5x.CEQS.Schema.Utils
{
    /// <summary>
    ///     Class GuidUtils.
    /// </summary>
    public static class GuidUtils
    {
        /// <summary>
        ///     Gets the new clean unique identifier.
        /// </summary>
        /// <value>The new clean unique identifier.</value>
        public static string NewCleanGuid => Guid.NewGuid().ToString("N");

        /// <summary>
        ///     Gets the null unique identifier.
        /// </summary>
        /// <value>The null unique identifier.</value>
        public static string NullGuid => Guid.Empty.ToString("N");
    }
}