using System;
using System.IO;
using System.Windows.Forms;
using EOSDigital.API;
using EOSDigital.SDK;

namespace WinForms_Basic_Net45
{
    public partial class MainForm : Form
    {
        CanonAPI Api;
        Camera MainCamera;

        public MainForm()
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (MainCamera != null) MainCamera.Dispose();
                if (Api != null) Api.Dispose();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private async void PhotoButton_Click(object sender, EventArgs e)
        {
            try { await MainCamera.TakePhoto(); }
            catch (Exception ex) { ShowError(ex); }
        }

        private async void SaveToComputerChBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (SaveToComputerChBox.Checked)
                {
                    MainCamera.SaveTo = SaveTo.Host;
                    await MainCamera.SetCapacity(4096, 999999999);
                }
                else MainCamera.SaveTo = SaveTo.Camera;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        #region SDK events

        private void Api_CameraAdded(CanonAPI sender)
        {
            try { Invoke((MethodInvoker)delegate { GetCamera(); }); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
        {
            Invoke((MethodInvoker)delegate { SetUI(false); });
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
                    CameraLabel.Text = MainCamera.DeviceName;
                    SetUI(true);
                }
            }
        }

        private void SetUI(bool enable)
        {
            PhotoButton.Enabled = enable;
            SaveToComputerChBox.Enabled = enable;
            SavePathTextBox.Enabled = enable;
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show("An error occurred:" + ex.Message);
        }

        #endregion
    }
}
