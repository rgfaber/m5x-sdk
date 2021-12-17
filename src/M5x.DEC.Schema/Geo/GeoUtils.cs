// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-19-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-19-2014
// ***********************************************************************
// <copyright file="GeoUtils.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace M5x.DEC.Schema.Geo;

/// <summary>
///     Class GeoUtils.
/// </summary>
public static class GeoUtils
{
    /// <summary>
    ///     Distances to.
    /// </summary>
    /// <param name="sourceLat">The source lat.</param>
    /// <param name="lat">The lat.</param>
    /// <param name="sourceLon">The source lon.</param>
    /// <param name="lng">The LNG.</param>
    /// <param name="dType">Type of the d.</param>
    /// <returns>System.Double.</returns>
    public static double DistanceTo(double sourceLat, double lat, double sourceLon, double lng, DistanceType dType)
    {
        var source = new GLatLng(sourceLat, sourceLon);
        return source.DistanceTo(lat, lng, dType);
    } // end DistanceTo


    /// <summary>
    ///     Distances to.
    /// </summary>
    /// <param name="sourceLat">The source lat.</param>
    /// <param name="lat">The lat.</param>
    /// <param name="sourceLon">The source lon.</param>
    /// <param name="lng">The LNG.</param>
    /// <param name="dType">Type of the d.</param>
    /// <returns>System.Double.</returns>
    public static double LongDistanceTo(double sourceLat, double lat, double sourceLon, double lng,
        DistanceType dType)
    {
        var source = new GLatLng(sourceLat, sourceLon);
        return source.LongDistanceTo(lat, lng, dType);
    } // end DistanceTo


    /// <summary>
    ///     Longs the rhumb distance to.
    /// </summary>
    /// <param name="sourceLat">The source lat.</param>
    /// <param name="lat">The lat.</param>
    /// <param name="sourceLon">The source lon.</param>
    /// <param name="lng">The LNG.</param>
    /// <param name="dType">Type of the d.</param>
    /// <returns>System.Double.</returns>
    public static double LongRhumbDistanceTo(double sourceLat, double lat, double sourceLon, double lng,
        DistanceType dType)
    {
        var source = new GLatLng(sourceLat, sourceLon);
        return source.LongRhumbDistanceTo(lat, lng, dType);
    } // end DistanceTo


    /// <summary>
    ///     Rhumbs the distance to.
    /// </summary>
    /// <param name="sourceLat">The source lat.</param>
    /// <param name="lat">The lat.</param>
    /// <param name="sourceLon">The source lon.</param>
    /// <param name="lng">The LNG.</param>
    /// <param name="dType">Type of the d.</param>
    /// <returns>System.Double.</returns>
    public static double RhumbDistanceTo(double sourceLat, double lat, double sourceLon, double lng,
        DistanceType dType)
    {
        var source = new GLatLng(sourceLat, sourceLon);
        return source.RhumbDistanceTo(lat, lng, dType);
    }

    /// <summary>
    ///     Rhumbs the bearing to.
    /// </summary>
    /// <param name="sourceLat">The source lat.</param>
    /// <param name="lat">The lat.</param>
    /// <param name="sourceLon">The source lon.</param>
    /// <param name="lng">The LNG.</param>
    /// <returns>System.Double.</returns>
    public static double RhumbBearingTo(double sourceLat, double lat, double sourceLon, double lng)
    {
        var source = new GLatLng(sourceLat, sourceLon);
        return source.RhumbBearingTo(lat, lng);
    }


    /// <summary>
    ///     Bearings to.
    /// </summary>
    /// <param name="sourceLat">The source lat.</param>
    /// <param name="lat">The lat.</param>
    /// <param name="sourceLon">The source lon.</param>
    /// <param name="lng">The LNG.</param>
    /// <returns>System.Double.</returns>
    public static double BearingTo(double sourceLat, double lat, double sourceLon, double lng)
    {
        var source = new GLatLng(sourceLat, sourceLon);
        return source.RhumbBearingTo(lat, lng);
    }
}