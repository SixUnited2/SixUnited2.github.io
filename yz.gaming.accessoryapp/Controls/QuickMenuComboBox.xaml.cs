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
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Utils;
using static yz.gaming.accessoryapp.Controls.ItemEffect;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// QuickMenuComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class QuickMenuComboBox : UserControl, IQuickMenuControl
    {
        private ItemEffect _itemEffect;

        public delegate void QuickMenuComboBoxSelectItemChangedHandler(IQuickMenuControl sender, object item);
        public delegate void QuickMenuComboBoxClickHandler(IQuickMenuControl sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
        static SolidColorBrush SELECTED_BRUSH = new SolidColorBrush(Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF));

        static Thickness DEFAULT_ICON_MARGIN = new Thickness(8, 8, 0, 8);
        static Thickness DEFAULT_TEXT_MARGIN = new Thickness(20, 16, 0, 16);

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event QuickMenuComboBoxSelectItemChangedHandler OnSelectItemChanged;
        public event QuickMenuComboBoxClickHandler OnClick;

        public QuickMenuComboBox()
        {
            InitializeComponent();

            DataContext = this;
            this.Loaded += QuickMenuComboBox_Loaded;

            _itemEffect = new ItemEffect()
            {
                Border = Border,
                MainGrid = MainGrid,
                DefaultBackgroundBrush = DEFAULT_BRUSH,
                SelectedBackgroundBrush = SELECTED_BRUSH,
                SelectedBorderBrush = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00)),
                SelectedBorderThickness = new Thickness(2 / SystemUtils.Instance.GetScreenScalingFactor())
            };
        }

        private void QuickMenuComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxCtr.ItemsSource = Data;
            ComboBoxCtr.SelectedIndex = SelectIndex;

            double scaling = SystemUtils.Instance.GetScreenScalingFactor();

            if (scaling != 1)
            {
                IconMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_ICON_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_ICON_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_ICON_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_ICON_MARGIN.Bottom / scaling))
                };
                TextMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_TEXT_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_TEXT_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_TEXT_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_TEXT_MARGIN.Bottom / scaling))
                };
            }
            else
            {
                IconMargin = DEFAULT_ICON_MARGIN;
                TextMargin = DEFAULT_TEXT_MARGIN;
            }

            SetButtonEffect(IsSelected, IsHoved);

            ComboBoxCtr.DropDownClosed += ComboBox_DropDownClosed;
            ComboBoxCtr.DropDownOpened += ComboBoxCtr_DropDownOpened;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(QuickMenuComboBox), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(QuickMenuComboBox), new PropertyMetadata(DEFUALT_ICON_PATH));

        public int SelectIndex
        {
            get { return (int)GetValue(SelectIndexProperty); }
            set
            {
                SetValue(SelectIndexProperty, value);
            }
        }

        public static readonly DependencyProperty SelectIndexProperty =
            DependencyProperty.Register("SelectIndex", typeof(int), typeof(QuickMenuComboBox), new PropertyMetadata(0));

        public List<ItemModel> Data
        {
            get { return (List<ItemModel>)GetValue(DataProperty); }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(List<ItemModel>), typeof(QuickMenuComboBox), new PropertyMetadata(null));


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                if (IsSelectable)
                {
                    SetValue(IsSelectedProperty, value);
                    SetButtonEffect(value, IsHoved);
                    OnSelectedStateChange?.Invoke(this, value);
                    SetDropDownOpen(value);
                }
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(QuickMenuComboBox), new PropertyMetadata(false));

        public bool IsHoved
        {
            get { return (bool)GetValue(IsHovedProperty); }
            set
            {
                if (IsSelectable)
                {
                    SetValue(IsHovedProperty, value);
                    SetButtonEffect(IsSelected, value);
                    OnHovedStateChange?.Invoke(this, value);
                }
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(QuickMenuComboBox), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(QuickMenuComboBox), new PropertyMetadata(0));


        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

        public static readonly DependencyProperty IsSelectableProperty =
            DependencyProperty.Register("IsSelectable", typeof(bool), typeof(QuickMenuComboBox), new PropertyMetadata(true));

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set
            {
                SetValue(IconMarginProperty, value);
            }
        }

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(QuickMenuComboBox), new PropertyMetadata(DEFAULT_ICON_MARGIN));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set
            {
                SetValue(TextMarginProperty, value);
            }
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(QuickMenuComboBox), new PropertyMetadata(DEFAULT_TEXT_MARGIN));

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(QuickMenuComboBox), new PropertyMetadata(null));

        public bool IsNeedSave { set; get; } = true;
        public bool IsChanged { set; get; } = false;
        public bool IsOpen => ComboBoxCtr.IsDropDownOpen;

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            IsSelected = true;
            //IsHoved = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            IsHoved = true;
            //IsSelected = true;
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            //IsHoved = false;
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);

            //IsHoved = true;
        }

        protected override void OnTouchEnter(TouchEventArgs e)
        {
            base.OnTouchEnter(e);

            //IsSelected = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsHoved = true;
        }

        public void SetButtonEffect(bool isSelected)
        {
            _itemEffect.SetButtonEffect(isSelected, IsHoved);
        }

        public void SetButtonEffect(bool isSelected, bool isHover)
        {
            _itemEffect.SetButtonEffect(isSelected, isHover);
        }

        public void SetDropDownOpen(bool isOpen)
        {
            ComboBoxCtr.IsDropDownOpen = isOpen;
            //IsNeedSave = isOpen;
            if (isOpen)
            {
                SelectedItem = ComboBoxCtr.SelectedItem;
            }
        }

        public void MoveSelectItem(int value)
        {
            if ((ComboBoxCtr.SelectedIndex + value) >= 0 && 
                (ComboBoxCtr.SelectedIndex + value) < ComboBoxCtr.Items.Count - 1)
            {
                ComboBoxCtr.SelectedIndex += value;
            }
        }

        public void ConfirmPressed()
        {
            if (!IsHoved) return;

            if (!IsSelected)
            {
                IsSelected = true;
            }
            else
            {
                SetDropDownOpen(false);
                IsNeedSave = true;
            }
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (SelectedItem != null && ComboBoxCtr.SelectedItem is ItemModel item &&
                !SelectedItem.Equals(item) &&
                (IsNeedSave || IsChanged))
            {
                SelectedItem = ComboBoxCtr.SelectedItem;
                OnSelectItemChanged?.Invoke(this, ComboBoxCtr.SelectedItem);
                IsChanged = false;
            }
            else
            {
                ComboBoxCtr.SelectedItem = SelectedItem;
            }

            IsSelected = false;
        }

        private void ComboBoxCtr_DropDownOpened(object sender, EventArgs e)
        {
            IsNeedSave = true;
            SelectedItem = ComboBoxCtr.SelectedItem;
        }
    }
}
