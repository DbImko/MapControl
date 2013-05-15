using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapControl.WPF.Core
{
    internal class Tile
    {
        internal event EventHandler NeedsUpdate;

        private Canvas _parentCanvas;
        private FrameworkElement _image;
        private readonly TileModel _tileModel;
        private bool _visible;
        private bool _changed;
        private bool _loaded;
        private readonly ITileUrlProvider _tileSource;
        private readonly bool _isInDesignMode;
        internal long Ticks { get; set; }

        internal TileModel TileModel
        {
            get { return _tileModel; }
        }

        private FrameworkElement Image
        {
            get { return _image; }
            set
            {
                if (Object.ReferenceEquals(_image, value))
                {
                    return;
                }
                _image = value;
                if (_image != null)
                {
                    Canvas.SetLeft(_image, 0.0);
                    Canvas.SetTop(_image, 0.0);
                    if (NeedsUpdate != null)
                    {
                        NeedsUpdate(this, EventArgs.Empty);
                    }
                }
                _changed = true;
            }
        }

        public Tile(Canvas parentCanvas, TileModel tileModel, ITileUrlProvider tileSource)
        {
            if (parentCanvas == null)
            {
                throw new ArgumentNullException("parentCanvas");
            }
            if (tileModel == null)
            {
                throw new ArgumentNullException("tileModel");
            }
            if (tileSource == null)
            {
                throw new ArgumentNullException("tileSource");
            }
            _parentCanvas = parentCanvas;
            _isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(_parentCanvas);
            _tileModel = tileModel;
            _tileSource = tileSource;
            _visible = false;
        }

        internal void SetVisible(bool value)
        {
            _visible = value;
            if (_visible)
            {
                return;
            }
            Clear();
        }

        internal void Update(MapContext context)
        {
            if (Image == null)
            {
                return;
            }
            
            if (_changed)
            {
                if (_parentCanvas != null)
                {
                    _parentCanvas.Children.Remove(Image);
                    _parentCanvas.Children.Add(Image);
                }
                _changed = false;
            }

            var point = new Point(TilePanel.TileSize*_tileModel.X, TilePanel.TileSize*_tileModel.Y);
            var mapSize = MapHelper.MapScale(context.RoundedZoomLevel);

            var scale = context.Scale / mapSize;
            var offsetX = point.X * scale + context.OffsetX;
            var offsetY = point.Y * scale + context.OffsetY;
            
            var tileToViewport = new Matrix(scale, 0, 0, scale, offsetX, offsetY);

            var matrixTransform = new MatrixTransform {Matrix = tileToViewport};
            Image.RenderTransform = matrixTransform;
        }

        internal void LoadTileAsync()
        {
            if (_parentCanvas == null)
            {
                return;
            }
            if (Image == null && !_loaded)
            {
                _loaded = true;
                System.Threading.ThreadPool.QueueUserWorkItem(LoadTileCallback);
            }
        }

        private void LoadTileCallback(object state)
        {
            if (_parentCanvas == null || _image != null)
            {
                return;
            }
            
            var uri = _tileSource.GetUri(_tileModel.ZoomLevel, _tileModel.X, _tileModel.Y);
            var bitmap = TileImageHelper.DownloadImage(uri, _isInDesignMode);
            if (bitmap != null && _parentCanvas != null)
            {
                _parentCanvas.Dispatcher.BeginInvoke(new Action(() =>
                                                                   {
                                                                       if (Image != null) return;
                                                                       var element = new Image
                                                                                     {
                                                                                         Width = TilePanel.TileSize + 1,
                                                                                         Height = TilePanel.TileSize + 1,
                                                                                         Source = bitmap,
                                                                                     };
                                                                       Image = element;
                                                                   }));
            }
        }

        internal void Clear()
        {
            if (_parentCanvas != null)
            {
                _parentCanvas.Children.Remove(Image);
                _parentCanvas = null;
            }
            Image = null;
        }
    }
}
