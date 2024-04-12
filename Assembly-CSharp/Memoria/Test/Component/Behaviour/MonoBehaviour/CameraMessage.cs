using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace Memoria.Test
{
	public class CameraMessage : BehaviourMessage
	{
		public CameraMessage()
		{
		}

		public CameraMessage(Camera camera)
			: base(camera)
		{
			FieldOfView = camera.fieldOfView;
			NearClipPlane = camera.nearClipPlane;
			FarClipPlane = camera.farClipPlane;
			RenderingPath = camera.renderingPath;
			ActualRenderingPath = camera.actualRenderingPath;
			Hdr = camera.hdr;
			OrthographicSize = camera.orthographicSize;
			Orthographic = camera.orthographic;
			OpaqueSortMode = camera.opaqueSortMode;
			TransparencySortMode = camera.transparencySortMode;
			Depth = camera.depth;
			Aspect = camera.aspect;
			CullingMask = camera.cullingMask;
			EventMask = camera.eventMask;
			BackgroundColor = camera.backgroundColor;
			Rect = camera.rect;
			PixelRect = camera.pixelRect;
			PixelWidth = camera.pixelWidth;
			PixelHeight = camera.pixelHeight;
			CameraToWorldMatrix = camera.cameraToWorldMatrix;
			WorldToCameraMatrix = camera.worldToCameraMatrix;
			ProjectionMatrix = camera.projectionMatrix;
			ClearFlags = camera.clearFlags;
			StereoEnabled = camera.stereoEnabled;
			StereoSeparation = camera.stereoSeparation;
			StereoConvergence = camera.stereoConvergence;
			CameraType = camera.cameraType;
			StereoMirrorMode = camera.stereoMirrorMode;
			TargetDisplay = camera.targetDisplay;
			UseOcclusionCulling = camera.useOcclusionCulling;
			//LayerCullDistances = camera.layerCullDistances;
			LayerCullSpherical = camera.layerCullSpherical;
			DepthTextureMode = camera.depthTextureMode;
			ClearStencilAfterLightingPass = camera.clearStencilAfterLightingPass;
			CommandBufferCount = camera.commandBufferCount;
		}

		public override void Serialize(BinaryWriter bw)
		{
			base.Serialize(bw);

			bw.Write(FieldOfView);
			bw.Write(NearClipPlane);
			bw.Write(FarClipPlane);
			bw.Write((Byte)RenderingPath);
			bw.Write((Byte)ActualRenderingPath);
			bw.Write(Hdr);
			bw.Write(OrthographicSize);
			bw.Write(Orthographic);
			bw.Write((Byte)OpaqueSortMode);
			bw.Write((Byte)TransparencySortMode);
			bw.Write(Depth);
			bw.Write(Aspect);
			bw.Write(CullingMask);
			bw.Write(EventMask);
			bw.Write(BackgroundColor);
			bw.Write(Rect);
			bw.Write(PixelRect);
			bw.Write(PixelWidth);
			bw.Write(PixelHeight);
			bw.Write(CameraToWorldMatrix);
			bw.Write(WorldToCameraMatrix);
			bw.Write(ProjectionMatrix);
			bw.Write((Byte)ClearFlags);
			bw.Write(StereoEnabled);
			bw.Write(StereoSeparation);
			bw.Write(StereoConvergence);
			bw.Write((Byte)CameraType);
			bw.Write(StereoMirrorMode);
			bw.Write(TargetDisplay);
			bw.Write(UseOcclusionCulling);
			//bw.Write(layerCullDistances);
			bw.Write(LayerCullSpherical);
			bw.Write((Byte)DepthTextureMode);
			bw.Write(ClearStencilAfterLightingPass);
			bw.Write(CommandBufferCount);
		}

		public override void Deserialize(BinaryReader br)
		{
			base.Deserialize(br);

			FieldOfView = br.ReadSingle();
			NearClipPlane = br.ReadSingle();
			FarClipPlane = br.ReadSingle();
			RenderingPath = (RenderingPath)br.ReadByte();
			ActualRenderingPath = (RenderingPath)br.ReadByte();
			Hdr = br.ReadBoolean();
			OrthographicSize = br.ReadSingle();
			Orthographic = br.ReadBoolean();
			OpaqueSortMode = (OpaqueSortMode)br.ReadByte();
			TransparencySortMode = (TransparencySortMode)br.ReadByte();
			Depth = br.ReadSingle();
			Aspect = br.ReadSingle();
			CullingMask = br.ReadInt32();
			EventMask = br.ReadInt32();
			BackgroundColor = br.ReadVector4();
			Rect = br.ReadRect();
			PixelRect = br.ReadRect();
			PixelWidth = br.ReadInt32();
			PixelHeight = br.ReadInt32();
			CameraToWorldMatrix = br.ReadMatrix4x4();
			WorldToCameraMatrix = br.ReadMatrix4x4();
			ProjectionMatrix = br.ReadMatrix4x4();
			ClearFlags = (CameraClearFlags)br.ReadByte();
			StereoEnabled = br.ReadBoolean();
			StereoSeparation = br.ReadSingle();
			StereoConvergence = br.ReadSingle();
			CameraType = (CameraType)br.ReadByte();
			StereoMirrorMode = br.ReadBoolean();
			TargetDisplay = br.ReadInt32();
			UseOcclusionCulling = br.ReadBoolean();
			//layerCullDistances = br.Read1();
			LayerCullSpherical = br.ReadBoolean();
			DepthTextureMode = (DepthTextureMode)br.ReadByte();
			ClearStencilAfterLightingPass = br.ReadBoolean();
			CommandBufferCount = br.ReadInt32();
		}

		/// <summary>
		///   <para>The field of view of the camera in degrees.</para>
		/// </summary>
		public Single FieldOfView;

		/// <summary>
		///   <para>The near clipping plane distance.</para>
		/// </summary>
		public Single NearClipPlane;

		/// <summary>
		///   <para>The far clipping plane distance.</para>
		/// </summary>
		public Single FarClipPlane;

		/// <summary>
		///   <para>Rendering path.</para>
		/// </summary>
		public RenderingPath RenderingPath;

		/// <summary>
		///   <para>Actually used rendering path (Read Only).</para>
		/// </summary>
		public RenderingPath ActualRenderingPath;

		/// <summary>
		///   <para>High dynamic range rendering.</para>
		/// </summary>
		public Boolean Hdr;

		/// <summary>
		///   <para>Camera's half-size when in orthographic mode.</para>
		/// </summary>
		public Single OrthographicSize;

		/// <summary>
		///   <para>Is the camera orthographic (true) or perspective (false)?</para>
		/// </summary>
		public Boolean Orthographic;

		/// <summary>
		///   <para>Opaque object sorting mode.</para>
		/// </summary>
		public OpaqueSortMode OpaqueSortMode;

		/// <summary>
		///   <para>Transparent object sorting mode.</para>
		/// </summary>
		public TransparencySortMode TransparencySortMode;

		/// <summary>
		///   <para>Camera's depth in the camera rendering order.</para>
		/// </summary>
		public Single Depth;

		/// <summary>
		///   <para>The aspect ratio (width divided by height).</para>
		/// </summary>
		public Single Aspect;

		/// <summary>
		///   <para>This is used to render parts of the scene selectively.</para>
		/// </summary>
		public Int32 CullingMask;

		/// <summary>
		///   <para>Mask to select which layers can trigger events on the camera.</para>
		/// </summary>
		public Int32 EventMask;

		/// <summary>
		///   <para>The color with which the screen will be cleared.</para>
		/// </summary>
		public Color BackgroundColor;

		/// <summary>
		///   <para>Where on the screen is the camera rendered in normalized coordinates.</para>
		/// </summary>
		public Rect Rect;

		/// <summary>
		///   <para>Where on the screen is the camera rendered in pixel coordinates.</para>
		/// </summary>
		public Rect PixelRect;

		/// <summary>
		///   <para>How wide is the camera in pixels (Read Only).</para>
		/// </summary>
		public Int32 PixelWidth;

		/// <summary>
		///   <para>How tall is the camera in pixels (Read Only).</para>
		/// </summary>
		public Int32 PixelHeight;

		/// <summary>
		///   <para>Matrix that transforms from camera space to world space (Read Only).</para>
		/// </summary>
		public Matrix4x4 CameraToWorldMatrix;

		/// <summary>
		///   <para>Matrix that transforms from world to camera space.</para>
		/// </summary>
		public Matrix4x4 WorldToCameraMatrix;

		/// <summary>
		///   <para>Set a custom projection matrix.</para>
		/// </summary>
		public Matrix4x4 ProjectionMatrix;

		/// <summary>
		///   <para>How the camera clears the background.</para>
		/// </summary>
		public CameraClearFlags ClearFlags;

		/// <summary>
		///   <para>Stereoscopic rendering.</para>
		/// </summary>
		public Boolean StereoEnabled;

		/// <summary>
		///   <para>Distance between the virtual eyes.</para>
		/// </summary>
		public Single StereoSeparation;

		/// <summary>
		///   <para>Distance to a point where virtual eyes converge.</para>
		/// </summary>
		public Single StereoConvergence;

		/// <summary>
		///   <para>Identifies what kind of camera this is.</para>
		/// </summary>
		public CameraType CameraType;

		/// <summary>
		///   <para>Render only once and use resulting image for both eyes.</para>
		/// </summary>
		public Boolean StereoMirrorMode;

		/// <summary>
		///   <para>Set the target display for this Camera.</para>
		/// </summary>
		public Int32 TargetDisplay;

		/// <summary>
		///   <para>Whether or not the Camera will use occlusion culling during rendering.</para>
		/// </summary>
		public Boolean UseOcclusionCulling;

		/// <summary>
		///   <para>Per-layer culling distances.</para>
		/// </summary>
		//public float[] layerCullDistances {  get;  set; }

		/// <summary>
		///   <para>How to perform per-layer culling for a Camera.</para>
		/// </summary>
		public Boolean LayerCullSpherical;

		/// <summary>
		///   <para>How and if camera generates a depth texture.</para>
		/// </summary>
		public DepthTextureMode DepthTextureMode;

		/// <summary>
		///   <para>Should the camera clear the stencil buffer after the deferred light pass?</para>
		/// </summary>
		public Boolean ClearStencilAfterLightingPass;

		/// <summary>
		///   <para>Number of command buffers set up on this camera (Read Only).</para>
		/// </summary>
		public Int32 CommandBufferCount;
	}
}
