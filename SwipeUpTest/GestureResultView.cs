//------------------------------------------------------------------------------
// <copyright file="GestureResultView.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SwipeUpTest
{
    using SwipeUpTest;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Stores discrete gesture results for the GestureDetector.
    /// Properties are stored/updated for display in the UI.
    /// </summary>
    public sealed class GestureResultView : INotifyPropertyChanged
    {
        /// <summary> Image to show when the 'detected' property is true for a tracked body </summary>
        private readonly ImageSource seatedImage = new BitmapImage(new Uri(@"Images\Seated.png", UriKind.Relative));

        /// <summary> Image to show when the 'detected' property is false for a tracked body </summary>
        private readonly ImageSource notSeatedImage = new BitmapImage(new Uri(@"Images\NotSeated.png", UriKind.Relative));

        /// <summary> Image to show when the body associated with the GestureResultView object is not being tracked </summary>
        private readonly ImageSource notTrackedImage = new BitmapImage(new Uri(@"Images\NotTracked.png", UriKind.Relative));

        /// <summary> Array of brush colors to use for a tracked body; array position corresponds to the body colors used in the KinectBodyView class </summary>
        private readonly Brush[] trackedColors = new Brush[] { Brushes.Red, Brushes.Orange, Brushes.Green, Brushes.Blue, Brushes.Indigo, Brushes.Violet };

        /// <summary> Brush color to use as background in the UI </summary>
        private Brush bodyColor = Brushes.Gray;

        /// <summary> The body index (0-5) associated with the current gesture detector </summary>
        private int bodyIndex = 0;

        /// <summary> Current confidence value reported by the discrete gesture </summary>
        private float confidence = 0.0f;

        /// <summary> True, if the discrete gesture is currently being detected </summary>
        private bool detected = false;

        /// <summary> Image to display in UI which corresponds to tracking/detection state </summary>
        private ImageSource imageSource = null;

        /// <summary> True, if the body is currently being tracked </summary>
        private bool isTracked = false;

        /// <summary>
        /// Initializes a new instance of the GestureResultView class and sets initial property values
        /// </summary>
        /// <param name="bodyIndex">Body Index associated with the current gesture detector</param>
        /// <param name="isTracked">True, if the body is currently tracked</param>
        /// <param name="detected">True, if the gesture is currently detected for the associated body</param>
        /// <param name="confidence">Confidence value for detection of the 'Seated' gesture</param>
        public GestureResultView(int bodyIndex, bool isTracked, bool detected, float confidence)
        {
            this.BodyIndex = bodyIndex;
            this.IsTracked = isTracked;
            this.Detected = detected;
            this.Confidence = confidence;
            this.ImageSource = this.notTrackedImage;
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> 
        /// Gets the body index associated with the current gesture detector result 
        /// </summary>
        public int BodyIndex
        {
            get
            {
                return this.bodyIndex;
            }

            private set
            {
                if (this.bodyIndex != value)
                {
                    this.bodyIndex = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets the body color corresponding to the body index for the result
        /// </summary>
        public Brush BodyColor
        {
            get
            {
                return this.bodyColor;
            }

            private set
            {
                if (this.bodyColor != value)
                {
                    this.bodyColor = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the body associated with the gesture detector is currently being tracked 
        /// </summary>
        public bool IsTracked
        {
            get
            {
                return this.isTracked;
            }

            private set
            {
                if (this.IsTracked != value)
                {
                    this.isTracked = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the discrete gesture has been detected
        /// </summary>
        public bool Detected
        {
            get
            {
                return this.detected;
            }

            private set
            {
                if (this.detected != value)
                {
                    this.detected = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a float value which indicates the detector's confidence that the gesture is occurring for the associated body 
        /// </summary>
        public float Confidence
        {
            get
            {
                return this.confidence;
            }

            private set
            {
                if (this.confidence != value)
                {
                    this.confidence = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets an image for display in the UI which represents the current gesture result for the associated body 
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }

            private set
            {
                if (this.ImageSource != value)
                {
                    this.imageSource = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Updates the values associated with the discrete gesture detection result
        /// </summary>
        /// <param name="isBodyTrackingIdValid">True, if the body associated with the GestureResultView object is still being tracked</param>
        /// <param name="isGestureDetected">True, if the discrete gesture is currently detected for the associated body</param>
        /// <param name="detectionConfidence">Confidence value for detection of the discrete gesture</param>
        public bool UpdateGestureResult(bool isBodyTrackingIdValid, bool isGestureDetected, float detectionConfidence, string gestureName)
        {
            this.IsTracked = isBodyTrackingIdValid;
            this.Confidence = 0.0f;
            bool gestureFound = false;

            if (!this.IsTracked)
            {
                this.ImageSource = this.notTrackedImage;
                this.Detected = false;
                this.BodyColor = Brushes.Gray;
            }
            else
            {
                this.Detected = isGestureDetected;
                this.BodyColor = this.trackedColors[this.BodyIndex];

                if (this.Detected)
                {
                    this.Confidence = detectionConfidence;
                    if (this.Confidence > 0.4)
                    {
                        this.ImageSource = this.seatedImage;
                        //https://stackoverflow.com/questions/15425495/change-wpf-mainwindow-label-from-another-class-and-separate-thread
                        //to change label in other class wpf
                        //http://the--semicolon.blogspot.co.id/p/change-wpf-window-label-content-from.html


                        var mainwin = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
                        //string state = App.Current.Properties["state"].ToString();
                        switch (gestureName)
                        {
                            case "SwipeUp_Right":

                                mainwin.setLabel();
                                int milliseconds = 500;
                                Thread.Sleep(milliseconds);
                                //swipeUp(state);
                                break;

                            case "SwipeUp_Left":
                                //swipeUp(state);
                                mainwin.setLabel5();
                                int milliseconds5 = 500;
                                Thread.Sleep(milliseconds5);
                                break;

                            case "SwipeDown_Right":
                                
                                break;

                            case "SwipeDown_Left":
                                
                                break;

                            case "SwipeLeft_Right":
                                mainwin.setLabel8();
                                int milliseconds8 = 500;
                                Thread.Sleep(milliseconds8);
                                break;

                            case "SwipeLeft_Left":
                                mainwin.setLabel4();
                                int milliseconds4 = 500;
                                Thread.Sleep(milliseconds4);
                                break;
                            case "HandsUp_Right":
                                this.Confidence = detectionConfidence;
                                mainwin.setLabel2();
                                int milliseconds2 = 500;
                                Thread.Sleep(milliseconds2);
                                break;
                            case "HandsUp_Left":
                                this.Confidence = detectionConfidence;
                                mainwin.setLabel3();
                                int milliseconds3 = 500;
                                Thread.Sleep(milliseconds3);
                                break;
                            case "SwipeRight_Left":
                                mainwin.setLabel6();
                                int milliseconds6 = 500;
                                Thread.Sleep(milliseconds6);
                                break;
                            case "SwipeRight_Right":
                                mainwin.setLabel7();
                                int milliseconds7 = 500;
                                Thread.Sleep(milliseconds7);
                                break;


                        }
                        //"HandsUp_Right"
                        //                            , "SwipeRight_Right"
                        //                            , "SwipeUp_Right"
                        //                            , "SwipeLeft_Right"
                        //                            , "HandsUp_Left"
                        //                            , "SwipeRight_Left"


                        gestureFound = true;
                    }

                }
                else
                {
                    this.ImageSource = this.notSeatedImage;
                }
            }
            return gestureFound;
        }

        

        

        /// <summary>
        /// Notifies UI that a property has changed
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param> 
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        //Routing for gesture start
        /// <summary>
        /// Take in the current screen, filtered swipe up gesture
        /// </summary>
        /// <param name="state"></param>
        //private void swipeUp(string state)
        //{
        //    if (state.Equals("Home"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        Home.swipeUp();
        //    }
        //    else if (state.Equals("ItemChoice"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        ItemChoice.swipeUp();
        //    }
        //    else if (state.Equals("ItemParentChoice"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        ItemParentChoice.swipeUp();
        //    }
        //    else if (state.Equals("LoanSummary"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        LoanSummary.swipeUp();
        //    }
        //    else if (state.Equals("Timeslot"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        Timeslot.swipeUp();
        //    }
        //    else if (state.Equals("ViewLoans"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        ViewLoans.swipeUp();
        //    }
        //    else if (state.Equals("WhatToDo"))
        //    {
        //        //int milliseconds = 500;
        //        //Thread.Sleep(milliseconds);
        //        //Home.swipeUp();
        //    }
        //}

        ///// <summary>
        ///// Take in the current screen, filtered swipe right gesture
        ///// </summary>
        ///// <param name="state"></param>
        //private void swipeRight(string state)
        //{
        //    if (state.Equals("Home"))
        //    {
        //        //int milliseconds = 500;
        //        //Thread.Sleep(milliseconds);
        //        //Home.swipeUp();
        //    }
        //    else if (state.Equals("ItemChoice"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        ItemChoice.swipeRight();
        //    }
        //    else if (state.Equals("ItemParentChoice"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        ItemParentChoice.swipeRight();
        //    }
        //    else if (state.Equals("LoanSummary"))
        //    {
        //        //int milliseconds = 500;
        //        //Thread.Sleep(milliseconds);
        //        //LoanSummary.swipeUp();
        //    }
        //    else if (state.Equals("Timeslot"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        Timeslot.swipeRight();
        //    }
        //    else if (state.Equals("ViewLoans"))
        //    {
        //        //int milliseconds = 500;
        //        //Thread.Sleep(milliseconds);
        //        //ViewLoans.swipeUp();
        //    }
        //    else if (state.Equals("WhatToDo"))
        //    {
        //        //int milliseconds = 500;
        //        //Thread.Sleep(milliseconds);
        //        //Home.swipeUp();
        //    }
        //}

        ///// <summary>
        ///// Take in the current screen, filtered swipe right gesture
        ///// </summary>
        ///// <param name="state"></param>
        //private void swipeLeft(string state)
        //{
        //    if (state.Equals("Home"))
        //    {
        //        //int milliseconds = 500;
        //        //Thread.Sleep(milliseconds);
        //        //Home.swipeUp();
        //    }
        //    else if (state.Equals("ItemChoice"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        ItemChoice.swipeLeft();
        //    }
        //    else if (state.Equals("ItemParentChoice"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        ItemParentChoice.swipeLeft();
        //    }
        //    else if (state.Equals("LoanSummary"))
        //    {
        //        //int milliseconds = 500;
        //        //Thread.Sleep(milliseconds);
        //        //LoanSummary.swipeUp();
        //    }
        //    else if (state.Equals("Timeslot"))
        //    {
        //        int milliseconds = 500;
        //        Thread.Sleep(milliseconds);
        //        Timeslot.swipeLeft();
        //    }
        //    else if (state.Equals("ViewLoans"))
        //    {
        //        //int milliseconds = 500;
        //        //Thread.Sleep(milliseconds);
        //        //ViewLoans.swipeUp();
        //    }
        //    else if (state.Equals("WhatToDo"))
        //    {
        //        //int milliseconds = 500;
        //        //Thread.Sleep(milliseconds);
        //        //Home.swipeUp();
        //    }
        //}





        //routing for gesture end



    }
}
