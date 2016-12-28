﻿//----------------------------------------------------------------------------
//  Copyright (C) 2004-2016 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------
#if ! (NETFX_CORE || __ANDROID__ || __IOS__ || UNITY_IOS || UNITY_ANDROID )

using System;
using System.Runtime.InteropServices;
using System.Drawing;
using Emgu.Util;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Emgu.CV
{
   /// <summary>
   /// Represents a 3D visualizer window. 
   /// </summary>
   public partial class Viz3d : UnmanagedObject
   {
      /// <summary>
      /// Create a new 3D visualizer windows
      /// </summary>
      /// <param name="windowName">The name of the windows</param>
      public Viz3d(String windowName)
      {
         using (CvString cvs = new CvString(windowName))
         {
            _ptr = CvInvoke.cveViz3dCreate(cvs);
         }
      }

      /// <summary>
      /// Show a widget in the window
      /// </summary>
      /// <param name="id">A unique id for the widget.</param>
      /// <param name="widget">The widget to be displayed in the window.</param>
      /// <param name="pose">Pose of the widget.</param>
      public void ShowWidget(String id, IWidget widget, Affine3d pose = null)
      {
         using (CvString cvsId = new CvString(id))
            CvInvoke.cveViz3dShowWidget(_ptr, cvsId, widget.GetWidget, pose);
      }

      public void RemoveWidget(String id)
      {
         using (CvString cvsId = new CvString(id))
         {
            CvInvoke.cveViz3dRemoveWidget(_ptr, cvsId);
         }
      }

      /// <summary>
      /// Sets pose of a widget in the window.
      /// </summary>
      /// <param name="id">The id of the widget whose pose will be set.</param>
      /// <param name="pose">The new pose of the widget.</param>
      public void SetWidgetPose(String id, Affine3d pose)
      {
         using (CvString cvsId = new CvString(id))
            CvInvoke.cveViz3dSetWidgetPose(_ptr, cvsId, pose);
      }

      /// <summary>
      /// The window renders and starts the event loop.
      /// </summary>
      public void Spin()
      {
         CvInvoke.cveViz3dSpin(_ptr);
      }

      /// <summary>
      /// Starts the event loop for a given time.
      /// </summary>
      /// <param name="time">	Amount of time in milliseconds for the event loop to keep running.</param>
      /// <param name="forceRedraw">If true, window renders.</param>
      public void SpinOnce(int time = 1, bool forceRedraw = false)
      {
         CvInvoke.cveViz3dSpinOnce(_ptr, time, forceRedraw);
      }

      /// <summary>
      /// Returns whether the event loop has been stopped.
      /// </summary>
      public bool WasStopped
      {
         get { return CvInvoke.cveViz3dWasStopped(_ptr); }

      }

      /// <summary>
      /// Set the background color
      /// </summary>
      public void SetBackgroundMeshLab()
      {
         CvInvoke.cveViz3dSetBackgroundMeshLab(_ptr);
      }

      /// <summary>
      /// Release the unmanaged memory associated with this Viz3d object
      /// </summary>
      protected override void DisposeObject()
      {
         if (!IntPtr.Zero.Equals(_ptr))
            CvInvoke.cveViz3dRelease(ref _ptr);
      }
   }

   /// <summary>
   /// Interface for all widgets
   /// </summary>
   public interface IWidget
   {
      /// <summary>
      /// Get the pointer to the widget object
      /// </summary>
      IntPtr GetWidget { get; }
   }

   /// <summary>
   /// Interface for all widget3D
   /// </summary>
   public interface IWidget3D : IWidget
   {
      /// <summary>
      /// Get the pointer to the widget3D object
      /// </summary>
      IntPtr GetWidget3D { get; }
   }

   /// <summary>
   /// Interface for all widget2D
   /// </summary>
   public interface IWidget2D : IWidget
   {
      /// <summary>
      /// Get the pointer to the widget2D object
      /// </summary>
      IntPtr GetWidget2D { get; }
   }

   /// <summary>
   /// This 3D Widget defines a point cloud.
   /// </summary>
   public class WCloud : UnmanagedObject, IWidget3D
   {
      private IntPtr _widgetPtr;
      private IntPtr _widget3dPtr;

      /// <summary>
      /// Constructs a WCloud.
      /// </summary>
      /// <param name="cloud">Set of points which can be of type: CV_32FC3, CV_32FC4, CV_64FC3, CV_64FC4.</param>
      /// <param name="color">Set of colors. It has to be of the same size with cloud.</param>
      public WCloud(IInputArray cloud, IInputArray color)
      {
         using (InputArray iaCloud = cloud.GetInputArray())
         using (InputArray iaColor = color.GetInputArray())
            CvInvoke.cveWCloudCreateWithColorArray(iaCloud, iaColor, ref _widget3dPtr, ref _widgetPtr);
      }

      /// <summary>
      /// Constructs a WCloud.
      /// </summary>
      /// <param name="cloud">Set of points which can be of type: CV_32FC3, CV_32FC4, CV_64FC3, CV_64FC4.</param>
      /// <param name="color">A single Color for the whole cloud.</param>
      public WCloud(IInputArray cloud, MCvScalar color)
      {
         using (InputArray iaCloud = cloud.GetInputArray())
            CvInvoke.cveWCloudCreateWithColor(iaCloud, ref color, ref _widget3dPtr, ref _widgetPtr);
      }

      /// <summary>
      /// Get the pointer to the Widget3D obj
      /// </summary>
      public IntPtr GetWidget3D
      {
         get { return _widget3dPtr; }
      }

      /// <summary>
      /// Get the pointer to the Widget obj
      /// </summary>
      public IntPtr GetWidget
      {
         get { return _widgetPtr; }
      }

      /// <summary>
      /// Release the unmanaged memory associated with this WCloud
      /// </summary>
      protected override void DisposeObject()
      {
         if (IntPtr.Zero != _ptr)
         {
            CvInvoke.cveWCloudRelease(ref _ptr);
            _widgetPtr = IntPtr.Zero;
            _widget3dPtr = IntPtr.Zero;
         }
      }
   }

   /// <summary>
   /// This 2D Widget represents text overlay.
   /// </summary>
   public class WText : UnmanagedObject, IWidget2D
   {
      private IntPtr _widgetPtr;
      private IntPtr _widget2dPtr;

      /// <summary>
      /// Constructs a WText.
      /// </summary>
      /// <param name="text">Text content of the widget.</param>
      /// <param name="pos">Position of the text.</param>
      /// <param name="fontSize">Font size.</param>
      /// <param name="color">Color of the text.</param>
      public WText(String text, Point pos, int fontSize, MCvScalar color)
      {
         using (CvString cvs = new CvString(text))
            _ptr = CvInvoke.cveWTextCreate(cvs, ref pos, fontSize, ref color, ref _widget2dPtr, ref _widgetPtr);

      }

      /// <summary>
      /// Get the pointer to the widget2D object
      /// </summary>
      public IntPtr GetWidget2D
      {
         get { return _widget2dPtr; }
      }

      /// <summary>
      /// Get the pointer to the widget object.
      /// </summary>
      public IntPtr GetWidget
      {
         get { return _widgetPtr; }
      }

      /// <summary>
      /// Release the unmanaged memory associated with this Viz3d object
      /// </summary>
      protected override void DisposeObject()
      {
         if (!IntPtr.Zero.Equals(_ptr))
            CvInvoke.cveViz3dRelease(ref _ptr);
         _widgetPtr = IntPtr.Zero;
         _widget2dPtr = IntPtr.Zero;
      }
   }

   /// <summary>
   /// This 3D Widget represents a coordinate system.
   /// </summary>
   public class WCoordinateSystem : UnmanagedObject, IWidget3D
   {
      private IntPtr _widgetPtr;
      private IntPtr _widget3dPtr;

      /// <summary>
      /// Constructs a WCoordinateSystem.
      /// </summary>
      /// <param name="scale">Determines the size of the axes.</param>
      public WCoordinateSystem(double scale)
      {
         _ptr = CvInvoke.cveWCoordinateSystemCreate(scale, ref _widget3dPtr, ref _widgetPtr);
      }

      /// <summary>
      /// Get the pointer to the Widget3D obj
      /// </summary>
      public IntPtr GetWidget3D
      {
         get { return _widget3dPtr; }
      }

      /// <summary>
      /// Get the pointer to the Widget obj
      /// </summary>
      public IntPtr GetWidget
      {
         get { return _widgetPtr; }
      }

      /// <summary>
      /// Release the unmanaged memory associated with this Viz3d object
      /// </summary>
      protected override void DisposeObject()
      {
         if (!IntPtr.Zero.Equals(_ptr))
            CvInvoke.cveWCoordinateSystemRelease(ref _ptr);
         _widgetPtr = IntPtr.Zero;
         _widget3dPtr = IntPtr.Zero;
      }
   }

   public static partial class CvInvoke
   {
      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern IntPtr cveViz3dCreate(IntPtr s);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveViz3dShowWidget(IntPtr viz, IntPtr id, IntPtr widget, IntPtr pose);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveViz3dSetWidgetPose(IntPtr viz, IntPtr id, IntPtr pose);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveViz3dRemoveWidget(IntPtr viz, IntPtr id);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveViz3dSpin(IntPtr viz);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveViz3dSpinOnce(
         IntPtr viz,
         int time,
         [MarshalAs(CvInvoke.BoolMarshalType)]
         bool forceRedraw);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      [return: MarshalAs(CvInvoke.BoolMarshalType)]
      internal static extern bool cveViz3dWasStopped(IntPtr viz);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveViz3dSetBackgroundMeshLab(IntPtr viz);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveViz3dRelease(ref IntPtr viz);


      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern IntPtr cveWTextCreate(IntPtr text, ref Point pos, int fontSize, ref MCvScalar color, ref IntPtr widget2D, ref IntPtr widget);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveWTextRelease(ref IntPtr text);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern IntPtr cveWCoordinateSystemCreate(double scale, ref IntPtr widget3d, ref IntPtr widget);

      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveWCoordinateSystemRelease(ref IntPtr system);


      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern IntPtr cveWCloudCreateWithColorArray(IntPtr cloud, IntPtr color, ref IntPtr widget3d, ref IntPtr widget);
      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern IntPtr cveWCloudCreateWithColor(IntPtr cloud, ref MCvScalar color, ref IntPtr widget3d, ref IntPtr widget);
      [DllImport(ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveWCloudRelease(ref IntPtr cloud);
   }
}
#endif