using System.Windows;
using System.Windows.Input;

namespace MapControl.WPF
{
    public partial class Map
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CaptureMouse())
            {
                _leftButtonMapPosition = ConvertScreenPointToMapPoint(e.GetPosition(this), ZoomLevel);
            }
            else
            {
                _leftButtonMapPosition = null;
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            _leftButtonMapPosition = null;
            ZoomPosition = e.GetPosition(this);
            UpdateZoomLevel(ZoomLevel + 1.0);
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            ZoomPosition = e.GetPosition(this);
            UpdateZoomLevel(ZoomLevel + e.Delta / 1000.0);
            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var position = e.GetPosition(this);
                var currentPosition = ConvertScreenPointToMapPoint(position, ZoomLevel);

                if (_leftButtonMapPosition.HasValue && !_leftButtonMapPosition.Value.Equals(currentPosition))
                {
                    var centerOfMap = ConvertScreenPointToMapPoint(new Point(ActualWidth / 2.0, ActualHeight / 2.0), ZoomLevel);
                    var newCenterPosition = new Point(_leftButtonMapPosition.Value.X - currentPosition.X + centerOfMap.X,
                        _leftButtonMapPosition.Value.Y - currentPosition.Y + centerOfMap.Y);
                    UpdateCenterAndZoomLevel(newCenterPosition, ZoomLevel);
                    ZoomPosition = position;
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            if (!IsMouseCaptured && !AreAnyTouchesCaptured)
            {
                _leftButtonMapPosition = null;
            }
            base.OnMouseLeftButtonUp(e);
        }
    }
}
