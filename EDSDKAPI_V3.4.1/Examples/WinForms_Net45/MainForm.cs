using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using EOSDigital.API;
using EOSDigital.SDK;

namespace WinForms_Net45
{
    public partial class MainForm : Form
    {
        #region Variables

        CanonAPI Api;
        Camera MainCamera;
        List<Camera> Cameras = new List<Camera>();

        bool IsUIInit = false;
        int BulbTime = 10;

        #endregion

        public MainForm()
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
                BulbTextBox.Enabled = false;
                RefreshCameras();
            }
            catch (DllNotFoundException)
            {
                MessageBox.Show("Canon DLLs not found. They should lie beside the executable.");
                SessionButton.Enabled = false;
                RefreshButton.Enabled = false;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsUIInit = false;
            if (MainCamera != null) MainCamera.Dispose();
            if (Api != null) Api.Dispose();
        }

        #region UI Events

        private void CameraListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (CameraListBox.SelectedIndex >= 0)
                {
                    var nCam = Cameras[CameraListBox.SelectedIndex];
                    if (MainCamera != null && nCam.Reference != MainCamera.Reference && MainCamera.SessionOpen) MainCamera.CloseSession();
                    MainCamera = nCam;
                    CameraNameLabel.Text = MainCamera.DeviceName;
                    CameraSessionLabel.Text = "Session " + (MainCamera.SessionOpen ? "Open" : "Closed");
                    SessionButton.Enabled = true;
                }
                else SessionButton.Enabled = false;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void SessionButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainCamera.SessionOpen) CloseSession();
                else OpenSession();
                CameraSessionLabel.Text = "Session " + (MainCamera.SessionOpen ? "Open" : "Closed");
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(SavePathTextBox.Text)) SaveFolderBrowser.SelectedPath = SavePathTextBox.Text;
                if (SaveFolderBrowser.ShowDialog() == DialogResult.OK) SavePathTextBox.Text = SaveFolderBrowser.SelectedPath;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void AvCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { if (IsUIInit) MainCamera.Av = AvValues.GetValue((string)AvCoBox.SelectedItem); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void TvCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsUIInit)
                {
                    MainCamera.Tv = TvValues.GetValue((string)TvCoBox.SelectedItem);
                    BulbTextBox.Enabled = (string)TvCoBox.SelectedItem == "Bulb";
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ISOCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { if (IsUIInit) MainCamera.ISO = ISOValues.GetValue((string)ISOCoBox.SelectedItem); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void WBCoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { if (IsUIInit) MainCamera.WhiteBalance = (WhiteBalance)Enum.Parse(typeof(WhiteBalance), (string)WBCoBox.SelectedItem); }
            catch (Exception ex) { ShowError(ex); }
        }

        private async void PhotoButton_Click(object sender, EventArgs e)
        {
            try
            {
                if ((string)TvCoBox.SelectedItem == "Bulb") await MainCamera.TakePhoto(BulbTime * 1000);
                else await MainCamera.TakePhoto();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void RecordButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainCamera.Record == Recording.Ready)
                {
                    MainCamera.StartFilming(true);
                    RecordButton.Text = "Stop Recording";
                }
                else if (MainCamera.IsFilming)
                {
                    MainCamera.StopFilming(!ST_CameraRdB.Checked);
                    RecordButton.Text = "Start Recording";
                    LVButton.Text = "Start Live View";
                }
                else
                {
                    const string message = "Camera is not ready to record. Put the camera into movie mode first.";
                    MessageBox.Show(message, "Camera not ready", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void LVButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainCamera.IsLiveViewOn)
                {
                    MainCamera.StopLiveView();
                    LVButton.Enabled = false;
                }
                else
                {
                    MainCamera.StartLiveView();
                    LVButton.Text = "Stop Live View";
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private async void SaveTo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsUIInit)
                {
                    if (ST_CameraRdB.Checked) { MainCamera.SaveTo = SaveTo.Camera; EnableSavePath(false); }
                    else
                    {
                        if (ST_ComputerRdB.Checked) MainCamera.SaveTo = SaveTo.Host;
                        else if (ST_BothRdB.Checked) MainCamera.SaveTo = SaveTo.Both;
                        EnableSavePath(true);
                        await MainCamera.SetCapacity(4096, 999999999);
                    }
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void BulbTextBox_TextChanged(object sender, EventArgs e)
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

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            try { RefreshCameras(); }
            catch (Exception ex) { ShowError(ex); }
        }

        #endregion

        #region SDK Events

        private void Api_CameraAdded(CanonAPI sender)
        {
            try { Invoke((MethodInvoker)delegate { RefreshCameras(); }); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
        {
            Invoke((MethodInvoker)delegate
            {
                EnableUI(false);
                EnableSavePath(false);
                RefreshButton.Enabled = false;
                CameraListBox.Enabled = false;
                SessionButton.Enabled = false;
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
                Invoke((MethodInvoker)delegate { path = SavePathTextBox.Text; });
                await MainCamera.DownloadFile(Info, path);
                Invoke((MethodInvoker)delegate { MainProgressBar.Value = 0; });
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void MainCamera_CameraHasShutdown(object sender, string Value)
        {
            try
            {
                if (!IsUIInit) return;

                Invoke((MethodInvoker)delegate
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

                Invoke((MethodInvoker)delegate
                {
                    LVButton.Text = "Start Live View";
                    LVButton.Enabled = true;
                });
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void MainCamera_ProgressChanged(object sender, int Progress, ref bool Cancel)
        {
            Invoke((MethodInvoker)delegate { MainProgressBar.Value = Progress; });
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

            if (Cameras.Count == 0) SessionButton.Enabled = false;
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

            SessionButton.Text = "Close Session";

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
            ST_CameraRdB.Checked = true;
            BulbTextBox.Enabled = MainCamera.Tv == "Bulb";

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
            BulbTextBox.Enabled = false;

            SessionButton.Text = "Open Session";
        }

        private void EnableUI(bool enabled)
        {
            AvCoBox.Enabled = enabled;
            TvCoBox.Enabled = enabled;
            ISOCoBox.Enabled = enabled;
            WBCoBox.Enabled = enabled;
            PhotoButton.Enabled = enabled;
            RecordButton.Enabled = enabled;
            LVButton.Enabled = enabled;
            SaveToGroupBox.Enabled = enabled;
        }

        private void EnableSavePath(bool enabled)
        {
            SavePathTextBox.Enabled = enabled;
            BrowseButton.Enabled = enabled;
        }

        #endregion
    }
}
