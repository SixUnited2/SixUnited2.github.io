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

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// SelectableTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class SelectableComboBox : UserControl, ISelectableItem
    {
        public delegate void SelectableComboBoxSelectItemChangedHandler(SelectableComboBox sender, object item);
        public delegate void SelectableComboBoxSelectedHandler(ISelectableItem sender);
        public delegate void SelectableComboBoxHovedHandler(ISelectableItem sender);
        public event SelectableComboBoxSelectItemChangedHandler OnSelectItemChanged;
        public event SelectableComboBoxSelectedHandler OnSelected;
        public event SelectableComboBoxSelectedHandler OnHoved;

        Thickness DEFAULT_BORDER_MARGIN = new Thickness(2);
        Thickness DEFAULT_BORDER_THICKNESS = new Thickness(1);

        Thickness HOVER_BORDER_MARGIN = new Thickness(0);
        Thickness HOVER_BORDER_THICKNESS = new Thickness(3);

        public SelectableComboBox()
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

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SelectableComboBox), new PropertyMetadata(string.Empty));

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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectableComboBox), new PropertyMetadata(false));

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
            DependencyProperty.Register("IsHover", typeof(bool), typeof(SelectableComboBox), new PropertyMetadata(false));


        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(SelectableComboBox), new PropertyMetadata(0));

        //public object SelectedItem
        //{
        //    get { return (object)GetValue(SelectedItemProperty); }
        //    set
        //    {
        //        SetValue(SelectedItemProperty, value);
        //        CmbBox.SelectedItem = SelectedItem;
        //    }
        //}

        //public static readonly DependencyProperty SelectedItemProperty =
        //    DependencyProperty.Register("SelectedItem", typeof(object), typeof(SelectableComboBox), new PropertyMetadata(null));

        public bool IsOpen => CmbBox.IsDropDownOpen;
        public object SelectedItem
        {
            get => CmbBox.SelectedItem;
            set => CmbBox.SelectedItem = value;
        }

        public void SetSource<T>(IEnumerable<T> source)
        {
            CmbBox.ItemsSource = source;
            CmbBox.SelectedIndex = 0;
            //SelectedItem = CmbBox.SelectedItem;
        }

        public void SetDropDownOpen(bool isOpen)
        {
            CmbBox.IsDropDownOpen = isOpen;
            //IsNeedSave = isOpen;
            //if (isOpen)
            //{
            //    SelectedItem = CmbBox.SelectedItem;
            //}
        }

        public void MoveSelectItem(int value)
        {
            if ((CmbBox.SelectedIndex + value) >= 0 &&
                (CmbBox.SelectedIndex + value) <= CmbBox.Items.Count - 1)
            {
                CmbBox.SelectedIndex += value;
            }
        }

        //private void ComboBox_DropDownClosed(object sender, EventArgs e)
        //{
        //    if (SelectedItem != null && CmbBox.SelectedItem is ItemModel item &&
        //        !SelectedItem.Equals(item))
        //    {
        //        SelectedItem = CmbBox.SelectedItem;
        //        OnSelectItemChanged?.Invoke(this, CmbBox.SelectedItem);
        //    }
        //    else
        //    {
        //        CmbBox.SelectedItem = SelectedItem;
        //    }

        //    IsSelected = false;
        //}

        //private void ComboBoxCtr_DropDownOpened(object sender, EventArgs e)
        //{
        //    SelectedItem = CmbBox.SelectedItem;
        //}
    }
}
