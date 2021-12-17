// ***********************************************************************
// <copyright file="GLatLng.cs" company="Flint Group">
//     Copyright (c) Flint Group. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace M5x.DEC.Schema.Geo;

/// <summary>
///     Class GLatLng.
/// </summary>
public class GLatLng
{
    /// <summary>
    ///     The earth radius in miles
    /// </summary>
    public const double EarthRadiusInMiles = 3956.0;

    /// <summary>
    ///     The earth radius in kilometers
    /// </summary>
    public const double EarthRadiusInKilometers = 6367.0;

    /// <summary>
    ///     The earth circ in miles
    /// </summary>
    public static double EarthCircInMiles = EarthRadiusInMiles * 2 * 3.14;

    /// <summary>
    ///     The earth circ in kilometers
    /// </summary>
    public static double EarthCircInKilometers = EarthRadiusInKilometers * 2 * 3.14;

    /// <summary>
    ///     The latitude
    /// </summary>
    private double _latitude;

    /// <summary>
    ///     The longitude
    /// </summary>
    private double _longitude;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GLatLng" /> class.
    /// </summary>
    /// <param name="latitude">The latitude.</param>
    /// <param name="longitude">The longitude.</param>
    public GLatLng(double latitude, double longitude)
    {
        _latitude = latitude;
        _longitude = longitude;
    }

    /// <summary>
    ///     Gets or sets the latitude.
    /// </summary>
    /// <value>The latitude.</value>
    public double Latitude
    {
        get => _latitude;
        set => _latitude = value;
    }

    /// <summary>
    ///     Gets or sets the longitude.
    /// </summary>
    /// <value>The longitude.</value>
    public double Longitude
    {
        get => _longitude;
        set => _longitude = value;
    }

    /// <summary>
    ///     Degrees to radian.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <returns>System.Double.</returns>
    public double DegreeToRadian(double angle)
    {
        return Math.PI * angle / 180.0;
    }

    /// <summary>
    ///     Radians to degree.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <returns>System.Double.</returns>
    public double RadianToDegree(double angle)
    {
        return 180.0 * angle / Math.PI;
    }


    /// <summary>
    ///     Distances to.
    /// </summary>
    /// <param name="lat">The lat.</param>
    /// <param name="lng">The LNG.</param>
    /// <param name="dType">Type of the d.</param>
    /// <returns>System.Double.</returns>
    public double DistanceTo(double lat, double lng, DistanceType dType)
    {
        var r = dType == DistanceType.Miles ? EarthRadiusInMiles : EarthRadiusInKilometers;
        var dLat = DegreeToRadian(lat) - DegreeToRadian(_latitude);
        var dLon = DegreeToRadian(lng) - DegreeToRadian(_longitude);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(DegreeToRadian(_latitude)) *
            Math.Cos(DegreeToRadian(lat)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = c * r;
        return Math.Round(distance, 2);
    } // end DistanceTo


    /// <summary>
    ///     Longs the distance to.
    /// </summary>
    /// <param name="lat">The lat.</param>
    /// <param name="lng">The LNG.</param>
    /// <param name="dType">Type of the d.</param>
    /// <returns>System.Double.</returns>
    public double LongDistanceTo(double lat, double lng, DistanceType dType)
    {
        var d = DistanceTo(lat, lng, dType);
        if (dType == DistanceType.Miles)
            return EarthCircInMiles - d;
        return EarthCircInKilometers - d;
    }


    /// <summary>
    ///     Longs the rhumb distance to.
    /// </summary>
    /// <param name="lat">The lat.</param>
    /// <param name="lng">The LNG.</param>
    /// <param name="dType">Type of the d.</param>
    /// <returns>System.Double.</returns>
    public double LongRhumbDistanceTo(double lat, double lng, DistanceType dType)
    {
        var d = RhumbDistanceTo(lat, lng, dType);
        if (dType == DistanceType.Miles)
            return EarthCircInMiles - d;
        return EarthCircInKilometers - d;
    }


    /// <summary>
    ///     Rhumbs the distance to.
    /// </summary>
    /// <param name="lat">The lat.</param>
    /// <param name="lng">The LNG.</param>
    /// <param name="dType">Type of the d.</param>
    /// <returns>System.Double.</returns>
    public double RhumbDistanceTo(double lat, double lng, DistanceType dType)
    {
        var r = dType == DistanceType.Miles ? EarthRadiusInMiles : EarthRadiusInKilometers;
        var lat1 = DegreeToRadian(_latitude);
        var lat2 = DegreeToRadian(lat);
        var dLat = DegreeToRadian(lat - _latitude);
        var dLon = DegreeToRadian(Math.Abs(lng - _longitude));

        var dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
        var q = Math.Cos(lat1);
        if (dPhi != 0) q = dLat / dPhi; // E-W line gives dPhi=0
        // if dLon over 180° take shorter rhumb across 180° meridian:
        if (dLon > Math.PI) dLon = 2 * Math.PI - dLon;
        var dist = Math.Sqrt(dLat * dLat + q * q * dLon * dLon) * r;
        return dist;
    } // end RhumbDistanceTo


    /// <summary>
    ///     Rhumbs the bearing to.
    /// </summary>
    /// <param name="lat">The lat.</param>
    /// <param name="lng">The LNG.</param>
    /// <returns>System.Double.</returns>
    public double RhumbBearingTo(double lat, double lng)
    {
        var lat1 = DegreeToRadian(_latitude);
        var lat2 = DegreeToRadian(lat);
        var dLon = DegreeToRadian(lng - _longitude);

        var dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
        if (Math.Abs(dLon) > Math.PI) dLon = dLon > 0 ? -(2 * Math.PI - dLon) : 2 * Math.PI + dLon;
        var brng = Math.Atan2(dLon, dPhi);

        return (RadianToDegree(brng) + 360) % 360;
    } // end RhumbBearingTo

    /// <summary>
    ///     Bearings to.
    /// </summary>
    /// <param name="lat">The lat.</param>
    /// <param name="lng">The LNG.</param>
    /// <returns>System.Double.</returns>
    public double BearingTo(double lat, double lng)
    {
        var lat1 = DegreeToRadian(_latitude);
        var lat2 = DegreeToRadian(lat);
        var dLon = DegreeToRadian(lng) - DegreeToRadian(_longitude);

        var y = Math.Sin(dLon) * Math.Cos(lat2);
        var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
        var brng = Math.Atan2(y, x);

        return (RadianToDegree(brng) + 360) % 360;
    } // end BearingTo
}