using System;

namespace MapControl.WPF
{
    public interface ITileUrlProvider
    {
        Uri GetUri(int zoom, int x, int ys);
    }
}
