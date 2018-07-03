using System;
using System.IO;
using System.Windows;
using System.ComponentModel;
using EOSDigital.API;
using EOSDigital.SDK;

namespace WPF_Basic_Net35
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CanonAPI Api;
        Camera MainCamera;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                SetUI(false);

                Api = new CanonAPI();
                Api.CameraAdded += Api_CameraAdded;

                ErrorHandler.NonSevereErrorHappened += ErrorHandler_NonSevereErrorHappened;
                ErrorHandler.SevereErrorHappened += ErrorHandler_SevereErrorHappened;

                SavePathTextBox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "RemotePhoto");

                GetCamera();
            }
            catch (DllNotFoundException)
            {
                SetUI(false);
                MessageBox.Show("Canon DLLs not found. They should lie beside the executable.");
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                if (MainCamera != null) MainCamera.Dispose();
                if (Api != null) Api.Dispose();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            try { MainCamera.TakePhoto(); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void SaveToComputerChBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SaveToComputerChBox.IsChecked == true)
                {
                    MainCamera.SaveTo = SaveTo.Host;
                    MainCamera.SetCapacity(4096, 999999999);
                }
                else MainCamera.SaveTo = SaveTo.Camera;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        #region SDK events

        private void Api_CameraAdded(CanonAPI sender)
        {
            try { Dispatcher.Invoke((Action)delegate { GetCamera(); }); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
        {
            Dispatcher.Invoke((Action)delegate { SetUI(false); });
            MessageBox.Show("Error: " + ex.Message);
        }

        private void ErrorHandler_NonSevereErrorHappened(object sender, ErrorCode ex)
        {
            if (ex == ErrorCode.TAKE_PICTURE_AF_NG) MessageBox.Show("Couldn't focus!");
            else MessageBox.Show("Something happened: " + ex.ToString());
        }

        private void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
        {
            try
            {
                string path = string.Empty;
                SavePathTextBox.Dispatcher.Invoke((Action)delegate { path = SavePathTextBox.Text; });
                MainCamera.DownloadFile(Info, path);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        #endregion

        #region Subroutines

        private void GetCamera()
        {
            if (MainCamera == null)
            {
                var camList = Api.GetCameraList();
                if (camList.Count > 0)
                {
                    MainCamera = camList[0];
                    MainCamera.OpenSession();
                    MainCamera.DownloadReady += MainCamera_DownloadReady;
                    CameraLabel.Content = MainCamera.DeviceName;
                    SetUI(true);
                }
            }
        }

        private void SetUI(bool enable)
        {
            PhotoButton.IsEnabled = enable;
            SaveToComputerChBox.IsEnabled = enable;
            SavePathTextBox.IsEnabled = enable;
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show("An error occurred:" + ex.Message);
        }

        #endregion
    }
}
