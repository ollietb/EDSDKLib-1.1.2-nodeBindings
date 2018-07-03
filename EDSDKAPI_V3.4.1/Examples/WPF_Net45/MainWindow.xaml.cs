using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using EOSDigital.API;
using EOSDigital.SDK;
using SWF = System.Windows.Forms;

namespace WPF_Net45
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        CanonAPI Api;
        Camera MainCamera;
        List<Camera> Cameras = new List<Camera>();
        SWF.FolderBrowserDialog SaveFolderBrowser = new SWF.FolderBrowserDialog();

        bool IsUIInit = false;
        int BulbTime = 10;

        #endregion

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                Api = new CanonAPI();
                Api.CameraAdded += Api_CameraAdded;

                ErrorHandler.NonSevereErrorHappened += ErrorHandler_NonSevereErrorHappened;
                ErrorHandler.SevereErrorHappened += ErrorHandler_SevereErrorHappened;

                SavePathTextBox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "RemotePhoto");
                EnableUI(false);
                EnableSavePath(false);
                BulbTextBox.IsEnabled = false;
                RefreshCameras();
            }
            catch (DllNotFoundException)
            {
                MessageBox.Show("Canon DLLs not found. They should lie beside the executable.");
                SessionButton.IsEnabled = false;
                RefreshButton.IsEnabled = false;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            IsUIInit = false;
            if (MainCamera != null) MainCamera.Dispose();
            if (Api != null) Api.Dispose();
        }

        #region UI Events

        private void CameraListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CameraListBox.SelectedIndex >= 0)
                {
                    var nCam = Cameras[CameraListBox.SelectedIndex];
                    if (MainCamera != null && nCam.Reference != MainCamera.Reference && MainCamera.SessionOpen) MainCamera.CloseSession();
                    MainCamera = nCam;
                    CameraNameLabel.Content = MainCamera.DeviceName;
                    CameraSessionLabel.Content = "Session " + (MainCamera.SessionOpen ? "Open" : "Closed");
                    SessionButton.IsEnabled = true;
                }
                else SessionButton.IsEnabled = false;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void SessionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainCamera.SessionOpen) CloseSession();
                else OpenSession();
                CameraSessionLabel.Content = "Session " + (MainCamera.SessionOpen ? "Open" : "Closed");
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.Exists(SavePathTextBox.Text)) SaveFolderBrowser.SelectedPath = SavePathTextBox.Text;
                if (SaveFolderBrowser.ShowDialog() == SWF.DialogResult.OK) SavePathTextBox.Text = SaveFolderBrowser.SelectedPath;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void AvCoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try { if (IsUIInit) MainCamera.Av = AvValues.GetValue((string)AvCoBox.SelectedItem); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void TvCoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (IsUIInit)
                {
                    MainCamera.Tv = TvValues.GetValue((string)TvCoBox.SelectedItem);
                    BulbTextBox.IsEnabled = (string)TvCoBox.SelectedItem == "Bulb";
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ISOCoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try { if (IsUIInit) MainCamera.ISO = ISOValues.GetValue((string)ISOCoBox.SelectedItem); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void WBCoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try { if (IsUIInit) MainCamera.WhiteBalance = (WhiteBalance)Enum.Parse(typeof(WhiteBalance), (string)WBCoBox.SelectedItem); }
            catch (Exception ex) { ShowError(ex); }
        }

        private async void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((string)TvCoBox.SelectedItem == "Bulb") await MainCamera.TakePhoto(BulbTime * 1000);
                else await MainCamera.TakePhoto();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainCamera.Record == Recording.Ready)
                {
                    MainCamera.StartFilming(true);
                    RecordButton.Content = "Stop Recording";
                }
                else if (MainCamera.IsFilming)
                {
                    MainCamera.StopFilming(!(bool)ST_CameraRdB.IsChecked);
                    RecordButton.Content = "Start Recording";
                    LVButton.Content = "Start Live View";
                }
                else
                {
                    const string message = "Camera is not ready to record. Put the camera into movie mode first.";
                    MessageBox.Show(message, "Camera not ready", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void LVButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainCamera.IsLiveViewOn)
                {
                    MainCamera.StopLiveView();
                    LVButton.IsEnabled = false;
                }
                else
                {
                    MainCamera.StartLiveView();
                    LVButton.Content = "Stop Live View";
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private async void SaveTo_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsUIInit)
                {
                    if ((bool)ST_CameraRdB.IsChecked) { MainCamera.SaveTo = SaveTo.Camera; EnableSavePath(false); }
                    else
                    {
                        if ((bool)ST_ComputerRdB.IsChecked) MainCamera.SaveTo = SaveTo.Host;
                        else if ((bool)ST_BothRdB.IsChecked) MainCamera.SaveTo = SaveTo.Both;
                        EnableSavePath(true);
                        await MainCamera.SetCapacity(4096, 999999999);
                    }
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void BulbTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!BulbTextBox.Text.All(t => char.IsNumber(t)))
                {
                    var nrs = BulbTextBox.Text.Where(t => char.IsNumber(t)).ToArray();
                    if (nrs.Length == 0)
                    {
                        BulbTextBox.Text = "1";
                        BulbTime = 1;
                    }
                    else BulbTextBox.Text = new string(nrs);
                }
                BulbTime = int.Parse(BulbTextBox.Text);
                BulbTime = Math.Max(1, BulbTime);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try { RefreshCameras(); }
            catch (Exception ex) { ShowError(ex); }
        }

        #endregion

        #region SDK Events

        private void Api_CameraAdded(CanonAPI sender)
        {
            try { Dispatcher.Invoke(delegate { RefreshCameras(); }); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
        {
            Dispatcher.Invoke(delegate
            {
                EnableUI(false);
                EnableSavePath(false);
                RefreshButton.IsEnabled = false;
                CameraListBox.IsEnabled = false;
            });
            ShowError(ex);
        }

        private void ErrorHandler_NonSevereErrorHappened(object sender, ErrorCode ex)
        {
            if (ex == ErrorCode.TAKE_PICTURE_AF_NG) MessageBox.Show("Couldn't focus!");
            else MessageBox.Show("Something happened: " + ex.ToString());
        }

        private async void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
        {
            try
            {
                string path = string.Empty;
                SavePathTextBox.Dispatcher.Invoke(delegate { path = SavePathTextBox.Text; });
                await MainCamera.DownloadFile(Info, path);
                MainProgressBar.Dispatcher.Invoke(delegate { MainProgressBar.Value = 0; });
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void MainCamera_CameraHasShutdown(object sender, string Value)
        {
            try
            {
                if (!IsUIInit) return;

                Dispatcher.Invoke(delegate
                {
                    EnableUI(false);
                    EnableSavePath(false);
                    RefreshCameras();
                });
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void MainCamera_LiveViewStopped(Camera sender)
        {
            try
            {
                if (!IsUIInit) return;

                LVButton.Dispatcher.Invoke(delegate
                {
                    LVButton.Content = "Start Live View";
                    LVButton.IsEnabled = true;
                });
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void MainCamera_ProgressChanged(object sender, int Progress, ref bool Cancel)
        {
            MainProgressBar.Dispatcher.Invoke(delegate { MainProgressBar.Value = Progress; });
        }

        #endregion

        #region Subroutines

        private void ShowError(Exception ex)
        {
            MessageBox.Show("An error occurred:" + ex.Message);
        }

        private void RefreshCameras()
        {
            Cameras = Api.GetCameraList();
            CameraListBox.Items.Clear();
            foreach (var cam in Cameras) CameraListBox.Items.Add(cam.DeviceName);

            if (Cameras.Count == 0) SessionButton.IsEnabled = false;
            else
            {
                int idx = Cameras.FindIndex(t => t.SessionOpen);
                if (idx != -1) CameraListBox.SelectedIndex = idx;
                else CameraListBox.SelectedIndex = 0;
            }
        }

        private void OpenSession()
        {
            IsUIInit = false;

            MainCamera.OpenSession();
            MainCamera.DownloadReady += MainCamera_DownloadReady;
            MainCamera.ProgressChanged += MainCamera_ProgressChanged;
            MainCamera.CameraHasShutdown += MainCamera_CameraHasShutdown;
            MainCamera.LiveViewStopped += MainCamera_LiveViewStopped;
            MainLiveView.MainCamera = MainCamera;

            SessionButton.Content = "Close Session";

            AvCoBox.Items.Clear();
            TvCoBox.Items.Clear();
            ISOCoBox.Items.Clear();

            var AvItems = MainCamera.AvSettingsList.Select(t => t.ToString());
            foreach (var Av in AvItems) AvCoBox.Items.Add(Av);
            var TvItems = MainCamera.TvSettingsList.Select(t => t.ToString());
            foreach (var Tv in TvItems) TvCoBox.Items.Add(Tv);
            var ISOItems = MainCamera.ISOSettingsList.Select(t => t.ToString());
            foreach (var ISO in ISOItems) ISOCoBox.Items.Add(ISO);

            AvCoBox.SelectedItem = MainCamera.Av.ToString();
            TvCoBox.SelectedItem = MainCamera.Tv.ToString();
            ISOCoBox.SelectedItem = MainCamera.ISO.ToString();

            var WB = MainCamera.WhiteBalance.ToString();
            if (WBCoBox.Items.Contains(WB)) WBCoBox.SelectedItem = WB;
            else WBCoBox.Text = WB;

            EnableUI(true);
            EnableSavePath(false);
            ST_CameraRdB.IsChecked = true;
            BulbTextBox.IsEnabled = MainCamera.Tv == "Bulb";

            IsUIInit = true;
        }

        private void CloseSession()
        {
            IsUIInit = false;

            MainCamera.CloseSession();

            MainCamera.DownloadReady -= MainCamera_DownloadReady;
            MainCamera.ProgressChanged -= MainCamera_ProgressChanged;
            MainCamera.CameraHasShutdown -= MainCamera_CameraHasShutdown;
            MainCamera.LiveViewStopped -= MainCamera_LiveViewStopped;

            AvCoBox.Items.Clear();
            TvCoBox.Items.Clear();
            ISOCoBox.Items.Clear();

            EnableUI(false);
            EnableSavePath(false);
            BulbTextBox.IsEnabled = false;

            SessionButton.Content = "Open Session";
        }

        private void EnableUI(bool enabled)
        {
            AvCoBox.IsEnabled = enabled;
            TvCoBox.IsEnabled = enabled;
            ISOCoBox.IsEnabled = enabled;
            WBCoBox.IsEnabled = enabled;
            PhotoButton.IsEnabled = enabled;
            RecordButton.IsEnabled = enabled;
            LVButton.IsEnabled = enabled;
            SaveToGroupBox.IsEnabled = enabled;
        }

        private void EnableSavePath(bool enabled)
        {
            SavePathTextBox.IsEnabled = enabled;
            BrowseButton.IsEnabled = enabled;
        }

        #endregion
    }
}
