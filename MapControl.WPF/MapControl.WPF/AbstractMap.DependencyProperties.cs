using System;
using System.Windows;
using System.Windows.Controls;
using MapControl.WPF.Core;
using System.Globalization;

namespace MapControl.WPF
{
    public abstract partial class AbstractMap : Control, IDisposable
    {
        public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register("ZoomLevel",
                                                                                                  typeof(double),
                                                                                                  typeof(AbstractMap),
                                                                                                  new PropertyMetadata(
                                                                                                      2.0,
                                                                                                      OnZoomLevelChanged));

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center",
                                                                                               typeof(GeoPoint),
                                                                                               typeof(AbstractMap),
                                                                                               new PropertyMetadata(
                                                                                                   new GeoPoint(),
                                                                                                   OnCenterChanged));

        public static readonly DependencyProperty MinZoomLevelProperty = DependencyProperty.Register("MinZoomLevel",
                                                                                               typeof(double),
                                                                                               typeof(AbstractMap),
                                                                                               new PropertyMetadata(
                                                                                                   2.0,
                                                                                                   OnMinZoomLevelChanged));

        public static readonly DependencyProperty MaxZoomLevelProperty = DependencyProperty.Register("MaxZoomLevel",
                                                                                               typeof(double),
                                                                                               typeof(AbstractMap),
                                                                                               new PropertyMetadata(
                                                                                                   18.0,
                                                                                                   OnMaxZoomLevelChanged));

        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set
            {
                if (value > TilePanel.MaxZoomLevel || value < TilePanel.MinZoomLevel)
                {
                    throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Zoom level must be between {0} and {1} ",
                        TilePanel.MinZoomLevel, TilePanel.MaxZoomLevel));
                }
                SetValue(ZoomLevelProperty, value);
            }
        }

        public GeoPoint Center
        {
            get { return (GeoPoint)GetValue(CenterProperty); }
            set
            {
                if (Center.Equals(value))
                {
                    return;
                }
                SetValue(CenterProperty, value);
            }
        }

        public double MinZoomLevel
        {
            get { return (double)GetValue(MinZoomLevelProperty); }
            set
            {
                if (value > TilePanel.MaxZoomLevel || value < TilePanel.MinZoomLevel)
                {
                    throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Min zoom level must be between {0} and {1} ",
                        TilePanel.MinZoomLevel, TilePanel.MaxZoomLevel));
                }
                SetValue(MinZoomLevelProperty, value);
            }
        }

        public double MaxZoomLevel
        {
            get { return (double)GetValue(MaxZoomLevelProperty); }
            set
            {
                if (value > TilePanel.MaxZoomLevel || value < TilePanel.MinZoomLevel)
                {
                    throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Max zoom level must be between {0} and {1} ",
                        TilePanel.MinZoomLevel, TilePanel.MaxZoomLevel));
                }
                SetValue(MaxZoomLevelProperty, value);
            }
        }

        private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AbstractMap)d).OnZoomLevelChanged(e);
        }

        private static void OnMinZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AbstractMap)d).OnZoomLevelRangeChanged();
        }

        private static void OnMaxZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AbstractMap)d).OnZoomLevelRangeChanged();
        }

        private static void OnCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AbstractMap)d).OnCenterChanged(e);
        }
    }
}
