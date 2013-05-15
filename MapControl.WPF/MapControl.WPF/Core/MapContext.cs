using System;
using System.Windows;
using System.Windows.Media;

namespace MapControl.WPF.Core
{
    internal struct MapContext
    {
        private readonly double _offsetX;
        private readonly double _offsetY;
        private readonly double _scale;
        private readonly double _zoomLevel;

        internal double OffsetX
        {
            get { return _offsetX; }
        }

        internal double OffsetY
        {
            get { return _offsetY; }
        }

        internal double Scale
        {
            get { return _scale; }
        }

        internal double ZoomLevel
        {
            get { return _zoomLevel; }
        }

        internal int RoundedZoomLevel
        {
            get { return (int)Math.Round(ZoomLevel); }
        }

        public MapContext(double scale, double offsetX, double offsetY, double zoomLevel)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
            _zoomLevel = zoomLevel;
            _scale = scale;
        }

        public Point ConvertPoint(Point point) 
        {
            var transformMatrix = new Matrix(_scale, 0, 0, _scale, _offsetX, _offsetY);
            transformMatrix.Invert();
            return transformMatrix.Transform(point);
        }

        public override int GetHashCode()
        {
            return _offsetX.GetHashCode() ^ _offsetY.GetHashCode() ^ _zoomLevel.GetHashCode() ^ _scale.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MapContext))
            {
                return false;
            }
            var o = (MapContext)obj;
            return o._offsetX == _offsetX && o._offsetY == _offsetY && o._scale == _scale && o._zoomLevel == _zoomLevel;
        }
    }
}
