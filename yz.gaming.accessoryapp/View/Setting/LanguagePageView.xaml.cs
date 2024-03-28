using CommunityToolkit.Mvvm.DependencyInjection;
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
using yz.gaming.accessoryapp.Languange;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.ControllerPage;

namespace yz.gaming.accessoryapp.View.Setting
{
    /// <summary>
    /// LanguagePageView.xaml 的交互逻辑
    /// </summary>
    public partial class LanguagePageView : Page, IPageViewInterface
    {
        LanguagePageViewModel _viewModel = null;
        Action<string> _setTitle = null;
        public IViewModel ViewModel => _viewModel;

        public LanguagePageView()
        {
            InitializeComponent();
            _viewModel = Ioc.Default.GetRequiredService<LanguagePageViewModel>();
            DataContext = _viewModel;

            _viewModel.LanguageComboBox = LanguageComboBox;
            LanguageComboBox.ItemsSource = _viewModel.LanguageData;
            LanguageComboBox.DropDownClosed += LanguageComboBox_DropDownClosed;
            Loaded += LanguagePageView_Loaded;
        }

        private void LanguagePageView_Loaded(object sender, RoutedEventArgs e)
        {
            var lang = _viewModel.GetModelByValue(Properties.Settings.Default.Languange);
            LanguageComboBox.SelectedIndex = lang.Index;
            _viewModel.SelectedItem = LanguageComboBox.SelectedItem;
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();

            if (_setTitle == null)
            {
                _setTitle = navigationParent.UpdateTitle;
            }

            return this;
        }

        private void LanguageComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ItemModel item &&
                !_viewModel.SelectedItem.Equals(item) &&
                _viewModel.IsNeedSave)
            {
                LanguangeManager.Instance.SetLanguage(item.Value.ToString());
                _viewModel.Title = _viewModel.GetString("Language");
                _setTitle(_viewModel.Title);
                Properties.Settings.Default.Languange = item.Value.ToString();
                Properties.Settings.Default.Save();

                _viewModel.SelectedItem = LanguageComboBox.SelectedItem;
            }
            else
            {
                LanguageComboBox.SelectedItem = _viewModel.SelectedItem;
            }

            _viewModel.IsNeedSave = true;
        }
    }
}
