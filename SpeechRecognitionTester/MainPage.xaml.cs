using SpeechRecognitionProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SpeechRecognitionTester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaCapture CaptureMedia = null;
        private IRandomAccessStream AudioStream;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;

        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void MediaCaptureOnRecordLimitationExceeded(MediaCapture sender)
        {
            throw new NotImplementedException();
        }

        private void MediaCaptureOnFailed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            throw new NotImplementedException();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CaptureMedia == null)
            {
                btnRecognition.Content = "Stop Voice Recognition";
                CaptureMedia = new MediaCapture();
                var captureInitSettings = new MediaCaptureInitializationSettings();
                captureInitSettings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                await CaptureMedia.InitializeAsync(captureInitSettings);
                CaptureMedia.Failed += MediaCaptureOnFailed;
                CaptureMedia.RecordLimitationExceeded += MediaCaptureOnRecordLimitationExceeded;

                MediaEncodingProfile encodingProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);
                AudioStream = new InMemoryRandomAccessStream();

                await CaptureMedia.StartRecordToStreamAsync(encodingProfile, AudioStream);
            }
            else
            {
                btnRecognition.Content = "Start Voice Recognition";
                await CaptureMedia.StopRecordAsync();

                var auth = new Authenticator();
                string token = await auth.Authenticate("97ca907166a84fa7baf8e3a7a3faca3f");

                var a = new SpeechToText();
                a.AuthorizationToken = token;

                byte[] buffer = null;

                //var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///whatstheweatherlike.wav"));
                //using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
                //{
                //    buffer = new byte[stream.Size];

                //    using (DataReader reader = new DataReader(stream))
                //    {
                //        await reader.LoadAsync((uint)stream.Size);
                //        reader.ReadBytes(buffer);
                //    }
                //}

                using (var dataReader = new DataReader(AudioStream.GetInputStreamAt(0)))
                {
                    await dataReader.LoadAsync((uint)AudioStream.Size);
                    buffer = new byte[(int)AudioStream.Size];
                    dataReader.ReadBytes(buffer);
                }

                var response = await a.Recognize(CancellationToken.None, buffer);
            }
        }
    }
}
