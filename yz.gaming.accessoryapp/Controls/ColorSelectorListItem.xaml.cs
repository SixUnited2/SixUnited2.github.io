using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using yz.gaming.accessoryapp.Utils;
using static yz.gaming.accessoryapp.Controls.ItemEffect;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// ColorSelectorListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ColorSelectorListItem : UserControl, IArrayPageListItem
    {
        private ItemEffect _itemEffect;

        public delegate void ColorSelectorListItemColorChangedHandler(IPageListItem sender, Color color);
        public delegate void ColorSelectorListItemClickHandler(IPageListItem sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event ColorSelectorListItemColorChangedHandler OnColorChanged;
        public event ColorSelectorListItemClickHandler OnClick;

        List<RadioButton> ListItems;

        public ColorSelectorListItem()
        {
            InitializeComponent();
            this.DataContext = this;

            this.Loaded += ColorSelectorListItem_Loaded;

            ListItems = new List<RadioButton>()
            {
                Red,
                Yellow,
                GreenYellow,
                Green,
                Wathet,
                Blue,
                Purple,
                White
            };

            _itemEffect = new ItemEffect() { Border = Border, MainGrid = MainGrid };
        }

        private void ColorSelectorListItem_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonEffect(IsSelected, IsHoved);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ColorSelectorListItem), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(ColorSelectorListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                SetButtonEffect(value, IsHoved);
                OnSelectedStateChange?.Invoke(this, value);
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ColorSelectorListItem), new PropertyMetadata(false));

        public bool IsHoved
        {
            get { return (bool)GetValue(IsHovedProperty); }
            set
            {
                SetValue(IsHovedProperty, value);
                SetButtonEffect(IsSelected, value);
                OnHovedStateChange?.Invoke(this, value);
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(ColorSelectorListItem), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(ColorSelectorListItem), new PropertyMetadata(0));

        public int SelectIndex
        {
            get { return (int)GetValue(SelectIndexProperty); }
            set
            {
                SetValue(SelectIndexProperty, value);

                Task.Run(() =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ListItems[value].IsChecked = true;
                        OnColorChanged(this, (ListItems[value].Background as SolidColorBrush).Color);
                    }));
                });

            }
        }

        public static readonly DependencyProperty SelectIndexProperty =
            DependencyProperty.Register("SelectIndex", typeof(int), typeof(ColorSelectorListItem), new PropertyMetadata(0));

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            IsHoved = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsSelected = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                OnClick?.Invoke(this);
            }
        }

        public void SetButtonEffect(bool isSelected, bool isHoved)
        {
            _itemEffect.SetButtonEffect(isSelected, isHoved);
        }

        public void SetColor(Color color)
        {
            foreach (var item in ListItems)
            {
                if (MediaColorUtils.AreColorsSimilar1(color, ((SolidColorBrush)item.Background).Color, 50))
                {
                    item.IsChecked = true;
                    break;
                }

                item.IsChecked = false;
            }
        }

        public void ConfirmPressed()
        {
            if (!IsSelected || !IsHoved) return;

            if (!IsSelected)
            {
                IsSelected = true;
            }
            else
            {
                LoopValue();
            }
        }

        private void LoopValue()
        {
            var value = 0;

            if (SelectIndex < ListItems.Count - 1)
            {
                value = SelectIndex + 1;
            }

            SelectIndex = value;
        }

        public void SelectNext()
        {
            if (SelectIndex < ListItems.Count - 1)
            {
                SelectIndex = SelectIndex + 1;
            }
        }

        public void SelectPrev()
        {
            if (SelectIndex > 0)
            {
                SelectIndex = SelectIndex - 1;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32(((Control)sender).Tag);

            if (SelectIndex != index)
            {
                SelectIndex = index;
            }
        }
    }
}
