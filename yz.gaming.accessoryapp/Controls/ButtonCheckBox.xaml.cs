
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// ButtonCheckBox.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonCheckBox : CheckBox
    {
        public ButtonCheckBox()
        {
            InitializeComponent();
        }

        public string UnCheckedText
        {
            get { return (string)GetValue(UnCheckedTextProperty); }
            set { SetValue(UnCheckedTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnCheckedTextProperty =
            DependencyProperty.Register("UnCheckedText", typeof(string), typeof(ButtonCheckBox), new PropertyMetadata("False"));


        public Brush CheckedBrush
        {
            get { return (Brush)GetValue(CheckedBrushProperty); }
            set { SetValue(CheckedBrushProperty, value); }
        }
        public static readonly DependencyProperty CheckedBrushProperty =
           DependencyProperty.Register("CheckedBrush", typeof(Brush), typeof(ButtonCheckBox), new PropertyMetadata(Brushes.Gold));


        public Brush HoverBrush
        {
            get { return (Brush)GetValue(HoverBrushProperty); }
            set { SetValue(HoverBrushProperty, value); }
        }
        public static readonly DependencyProperty HoverBrushProperty =
           DependencyProperty.Register("HoverBrush", typeof(Brush), typeof(ButtonCheckBox), new PropertyMetadata(Brushes.CornflowerBlue));



        public Brush CheckedForeground
        {
            get { return (Brush)GetValue(CheckedForegroundProperty); }
            set { SetValue(CheckedForegroundProperty, value); }
        }
        public static readonly DependencyProperty CheckedForegroundProperty =
           DependencyProperty.Register("CheckedForeground", typeof(Brush), typeof(ButtonCheckBox), new PropertyMetadata(Brushes.White));



    }
}
