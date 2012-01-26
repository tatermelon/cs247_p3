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

    // Open bag, select photo
    class CustomController2 : SkeletonController
    {

        private MainWindow window;
        
        //hand positions
        public Point curHandPoint, lastHandPoint;
        public int leftCount = 0;

        public CustomController2(MainWindow win) : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            /* YOUR CODE HERE*/
            //Scale the joints to the size of the window
            Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);

            if (targets[1].isSelected())
            {
                targets[1].setTargetPosition(leftHand.Position.X-40, leftHand.Position.Y-40);
            }

            foreach (var target in targets)
            {
                Target cur = target.Value;
                int targetID = cur.id; //ID in range [1..5]

                if (targetID == 1)
                {
                    //Calculate how far our left hand is from the target in both x and y directions
                    double deltaX_left = Math.Abs(leftHand.Position.X - cur.getXPosition());
                    double deltaY_left = Math.Abs(leftHand.Position.Y - cur.getYPosition());
                    if (deltaX_left < 15 && deltaY_left < 15)
                    {
                        cur.setTargetSelected(); // select target 1 (container)

                        //show other targets (objects)
                        targets[2].showTarget();
                        targets[2].setTargetPosition(400, 80);
                        targets[3].showTarget();
                        targets[3].setTargetPosition(400, 180);
                        targets[4].showTarget();
                        targets[4].setTargetPosition(400, 260);
                        targets[5].showTarget();
                        targets[5].setTargetPosition(400, 350);
                    }
                }
                else
                {
                    //Calculate how far our right hand is from the target in both x and y directions
                    double deltaX_right = Math.Abs(rightHand.Position.X - cur.getXPosition());
                    double deltaY_right = Math.Abs(rightHand.Position.Y - cur.getYPosition());

                    if (deltaX_right < 15 && deltaY_right < 15)
                    {
                        cur.setTargetSelected();
                    }
                }


                

                

                //If we have a hit in a reasonable range, highlight the target
                //if (deltaX_left < 15 && deltaY_left < 15 || deltaX_right < 15 && deltaY_right < 15 && targetID == 1)
                //{
                    
                //}
               // else
                //{
                  //  cur.setTargetUnselected();
                //}
            }


        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {

            /* YOUR CODE HERE */
            //adjustScale(1.1f);
            targets[1].setTargetPosition(260, 200); //set "container" start position
            targets[2].hideTarget();
            targets[3].hideTarget();
            targets[4].hideTarget();
            targets[5].hideTarget();
        }
    }
}
