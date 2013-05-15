using MapControl.WPF.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapControl.WPF
{
    public abstract partial class AbstractMap : Control, IDisposable
    {
        private Grid _mapGrid;
        private readonly Grid _tilePanelGrid;
        private readonly TilePanel _tilePanel;
        private Point? _zoomPosition;
        private Point _centerPosition;

        internal Point? ZoomPosition
        {
            set { _zoomPosition = value; }
        }

        static AbstractMap()
        {
        }

        protected AbstractMap()
        {
            _zoomPosition = null;
            _centerPosition = ConvertGeoPointToMapPoint(Center);
            _tilePanel = new TilePanel(new OsmTileUrlProvider());
            _tilePanelGrid = new Grid();
            _tilePanelGrid.Children.Add(_tilePanel);
            Update();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _mapGrid = (Grid)GetTemplateChild("MapGrid");
            if (_mapGrid != null)
            {
                _mapGrid.Clip = new RectangleGeometry();
                _mapGrid.SizeChanged += MapGridSizeChanged;
                _mapGrid.Children.Add(_tilePanelGrid);
            }
        }

        private void OnZoomLevelChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateZoomLevel((double)e.NewValue);
        }

        private void OnZoomLevelRangeChanged()
        {
            UpdateZoomLevel(ZoomLevel);
        }

        private void OnCenterChanged(DependencyPropertyChangedEventArgs e)
        {
            _zoomPosition = null;
            UpdateCenterAndZoomLevel(ConvertGeoPointToMapPoint((GeoPoint)e.NewValue), ZoomLevel);
        }

        internal void UpdateZoomLevel(double zoomLevel)
        {
            UpdateCenterAndZoomLevel(_centerPosition, zoomLevel);
        }

        internal Point ConvertScreenPointToMapPoint(Point point, double zoomLevel)
        {
            var mapContext = GetMapContext(zoomLevel);
            return mapContext.ConvertPoint(point);
        }

        private MapContext GetMapContext(double zoomLevel)
        {
            var mapScale = MapHelper.MapScale(zoomLevel);
            var leftOffset = (-_centerPosition.X * mapScale) + ActualWidth / 2.0;
            var topOffset = (-_centerPosition.Y * mapScale) + ActualHeight / 2.0;
            return new MapContext(mapScale, leftOffset, topOffset, ZoomLevel);
        }

        private void Update()
        {
            Center = ConvertMapPointToGeoPoint(_centerPosition);
            if (_tilePanel != null)
            {
                _tilePanel.UpdateMapContext(GetMapContext(ZoomLevel));
            }
        }

        private void MapGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((RectangleGeometry)_mapGrid.Clip).Rect = new Rect(0.0, 0.0, e.NewSize.Width, e.NewSize.Height);
            Update();
        }

        protected void UpdateCenterAndZoomLevel(Point center, double newZoomLevel)
        {
            newZoomLevel = Math.Min(MaxZoomLevel, Math.Max(MinZoomLevel, newZoomLevel));

            if (newZoomLevel != ZoomLevel && _zoomPosition.HasValue)
            {
                var zoomPointWithCurrentZoom = ConvertScreenPointToMapPoint(_zoomPosition.Value, ZoomLevel);
                var zoomPointWithNewZoom = ConvertScreenPointToMapPoint(_zoomPosition.Value, newZoomLevel);
                _centerPosition.X += zoomPointWithCurrentZoom.X - zoomPointWithNewZoom.X;
                _centerPosition.Y += zoomPointWithCurrentZoom.Y - zoomPointWithNewZoom.Y;
            }
            else
            {
                _centerPosition = center;
            }
            ZoomLevel = newZoomLevel;
            Update();
        }

        public GeoPoint ConvertMapPointToGeoPoint(Point point)
        {
            return MapHelper.ToGeoPoint(point).Normalize();
        }

        public Point ConvertGeoPointToMapPoint(GeoPoint geoPoint)
        {
            return MapHelper.FromGeoPoint(geoPoint);
        }

        public void Dispose()
        {
            if (_mapGrid != null)
            {
                _mapGrid.SizeChanged -= MapGridSizeChanged;
            }
            if (_tilePanel != null)
            {
                _tilePanel.Dispose();
            }
        }
    }
}
