using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EOSDigital.SDK;

namespace EOSDigital.API
{
    public class Camera : IDisposable
    {
        protected DeviceInfo Info;
        protected STAThread MainThread;

        protected internal Camera(IntPtr camRef);

        ~Camera();

        protected static bool GlobalIsLiveViewOn { get; }
        [CameraProperty(PropertyID.Evf_ZoomPosition)]
        public Point Evf_ZoomPosition { set; }
        [CameraProperty(PropertyID.CurrentStorage)]
        public StorageType CurrentStorage { get; }
        [CameraProperty(PropertyID.SaveTo)]
        public SaveTo SaveTo { get; set; }
        [CameraProperty(PropertyID.Unknown)]
        public bool IsACPowered { get; }
        [CameraProperty(PropertyID.BatteryLevel)]
        public uint BatteryLevel { get; }
        [CameraProperty(PropertyID.FirmwareVersion)]
        public string FirmwareVersion { get; }
        [CameraProperty(PropertyID.DateTime)]
        public Time DateTime { get; }
        [CameraProperty(PropertyID.MakerName)]
        public string MakerName { get; }
        [CameraProperty(PropertyID.Copyright, MaxLength = 64)]
        public string Copyright { get; set; }
        [CameraProperty(PropertyID.Artist, MaxLength = 64)]
        public string Artist { get; set; }
        [CameraProperty(PropertyID.OwnerName, MaxLength = 256)]
        public string OwnerName { get; set; }
        [CameraProperty(PropertyID.ProductName, MaxLength = 256)]
        public string ProductName { get; }
        [CameraPropertyList(PropertyID.ExposureCompensation)]
        public CameraValue[] ExposureCompensationSettingsList { get; }
        [CameraPropertyList(PropertyID.MeteringMode)]
        public CameraValue[] MeteringModeSettingsList { get; }
        [CameraPropertyList(PropertyID.AEModeSelect)]
        public CameraValue[] AEModeSettingsList { get; }
        [CameraPropertyList(PropertyID.ISO)]
        public CameraValue[] ISOSettingsList { get; }
        [CameraPropertyList(PropertyID.Av)]
        public CameraValue[] AvSettingsList { get; }
        [CameraPropertyList(PropertyID.Tv)]
        public CameraValue[] TvSettingsList { get; }
        public bool KeepOn { get; set; }
        public EvfMetadata EvfImageInfo { get; }
        public bool IsRecordAvailable { get; }
        public bool IsShutterButtonAvailable { get; }
        public bool IsFilming { get; }
        public bool IsLiveViewPaused { get; protected set; }
        public bool IsLiveViewOn { get; protected set; }
        public string PortName { get; }
        public string DeviceName { get; }
        public bool IsDisposed { get; }
        public bool SessionOpen { get; protected set; }
        public long ID { get; }
        [CameraProperty(PropertyID.CurrentFolder)]
        public string CurrentFolder { get; }
        [CameraProperty(PropertyID.BatteryQuality)]
        public BatteryQuality BatteryQuality { get; }
        [CameraProperty(PropertyID.BodyIDEx)]
        public string BodyID { get; }
        [CameraProperty(PropertyID.HDDirectoryStructure, MaxLength = 256)]
        public string HDDirectoryStructure { get; set; }
        [CameraProperty(PropertyID.Evf_Zoom)]
        public EvfZoom Evf_Zoom { set; }
        [CameraProperty(PropertyID.Evf_DepthOfFieldPreview)]
        public bool Evf_DepthOfFieldPreview { get; set; }
        [CameraProperty(PropertyID.Evf_ColorTemperature)]
        public uint Evf_ColorTemperature { get; set; }
        [CameraProperty(PropertyID.Evf_WhiteBalance)]
        public WhiteBalance Evf_WhiteBalance { get; set; }
        [CameraProperty(PropertyID.Evf_Mode)]
        public bool Evf_Mode { get; set; }
        [CameraProperty(PropertyID.Evf_OutputDevice)]
        public EvfOutputDevice Evf_OutputDevice { get; set; }
        [CameraProperty(PropertyID.Record)]
        public Recording Record { get; protected set; }
        [CameraProperty(PropertyID.LensStatus)]
        public bool LensStatus { get; }
        [CameraProperty(PropertyID.WhiteBalanceBracket)]
        public WhiteBalanceBracket WhiteBalanceBracket { get; }
        [CameraProperty(PropertyID.Bracket)]
        public BracketMode Bracket { get; }
        [CameraProperty(PropertyID.AvailableShots)]
        public uint AvailableShots { get; }
        [CameraProperty(PropertyID.AFMode)]
        public AFMode AFMode { get; set; }
        [CameraProperty(PropertyID.FlashCompensation)]
        public CameraValue FlashCompensation { get; }
        [CameraProperty(PropertyID.ExposureCompensation)]
        public CameraValue ExposureCompensation { get; set; }
        [CameraProperty(PropertyID.Evf_AFMode)]
        public EvfAFMode Evf_AFMode { get; set; }
        [CameraProperty(PropertyID.Tv)]
        public CameraValue Tv { get; set; }
        [CameraProperty(PropertyID.MeteringMode)]
        public CameraValue MeteringMode { get; set; }
        [CameraProperty(PropertyID.ISO)]
        public CameraValue ISO { get; set; }
        [CameraProperty(PropertyID.DriveMode)]
        public DriveMode DriveMode { get; set; }
        [CameraProperty(PropertyID.AEMode)]
        public CameraValue AEMode { get; set; }
        [CameraProperty(PropertyID.PictureStyleDesc)]
        public PictureStyleDesc PictureStyleDesc { get; set; }
        [CameraProperty(PropertyID.ParameterSet)]
        [Obsolete("Valid only for the EOS 1D Mark II and EOS 1Ds Mark II")]
        public ProcessingParameter ParameterSet { get; set; }
        [CameraProperty(PropertyID.PictureStyle)]
        public PictureStyle PictureStyle { get; set; }
        [CameraProperty(PropertyID.ColorSpace)]
        public ColorSpace ColorSpace { get; set; }
        [CameraProperty(PropertyID.WhiteBalanceShift)]
        public WhiteBalanceShift WhiteBalanceShift { get; set; }
        [CameraProperty(PropertyID.ColorTemperature)]
        [Obsolete("Invalid for models which don't support ColorTemperature whitebalance setting.")]
        public uint ColorTemperature { get; set; }
        [CameraProperty(PropertyID.WhiteBalance)]
        public WhiteBalance WhiteBalance { get; set; }
        [CameraProperty(PropertyID.FocusInfo)]
        public FocusInfo FocusInfo { get; set; }
        [CameraProperty(PropertyID.ImageQuality)]
        public ImageQuality ImageQuality { get; set; }
        public IntPtr Reference { get; }
        [CameraProperty(PropertyID.Av)]
        public CameraValue Av { get; set; }
        [CameraProperty(PropertyID.MyMenu)]
        public MyMenuItems MyMenu { get; set; }

        public static event CameraSessionHandler SessionChanged;
        public event CameraUpdateHandler LiveViewStopped;
        public event StringUpdate CameraHasShutdown;
        public event ObjectChangeHandler ObjectChanged;
        public event StateChangeHandler StateChanged;
        public event PropertyChangeHandler PropertyChanged;
        public event DownloadHandler DownloadReady;
        public event LiveViewUpdate LiveViewUpdated;
        public event ProgressHandler ProgressChanged;
        public event CameraSessionHandler SessionStateChanged;
        protected event SDKPropertyEventHandler SDKPropertyEvent;
        protected event SDKObjectEventHandler SDKObjectEvent;
        protected event SDKStateEventHandler SDKStateEvent;
        protected event SDKProgressCallback SDKProgressCallbackEvent;

        [AsyncStateMachine(typeof(<CancelDownload>d__324))]
        public Task CancelDownload(DownloadInfo Info);
        public void CloseSession();
        [AsyncStateMachine(typeof(<DeleteFiles>d__320))]
        public Task DeleteFiles(params CameraFileEntry[] files);
        public void Dispose();
        [AsyncStateMachine(typeof(<DownloadFile>d__322))]
        public Task DownloadFile(DownloadInfo Info, string directory);
        [AsyncStateMachine(typeof(<DownloadFile>d__323))]
        public Task<Stream> DownloadFile(DownloadInfo Info);
        [AsyncStateMachine(typeof(<DownloadFiles>d__319))]
        public Task DownloadFiles(string folderpath, params CameraFileEntry[] files);
        [AsyncStateMachine(typeof(<FormatVolume>d__316))]
        public Task FormatVolume(CameraFileEntry volume);
        [AsyncStateMachine(typeof(<GetAllEntries>d__321))]
        public Task<CameraFileEntry> GetAllEntries();
        [AsyncStateMachine(typeof(<GetAllImages>d__318))]
        public Task<CameraFileEntry[]> GetAllImages();
        [AsyncStateMachine(typeof(<GetAllVolumes>d__317))]
        public Task<CameraFileEntry[]> GetAllVolumes();
        public CFnSelection GetCFn(CFnDescription description);
        public CFnSelection GetCFn(int typeID);
        public CFnSelection GetCFn(CFnValue type);
        public void OpenSession();
        public void PauseLiveView();
        public void ResumeLiveView();
        [AsyncStateMachine(typeof(<SC_BulbEnd>d__296))]
        [CameraCommand(CameraCommand.BulbEnd)]
        public Task SC_BulbEnd();
        [AsyncStateMachine(typeof(<SC_BulbStart>d__295))]
        [CameraCommand(CameraCommand.BulbStart)]
        public Task SC_BulbStart();
        [AsyncStateMachine(typeof(<SC_DoClickWBEvf>d__301))]
        [CameraCommand(CameraCommand.DoClickWBEvf, new[] { 0, 0 })]
        public Task SC_DoClickWBEvf(ushort x, ushort y);
        [AsyncStateMachine(typeof(<SC_DoEvfAf>d__298))]
        [CameraCommand(CameraCommand.DoEvfAf, new[] { false })]
        public Task SC_DoEvfAf(bool state);
        [AsyncStateMachine(typeof(<SC_DriveLensEvf>d__300))]
        [CameraCommand(CameraCommand.DriveLensEvf, new[] { DriveLens.Far1 })]
        public Task SC_DriveLensEvf(DriveLens focus);
        [AsyncStateMachine(typeof(<SC_ExtendShutDownTimer>d__294))]
        [CameraCommand(CameraCommand.ExtendShutDownTimer)]
        public Task SC_ExtendShutDownTimer();
        [AsyncStateMachine(typeof(<SC_MovieModeOff>d__293))]
        [CameraCommand(CameraCommand.MovieModeOff)]
        public Task SC_MovieModeOff();
        [AsyncStateMachine(typeof(<SC_MovieModeOn>d__292))]
        [CameraCommand(CameraCommand.MovieModeOn)]
        public Task SC_MovieModeOn();
        [AsyncStateMachine(typeof(<SC_OpenInternalFlash>d__291))]
        [CameraCommand(CameraCommand.OpenInternalFlash)]
        public Task SC_OpenInternalFlash();
        [AsyncStateMachine(typeof(<SC_PressShutterButton>d__297))]
        [CameraCommand(CameraCommand.PressShutterButton, new[] { ShutterButton.OFF })]
        public Task SC_PressShutterButton(ShutterButton state);
        [AsyncStateMachine(typeof(<SC_TakePicture>d__290))]
        [CameraCommand(CameraCommand.TakePicture)]
        public Task SC_TakePicture();
        [AsyncStateMachine(typeof(<SetCapacity>d__325))]
        public Task SetCapacity(int BytesPerSector, int NumberOfFreeClusters);
        [AsyncStateMachine(typeof(<SSC_EnterDirectTransfer>d__305))]
        [CameraStatusCommand(CameraStatusCommand.EnterDirectTransfer)]
        public Task SSC_EnterDirectTransfer();
        [AsyncStateMachine(typeof(<SSC_ExitDirectTransfer>d__306))]
        [CameraStatusCommand(CameraStatusCommand.ExitDirectTransfer)]
        public Task SSC_ExitDirectTransfer();
        [AsyncStateMachine(typeof(<SSC_UILock>d__303))]
        [CameraStatusCommand(CameraStatusCommand.UILock)]
        public Task SSC_UILock();
        [AsyncStateMachine(typeof(<SSC_UIUnLock>d__304))]
        [CameraStatusCommand(CameraStatusCommand.UIUnLock)]
        public Task SSC_UIUnLock();
        public void StartFilming(bool PCLiveview);
        public void StartLiveView();
        public void StopFilming(bool saveFilm);
        public void StopFilming(bool saveFilm, bool stopLiveView);
        public void StopLiveView(bool LVOff = true);
        [AsyncStateMachine(typeof(<TakePhoto>d__308))]
        public Task TakePhoto(int bulbTime);
        [AsyncStateMachine(typeof(<TakePhoto>d__309))]
        public Task TakePhoto(uint delay);
        [AsyncStateMachine(typeof(<TakePhoto>d__310))]
        public Task TakePhoto(uint delay, int bulbTime);
        [AsyncStateMachine(typeof(<TakePhoto>d__312))]
        public Task TakePhoto(bool useAf);
        [AsyncStateMachine(typeof(<TakePhoto>d__313))]
        public Task TakePhoto(uint delay, bool useAf);
        [AsyncStateMachine(typeof(<TakePhoto>d__307))]
        public Task TakePhoto();
        [AsyncStateMachine(typeof(<UILock>d__326))]
        public Task UILock(bool LockState);
        protected void CheckState(bool checkSession = true);
        protected virtual void Dispose(bool managed);
        protected void DownloadData(DownloadInfo Info, IntPtr stream);
        protected void DownloadEvfMetadata(IntPtr EvfImageRef);
        protected void DownloadToFile(DownloadInfo Info, string filepath);
        protected Stream DownloadToStream(DownloadInfo Info);
        protected bool GetBool(int val);
        protected bool[] GetBoolArrSetting(PropertyID propID, int inParam = 0);
        protected bool GetBoolSetting(PropertyID propID, int inParam = 0);
        protected byte[] GetByteArrSetting(PropertyID propID, int inParam = 0);
        protected byte GetByteSetting(PropertyID propID, int inParam = 0);
        protected CameraFileEntry GetChild(IntPtr ptr, int index);
        protected CameraFileEntry[] GetChildren(IntPtr ptr);
        protected uint GetCoordinates(ushort x, ushort y);
        protected double GetDoubleSetting(PropertyID propID, int inParam = 0);
        protected void GetHistogram(PropertyID propID, IntPtr evfImageRef, uint[] histogram);
        protected short[] GetInt16ArrSetting(PropertyID propID, int inParam = 0);
        protected short GetInt16Setting(PropertyID propID, int inParam = 0);
        protected int[] GetInt32ArrSetting(PropertyID propID, int inParam = 0);
        protected int GetInt32Setting(PropertyID propID, int inParam = 0);
        protected long GetInt64Setting(PropertyID propID, int inParam = 0);
        protected Rational[] GetRationalArrSetting(PropertyID propID, int inParam = 0);
        protected sbyte[] GetSByteArrSetting(PropertyID propID, int inParam = 0);
        protected sbyte GetSByteSetting(PropertyID propID, int inParam = 0);
        protected float GetSingleSetting(PropertyID propID, int inParam = 0);
        protected string GetStringSetting(PropertyID propID, int inParam = 0);
        protected T GetStructSetting<T>(PropertyID propID, int inParam = 0) where T : struct;
        protected ushort[] GetUInt16ArrSetting(PropertyID propID, int inParam = 0);
        protected ushort GetUInt16Setting(PropertyID propID, int inParam = 0);
        protected uint[] GetUInt32ArrSetting(PropertyID propID, int inParam = 0);
        protected uint GetUInt32Setting(PropertyID propID, int inParam = 0);
        protected ulong GetUInt64Setting(PropertyID propID, int inParam = 0);
        protected CameraValue[] GetUIntList(PropertyID propID);
        [AsyncStateMachine(typeof(<SendCommand>d__289))]
        protected Task SendCommand(CameraCommand command, uint inParam = 0);
        [AsyncStateMachine(typeof(<SendStatusCommand>d__302))]
        protected Task SendStatusCommand(CameraStatusCommand command, int inParam = 0);
        protected void SetCapacityBase(int BytesPerSector, int NumberOfFreeClusters);
        protected void SetSetting(PropertyID propID, object value, int inParam = 0);
        protected void SetSetting(PropertyID propID, string value, int inParam = 0, int MAX = 32);
        [Obsolete("Use SetSetting directly because internally it does the same")]
        protected void SetStructSetting<T>(PropertyID propID, T value, int inParam = 0) where T : struct;
        [AsyncStateMachine(typeof(<TakeBulbPhotoBase>d__311))]
        protected Task TakeBulbPhotoBase(int bulbTime);
        [AsyncStateMachine(typeof(<TakePhotoBase>d__315))]
        protected Task TakePhotoBase(bool useAf);
        [AsyncStateMachine(typeof(<TakePhotoBase>d__314))]
        protected Task TakePhotoBase();
    }
}