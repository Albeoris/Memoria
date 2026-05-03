using Memoria;
using Memoria.Prime;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = System.Object;

namespace Assets.Sources.Graphics.Movie
{
    public class MovieMaterial
    {
        private MovieMaterial()
        {
            this.advance = true;
            this.loopCount = 1;
            this.playSpeed = 1f;
            this.scanDuration = true;
            this.m_nativeContext = IntPtr.Zero;
            this.m_nativeTextureContext = IntPtr.Zero;
            this.m_ChannelTextures = new Texture2D[3];
            this.m_hasFinished = true;
            this.currentFPS = -1.0;
            this.currentDuration = -1.0;
            this.Width = 0;
            this.Height = 0;
            try
            {
                MobileMovieManager.NativeGraphicsInitialize();
                this.m_nativeContext = MovieMaterial.CreateContext();
            }
            catch (Exception arg)
            {
                global::Debug.Log("[Movie.MovieMaterial.GLPlugin] Error when creating a native context more info: " + arg);
                throw;
            }
            if (this.m_nativeContext == IntPtr.Zero)
            {
                global::Debug.Log("[Movie.MovieMaterial.GLPlugin] Unable to create Mobile Movie Texture native context");
                throw new Exception("[Movie.MovieMaterial.GLPlugin] Unable to create Mobile Movie Texture native context");
            }
        }

        private String MovieFile
        {
            get
            {
                return this.movieKey + ".bytes";
            }
        }

        public Int32 Width { get; private set; }

        public Int32 Height { get; private set; }

        public Single AspectRatio
        {
            get
            {
                if (this.m_nativeContext != IntPtr.Zero)
                {
                    return MovieMaterial.GetAspectRatio(this.m_nativeContext);
                }
                return 1f;
            }
        }

        public Double FPS
        {
            get
            {
                if (this.m_nativeContext != IntPtr.Zero)
                {
                    return MovieMaterial.GetVideoFPS(this.m_nativeContext);
                }
                return 1.0;
            }
        }

        [DllImport("theorawrapper")]
        private static extern IntPtr CreateContext();

        [DllImport("theorawrapper")]
        private static extern void DestroyContext(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Boolean OpenStream(IntPtr context, String path, Int32 offset, Int32 size, Boolean pot, Boolean scanDuration, Int32 maxSkipFrames);

        [DllImport("theorawrapper")]
        private static extern void CloseStream(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Int32 GetPicWidth(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Int32 GetPicHeight(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Int32 GetPicX(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Int32 GetPicY(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Int32 GetYStride(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Int32 GetYHeight(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Int32 GetUVStride(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Int32 GetUVHeight(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Boolean HasFinished(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Double GetDecodedFrameTime(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Double GetUploadedFrameTime(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Double GetTargetDecodeFrameTime(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern void SetTargetDisplayDecodeTime(IntPtr context, Double targetTime);

        [DllImport("theorawrapper")]
        private static extern Double GetVideoFPS(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Single GetAspectRatio(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern Double Seek(IntPtr context, Double seconds, Boolean waitKeyFrame);

        [DllImport("theorawrapper")]
        private static extern Double GetDuration(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern IntPtr GetNativeHandle(IntPtr context, Int32 planeIndex);

        [DllImport("theorawrapper")]
        private static extern IntPtr GetNativeTextureContext(IntPtr context);

        [DllImport("theorawrapper")]
        private static extern void SetPostProcessingLevel(IntPtr context, Int32 level);

        public void Update()
        {
            if (this.m_nativeContext != IntPtr.Zero && !this.m_hasFinished)
            {
                IntPtr nativeTextureContext = MovieMaterial.GetNativeTextureContext(this.m_nativeContext);
                if (nativeTextureContext != this.m_nativeTextureContext)
                {
                    this.DestroyTextures();
                    this.AllocateTexures();
                    this.m_nativeTextureContext = nativeTextureContext;
                }
                this.m_hasFinished = MovieMaterial.HasFinished(this.m_nativeContext);
                if (!this.m_hasFinished)
                {
                    if (this.advance)
                    {
                        int soundID = SoundLib.MovieAudioPlayer.GetActiveSoundID();
                        double elapsedTime = ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(soundID) / 1000.0;

                        // We allow +/- 66ms of delay
                        if (elapsedTime > 0 && Math.Abs(elapsedTime - m_elapsedTime) > 0.066)
                        {
                            // Forces sync with audio
                            this.m_elapsedTime = elapsedTime;
                        }
                        if (elapsedTime > 0 && Math.Abs(elapsedTime - m_elapsedTime) > 0.1 && (Time.realtimeSinceStartup - m_syncTime > 10f))
                        {
                            // Forces sync with audio
                            Log.Message($"[FMV] Audio sync ({Math.Round((m_elapsedTime - elapsedTime) * 1000f)}ms)");
                            this.m_elapsedTime = elapsedTime;
                            m_syncTime = Time.realtimeSinceStartup;
                        }

                        if (this.isFMV && !Configuration.Graphics.VSync)
                        {
                            if (this.preciseTimeCycleCounter == 0)
                            {
                                this.m_elapsedTime += 0.066 * (double)this.playSpeed;
                            }
                            else
                            {
                                this.m_elapsedTime += 0.067 * (double)this.playSpeed;
                            }
                            if (this.preciseTimeCycleCounter == 2)
                            {
                                this.preciseTimeCycleCounter = 0;
                            }
                            else
                            {
                                this.preciseTimeCycleCounter++;
                            }
                        }
                        else
                        {
                            this.m_elapsedTime += (double)(Mathf.Min(Time.deltaTime, 0.067f) * Mathf.Max(this.playSpeed, 0f));
                        }
                        /*if (this.shouldSync)
                        {
                            SoundLib.SeekMovieAudio(this.movieKey, this.PlayPosition);
                            this.shouldSync = false;
                        }
                        if (this.playSpeed > 1f)
                        {
                            this.syncElapsed += Time.deltaTime;
                            if (this.syncElapsed >= 4f)
                            {
                                SoundLib.SeekMovieAudio(this.movieKey, this.PlayPosition);
                                this.syncElapsed = 0f;
                            }
                        }*/
                    }
                }
                else
                {
                    if (this.loopCount - 1 <= 0 && this.loopCount != -1)
                    {
                        SoundLib.StopMovieMusic(this.movieKey, false);
                        if (this.OnFinished != null)
                        {
                            this.m_elapsedTime = MovieMaterial.GetDecodedFrameTime(this.m_nativeContext);
                            this.OnFinished();
                        }
                        return;
                    }
                    if (this.loopCount != -1)
                    {
                        this.loopCount--;
                    }
                    this.m_elapsedTime %= MovieMaterial.GetDecodedFrameTime(this.m_nativeContext);
                    MovieMaterial.Seek(this.m_nativeContext, 0.0, false);
                    this.m_hasFinished = false;
                }
                MovieMaterial.SetTargetDisplayDecodeTime(this.m_nativeContext, this.m_elapsedTime);
                if (!this.getFirstFrame)
                {
                    double uploadedFrameTime = MovieMaterial.GetUploadedFrameTime(this.m_nativeContext);
                    double num2 = 0.066666666666666666;
                    if (uploadedFrameTime > num2)
                    {
                        this.m_elapsedTime = 0.0;
                        this.preciseTimeCycleCounter = 0;
                        this.getFirstFrame = true;
                        if (this.Material != null)
                        {
                            this.Material.SetColor("_TintColor", this.tintColor);
                        }
                        if (this.advance)
                        {
                            SoundLib.PlayMovieMusic(this.movieKey, 0);
                        }
                    }
                }
            }
        }

        private void Open()
        {
            RuntimePlatform platform = Application.platform;
            String moviePath = "ma/" + this.MovieFile;
            String fullPath = "";
            Int64 fileOffset = 0L;
            Int64 fileLength = 0L;
            switch (platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.WindowsPlayer:
                    foreach (AssetManager.AssetFolder folder in AssetManager.FolderHighToLow)
                        if (folder.TryFindAssetInModOnDisc(moviePath, out fullPath, AssetManagerUtil.GetStreamingAssetsPath() + "/"))
                            break;
                    break;
                case RuntimePlatform.Android:
                    fullPath = Application.dataPath;
                    if (!AssetStream.GetZipFileOffsetLength(Application.dataPath, moviePath, out fileOffset, out fileLength))
                        throw new Exception("[Movie.MovieMaterial.GLPlugin] Error opening movie via AssetStream");
                    break;
                default:
                    throw new Exception("[Movie.MovieMaterial.GLPlugin]  Platform: " + Application.platform + " Not supported.");
            }
            this.isFMV = this.MovieFile.StartsWith("FMV");
            if (this.m_nativeContext != IntPtr.Zero && MovieMaterial.OpenStream(this.m_nativeContext, fullPath, (Int32)fileOffset, (Int32)fileLength, false, this.scanDuration, 16))
            {
                this.Width = MovieMaterial.GetPicWidth(this.m_nativeContext);
                this.Height = MovieMaterial.GetPicHeight(this.m_nativeContext);
                this.m_picX = MovieMaterial.GetPicX(this.m_nativeContext);
                this.m_picY = MovieMaterial.GetPicY(this.m_nativeContext);
                this.m_yStride = MovieMaterial.GetYStride(this.m_nativeContext);
                this.m_yHeight = MovieMaterial.GetYHeight(this.m_nativeContext);
                this.m_uvStride = MovieMaterial.GetUVStride(this.m_nativeContext);
                this.m_uvHeight = MovieMaterial.GetUVHeight(this.m_nativeContext);
                this.currentFPS = this.FPS;
                this.currentDuration = MovieMaterial.GetDuration(this.m_nativeContext);
                this.playSpeed = (Single)(15.0 / currentFPS);
                this.CalculateUVScaleOffset();
                return;
            }
            throw new FileLoadException("Error opening movie during stream opening at path: " + fullPath);
        }

        private void AllocateTexures()
        {
            this.m_ChannelTextures[0] = Texture2D.CreateExternalTexture(this.m_yStride, this.m_yHeight, TextureFormat.Alpha8, false, false, MovieMaterial.GetNativeHandle(this.m_nativeContext, 0));
            this.m_ChannelTextures[1] = Texture2D.CreateExternalTexture(this.m_uvStride, this.m_uvHeight, TextureFormat.Alpha8, false, false, MovieMaterial.GetNativeHandle(this.m_nativeContext, 1));
            this.m_ChannelTextures[2] = Texture2D.CreateExternalTexture(this.m_uvStride, this.m_uvHeight, TextureFormat.Alpha8, false, false, MovieMaterial.GetNativeHandle(this.m_nativeContext, 2));
            this.m_ChannelTextures[0].wrapMode = TextureWrapMode.Clamp;
            this.m_ChannelTextures[1].wrapMode = TextureWrapMode.Clamp;
            this.m_ChannelTextures[2].wrapMode = TextureWrapMode.Clamp;
            if (this.Material != (UnityEngine.Object)null)
            {
                this.SetTextures(this.Material);
            }
        }

        private void SetTextures(Material material)
        {
            material.SetTexture("_YTex", this.m_ChannelTextures[0]);
            material.SetTexture("_CbTex", this.m_ChannelTextures[1]);
            material.SetTexture("_CrTex", this.m_ChannelTextures[2]);
            material.SetTextureScale("_YTex", this.m_uvYScale);
            material.SetTextureOffset("_YTex", this.m_uvYOffset);
            material.SetTextureScale("_CbTex", this.m_uvCrCbScale);
            material.SetTextureOffset("_CbTex", this.m_uvCrCbOffset);
        }

        private void RemoveTextures(Material material)
        {
            material.SetTexture("_YTex", (Texture)null);
            material.SetTexture("_CbTex", (Texture)null);
            material.SetTexture("_CrTex", (Texture)null);
        }

        private void CalculateUVScaleOffset()
        {
            Single w = (Single)this.Width;
            Single h = (Single)this.Height;
            Single x = (Single)this.m_picX;
            Single y = (Single)this.m_picY;
            Single yStride = (Single)this.m_yStride;
            Single yHeight = (Single)this.m_yHeight;
            Single uvStride = (Single)this.m_uvStride;
            Single uvHeight = (Single)this.m_uvHeight;
            this.m_uvYScale = new Vector2(w / yStride, -(h / yHeight));
            this.m_uvYOffset = new Vector2(x / yStride, (h + y) / yHeight);
            this.m_uvCrCbScale = default(Vector2);
            this.m_uvCrCbOffset = default(Vector2);
            if (this.m_uvStride == this.m_yStride)
            {
                this.m_uvCrCbScale.x = this.m_uvYScale.x;
            }
            else
            {
                this.m_uvCrCbScale.x = w / 2f / uvStride;
            }
            if (this.m_uvHeight == this.m_yHeight)
            {
                this.m_uvCrCbScale.y = this.m_uvYScale.y;
                this.m_uvCrCbOffset = this.m_uvYOffset;
            }
            else
            {
                this.m_uvCrCbScale.y = -(h / 2f / uvHeight);
                this.m_uvCrCbOffset = new Vector2(x / 2f / uvStride, (h + y) / 2f / uvHeight);
            }
        }

        private void DestroyTextures()
        {
            if (this.Material != (UnityEngine.Object)null)
            {
                this.RemoveTextures(this.Material);
            }
            for (Int32 i = 0; i < 3; i++)
            {
                if (this.m_ChannelTextures[i] != (UnityEngine.Object)null)
                {
                    UnityEngine.Object.Destroy(this.m_ChannelTextures[i]);
                    this.m_ChannelTextures[i] = (Texture2D)null;
                }
            }
        }

        public Material Material { get; private set; }

        public Single Duration
        {
            get
            {
                return (Single)this.currentDuration;
            }
        }

        public Single PlayPosition
        {
            get
            {
                return (Single)this.m_elapsedTime;
            }
            private set
            {
                if (this.m_nativeContext != IntPtr.Zero)
                {
                    this.m_elapsedTime = MovieMaterial.Seek(this.m_nativeContext, (Double)value, this.seekKeyFrame);
                }
            }
        }

        public MovieMaterial.FastForwardMode FastForward
        {
            get
            {
                return this.playMode;
            }
            set
            {
                // It's realy bad idea to speed up movies!
                // this.playMode = value;
                // this.shouldSync = true;
                // this.playSpeed = 1f; //((this.playMode != MovieMaterial.FastForwardMode.Normal) ? ((Single)FF9StateSystem.Settings.FastForwardFactor) : 1f);
            }
        }

        public Boolean Loop
        {
            get
            {
                return this.loopCount == -1;
            }
            set
            {
                this.loopCount = (Int32)((!value) ? this.loopCount : -1);
            }
        }

        public Int32 Frame
        {
            get
            {
                if (this.isFMV)
                {
                    return Mathf.RoundToInt((float)(this.currentFPS * this.m_elapsedTime));
                }
                return Mathf.FloorToInt((Single)(this.currentFPS * this.m_elapsedTime));
            }
        }

        public Int32 TotalFrame
        {
            get
            {
                return (Int32)(this.currentFPS * (Double)this.Duration);
            }
        }

        public Single Transparency
        {
            get
            {
                return this.transparency;
            }
            set
            {
                this.transparency = value;
                if (this.transparency >= 1f)
                {
                    this.transparency = 1f;
                }
                if (this.transparency <= 0f)
                {
                    this.transparency = 0f;
                }
                this.tintColor.a = this.transparency;
                if (this.getFirstFrame && this.Material != (UnityEngine.Object)null)
                {
                    this.Material.SetColor("_TintColor", this.tintColor);
                }
            }
        }

        public Boolean GetFirstFrame
        {
            get
            {
                return this.getFirstFrame;
            }
        }

        public static MovieMaterial New(MovieMaterialProcessor processor)
        {
            if (processor == null)
                throw new NullReferenceException("MovieMaterialProcessor cannot be null");
            Material material = AssetManager.Load<Material>("EmbeddedAsset/Shaders/Movie/Movie", false);
            if (material == null)
                throw new ArgumentException("Failed to load material Movie/Movie");

            material.SetColor("_TintColor", Color.clear);
            MovieMaterial movieMaterial = new MovieMaterial
            {
                Material = material
            };
            processor.MovieMaterial = movieMaterial;
            return movieMaterial;
        }

        public void Load(String movieKey)
        {
            if (this.hasStream)
            {
                this.Stop();
            }
            this.getFirstFrame = false;
            this.preciseTimeCycleCounter = 0;
            this.movieKey = movieKey;
            try
            {
                this.m_elapsedTime = 0.0;
                this.Open();
                this.advance = false;
                this.hasStream = true;
                this.m_hasFinished = false;
            }
            catch (Exception ex)
            {
                global::Debug.Log(String.Concat(new Object[]
                {
                    "[Movie.MovieMaterial.GLPlugin] Error when loading: ",
                    this.MovieFile,
                    " more info: ",
                    ex
                }));
                throw;
            }
        }

        public void Play()
        {
            if (!this.hasStream)
            {
                throw new InvalidOperationException("[Movie.MovieMaterial.GLPlugin] Stream is not available");
            }
            this.Transparency = 1f;
            this.advance = true;
        }

        public void Pause()
        {
            if (!this.hasStream)
            {
                throw new InvalidOperationException("[Movie.MovieMaterial.GLPlugin] Stream is not available");
            }
            this.advance = false;
            SoundLib.PauseMovieMusic(this.movieKey);
        }

        public void Resume()
        {
            this.Play();
            if (this.getFirstFrame)
            {
                SoundLib.PlayMovieMusic(this.movieKey, 0);
            }
        }

        public void Stop()
        {
            if (!this.hasStream)
            {
                return;
            }
            this.Transparency = 0f;
            MovieMaterial.CloseStream(this.m_nativeContext);
            this.m_hasFinished = true;
            this.hasStream = false;
            SoundLib.StopMovieMusic(this.movieKey, false);
        }

        public void Destroy()
        {
            if (this.hasStream)
            {
                this.Stop();
            }
            this.DestroyTextures();
            MovieMaterial.DestroyContext(this.m_nativeContext);
        }

        private const Single SyncRate = 4f;

        private const Int32 CHANNELS = 3;

        private const String PLATFORM_DLL = "theorawrapper";

        private const String LogTag = "[Movie.MovieMaterial.GLPlugin] ";

        public Action OnFinished;

        public String movieKey;

        private Boolean advance;

        private Int32 loopCount;

        private Single playSpeed;

        private Boolean scanDuration;

        private Boolean seekKeyFrame;

        private Boolean hasStream;

        private Single syncElapsed;

        private Boolean shouldSync;

        private IntPtr m_nativeContext;

        private IntPtr m_nativeTextureContext;

        private Int32 m_picX;

        private Int32 m_picY;

        private Int32 m_yStride;

        private Int32 m_yHeight;

        private Int32 m_uvStride;

        private Int32 m_uvHeight;

        private Vector2 m_uvYScale;

        private Vector2 m_uvYOffset;

        private Vector2 m_uvCrCbScale;

        private Vector2 m_uvCrCbOffset;

        private Texture2D[] m_ChannelTextures;

        private Single m_syncTime = 0f;

        private Double m_elapsedTime;

        private Boolean m_hasFinished;

        private Double currentFPS;

        private Double currentDuration;

        private int preciseTimeCycleCounter;

        private bool isFMV;

        public static readonly Vector3 ScaleVector = Vector3.one;

        private Color tintColor = new Color(1f, 1f, 1f, 1f);

        private Single transparency;

        private Boolean needUpdateTintColor;

        private Boolean getFirstFrame;

        private MovieMaterial.FastForwardMode playMode;

        public enum FastForwardMode
        {
            Normal,
            HighSpeed
        }
    }
}
