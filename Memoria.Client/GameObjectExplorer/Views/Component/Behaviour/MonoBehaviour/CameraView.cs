using Memoria.Client.GameObjectExplorer.Views.TypeEditors;
using Memoria.Test;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

namespace Memoria.Client
{
    public class CameraView<T> : ComponentView<T> where T : CameraMessage
    {
        public CameraView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        /// <summary>
        ///   <para>The field of view of the camera in degrees.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("FieldOfView")]
        [Description("The field of view of the camera in degrees.")]
        public Single FieldOfView
        {
            get { return Native.FieldOfView; }
            set
            {
                if (Native.FieldOfView != value)
                {
                    Native.FieldOfView = value;
                    IValueMessage valueMessage = new SingleMessage(value);
                    SendPropertyChanged(nameof(Camera.fieldOfView), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>The near clipping plane distance.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("NearClipPlane")]
        [Description("The near clipping plane distance.")]
        public Single NearClipPlane
        {
            get { return Native.NearClipPlane; }
            set
            {
                if (Native.NearClipPlane != value)
                {
                    Native.NearClipPlane = value;
                    IValueMessage valueMessage = new SingleMessage(value);
                    SendPropertyChanged(nameof(Camera.nearClipPlane), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>The far clipping plane distance.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("FarClipPlane")]
        [Description("The far clipping plane distance.")]
        public Single FarClipPlane
        {
            get { return Native.FarClipPlane; }
            set
            {
                if (Native.FarClipPlane != value)
                {
                    Native.FarClipPlane = value;
                    IValueMessage valueMessage = new SingleMessage(value);
                    SendPropertyChanged(nameof(Camera.farClipPlane), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Rendering path.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("RenderingPath")]
        [Description(">Rendering path.")]
        public RenderingPath RenderingPath
        {
            get { return Native.RenderingPath; }
            set
            {
                if (Native.RenderingPath != value)
                {
                    Native.RenderingPath = value;
                    IValueMessage valueMessage = new UInt8Message((Byte)value);
                    SendPropertyChanged(nameof(Camera.renderingPath), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Actually used rendering path (Read Only).</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("ActualRenderingPath")]
        [Description("Actually used rendering path (Read Only).")]
        public RenderingPath ActualRenderingPath
        {
            get { return Native.ActualRenderingPath; }
        }

        /// <summary>
        ///   <para>High dynamic range rendering.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("Hdr")]
        [Description(">High dynamic range rendering.")]
        public Boolean Hdr
        {
            get { return Native.Hdr; }
            set
            {
                if (Native.Hdr != value)
                {
                    Native.Hdr = value;
                    IValueMessage valueMessage = new BooleanMessage(value);
                    SendPropertyChanged(nameof(Camera.hdr), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Camera's half-size when in orthographic mode.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("OrthographicSize")]
        [Description("Camera's half-size when in orthographic mode.")]
        public Single OrthographicSize
        {
            get { return Native.OrthographicSize; }
            set
            {
                if (Native.OrthographicSize != value)
                {
                    Native.OrthographicSize = value;
                    IValueMessage valueMessage = new SingleMessage(value);
                    SendPropertyChanged(nameof(Camera.orthographicSize), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Is the camera orthographic (true) or perspective (false)?</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("Orthographic")]
        [Description("Is the camera orthographic (true) or perspective (false)?")]
        public Boolean Orthographic
        {
            get { return Native.Orthographic; }
            set
            {
                if (Native.Orthographic != value)
                {
                    Native.Orthographic = value;
                    IValueMessage valueMessage = new BooleanMessage(value);
                    SendPropertyChanged(nameof(Camera.orthographic), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Opaque object sorting mode.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("OpaqueSortMode")]
        [Description("Opaque object sorting mode.")]
        public OpaqueSortMode OpaqueSortMode
        {
            get { return Native.OpaqueSortMode; }
            set
            {
                if (Native.OpaqueSortMode != value)
                {
                    Native.OpaqueSortMode = value;
                    IValueMessage valueMessage = new UInt8Message((Byte)value);
                    SendPropertyChanged(nameof(Camera.opaqueSortMode), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Transparent object sorting mode.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("TransparencySortMode")]
        [Description("Transparent object sorting mode.")]
        public TransparencySortMode TransparencySortMode
        {
            get { return Native.TransparencySortMode; }
            set
            {
                if (Native.TransparencySortMode != value)
                {
                    Native.TransparencySortMode = value;
                    IValueMessage valueMessage = new UInt8Message((Byte)value);
                    SendPropertyChanged(nameof(Camera.transparencySortMode), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Camera's depth in the camera rendering order.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("Depth")]
        [Description("Camera's depth in the camera rendering order.")]
        public Single Depth
        {
            get { return Native.Depth; }
            set
            {
                if (Native.Depth != value)
                {
                    Native.Depth = value;
                    IValueMessage valueMessage = new SingleMessage(value);
                    SendPropertyChanged(nameof(Camera.depth), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>The aspect ratio (width divided by height).</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("Aspect")]
        [Description("The aspect ratio (width divided by height).")]
        public Single Aspect
        {
            get { return Native.Aspect; }
            set
            {
                if (Native.Aspect != value)
                {
                    Native.Aspect = value;
                    IValueMessage valueMessage = new SingleMessage(value);
                    SendPropertyChanged(nameof(Camera.aspect), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>This is used to render parts of the scene selectively.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("CullingMask")]
        [Description("This is used to render parts of the scene selectively.")]
        public Int32 CullingMask
        {
            get { return Native.CullingMask; }
            set
            {
                if (Native.CullingMask != value)
                {
                    Native.CullingMask = value;
                    IValueMessage valueMessage = new Int32Message(value);
                    SendPropertyChanged(nameof(Camera.cullingMask), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Mask to select which layers can trigger events on the camera.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("EventMask")]
        [Description("Mask to select which layers can trigger events on the camera.")]
        public Int32 EventMask
        {
            get { return Native.EventMask; }
            set
            {
                if (Native.EventMask != value)
                {
                    Native.EventMask = value;
                    IValueMessage valueMessage = new Int32Message(value);
                    SendPropertyChanged(nameof(Camera.eventMask), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>The color with which the screen will be cleared.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("BackgroundColor")]
        [Description("The color with which the screen will be cleared.")]
        public Color BackgroundColor
        {
            get { return Native.BackgroundColor; }
            set
            {
                if (Native.BackgroundColor != value)
                {
                    Native.BackgroundColor = value;
                    IValueMessage valueMessage = new Vector4Message(value);
                    SendPropertyChanged(nameof(Camera.backgroundColor), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Where on the screen is the camera rendered in normalized coordinates.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("Rect")]
        [Description("Where on the screen is the camera rendered in normalized coordinates.")]
        public Rect Rect
        {
            get { return Native.Rect; }
            set
            {
                if (Native.Rect != value)
                {
                    Native.Rect = value;
                    IValueMessage valueMessage = new RectMessage(value);
                    SendPropertyChanged(nameof(Camera.rect), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Where on the screen is the camera rendered in pixel coordinates.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("PixelRect")]
        [Description("Where on the screen is the camera rendered in pixel coordinates.")]
        [Editor(typeof(RectEditor), typeof(RectEditor))]
        public Rect PixelRect
        {
            get { return Native.PixelRect; }
            set
            {
                if (Native.PixelRect != value)
                {
                    Native.PixelRect = value;
                    IValueMessage valueMessage = new RectMessage(value);
                    SendPropertyChanged(nameof(Camera.pixelRect), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>How wide is the camera in pixels (Read Only).</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("PixelWidth")]
        [Description("How wide is the camera in pixels (Read Only).")]
        public Int32 PixelWidth
        {
            get { return Native.PixelWidth; }
        }

        /// <summary>
        ///   <para>How tall is the camera in pixels (Read Only).</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("PixelHeight")]
        [Description("How tall is the camera in pixels (Read Only).")]
        public Int32 PixelHeight
        {
            get { return Native.PixelHeight; }
        }

        /// <summary>
        ///   <para>Matrix that transforms from camera space to world space (Read Only).</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("CameraToWorldMatrix")]
        [Description("Matrix that transforms from camera space to world space (Read Only).")]
        public Matrix4x4 CameraToWorldMatrix
        {
            get { return Native.CameraToWorldMatrix; }
        }

        /// <summary>
        ///   <para>Matrix that transforms from world to camera space.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("WorldToCameraMatrix")]
        [Description("Matrix that transforms from world to camera space.")]
        public Matrix4x4 WorldToCameraMatrix
        {
            get { return Native.WorldToCameraMatrix; }
            set
            {
                if (Native.WorldToCameraMatrix != value)
                {
                    Native.WorldToCameraMatrix = value;
                    IValueMessage valueMessage = new Matrix4x4Message(value);
                    SendPropertyChanged(nameof(Camera.worldToCameraMatrix), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Set a custom projection matrix.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("ProjectionMatrix")]
        [Description("Set a custom projection matrix.")]
        public Matrix4x4 ProjectionMatrix
        {
            get { return Native.ProjectionMatrix; }
            set
            {
                if (Native.ProjectionMatrix != value)
                {
                    Native.ProjectionMatrix = value;
                    IValueMessage valueMessage = new Matrix4x4Message(value);
                    SendPropertyChanged(nameof(Camera.projectionMatrix), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>How the camera clears the background.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("ClearFlags")]
        [Description("How the camera clears the background.")]
        public CameraClearFlags ClearFlags
        {
            get { return Native.ClearFlags; }
            set
            {
                if (Native.ClearFlags != value)
                {
                    Native.ClearFlags = value;
                    IValueMessage valueMessage = new UInt8Message((Byte)value);
                    SendPropertyChanged(nameof(Camera.clearFlags), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Stereoscopic rendering.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("StereoEnabled")]
        [Description("Stereoscopic rendering.")]
        public Boolean StereoEnabled
        {
            get { return Native.StereoEnabled; }
        }

        /// <summary>
        ///   <para>Distance between the virtual eyes.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("StereoSeparation")]
        [Description("Distance between the virtual eyes.")]
        public Single StereoSeparation
        {
            get { return Native.StereoSeparation; }
            set
            {
                if (Native.StereoSeparation != value)
                {
                    Native.StereoSeparation = value;
                    IValueMessage valueMessage = new SingleMessage(value);
                    SendPropertyChanged(nameof(Camera.stereoSeparation), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Distance to a point where virtual eyes converge.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("StereoConvergence")]
        [Description("Distance to a point where virtual eyes converge.")]
        public Single StereoConvergence
        {
            get { return Native.StereoConvergence; }
            set
            {
                if (Native.StereoConvergence != value)
                {
                    Native.StereoConvergence = value;
                    IValueMessage valueMessage = new SingleMessage(value);
                    SendPropertyChanged(nameof(Camera.stereoConvergence), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Identifies what kind of camera this is.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("CameraType")]
        [Description("Identifies what kind of camera this is.")]
        public CameraType CameraType
        {
            get { return Native.CameraType; }
            set
            {
                if (Native.CameraType != value)
                {
                    Native.CameraType = value;
                    IValueMessage valueMessage = new UInt8Message((Byte)value);
                    SendPropertyChanged(nameof(Camera.cameraType), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Render only once and use resulting image for both eyes.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("StereoMirrorMode")]
        [Description("Render only once and use resulting image for both eyes.")]
        public Boolean StereoMirrorMode
        {
            get { return Native.StereoMirrorMode; }
            set
            {
                if (Native.StereoMirrorMode != value)
                {
                    Native.StereoMirrorMode = value;
                    IValueMessage valueMessage = new BooleanMessage(value);
                    SendPropertyChanged(nameof(Camera.stereoMirrorMode), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Set the target display for this Camera.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("TargetDisplay")]
        [Description("Set the target display for this Camera.")]
        public Int32 TargetDisplay
        {
            get { return Native.TargetDisplay; }
            set
            {
                if (Native.TargetDisplay != value)
                {
                    Native.TargetDisplay = value;
                    IValueMessage valueMessage = new Int32Message(value);
                    SendPropertyChanged(nameof(Camera.targetDisplay), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Whether or not the Camera will use occlusion culling during rendering.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("UseOcclusionCulling")]
        [Description("Whether or not the Camera will use occlusion culling during rendering.")]
        public Boolean UseOcclusionCulling
        {
            get { return Native.UseOcclusionCulling; }
            set
            {
                if (Native.UseOcclusionCulling != value)
                {
                    Native.UseOcclusionCulling = value;
                    IValueMessage valueMessage = new BooleanMessage(value);
                    SendPropertyChanged(nameof(Camera.useOcclusionCulling), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Per-layer culling distances.</para>
        /// </summary>
        //[Category("Camera")]
        //[DisplayName("LayerCullDistances")]
        //[Description("Per-layer culling distances.")]
        //public float[] LayerCullDistances {  get;  set; }

        /// <summary>
        ///   <para>How to perform per-layer culling for a Camera.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("LayerCullSpherical")]
        [Description("How to perform per-layer culling for a Camera.")]
        public Boolean LayerCullSpherical
        {
            get { return Native.LayerCullSpherical; }
            set
            {
                if (Native.LayerCullSpherical != value)
                {
                    Native.LayerCullSpherical = value;
                    IValueMessage valueMessage = new BooleanMessage(value);
                    SendPropertyChanged(nameof(Camera.layerCullSpherical), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>How and if camera generates a depth texture.</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("DepthTextureMode")]
        [Description("How and if camera generates a depth texture.")]
        public DepthTextureMode DepthTextureMode
        {
            get { return Native.DepthTextureMode; }
            set
            {
                if (Native.DepthTextureMode != value)
                {
                    Native.DepthTextureMode = value;
                    IValueMessage valueMessage = new UInt8Message((Byte)value);
                    SendPropertyChanged(nameof(Camera.depthTextureMode), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Should the camera clear the stencil buffer after the deferred light pass?</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("ClearStencilAfterLightingPass")]
        [Description("Should the camera clear the stencil buffer after the deferred light pass?")]
        public Boolean ClearStencilAfterLightingPass
        {
            get { return Native.ClearStencilAfterLightingPass; }
            set
            {
                if (Native.ClearStencilAfterLightingPass != value)
                {
                    Native.ClearStencilAfterLightingPass = value;
                    IValueMessage valueMessage = new BooleanMessage(value);
                    SendPropertyChanged(nameof(Camera.clearStencilAfterLightingPass), valueMessage);
                }
            }
        }

        /// <summary>
        ///   <para>Number of command buffers set up on this camera (Read Only).</para>
        /// </summary>
        [Category("Camera")]
        [DisplayName("CommandBufferCount")]
        [Description("Number of command buffers set up on this camera (Read Only).")]
        public Int32 CommandBufferCount
        {
            get { return Native.CommandBufferCount; }
            set
            {
                if (Native.CommandBufferCount != value)
                {
                    Native.CommandBufferCount = value;
                    IValueMessage valueMessage = new Int32Message(value);
                    SendPropertyChanged(nameof(Camera.commandBufferCount), valueMessage);
                }
            }
        }
    }
}
