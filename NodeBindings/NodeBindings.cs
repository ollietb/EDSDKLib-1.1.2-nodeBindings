using System;
using System.Collections.Generic;
using System.IO;
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

        //buffer for preview images
        static MemoryStream PreviewBuffer;

        public async Task<object> BeginSession(dynamic input)
        {
            LogMessage("Beginning session");

            var result = new NodeResult();

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

                result.message = $"Opened session with camera: {MainCamera.DeviceName}";
                result.success= true;

            }catch  (Exception ex)
            {
                result.message = ex.Message;
                result.success = false;
            }
            return result;
        }

        public async Task<object> EndSession(dynamic input)
        {
            LogMessage("Ending session");

            var result = new NodeResult();

            MainCamera?.Dispose();
            MainCamera = null;
            Api.Dispose();
            Api = null;

            result.message = "Camera session ended.";
            result.success = true;
            return result;
        }

        private static void OpenSession(Camera camera)
        {
            LogMessage($"Opening session with camera: {camera.DeviceName}");
            MainCamera = camera;
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
            LogMessage($"Opening session with camera: {MainCamera.DeviceName}");

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
            var result = new NodeResult();
            try
            {
                LogMessage($"Setting output path to \"{input.outputPath}\"");
                SaveDirectory = (string)input.outputPath;
                result.message = "Set output path to " + (string)input.outputPath;
                result.success = true;
            }
            catch (Exception ex){
                //Can't use requested path, resetting to default
                SaveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "RemotePhoto");
                LogMessage(ex.Message.ToString());
                LogMessage("Can't use requested path, resetting to default:" + SaveDirectory.ToString());
                result.message = ex.Message.ToString();
                result.success = false;
            }
            return result;
        }

        private static void APIHandler_CameraAdded(CanonAPI sender)
        {
           try
           {
               LogMessage("Camera added event received");
               if (!RetrySession()) { LogMessage("Sorry, something went wrong. No camera");}
           }
           catch (Exception ex) { LogMessage("Error: " + ex.Message);}
           finally { CameraAddedWaiter.Set(); }
        }

        public async Task<object> StartLiveView(dynamic input)
        {
            LogMessage("Starting live view");

            NodeResult result = new NodeResult();

            try
            {
                LiveViewUpdates = 0;
                MainCamera.StartLiveView();

                LogMessage("Waiting for first live view...");
                LiveViewWaiter.WaitOne();

                result.message = "Starting LiveView";
                result.success = true;
            }
            catch (Exception ex) {
                result.message="Error: " + ex.Message;
                result.success = false;
            }
            
            return result;
        }

        public async Task<object> StopLiveView(dynamic input)
        {
            LogMessage("Stopping live view");

            NodeResult result = new NodeResult();

            try
            {
                MainCamera.StopLiveView();
                result.message = "Stopping LiveView";
                result.success = true;
            }
            catch (Exception ex)
            {
                result.message = "Error: " + ex.Message;
                result.success = false;
            }

            return result;
        }

        public async Task<object> TakePhoto(dynamic input)
        {
            LogMessage("Taking photo");

            var result = new MediaResult();
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

                CaptureSuccess = false;
                while (true) {
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

                result.message = "Photo taken";
                result.path = Path.Combine(SaveDirectory, LastCapturedFileInfo.FileName);
                result.success = true;
            }
            catch(Exception ex) {
                result.message = ex.Message;
                result.success = false;
            }
            finally
            {
                if (shouldRestartLiveView) {
                    MainCamera.StartLiveView();
                }
            }


            return result;
        }

        private static async void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
        {
            try
            {
                LastCapturedFileInfo = Info;
                CaptureSuccess = true;
            }
            catch (Exception ex) {
                LogMessage("Error: " + ex.Message);
                CaptureSuccess = false;
            }
            finally { DownloadReadyWaiter.Set(); }
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
            var result = new NodeResult();

            //Method work goes here...
            try
            {
                MainCamera.StartFilming(true);
                result.message = "Started recording video.";
                result.success = true;
            }
            catch (Exception ex){
                result.message = ex.Message;
                result.success = false;
            }

            return result;
        }

        public async Task<object> StopVideo(dynamic input)
        {
            LogMessage("Stopping video capture");

            var result = new NodeResult();

            bool shouldRestartLiveView = HasInputValue(input, "shouldRestartLiveView")
                ? (bool)input.shouldRestartLiveView
                : false;

            try
            {
                bool save = true;
                MainCamera.StopFilming(save);
                DownloadReadyWaiter.WaitOne();

                result.message = "Stopped recording video.";
                result.success = true;
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
                result.success = false;
            }
            finally
            {
                if (!shouldRestartLiveView) {
                    MainCamera.StopLiveView();
                }
            }

            return result;
        }

        public async Task<object> DownloadLastCapturedFile(dynamic input)
        {
            LogMessage($"Downloading last captured file");

            var result = new MediaResult();

            if (LastCapturedFileInfo == null) {
                result.message = "No file has been captured yet";
                result.success = false;
                return result;
            }

            try
            {
                string FileDownloadPath = Path.Combine(SaveDirectory, LastCapturedFileInfo.FileName);
                LogMessage($"Downloading file to \"{FileDownloadPath}\"");
                await MainCamera.DownloadFile(LastCapturedFileInfo, SaveDirectory);
                LogMessage($"File downloaded to \"{FileDownloadPath}\"");
                result.message = "File downloaded";
                result.path = FileDownloadPath;
                result.success = true;
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
                result.success = false;
            }

            return result;
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
            var result = new PreviewImageResult();
            try
            {
                result.bitmap = PreviewBuffer.GetBuffer();
                result.message = "Preview Image retrieved from buffer";
                result.success = true;
            }
            catch (Exception exp)
            {
                LogMessage(exp.Message);
                result.message = exp.Message;
                result.success = false;
            }
            
            return result;
        }

        public async Task<object> CallCameraMethod(dynamic input)
        {
            var result = new NodeResult();

            LogMessage($"Calling camera method: {input.method}");

            try
            {
                typeof(Camera).GetMethod((string)input.method).Invoke(MainCamera, new object[] {});
                result.message = $"CallCameraMethod: ";
                result.success = true;
            }
            catch (Exception exp) {
                LogMessage(exp.Message);
                result.message = exp.Message;
                result.success = false;
            }


            return result;
        }

        private static void MainCamera_LiveViewUpdated(Camera sender, Stream img)
        {
            LiveViewUpdates++;

            if (LiveViewUpdates == 1 || LiveViewUpdates % 100 == 0)
            {
                // Log only every 100 updates
                LogMessage($"LiveView updated #{LiveViewUpdates}");
            }

            PreviewImageResult result = new PreviewImageResult();

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
                    PreviewBuffer = new MemoryStream();
                    using (PreviewBuffer)
                        encoder.Save(PreviewBuffer);
                }

            }
            catch (Exception ex)
            {
                LogMessage("Error: " + ex.Message);
                result.message = "Error: " + ex.Message;
                result.success = false;
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
    }
}
