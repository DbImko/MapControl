using MapControl.WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MapControl.WPF
{
    internal class MapHelper
    {
        internal const double LatitudeLimit = 85.0511;
        internal const double RadiansPerDegrees = Math.PI / GeoPoint.MaxLongitude;
        internal const double DegreesPerRadians = GeoPoint.MaxLongitude / Math.PI;

        internal static GeoPoint ToGeoPoint(Point point)
        {
            var latitude = GeoPoint.MaxLatitude - 2.0 * Math.Atan(Math.Exp((point.Y * 2.0 - 1.0) * Math.PI)) * DegreesPerRadians;
            var longitude = (point.X - 0.5) * 360.0;
            return new GeoPoint(latitude, longitude);
        }

        internal static Point FromGeoPoint(GeoPoint location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            var x = location.Longitude / 360.0 + 0.5;
            double y = 0.0;
            if (location.Latitude >= LatitudeLimit)
            {
                y = 0.0;
            }
            else if (location.Latitude <= -LatitudeLimit)
            {
                y = 1.0;
            }
            else
            {
                var sinLatitude = Math.Sin(location.Latitude * RadiansPerDegrees);
                y = 0.5 - Math.Log((1.0 + sinLatitude) / (1.0 - sinLatitude)) / (4.0 * Math.PI);
            }
            return new Point(x, y);
        }

        internal static double MapScale(double zoomLevel)
        {
            return TilePanel.TileSize * Math.Pow(2.0, zoomLevel);
        }
    }
}
