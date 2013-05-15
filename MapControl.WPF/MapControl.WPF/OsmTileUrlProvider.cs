using System;

namespace MapControl.WPF
{
    public class OsmTileUrlProvider : ITileUrlProvider
    {
        private const String UrlFormat = "http://tile.openstreetmap.org/{0}/{1}/{2}.png";

        public Uri GetUri(int zoom, int x, int y)
        {
            return new Uri(String.Format(UrlFormat, zoom, x, y));
        }
    }
}
