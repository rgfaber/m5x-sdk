// ***********************************************************************
// <copyright file="CurrentWindowsUser.cs" company="Flint Group">
//     Copyright (c) Flint Group. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Globalization;
using System.Security.Principal;
using System.Threading;

namespace M5x.CEQS.Schema.Utils
{
    /// <summary>
    ///     Class CurrentWindowsUser.
    /// </summary>
    public class CurrentWindowsUser
    {
        /// <summary>
        ///     The culture
        /// </summary>
        public static CultureInfo Culture = Thread.CurrentThread.CurrentCulture;

        /// <summary>
        ///     The user name
        /// </summary>
        public static string UserName = WindowsIdentity.GetCurrent().Name;

        /// <summary>
        ///     The is anonymous
        /// </summary>
        public static bool IsAnonymous = WindowsIdentity.GetCurrent().IsAnonymous;

        /// <summary>
        ///     The is authenticated
        /// </summary>
        public static bool IsAuthenticated = WindowsIdentity.GetCurrent().IsAuthenticated;

        /// <summary>
        ///     The is guest
        /// </summary>
        public static bool IsGuest = WindowsIdentity.GetCurrent().IsGuest;

        /// <summary>
        ///     The is system
        /// </summary>
        public static bool IsSystem = WindowsIdentity.GetCurrent().IsSystem;

        /// <summary>
        ///     The sid
        /// </summary>
        public static SecurityIdentifier Sid = WindowsIdentity.GetCurrent().User;

        /// <summary>
        ///     All cultures
        /// </summary>
        public static CultureInfo[] AllCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
    }
}