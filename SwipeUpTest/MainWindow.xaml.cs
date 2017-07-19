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
using Microsoft.Kinect;
using SwipeUpTest;

namespace SwipeUpTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary> Active Kinect sensor </summary>
        private KinectSensor kinectSensor = null;

        /// <summary> Array for the bodies (Kinect will track up to 6 people simultaneously) </summary>
        private Body[] bodies = null;

        /// <summary> Reader for body frames </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary> Current status text to display </summary>
        private string statusText = null;

        /// <summary> KinectBodyView object which handles drawing the Kinect bodies to a View box in the UI </summary>
        private KinectBodyView kinectBodyView = null;

        /// <summary> List of gesture detectors, there will be one detector created for each potential body (max of 6) </summary>
        private List<GestureDetector> gestureDetectorList = null;


        /// <summary>
        /// Reader for color frames
        /// </summary>
        private ColorFrameReader colorFrameReader = null;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap colorBitmap = null;

        /// <summary>
        /// Current status text to display
        /// </summary>
        //private string statusText = null;

        MultiSourceFrameReader _reader;
        IList<Body> _bodies;



        static int number = 0;
        static int number2 = 0;
        static int number3 = 0;
        static int number4 = 0;
        static int number5 = 0;
        static int number6 = 0;
        static int number7 = 0;
        static int number8 = 0;
        public MainWindow()
        {
            // get the kinectSensor object
            this.kinectSensor = KinectSensor.GetDefault();

            //// open the reader for the color frames
            //this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            //// wire handler for frame arrival, Capture photo to send to cognitive
            ////this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

            //// create the colorFrameDescription from the ColorFrameSource using Bgra format
            //FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            //// create the bitmap to display
            //this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            //// set IsAvailableChanged event notifier
            ////this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;


            _reader = kinectSensor.OpenMultiSourceFrameReader(
                                                         FrameSourceTypes.Body);
            _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;


            // open the sensor
            this.kinectSensor.Open();

            InitializeComponent();

            //kinect code start


            // only one sensor is currently supported
            //this.kinectSensor = KinectSensor.GetDefault();


            // open the sensor
            //this.kinectSensor.Open();

            // open the reader for the body frames
            //this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // set the BodyFramedArrived event notifier
            //this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;

            // initialize the BodyViewer object for displaying tracked bodies in the UI
            this.kinectBodyView = new KinectBodyView(this.kinectSensor);

            // initialize the gesture detection objects for our gestures
            this.gestureDetectorList = new List<GestureDetector>();

            // set our data context objects for display in UI
            this.DataContext = this;
            this.kinectBodyViewbox.DataContext = this.kinectBodyView;
            //this.kinectBodyViewbox.Visibility = Visibility.Hidden;

            // create a gesture detector for each body (6 bodies => 6 detectors) and create content controls to display results in the UI
            int col0Row = 0;
            int col1Row = 0;
            int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
            for (int i = 0; i < maxBodies; ++i)
            {
                GestureResultView result = new GestureResultView(i, false, false, 0.0f);
                GestureDetector detector = new GestureDetector(this.kinectSensor, result);
                this.gestureDetectorList.Add(detector);

                // split gesture results across the first two columns of the content grid
                ContentControl contentControl = new ContentControl();
                contentControl.Content = this.gestureDetectorList[i].GestureResultView;

                if (i % 2 == 0)
                {
                    // Gesture results for bodies: 0, 2, 4
                    Grid.SetColumn(contentControl, 0);
                    Grid.SetRow(contentControl, col0Row);
                    ++col0Row;
                }
                else
                {
                    // Gesture results for bodies: 1, 3, 5
                    Grid.SetColumn(contentControl, 1);
                    Grid.SetRow(contentControl, col1Row);
                    ++col1Row;
                }

            }
            //kinect code end
        }


        public void setLabel()
        {
            number++;
            lbtext.Content = "swipe up count: " + number;
            
        }
        public void setLabel2()
        {
            number2++;
            lbtext2.Content = "Hands up Right count: " + number2;

        }
        public void setLabel3()
        {
            number3++;
            lbtext3.Content = "Hands up Left count: " + number3;

        }
        public void setLabel4()
        {
            number4++;
            lbtext4.Content = "Swipe down right count: " + number4;

        }
        public void setLabel5()
        {
            number5++;
            lbtext5.Content = "Swipe up left count: " + number5;

        }

        public void setLabel6()
        {
            number6++;
            lbtext6.Content = "Swipe right, left count: " + number6;

        }
        public void setLabel7()
        {
            number7++;
            lbtext7.Content = "Swipe Down, left count: " + number7;

        }
        public void setLabel8()
        {
            number8++;
            lbtext8.Content = "Swipe Left, Right count: " + number8;

        }

        //Kinect code start

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {


            var reference = e.FrameReference.AcquireFrame();

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                bool dataReceived = false;



                using (BodyFrame bodyFrame = frame)
                {
                    if (bodyFrame != null)
                    {
                        if (this.bodies == null)
                        {
                            // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                            this.bodies = new Body[bodyFrame.BodyCount];
                        }

                        // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                        // As long as those body objects are not disposed and not set to null in the array,
                        // those body objects will be re-used.
                        bodyFrame.GetAndRefreshBodyData(this.bodies);
                        dataReceived = true;
                    }
                }

                if (dataReceived)
                {
                    // visualize the new body data
                    this.kinectBodyView.UpdateBodyFrame(this.bodies);

                    // we may have lost/acquired bodies, so update the corresponding gesture detectors
                    if (this.bodies != null)
                    {
                        // loop through all bodies to see if any of the gesture detectors need to be updated
                        int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
                        for (int i = 0; i < maxBodies; ++i)
                        {
                            Body body = this.bodies[i];
                            ulong trackingId = body.TrackingId;

                            // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                            if (trackingId != this.gestureDetectorList[i].TrackingId)
                            {
                                this.gestureDetectorList[i].TrackingId = trackingId;

                                // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                                // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                                this.gestureDetectorList[i].IsPaused = trackingId == 0;
                            }
                        }
                    }
                }






            }
        }
    }
}
