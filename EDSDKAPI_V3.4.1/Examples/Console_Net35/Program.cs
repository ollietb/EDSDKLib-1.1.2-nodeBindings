﻿using System;
using EOSDigital.API;
using EOSDigital.SDK;
using System.Threading;

namespace Console_Net35
{
    class Program
    {
        static Camera MainCamera;
        static CanonAPI Api;
        static AutoResetEvent Waiter;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting up...");
                Waiter = new AutoResetEvent(false);
                Api = new CanonAPI();

                var camList = Api.GetCameraList();
                if (camList.Count == 0)
                {
                    Api.CameraAdded += Api_CameraAdded;
                    Console.WriteLine("Please connect a camera...");
                    Waiter.WaitOne();
                }
                else MainCamera = camList[0];

                Console.WriteLine("Open session with " + MainCamera.DeviceName + "...");
                MainCamera.OpenSession();
                MainCamera.DownloadReady += MainCamera_DownloadReady;
                MainCamera.FileDownloaded += MainCamera_FileDownloaded;
                MainCamera.SaveTo = SaveTo.Host;
                MainCamera.SetCapacity(4096, 999999999);

                Console.WriteLine("Press any key to take a photo...");
                Console.ReadKey();
                if (MainCamera.IsShutterButtonAvailable)
                {
                    MainCamera.SC_PressShutterButton(ShutterButton.Completely);
                    MainCamera.SC_PressShutterButton(ShutterButton.OFF);
                }
                else MainCamera.SC_TakePicture();

                Console.WriteLine("Waiting for download...");
                Waiter.WaitOne();

                Console.WriteLine("Closing session...");
                MainCamera.DownloadReady -= MainCamera_DownloadReady;
                MainCamera.FileDownloaded -= MainCamera_FileDownloaded;
                MainCamera.CloseSession();
            }
            catch (DllNotFoundException) { Console.WriteLine("Canon DLLs not found. They should lie beside the executable."); }
            catch (SDKException SDKex)
            {
                if (SDKex.Error == ErrorCode.TAKE_PICTURE_AF_NG) Console.WriteLine("Couldn't focus");
                else throw;
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            finally
            {
                Console.WriteLine("Disposing resources...");
                if (MainCamera != null) MainCamera.Dispose();
                if (Api != null) Api.Dispose();
                Console.WriteLine("Press any key to close...");
                Console.ReadKey();
            }
        }

        static void MainCamera_FileDownloaded(object sender, string Value)
        {
            Console.WriteLine("Downloaded image to " + Value);
            Waiter.Set();
        }

        static void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
        {
            sender.DownloadFile(Info, "Images");
        }

        static void Api_CameraAdded(CanonAPI sender)
        {
            var camList = sender.GetCameraList();
            MainCamera = camList[0];
            Waiter.Set();
        }
    }
}
