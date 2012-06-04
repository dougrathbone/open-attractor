using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenAttractor
{
    public partial class ImageViewer : UserControl
    {
        public ImageSource Source { get { return (ImageSource)GetValue(_source); } set { SetValue(_source, value); } }
        public static readonly DependencyProperty _source = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageViewer), new FrameworkPropertyMetadata(null));

        public ImageViewer()
        {
            InitializeComponent();
        }
    }
}
