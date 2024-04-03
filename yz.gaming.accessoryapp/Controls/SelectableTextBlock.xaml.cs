using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// SelectableTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class SelectableTextBlock : UserControl, ISelectableItem
    {
        public delegate void SelectableTextBlockSelectedHandler(ISelectableItem sender);
        public delegate void SelectableTextBlockHovedHandler(ISelectableItem sender);
        public event SelectableTextBlockSelectedHandler OnSelected;
        public event SelectableTextBlockSelectedHandler OnHoved;

        Thickness DEFAULT_BORDER_MARGIN = new Thickness(2);
        Thickness DEFAULT_BORDER_THICKNESS = new Thickness(1);

        Thickness HOVER_BORDER_MARGIN = new Thickness(0);
        Thickness HOVER_BORDER_THICKNESS = new Thickness(3);

        public SelectableTextBlock()
        {
            InitializeComponent();

            ChooseControl.OnChoose += ChooseControl_OnChoose;

            DataContext = this;
        }

        private void ChooseControl_OnChoose(ChooseEffectGrid sender)
        {
            IsSelected = sender.IsChoose;
            OnSelected?.Invoke(this);
        }

        public byte Key { get; set; } = 0xFF;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SelectableTextBlock), new PropertyMetadata(string.Empty));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                if (value)
                {
                    ChooseControl.Choose();
                    IsHover = value;
                    OnHoved?.Invoke(this);
                }
                else
                {
                    ChooseControl.UnChoose();
                }
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectableTextBlock), new PropertyMetadata(false));

        public bool IsHover
        {
            get { return (bool)GetValue(IsHoverProperty); }
            set
            {
                SetValue(IsHoverProperty, value);
                EffectBorder.Margin = value ? HOVER_BORDER_MARGIN : DEFAULT_BORDER_MARGIN;
                EffectBorder.BorderThickness = value ? HOVER_BORDER_THICKNESS : DEFAULT_BORDER_THICKNESS;
            }
        }

        public static readonly DependencyProperty IsHoverProperty =
            DependencyProperty.Register("IsHover", typeof(bool), typeof(SelectableTextBlock), new PropertyMetadata(false));


        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(SelectableTextBlock), new PropertyMetadata(0));
    }
}
