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
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.ControllerPage;
using System.Management;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace yz.gaming.accessoryapp.View.ControllerPage
{
    /// <summary>
    /// KeyPageView.xaml 的交互逻辑
    /// </summary>
    public partial class KeyPageView : Page, IPageViewInterface
    {
        KeyPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public KeyPageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<KeyPageViewModel>();
            DataContext = _viewModel;
            string ModelNo = "";
            ModelNo = GetModel();

            if (ModelNo == "ARP31")
            {
                _viewModel.ChildPageMap = new Dictionary<IPageListItem, IPageViewInterface>
                        {
                            //{ KeyDescription, new KeyDescriptionPageView() },
                            { KeyDescription, new KeyDescriptionPageViewARP31() },
                            { ControllerMode, null },
                            { UseNintendoLayout, null },
                            { RepeatMode, new RepetPageView() },
                            { CalibrationAndAdvancedSettings, new CalibrationAndAdvancedSettingPageView() }
                        };
            }
            else if (ModelNo == "ARP31B")
            {
                _viewModel.ChildPageMap = new Dictionary<IPageListItem, IPageViewInterface>
                        {
                            { KeyDescription, new KeyDescriptionPageView() },
                            { ControllerMode, null },
                            { UseNintendoLayout, null },
                            { RepeatMode, new RepetPageView() },
                            { CalibrationAndAdvancedSettings, new CalibrationAndAdvancedSettingPageView() }
                        };
            }
            else
            {
                _viewModel.ChildPageMap = new Dictionary<IPageListItem, IPageViewInterface>
                        {
                            //{ KeyDescription, new KeyDescriptionPageView() },
                            { KeyDescription, new KeyDescriptionPageViewARP31() },
                            { ControllerMode, null },
                            { UseNintendoLayout, null },
                            { RepeatMode, new RepetPageView() },
                            { CalibrationAndAdvancedSettings, new CalibrationAndAdvancedSettingPageView() }
                        };
            }

            KeyDescription.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            ControllerMode.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            UseNintendoLayout.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RepeatMode.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            CalibrationAndAdvancedSettings.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            KeyDescription.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            ControllerMode.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            UseNintendoLayout.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RepeatMode.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            CalibrationAndAdvancedSettings.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;

            KeyDescription.OnClick += _viewModel.OnButtonClick;
            ControllerMode.OnClick += _viewModel.OnButtonClick;
            UseNintendoLayout.OnClick += _viewModel.OnButtonClick;
            RepeatMode.OnClick += _viewModel.OnButtonClick;
            CalibrationAndAdvancedSettings.OnClick += _viewModel.OnButtonClick;

            this.Loaded += KeyPage_Loaded;
        }

        public string GetModel()
        {
            try
            {
                // 获取当前应用程序的目录
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //string dmidecodePath = System.IO.Path.Combine(currentDirectory, "dmidecode.exe");
                string dmidecodePath = "dmidecode.exe";

                // 准备启动 cmd 进程
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = currentDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true; // 设置为隐藏窗口
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                // 启动 cmd 进程
                process.Start();
                process.StandardInput.WriteLine($"{dmidecodePath} -t 11 | find \"String 10:\"");
                process.StandardInput.WriteLine("exit"); // 退出命令行窗口
                process.StandardInput.Flush();
                process.StandardInput.Close();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();

                if (output.Contains("String 10: ARP31B"))
                {
                    return "ARP31B";
                }
                else
                {
                    return "ARP31";
                }
            }
            catch (Exception ex)
            {
                // 捕获并处理异常
                //MessageBox.Show($"访问 dmidecode.exe 失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return "ARP31";
            }

        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.OnChildPageNavigation += navigationParent.ChildPageNavgation;
            _viewModel.Initialization();

            foreach (var item in _viewModel.ChildPageMap)
            {
                item.Value?.Init(navigationParent);
            }

            KeyDescription.IsHoved = true;
            ControllerMode.LeftElement = (byte)0;
            ControllerMode.RightElement = (byte)1;
            _viewModel.Model.OnProfileReresh += OnProfileReresh;

            return this;
        }

        private void KeyPage_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.UnSelectAllItem();
            _viewModel.Initialization();

            if (_viewModel.HovedItem != null)
            {
                _viewModel.HovedItem.IsHoved = true;
                _viewModel.HovedItem.IsSelected = true;
            }
            
            ControllerMode.SelectElement = _viewModel.ControllerMode;
            UseNintendoLayout.IsChecked = _viewModel.UseNintendoLayout;
        }

        private void UseNintendoLayout_OnCheckedStateChanged(IPageListItem sender, bool isChecked)
        {
            _viewModel.UseNintendoLayout = isChecked;
        }

        private void ControllerMode_OnSelectedElementChanged(IPageListItem sender, object value)
        {
            _viewModel.ControllerMode = (byte)value;
        }

        private void OnProfileReresh(YzProfileModel model)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //System.Diagnostics.Debug.WriteLine($"Trace : OnProfileReresh [ControllerMode:{model.ControllerMode}] [KeyLayout:{model.KeyLayout}]");
                _viewModel.ControllerMode = model.ControllerMode;
                _viewModel.UseNintendoLayout = model.KeyLayout == 1;

                ControllerMode.SelectElement = _viewModel.ControllerMode;
                UseNintendoLayout.IsChecked = _viewModel.UseNintendoLayout;
            }));
        }
    }
}
