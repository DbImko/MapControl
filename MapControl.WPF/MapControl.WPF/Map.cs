using System.Windows;

namespace MapControl.WPF
{
    public partial class Map : AbstractMap
    {
        private Point? _leftButtonMapPosition;
        private bool _firstTime;

        static Map()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Map), new FrameworkPropertyMetadata(typeof(Map)));
        }

        public Map()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_firstTime)
            {
                return;
            }
            _firstTime = true;
        }

        ~Map()
        {
            Dispose();
        }
    }
}
