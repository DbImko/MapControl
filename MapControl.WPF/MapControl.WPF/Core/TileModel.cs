using System;
using System.Globalization;

namespace MapControl.WPF.Core
{
    [Serializable]
    internal sealed class TileModel
    {
        private readonly int _zoomLevel;
        private readonly int _x;
        private readonly int _y;

        public int ZoomLevel
        {
            get { return _zoomLevel; }
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public TileModel(int zoomLevel, int x, int y)
        {
            _zoomLevel = zoomLevel;
            _x = x;
            _y = y;
        }

        public override int GetHashCode()
        {
            return _zoomLevel ^ _x ^ _y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TileModel))
            {
                return false;
            }
            var o = (TileModel)obj;
            if (_x == o._x && _y == o._y)
            {
                return _zoomLevel == o._zoomLevel;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Tile {0} ({1},{2})", _zoomLevel, _x, _y);
        }
        
    }
}
