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

namespace M5x.DEC.Schema.Utils;

/// <summary>
///     Class GuidUtils.
/// </summary>
public static class GuidUtils
{
    public const string TEST_GUID = "7e577e57-7e57-7e57-7e57-7e577e577e57";

    /// <summary>
    ///     Gets the new clean unique identifier.
    /// </summary>
    /// <value>The new clean unique identifier.</value>
    public static string NewCleanGuid => Guid.NewGuid().ToString("N");

    /// <summary>
    ///     Gets the null unique identifier.
    /// </summary>
    /// <value>The null unique identifier.</value>
    public static string NullCleanGuid => Guid.Empty.ToString("N");

    public static string NewGuid => Guid.NewGuid().ToString();
    public static string LowerCaseGuid => NewGuid.ToLowerInvariant();
}