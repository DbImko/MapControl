using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapControl.WPF
{
    public class GeoPoint
    {
        public const double MaxLatitude = 90.0;
        public const double MinLatitude = -90.0;
        public const double MaxLongitude = 180.0;
        public const double MinLongitude = -180.0;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public GeoPoint()
            : this(0.0, 0.0)
        {
        }

        public GeoPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static double NormalizeLongitude(double longitude)
        {
            if (longitude < MinLongitude || longitude > MaxLongitude)
            {
                return longitude - Math.Floor((longitude + MaxLongitude) / 360.0) * 360.0;
            }
            return longitude;
        }

        public GeoPoint Normalize()
        {
            var latitude = Math.Min(Math.Max(Latitude, MinLatitude), MaxLatitude);
            return new GeoPoint(latitude, NormalizeLongitude(Longitude));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is GeoPoint))
            {
                return false;
            }
            var o = obj as GeoPoint;
            return o.Latitude == Latitude && o.Longitude == Longitude;
        }

        public override int GetHashCode()
        {
            return Latitude.GetHashCode() ^ Longitude.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1}", (object)Latitude, (object)Longitude);
        }
    }
}
