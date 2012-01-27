using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{


    class CustomController2 : SkeletonController
    {
        private MainWindow window;

        public CustomController2(MainWindow win)
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            /* YOUR CODE HERE*/

            foreach (var target in targets)
            {
                Target cur = target.Value;
                int targetID = cur.id; //ID in range [1..5]

                //Scale the joints to the size of the window
                //Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, 0.5f, 0.5f);
                //Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, 0.5f, 0.5f);
                //Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
                //Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
                Joint leftFoot = skeleton.Joints[JointID.FootLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
                Joint rightFoot = skeleton.Joints[JointID.FootRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);

                //Calculate how far our left hand is from the target in both x and y directions
                //double deltaX_left = Math.Abs(leftHand.Position.X - cur.getXPosition());
                //double deltaY_left = Math.Abs(leftHand.Position.Y - cur.getYPosition());

                //Calculate how far our right hand is from the target in both x and y directions
                //double deltaX_right = Math.Abs(rightHand.Position.X - cur.getXPosition());
                //double deltaY_right = Math.Abs(rightHand.Position.Y - cur.getYPosition());

                //Calculate how far our left foot is from the target in both x and y directions
                double deltaX_leftFoot = Math.Abs(leftFoot.Position.X - cur.getXPosition());
                double deltaY_leftFoot = Math.Abs(leftFoot.Position.Y - cur.getYPosition());

                //Calculate how far our right hand is from the target in both x and y directions
                double deltaX_rightFoot = Math.Abs(rightFoot.Position.X - cur.getXPosition());
                double deltaY_rightFoot = Math.Abs(rightFoot.Position.Y - cur.getYPosition());

                //If we have a hit in a reasonable range, highlight the target
                if (deltaX_leftFoot < 15 && deltaY_leftFoot < 15 || deltaX_rightFoot < 15 && deltaY_rightFoot < 15)
                {
                    cur.setTargetSelected();
                }
                else
                {
                    cur.setTargetUnselected();
                }
            }

        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {

            /* YOUR CODE HERE */
            adjustScale(1.1f);
            targets[5].setTargetPosition(30, 30);
            targets[2].hideTarget();
            //targets[2].showTarget();
            //targets[5].isHidden();
            targets[3].setTargetHighlighted();

        }
    }
}
