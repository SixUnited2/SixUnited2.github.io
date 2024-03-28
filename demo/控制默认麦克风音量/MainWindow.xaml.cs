using NAudio.CoreAudioApi;
using System;
using System.Data;
using System.Windows;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private MMDevice defaultMicrophone;
        private MMDevice defaultSpeaker;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMicrophone();
        }

        private void InitializeMicrophone()
        {
            var enumerator = new MMDeviceEnumerator();

            try
            {
                defaultMicrophone = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                micphoneSlider.Value = defaultMicrophone.AudioEndpointVolume.MasterVolumeLevelScalar * 100;

                defaultMicrophone.AudioEndpointVolume.OnVolumeNotification += p =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        micphoneSlider.Value = p.MasterVolume * 100;
                    }));
                };

                defaultSpeaker = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);
                speakerSlider.Value = defaultSpeaker.AudioEndpointVolume.MasterVolumeLevelScalar * 100;

                defaultSpeaker.AudioEndpointVolume.OnVolumeNotification += p =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        speakerSlider.Value = p.MasterVolume * 100;
                    }));
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void OnSetVolume(object sender, RoutedEventArgs e)
        {
            if (defaultMicrophone != null)
            {
               defaultMicrophone.AudioEndpointVolume.MasterVolumeLevelScalar = (float)micphoneSlider.Value / 100;
            }
        }
    }
}
