//Microsoft CodeFunDo 2015
//Contributors: Arvind, Mohsin

namespace BioGait
{
    using Microsoft.Kinect;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using IronPlot;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl, ISwitchable
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Lists containing gait parameter data
        /// </summary>
        List<double> list1 = new List<double>();
        List<double> list2 = new List<double>();
        List<double> list3 = new List<double>();
        List<double> list4 = new List<double>();
        List<double> list5 = new List<double>();
        List<double> list6 = new List<double>();
        List<double> list7 = new List<double>();
        List<double> list8 = new List<double>();
        List<double> list9 = new List<double>();
        List<double> list10 = new List<double>();
        List<Coordinate> list11= new List<Coordinate>();

        ///Global variables list///
        private Coordinate prevCM = new Coordinate();
        private Coordinate earlyCM = new Coordinate();
        private Coordinate prev_velCM = new Coordinate();
        private DateTime timestampPrev = new DateTime();
        private DateTime timestampEarly = new DateTime();
        private double Mass=80.00;
        private Coordinate gravity =  new Coordinate(0,-9.8,0);

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
    
        public MainWindow()
        {
            InitializeComponent();
        }

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Buttonhandler1(object sender, RoutedEventArgs e)
        {
            // Add code to perform some action here.
            MessageBox.Show("Click OK to begin analysis","BioGait");
            list1.Clear();
            list2.Clear();
            list3.Clear();
            list4.Clear();
            list5.Clear();
            list6.Clear();
            list7.Clear();
            list8.Clear();
            list9.Clear();
            list10.Clear();
            list11.Clear();

        }

        private void Buttonhandler2(object sender, RoutedEventArgs e)
        {
            // Add code to perform some action here.
            MessageBox.Show("BioGait is an analysis tool used for diagnostic studies on human body mechanics. Press RECORD to begin the gait analysis. Press SAVE when you want to stop the data acquisition to begin generating the results of the analysis. ","BioGait");
            
        }

        private void Buttonhandler3(object sender, RoutedEventArgs e)
        {
            // Add code to perform some action here.
            MessageBox.Show("Generating results of the analysis...", "BioGait");

            Switcher.Switch(new Plots(list1, list2, list3, list4, list5, list6, list7, list8, list9,list10,list11));

        }
        #endregion


        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (Exception)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                //this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Function to get angle subtended by 3 joint positions
        /// </summary>
        /// <param name="delxVec1"></param>
        /// <param name="delyVec1"></param>
        /// <param name="delzVec1"></param>
        /// <param name="delxVec2"></param>
        /// <param name="delyVec2"></param>
        /// <param name="delzVec2"></param>
        /// <returns></returns>

        private double getTheta(double delxVec1, double delyVec1, double delzVec1, double delxVec2, double delyVec2, double delzVec2)
        {

            double dotPrdct = delxVec1 * delxVec2 + delyVec1 * delyVec2 + delzVec1 * delzVec2;
            double magnVec1 = Math.Sqrt(delxVec1 * delxVec1 + delyVec1 * delyVec1 + delzVec1 * delzVec1);
            double magnVec2 = Math.Sqrt(delxVec2 * delxVec2 + delyVec2 * delyVec2 + delzVec2 * delzVec2);

            double Theta = Math.Acos(dotPrdct / (magnVec1 * magnVec2)) * 180 / Math.PI;
            return Theta;
        }

        private double getAngle(SkeletonPoint b1, SkeletonPoint b2, SkeletonPoint b3)
        {
            double delxVec1 = b2.X - b1.X;
            double delyVec1 = b2.Y - b1.Y;
            double delzVec1 = b2.Z - b1.Z;

            double delxVec2 = b3.X - b2.X;
            double delyVec2 = b3.Y - b2.Y;
            double delzVec2 = b3.Z - b2.Z;

            double angle = getTheta(delxVec1, delyVec1, delzVec1, delxVec2, delyVec2, delzVec2);
            return angle;
        }

        /// <summary>
        /// Function to calculate center of mass of the body
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="b3"></param>
        /// <param name="b4"></param>
        /// <param name="b5"></param>
        /// <param name="b6"></param>
        /// <param name="b7"></param>
        /// <param name="b8"></param>
        /// <param name="b9"></param>
        /// <param name="b10"></param>
        /// <param name="b11"></param>
        /// <param name="b12"></param>
        /// <param name="b13"></param>
        /// <param name="b14"></param>
        /// <param name="b15"></param>
        /// <param name="b16"></param>
        /// <param name="b17"></param>
        /// <returns></returns>

        private Coordinate center_mass(SkeletonPoint b1, SkeletonPoint b2, SkeletonPoint b3, SkeletonPoint b4, SkeletonPoint b5, SkeletonPoint b6, SkeletonPoint b7, SkeletonPoint b8, SkeletonPoint b9, SkeletonPoint b10, SkeletonPoint b11, SkeletonPoint b12, SkeletonPoint b13, SkeletonPoint b14, SkeletonPoint b15, SkeletonPoint b16, SkeletonPoint b17)
        {
            ///Alternative method to calculate center of mass
            /*float delxVec1 = (b1.X + b2.X + b3.X + b4.X + b5.X + b6.X + b7.X + b8.X + b9.X+ b10.X + b11.X+ b12.X + b13.X + b14.X + b15.X + b16.X + b17.X) / 17;
            float delyVec1 = (b1.Y + b2.Y + b3.Y + b4.Y + b5.Y + b6.Y + b7.Y + b8.Y + b9.Y + b10.Y + b11.Y + b12.Y + b13.Y + b14.Y + b15.Y + b16.Y + b17.Y)/17;
            float delzVec1 = (b1.Z + b2.Z + b3.Z + b4.Z + b5.Z + b6.Z + b7.Z + b8.Z + b9.Z + b10.Z + b11.Z + b12.Z + b13.Z + b14.Z + b15.Z + b16.Z + b17.Z)/17;
            */

            Coordinate c = new Coordinate();
            c.X = b7.X;
            c.Y = b7.Y;
            c.Z = b7.Z;
            return c;
        }

        private Coordinate getVelocity(Coordinate current, DateTime timeStampCurrent, Coordinate prev,DateTime timeStampPrev)
        {
            double delTime=timeStampCurrent.Subtract(timeStampPrev).TotalSeconds;

            Coordinate vel = new Coordinate();
            vel.X= (current.X- prev.X)/ (float) delTime;
            vel.Y= (current.Y- prev.Y)/ (float) delTime;
            vel.Z= (current.Z- prev.Z)/ (float) delTime;
            return vel;
        }

        private Coordinate getAcceleration(Coordinate current, Coordinate prev, Coordinate next,DateTime timeStampCurrent,DateTime timeStampPrev,DateTime timeStampNext)
        {
            double delTime1 = timeStampCurrent.Subtract(timeStampPrev).TotalSeconds;
            double delTime2 = timeStampNext.Subtract(timeStampCurrent).TotalSeconds;		            
            
            Coordinate acc = new Coordinate();
            acc.X = (2 * ((prev.X * (float)delTime2) - ((float)(delTime1 + delTime2) * current.X) + (next.X * (float)delTime1))) / (float) (delTime1 * delTime2 * (delTime1 + delTime2));
            acc.Y = (2 * ((prev.Y * (float)delTime2) - ((float)(delTime1 + delTime2) * current.Y) + (next.Y * (float)delTime1))) / (float)(delTime1 * delTime2 * (delTime1 + delTime2));
            acc.Z = (2 * ((prev.Z * (float)delTime2) - ((float)(delTime1 + delTime2) * current.Z) + (next.Z * (float)delTime1))) / (float)(delTime1 * delTime2 * (delTime1 + delTime2));
            return acc;
        }

        private Coordinate getGRF(Coordinate acceleration, double mass)
        {
            Coordinate GRF = new Coordinate();
            GRF.X = mass * (acceleration.X - gravity.X);
            GRF.Y = mass * (acceleration.Y - gravity.Y);
            GRF.Z = mass * (acceleration.Z - gravity.Z);
            return GRF;
        }

        /// <summary>
        /// Defining joint positions
        /// </summary>
        SkeletonPoint pointShoulder = new SkeletonPoint();
        SkeletonPoint pointElbow = new SkeletonPoint();
        SkeletonPoint pointWrist = new SkeletonPoint();
        SkeletonPoint pointHand = new SkeletonPoint();
        SkeletonPoint pointShoulder2 = new SkeletonPoint();
        SkeletonPoint pointElbow2 = new SkeletonPoint();
        SkeletonPoint pointWrist2 = new SkeletonPoint();
        SkeletonPoint pointHand2 = new SkeletonPoint();
        SkeletonPoint pointHipCenter = new SkeletonPoint();
        SkeletonPoint pointHipLeft = new SkeletonPoint();
        SkeletonPoint pointHipRight = new SkeletonPoint();
        SkeletonPoint pointKneeLeft = new SkeletonPoint();
        SkeletonPoint pointKneeRight = new SkeletonPoint();
        SkeletonPoint pointAnkleLeft = new SkeletonPoint();
        SkeletonPoint pointAnkleRight = new SkeletonPoint();
        SkeletonPoint pointFootLeft = new SkeletonPoint();
        SkeletonPoint pointFootRight = new SkeletonPoint();

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);
 
            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;
                JointType jointType = joint.JointType;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                    SkeletonPoint jointPosition = joint.Position;

                    if (jointType.Equals(JointType.ShoulderRight))
                    {
                        pointShoulder = jointPosition;
                    }
                    else if (jointType.Equals(JointType.ElbowRight))
                    {
                        pointElbow = jointPosition;
                    }
                    else if (jointType.Equals(JointType.WristRight))
                    {
                        pointWrist = jointPosition;
                    }
                    else if (jointType.Equals(JointType.HandRight))
                    {
                        pointHand = jointPosition;
                    }
                    if (jointType.Equals(JointType.ShoulderLeft))
                    {
                        pointShoulder2 = jointPosition;
                    }
                    else if (jointType.Equals(JointType.ElbowLeft))
                    {
                        pointElbow2 = jointPosition;
                    }
                    else if (jointType.Equals(JointType.WristLeft))
                    {
                        pointWrist2 = jointPosition;
                    }
                    else if (jointType.Equals(JointType.HandLeft))
                    {
                        pointHand2 = jointPosition;
                    }

                    else if (jointType.Equals(JointType.HipCenter))
                    {
                        pointHipCenter = jointPosition;
                    }
                    else if (jointType.Equals(JointType.HipLeft))
                    {
                        pointHipLeft = jointPosition;
                    }
                    else if (jointType.Equals(JointType.HipRight))
                    {
                        pointHipRight = jointPosition;
                    }
                    else if (jointType.Equals(JointType.KneeLeft))
                    {
                        pointKneeLeft = jointPosition;
                    }
                    else if (jointType.Equals(JointType.KneeRight))
                    {
                        pointKneeRight = jointPosition;
                    }
                    else if (jointType.Equals(JointType.AnkleLeft))
                    {
                        pointAnkleLeft = jointPosition;
                    }
                    else if (jointType.Equals(JointType.AnkleRight))
                    {
                        pointAnkleRight = jointPosition;
                    }
                    else if (jointType.Equals(JointType.FootLeft))
                    {
                        pointFootLeft = jointPosition;
                    }
                    else if (jointType.Equals(JointType.FootRight))
                    {
                        pointFootRight = jointPosition;
                    }
                 

                }

                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;                    
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
            
            ///Defining Gait parameters
            double pelvis_obliquity = 180/Math.PI*Math.Atan((pointHipLeft.Y - pointHipRight.Y) / Math.Sqrt(Math.Pow(pointHipLeft.X - pointHipRight.X,2) + Math.Pow(pointHipLeft.Z - pointHipRight.Z,2)));
            double pelvic_rotation = 180 / Math.PI * Math.Atan((pointHipLeft.Z - pointHipRight.Z) / Math.Sqrt(Math.Pow(pointHipLeft.X - pointHipRight.X, 2) + Math.Pow(pointHipLeft.Y - pointHipRight.Y, 2)));

            double hip_ab_right = 180 / Math.PI * Math.Atan((pointKneeRight.X - pointHipRight.X) / Math.Sqrt(Math.Pow(pointKneeRight.Y - pointHipRight.Y, 2) + Math.Pow(pointKneeRight.Z - pointHipRight.Z, 2)));
            double hip_flex_right = 180 / Math.PI * Math.Atan(-(pointKneeRight.Z - pointHipRight.Z) / Math.Sqrt(Math.Pow(pointKneeRight.Y - pointHipRight.Y, 2) + Math.Pow(pointKneeRight.X - pointHipRight.X, 2)));

            double hip_ab_left = 180 / Math.PI * Math.Atan(-(pointKneeLeft.X - pointHipLeft.X) / Math.Sqrt(Math.Pow(pointKneeLeft.Y - pointHipLeft.Y, 2) + Math.Pow(pointKneeLeft.Z - pointHipLeft.Z, 2)));
            double hip_flex_left = 180 / Math.PI * Math.Atan(-(pointKneeLeft.Z - pointHipLeft.Z) / Math.Sqrt(Math.Pow(pointKneeLeft.Y - pointHipLeft.Y, 2) + Math.Pow(pointKneeLeft.X - pointHipLeft.X, 2)));

            double knee_flex_right= getAngle(pointHipRight, pointKneeRight, pointAnkleRight);
            double knee_flex_left = getAngle(pointHipLeft, pointKneeLeft, pointAnkleLeft);

            double ankle_left = getAngle(pointKneeLeft, pointAnkleLeft, pointFootLeft);
            double ankle_right = getAngle(pointKneeRight, pointAnkleRight, pointFootRight);
           
            ///Calculating center of mass 
            Coordinate CM = new Coordinate();
            CM = center_mass(pointAnkleLeft, pointAnkleRight, pointFootLeft, pointFootRight, pointHand, pointHand2, pointHipCenter, pointHipLeft, pointHipRight, pointKneeLeft, pointKneeRight, pointShoulder, pointShoulder2, pointElbow, pointElbow2, pointWrist, pointWrist2);
            
            ///Timestamp
            DateTime timestamp;
            timestamp = DateTime.Now;
            
            ///Calculating velocity of center of mass
            Coordinate velocity = new Coordinate();
            Coordinate acceleration = new Coordinate();
            velocity = getVelocity(CM, timestamp, prevCM, timestampPrev);
            acceleration = getAcceleration(prevCM, earlyCM, CM, timestampPrev, timestampEarly, timestamp);
            
            prev_velCM = velocity;
            timestampEarly = timestampPrev;
            timestampPrev = timestamp;
            earlyCM = prevCM;
            prevCM = CM;

            ///Calculating GRF
            Coordinate GRF = new Coordinate();
            GRF = getGRF(acceleration,Mass);

            ///For debugging purposes only
            /*System.Console.WriteLine("acc is " + acceleration.X + " " + acceleration.Y + " " + acceleration.Z);
            System.Console.WriteLine("Coordinate is " + pointHipCenter.X + " " + pointHipCenter.Y + " " + pointHipCenter.Z );
            System.Console.WriteLine("velocity is " + velocity.X + " " + velocity.Y + " " + velocity.Z);
            System.Console.WriteLine("GRF is " + GRF.X + " " + GRF.Y + " " + GRF.Z);*/

            ///Add Gait parameters to the lists
            list1.Add(pelvis_obliquity);
            list2.Add(hip_ab_left);
            list3.Add(hip_ab_right);
            list4.Add(knee_flex_left);
            list5.Add(knee_flex_right);
            list6.Add(hip_flex_left);
            list7.Add(hip_flex_right);
            list8.Add(ankle_left);
            list9.Add(ankle_right);
            list10.Add(pelvic_rotation);
            list11.Add(GRF);
        
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>


        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// <summary>
        /// Handles the checking or unchecking of the seated mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        
    }
}