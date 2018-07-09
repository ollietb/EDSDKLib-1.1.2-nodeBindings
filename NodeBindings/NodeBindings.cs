#pragma warning disable CS1998

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using EdgeJs;
using EOSDigital.API;
using EOSDigital.SDK;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NodeBindings
{
    public class NodeResult
    {
        public string message = "NodeResult: Empty message";
        public bool success = false;
    }

    public class PreviewImageResult
    {
        public string message = "PreviewImageResult: Empty message";
        public byte[] bitmap = null;
        public bool success = false;
    }

    public class MediaResult
    {
        public string message = "MediaResult: Empty message";
        public string path = null;
        public bool success = false;
    }

    class Program
    {
        static Camera MainCamera;
        static CanonAPI Api;

        static string SaveDirectory;
        static DownloadInfo LastCapturedFileInfo;
        static bool CaptureSuccess = false;
        static int LiveViewUpdates = 0;

        static AutoResetEvent CameraAddedWaiter = new AutoResetEvent(false);
        static AutoResetEvent LiveViewWaiter = new AutoResetEvent(false);
        static AutoResetEvent DownloadReadyWaiter = new AutoResetEvent(false);

        // buffer for preview images
        static MemoryStream PreviewBuffer;
        static MemoryStream TempPreviewBuffer;

        public async Task<object> BeginSession(dynamic input)
        {
            LogMessage("Beginning session");

            try
            {
                CameraAddedWaiter = new AutoResetEvent(false);
                if (Api == null )
                {
                    Api = new CanonAPI();
                }

                LogMessage("APIHandler initialised");
                List<Camera> cameras = Api.GetCameraList();

                foreach (var camera in cameras)
                {
                    LogMessage("APIHandler GetCameraList:" + camera);
                }

                if (cameras.Count > 0)
                {
                    OpenSession(cameras[0]);
                }
                else
                {
                    LogMessage("No camera found. Please plug in camera");
                    Api.CameraAdded += APIHandler_CameraAdded;
                    CameraAddedWaiter.WaitOne();
                    CameraAddedWaiter.Reset();
                }

                var result = new NodeResult();
                result.message = $"Opened session with camera: {MainCamera.DeviceName}";
                result.success= true;

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<object> EndSession(dynamic input)
        {
            LogMessage("Ending session");

            MainCamera?.Dispose();
            MainCamera = null;
            Api.Dispose();
            Api = null;

            var result = new NodeResult();
            result.message = "Camera session ended.";
            result.success = true;

            return result;
        }

        private static void OpenSession(Camera camera)
        {
            LogMessage($"Opening session with camera: {camera.DeviceName}");
            MainCamera = camera;

            if (MainCamera.SessionOpen) {
                LogMessage("Camera session already opened");
                return;
            }

            // TODO: Use individual lambda expressions for each action
            // https://stackoverflow.com/questions/2465040/using-lambda-expressions-for-event-handlers
            MainCamera.DownloadReady += MainCamera_DownloadReady;
            MainCamera.LiveViewUpdated += MainCamera_LiveViewUpdated;
            MainCamera.StateChanged += MainCamera_StateChanged;
            MainCamera.OpenSession();

            LogMessage($"Opened session with camera: {MainCamera.DeviceName}");
        }

        public static bool RetrySession()
        {
            List<Camera> cameras = Api.GetCameraList();
            if (cameras.Count > 0)
            {
                OpenSession(cameras[0]);
                return true;
            }
            else return false;
        }

        public async Task<object> SetOutputPath(dynamic input)
        {
            try
            {
                LogMessage($"Setting output path to \"{input.outputPath}\"");
                SaveDirectory = (string)input.outputPath;

                var result = new NodeResult();
                result.message = "Set output path to " + (string)input.outputPath;
                result.success = true;

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private static void APIHandler_CameraAdded(CanonAPI sender)
        {
           try
           {
               LogMessage("Camera added event received");
               if (!RetrySession()) { LogMessage("Sorry, something went wrong. No camera");}
           }
           catch (Exception ex) { LogMessage("Error: " + ex.ToString());}
           finally { CameraAddedWaiter.Set(); }
        }

        public async Task<object> StartLiveView(dynamic input)
        {
            LogMessage("Starting live view");

            try
            {
                LiveViewUpdates = 0;
                LiveViewWaiter.Reset();
                MainCamera.StartLiveView();

                LogMessage("Waiting for first live view");
                LiveViewWaiter.WaitOne();
                LogMessage("Live view started");

                NodeResult result = new NodeResult();
                result.message = "Live view started";
                result.success = true;

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<object> StopLiveView(dynamic input)
        {
            LogMessage("Stopping live view");

            try
            {
                MainCamera.StopLiveView();
                LogMessage("Live view stopped");

                NodeResult result = new NodeResult();
                result.message = "Live view stopped";
                result.success = true;

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<object> TakePhoto(dynamic input)
        {
            LogMessage("Taking photo");

            var maxTries = 5;
            var tries = 0;
            bool shouldRestartLiveView = false;

            try
            {
                MainCamera.SaveTo = SaveTo.Host;

                if (MainCamera.IsLiveViewOn) {
                    shouldRestartLiveView = HasInputValue(input, "shouldRestartLiveView")
                        ? (bool)input.shouldRestartLiveView
                        : true;
                    MainCamera.StopLiveView();
                }

                await MainCamera.SetCapacity(4096, 999999999);

                while (true) {
                    CaptureSuccess = false;

                    DownloadReadyWaiter.Reset();
                    await MainCamera.TakePhoto();
                    // TODO: make it timeout
                    // https://stackoverflow.com/questions/22678428/any-way-to-trigger-timeout-on-waitone-immediately
                    DownloadReadyWaiter.WaitOne();

                    if (CaptureSuccess) {
                        break;
                    } else if (tries >= maxTries) {
                        throw new Exception($"Photo not taken in {tries} tries");
                    }
                    else {
                        LogMessage("Retrying to take photo");
                        await Task.Delay(100);
                        tries++;
                    }
                }

                string downloadedFilePath = await DownloadFile(LastCapturedFileInfo);

                var result = new MediaResult();
                result.message = "Photo taken";
                result.path = downloadedFilePath;
                result.success = true;

                return result;
            }
            catch(Exception ex) {
                return HandleException(ex);
            }
            finally
            {
                if (shouldRestartLiveView) {
                    MainCamera.StartLiveView();
                }
            }
        }

        private static async void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
        {
            try
            {
                LastCapturedFileInfo = Info;
                CaptureSuccess = true;
            }
            catch (Exception ex) {
                LogMessage("Error: " + ex.ToString());
                CaptureSuccess = false;
            }
            finally
            {
                DownloadReadyWaiter.Set();
            }
        }

        private static void MainCamera_StateChanged(Camera sender, StateEventID eventID, uint parameter)
        {
            LogMessage("StateChanged "+ eventID);

            if (eventID == StateEventID.CaptureError) {
                CaptureSuccess = false;
                DownloadReadyWaiter.Set();
            }
        }

        public async Task<object> StartVideo(dynamic input)
        {
            LogMessage("Starting video capture");

            // needs live view on (at least in 70D)

            try
            {
                MainCamera.StartFilming(true);
                LogMessage("Started video capture");

                var result = new NodeResult();
                result.message = "Started recording video";
                result.success = true;

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<object> StopVideo(dynamic input)
        {
            LogMessage("Stopping video capture");

            bool shouldRestartLiveView = HasInputValue(input, "shouldRestartLiveView")
                ? (bool)input.shouldRestartLiveView
                : false;

            try
            {
                DownloadReadyWaiter.Reset();
                MainCamera.StopFilming(saveFilm: true, stopLiveView: false);
                DownloadReadyWaiter.WaitOne();

                var result = new NodeResult();
                result.message = "Stopped recording video.";
                result.success = true;

                return result;
            }
            catch (Exception ex)
            {
               return HandleException(ex);
            }
            finally
            {
                if (!shouldRestartLiveView) {
                    MainCamera.StopLiveView();
                }
            }
        }

        public async Task<object> DownloadLastCapturedFile(dynamic input)
        {
            LogMessage($"Downloading last captured file");

            if (LastCapturedFileInfo == null) {
                var result = new NodeResult();
                result.message = "No file has been captured yet";
                result.success = false;

                return result;
            }

            try
            {
                string downloadedFilePath = await DownloadFile(LastCapturedFileInfo);

                var result = new MediaResult();
                result.message = "Downloaded file";
                result.path = downloadedFilePath;
                result.success = true;

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

        }

        public static async Task<string> DownloadFile(DownloadInfo downloadInfo)
        {
            string downloadedFilePath = Path.Combine(SaveDirectory, downloadInfo.FileName);
            double sizeInMb = (downloadInfo.Size / 1024f) / 1024f;
            string sizeInMbString = String.Format("{0:0.00}", sizeInMb);
            LogMessage($"Downloading file ({sizeInMbString}MB) to \"{downloadedFilePath}\"");
            await MainCamera.DownloadFile(downloadInfo, SaveDirectory);
            LogMessage($"File downloaded to \"{downloadedFilePath}\"");

            return downloadedFilePath;
        }

        public async Task<object> GetLastCapturedFileName(dynamic input)
        {
            LogMessage("Getting last captured file name");

            var result = new NodeResult();

            if (LastCapturedFileInfo == null) {
                result.message = "No file has been captured yet";
                result.success = false;

                return result;
            }

            string fileName = LastCapturedFileInfo.FileName;
            LogMessage($"Last captured file name \"{fileName}\"");

            result.message = fileName;
            result.success = true;
            return result;
        }
      
        public async Task<object> GetPreviewImage(dynamic input)
        {
            try
            {
                var result = new PreviewImageResult();
                result.bitmap = PreviewBuffer.GetBuffer();
                result.message = "Preview Image retrieved from buffer";
                result.success = true;

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<object> CallCameraMethod(dynamic input)
        {
            LogMessage($"Calling camera method: {input.method}");

            try
            {
                typeof(Camera).GetMethod((string)input.method).Invoke(MainCamera, new object[] {});

                var result = new NodeResult();
                result.message = $"CallCameraMethod: ";
                result.success = true;

                return result;
            }
            catch (Exception ex) {
                return HandleException(ex);
            }
        }

        private static void MainCamera_LiveViewUpdated(Camera sender, Stream img)
        {
            LiveViewUpdates++;

            if (LiveViewUpdates == 1 || LiveViewUpdates % 100 == 0)
            {
                // Log only every 100 updates
                LogMessage($"Live view updated #{LiveViewUpdates}");
            }

            try
            {
                using (WrapStream s = new WrapStream(img))
                {
                    img.Position = 0;
                    BitmapImage EvfImage = new BitmapImage();
                    EvfImage.BeginInit();
                    EvfImage.StreamSource = s;
                    EvfImage.CacheOption = BitmapCacheOption.OnLoad;
                    EvfImage.EndInit();
                    EvfImage.Freeze();

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapImage)EvfImage));
                    TempPreviewBuffer = new MemoryStream();
                    encoder.Save(TempPreviewBuffer);
                    PreviewBuffer = TempPreviewBuffer;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                LiveViewWaiter.Set();
            }
        }

        private static void LogMessage(string message)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine(String.Format("{0:s}.{1}", now, now.Millisecond) + " [CanonCameraV2] " + message);
        }

        private static bool HasInputValue(dynamic input, string key)
        {
            return ((IDictionary<String, object>)input).ContainsKey(key);
        }

        private static NodeResult HandleException(Exception ex)
        {
            LogMessage("Error: " + ex.ToString());

            var result = new NodeResult();
            result.message = "Error: " + ex.Message;
            result.success = false;

            return result;
        }
    }
}
