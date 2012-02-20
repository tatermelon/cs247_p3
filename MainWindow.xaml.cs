﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui; //has a runtime class for tilt, camera, skeletal tracking
using Microsoft.Research.Kinect.Audio; //added today
using Coding4Fun.Kinect.Wpf; //

namespace SkeletalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Kinect Runtime
        Runtime nui;

        //Targets and skeleton controller
        SkeletonController exampleController;
        CustomController1 yourController1;
        CustomController2 yourController2;
        CustomController3 yourController3;

        //Holds the currently active controller
        SkeletonController currentController;
       
        Dictionary<int, Target> targets = new Dictionary<int, Target>();

        //Scaling constants
        public float k_xMaxJointScale = 1.5f;
        public float k_yMaxJointScale = 1.5f;

        int i;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupKinect();
            exampleController = new SkeletonController(this);
            yourController1 = new CustomController1(this);
            yourController2 = new CustomController2(this);
            yourController3 = new CustomController3(this);
            currentController = exampleController;
            InitTargets();
            i = 0;
        }
        
        private void InitTargets()
        {
            targets.Add(1, new Target(target1, 1));
            targets.Add(2, new Target(target2, 2));
            targets.Add(3, new Target(target3, 3));
            targets.Add(4, new Target(target4, 4));
            targets.Add(5, new Target(target5, 5));
            targets.Add(6, new Target(targetLeft, 6));
            targets.Add(7, new Target(targetRight, 7));
            currentController.controllerActivated(targets);
            Canvas.SetZIndex(target1, 100);
            Canvas.SetZIndex(target2, 100);
            Canvas.SetZIndex(target3, 100);
            Canvas.SetZIndex(target4, 100);
            Canvas.SetZIndex(target5, 100);
            Canvas.SetZIndex(targetLeft, 100);
            Canvas.SetZIndex(targetRight, 100);
        }

        private void SetupKinect()
        {
            if (Runtime.Kinects.Count == 0)
            {
                this.Title = "No Kinect connected"; 
            }
            else
            {
                //use first Kinect
                nui = Runtime.Kinects[0];

                //Initialize to do skeletal tracking
                nui.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor | RuntimeOptions.UseDepthAndPlayerIndex);

                //add event to receive skeleton data
                nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

                //add event to receive video data
                nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);

                //to experiment, toggle TransformSmooth between true & false and play with parameters            
                nui.SkeletonEngine.TransformSmooth = true;
                TransformSmoothParameters parameters = new TransformSmoothParameters();
                // parameters used to smooth the skeleton data
                parameters.Smoothing = 0.3f;
                parameters.Correction = 0.3f;
                parameters.Prediction = 0.4f;
                parameters.JitterRadius = 0.7f;
                parameters.MaxDeviationRadius = 0.2f;
                nui.SkeletonEngine.SmoothParameters = parameters;

                //Open the video stream
                nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                
                //Force video to the background
                Canvas.SetZIndex(image1, -10000);
            }
        }

        void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            //Automagically create BitmapSource for Video
            image1.Source = e.ImageFrame.ToBitmapSource();            
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            
            SkeletonFrame allSkeletons = e.SkeletonFrame;

            //get the first tracked skeleton
            SkeletonData skeleton = (from s in allSkeletons.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();

            if(skeleton != null)
            {
                // tracking hand position
                Point rightHandPosition;
                Joint handJoint = skeleton.Joints[JointID.HandRight];
                rightHandPosition = new Point(handJoint.Position.X, handJoint.Position.Y);
                rightHandYText.Content = rightHandPosition.Y;

                Point leftHandPosition;
                Joint leftHandJoint = skeleton.Joints[JointID.HandLeft];
                leftHandPosition = new Point(leftHandJoint.Position.X, leftHandJoint.Position.Y);
                deltaXText.Content = Math.Abs(leftHandPosition.X - rightHandPosition.X);
                deltaYText.Content = Math.Abs(leftHandPosition.Y - rightHandPosition.Y);

                Point headPosition;
                Joint headJoint = skeleton.Joints[JointID.Head];
                headPosition = new Point(headJoint.Position.X, headJoint.Position.Y);
                headYText.Content = headPosition.Y;

                Point rightElbowPosition;
                Joint rightElbowJoint = skeleton.Joints[JointID.ElbowRight];
                rightElbowPosition = new Point(rightElbowJoint.Position.X, rightElbowJoint.Position.Y);
                rightElbowYText.Content = rightElbowPosition.Y;

                Point rightShoulderPosition;
                Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight];
                rightShoulderPosition = new Point(rightShoulder.Position.X, rightShoulder.Position.Z);
                rightShoulderText.Content = rightShoulderPosition.Y;
                rightShoulderXText.Content = rightShoulderPosition.X;
                rightShoulderYText.Content = rightShoulder.Position.Y;

                Point leftShoulderPosition;
                Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft];
                leftShoulderPosition = new Point(leftShoulder.Position.X, leftShoulder.Position.Z);
                leftShoulderText.Content = leftShoulderPosition.Y;
                leftShoulderXText.Content = leftShoulderPosition.X;

                rightShoulderPosition.Y = 0;
                leftShoulderPosition.Y = 0;

                rightShoulderPosition.X = 0;
                leftShoulderPosition.X = 0;

                SetEllipsePosition(headEllipse, skeleton.Joints[JointID.Head]);
                SetEllipsePosition(leftEllipse, skeleton.Joints[JointID.HandLeft]);
                SetEllipsePosition(rightEllipse, skeleton.Joints[JointID.HandRight]);
                SetEllipsePosition(shoulderCenter, skeleton.Joints[JointID.ShoulderCenter]);
                SetEllipsePosition(shoulderRight, skeleton.Joints[JointID.ShoulderRight]);
                SetEllipsePosition(shoulderLeft, skeleton.Joints[JointID.ShoulderLeft]);
                SetEllipsePosition(ankleRight, skeleton.Joints[JointID.AnkleRight]);
                SetEllipsePosition(ankleLeft, skeleton.Joints[JointID.AnkleLeft]);
                SetEllipsePosition(footLeft, skeleton.Joints[JointID.FootLeft]);
                SetEllipsePosition(footRight, skeleton.Joints[JointID.FootRight]);
                SetEllipsePosition(wristLeft, skeleton.Joints[JointID.WristLeft]);
                SetEllipsePosition(wristRight, skeleton.Joints[JointID.WristRight]);
                SetEllipsePosition(elbowLeft, skeleton.Joints[JointID.ElbowLeft]);
                SetEllipsePosition(elbowRight, skeleton.Joints[JointID.ElbowRight]);
                SetEllipsePosition(ankleLeft, skeleton.Joints[JointID.AnkleLeft]);
                SetEllipsePosition(footLeft, skeleton.Joints[JointID.FootLeft]);
                SetEllipsePosition(footRight, skeleton.Joints[JointID.FootRight]);
                SetEllipsePosition(wristLeft, skeleton.Joints[JointID.WristLeft]);
                SetEllipsePosition(wristRight, skeleton.Joints[JointID.WristRight]);
                SetEllipsePosition(kneeLeft, skeleton.Joints[JointID.KneeLeft]);
                SetEllipsePosition(kneeRight, skeleton.Joints[JointID.KneeRight]);
                SetEllipsePosition(hipCenter, skeleton.Joints[JointID.HipCenter]);
                currentController.processSkeletonFrame(skeleton, targets);

            }
        }

        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {    
            var scaledJoint = joint.ScaleTo(640, 480, k_xMaxJointScale, k_yMaxJointScale);
            //var scaledJoint = joint.ScaleTo(640, 480, k_xMax, k_yMaxJointScale);

            Canvas.SetLeft(ellipse, scaledJoint.Position.X - (double)ellipse.GetValue(Canvas.WidthProperty) / 2 );
            Canvas.SetTop(ellipse, scaledJoint.Position.Y - (double)ellipse.GetValue(Canvas.WidthProperty) / 2);
            Canvas.SetZIndex(ellipse, (int) -Math.Floor(scaledJoint.Position.Z*100));
            if (joint.ID == JointID.HandLeft || joint.ID == JointID.HandRight || joint.ID == JointID.ShoulderLeft || joint.ID == JointID.ShoulderRight)
            {   
                byte val = (byte)(Math.Floor((joint.Position.Z - 0.8)* 255 / 2));
                ellipse.Fill = new SolidColorBrush(Color.FromRgb(val, val, val));
            }
        }



        private void Window_Closed(object sender, EventArgs e)
        {
            //Cleanup
            nui.Uninitialize();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                currentController = exampleController;
                controllerText.Content = "Example Controller";
                currentController.controllerActivated(targets);
            }

            if (e.Key == Key.D2)
            {
                currentController = yourController1;
                controllerText.Content = "Controller 1";
                currentController.controllerActivated(targets);
            }

            if (e.Key == Key.D3)
            {
                currentController = yourController2;
                controllerText.Content = "Controller 2";
                currentController.controllerActivated(targets);
            }

            if (e.Key == Key.D4)
            {
                currentController = yourController3;
                controllerText.Content = "Controller 3";
                currentController.controllerActivated(targets);
            }
        }
    }


}
