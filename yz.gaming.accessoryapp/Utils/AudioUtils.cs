using NAudio.CoreAudioApi;
using NLog;
using System;

namespace yz.gaming.accessoryapp.Utils
{
    public class AudioUtils
    {
        public delegate void VolumeNotificationHandler(double value);

        private MMDevice defaultMicrophone;
        private MMDevice defaultSpeaker;

        private static AudioUtils _instance = null;
        private double _microphoneVolume = 0;
        private double _speakerVolume = 0;
        private bool _mute = false;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public event VolumeNotificationHandler OnMicrophoneVolumeChanged;
        public event VolumeNotificationHandler OnSpeakerVolumeChanged;

        public double MicrophoneVolume 
        {
            get => _microphoneVolume;
            set
            {
                _microphoneVolume = value;
                if (defaultMicrophone != null)
                {
                    defaultMicrophone.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(_microphoneVolume / 100.00d);
                }
            }
        }

        public double SpeakerVolume
        {
            get => _speakerVolume;
            set
            {
                _speakerVolume = value;
                if (defaultSpeaker != null)
                {
                    defaultSpeaker.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(_speakerVolume / 100.00d);
                }
            }
        }

        public bool Mute
        {
            get => _mute;
            set
            {
                _mute = value;
                if (defaultSpeaker != null)
                {
                    defaultSpeaker.AudioEndpointVolume.Mute = value;
                }
            }
        }

        public static AudioUtils Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AudioUtils();
                    _instance.InitializeDevice();
                }

                return _instance;
            }
        }

        private void InitializeDevice()
        {
            try
            {
                var enumerator = new MMDeviceEnumerator();
                //defaultMicrophone = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                //_microphoneVolume = defaultMicrophone.AudioEndpointVolume.MasterVolumeLevelScalar * 100.00d;

                //defaultMicrophone.AudioEndpointVolume.OnVolumeNotification += p =>
                //{
                //    _microphoneVolume = p.MasterVolume * 100.00d;
                //    OnMicrophoneVolumeChanged?.Invoke(_microphoneVolume);
                //};

                defaultSpeaker = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);
                _speakerVolume = defaultSpeaker.AudioEndpointVolume.MasterVolumeLevelScalar * 100.00d;
                _mute = defaultSpeaker.AudioEndpointVolume.Mute;

                defaultSpeaker.AudioEndpointVolume.OnVolumeNotification += p =>
                {
                    _speakerVolume = p.MasterVolume * 100.00d;
                    _mute = p.Muted;
                    OnSpeakerVolumeChanged?.Invoke(_speakerVolume);
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

        }
    }
}
