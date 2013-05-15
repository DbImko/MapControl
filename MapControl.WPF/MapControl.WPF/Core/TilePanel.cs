using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapControl.WPF.Core
{
    internal class TilePanel : Grid
    {
        public const int TileSize = 256;
        public const int MinZoomLevel = 2;
        public const int MaxZoomLevel = 18;

        private readonly Canvas _tilesCanvas;
        private readonly ITileUrlProvider _tileSource;
        private readonly Dictionary<TileModel, Tile> _tiles;
        private readonly List<TileModel> _visibleTileModels;
        private MapContext _context;
        private long _ticks;

        public TilePanel(ITileUrlProvider tileSource)
        {
            if (tileSource == null)
            {
                throw new ArgumentNullException("tileSource");
            }
            _tileSource = tileSource;
            SizeChanged += TilePanelSizeChanged;

            _tilesCanvas = new Canvas { Background = new SolidColorBrush(Colors.Gray) };
            Children.Add(_tilesCanvas);

            _tiles = new Dictionary<TileModel, Tile>();
            _visibleTileModels = new List<TileModel>();
        }

        internal void UpdateMapContext(MapContext context)
        {
            _context = context;
            Update();
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);
            Update();
            return size;
        }

        private void TilePanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            AddNewTiles();
            LoadAndCleanUpTiles();
        }

        private void AddNewTiles()
        {
            ReinitVisibleTiles();
            _ticks = DateTime.Now.Ticks;
            foreach (var tileModel in _visibleTileModels)
            {
                if (tileModel.ZoomLevel >= MinZoomLevel)
                {
                    Tile tile;
                    if (!_tiles.ContainsKey(tileModel))
                    {
                        tile = new Tile(_tilesCanvas, tileModel, _tileSource);
                        tile.NeedsUpdate += TileNeedsUpdate;
                        _tiles.Add(tileModel, tile);
                    }
                    else
                    {
                        tile = _tiles[tileModel];
                    }
                    tile.Ticks = _ticks;
                }
            }
        }

        private void LoadAndCleanUpTiles()
        {
            var tilesToRemove = new List<TileModel>(_tiles.Count);
            foreach (var pair in _tiles)
            {
                var tile = pair.Value;
                if (tile.Ticks != _ticks)
                {
                    tile.SetVisible(false);
                    tile.NeedsUpdate -= TileNeedsUpdate;
                    tilesToRemove.Add(pair.Key);
                }
                else
                {
                    if (tile.TileModel.ZoomLevel < _context.RoundedZoomLevel)
                    {
                        tile.SetVisible(false);
                    }
                    else
                    {
                        tile.LoadTileAsync();
                        tile.SetVisible(true);
                        tile.Update(_context);
                    }
                }
            }
            _tiles.RemoveElementsByKeys(tilesToRemove);
        }

        private void TileNeedsUpdate(object sender, EventArgs e)
        {
            InvalidateArrange();
        }

        private void ReinitVisibleTiles()
        {
            _visibleTileModels.Clear();

            var realTileSize = TileSize * Math.Pow(2, _context.ZoomLevel) / Math.Pow(2, _context.RoundedZoomLevel);

            var leftTile = 0;
            if (_context.OffsetX < 0)
            {
                var left = (int)Math.Floor(Math.Abs(_context.OffsetX) / realTileSize);
                leftTile = Math.Max(0, left);
            }
            var topTile = 0;
            if (_context.OffsetY < 0)
            {
                var top = (int)Math.Floor(Math.Abs(_context.OffsetY) / realTileSize);
                topTile = Math.Max(0, top);
            }

            var rows = (int)Math.Ceiling(ActualHeight / realTileSize) + 1;
            var columns = (int)Math.Ceiling(ActualWidth / realTileSize) + 1;

            var maxTileCount = Math.Pow(2, _context.RoundedZoomLevel) - 1;

            for (var x = leftTile; x < leftTile + columns; ++x)
            {
                for (var y = topTile; y < topTile + rows; ++y)
                {
                    if ((x <= maxTileCount && y <= maxTileCount) && 
                        (x >= 0 && y >= 0))
                    {
                        var tile = new TileModel(_context.RoundedZoomLevel, x, y);
                        _visibleTileModels.Add(tile);
                    }
                }
            }
        }

        internal void Dispose()
        {
            foreach (var pair in _tiles)
            {
                var tile = pair.Value;
                if (tile.Ticks != _ticks)
                {
                    tile.SetVisible(false);
                    tile.NeedsUpdate -= TileNeedsUpdate;
                }
            }
            _tiles.Clear();
            _visibleTileModels.Clear();
        }
    }
}
