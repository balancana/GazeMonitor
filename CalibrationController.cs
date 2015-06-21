using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tobii.Gaze.Core;
using System.Diagnostics;


namespace GazeMonitor
{
    struct CalibrationPoints
    {
        public List<Point> Left;
        public List<Point> Right;
    }

    class CalibrationController
    {
        private CalibrationGUI gui;

        CalibrationPoints calibrationPoints;
        List<Point>.Enumerator currentPointLeft;
        List<Point>.Enumerator currentPointRight;

        private TrackerController trackerController;
        private MainWindow mainWindow;

        public CalibrationController(MainWindow mainWindow, TrackerController trackerController)
        {
            this.trackerController = trackerController;
            this.mainWindow = mainWindow;

            calibrationPoints.Left = new List<Point>();
            calibrationPoints.Left.Add(new Point(0.1, 0.1));
            calibrationPoints.Left.Add(new Point(0.1, 0.5));
            calibrationPoints.Left.Add(new Point(0.1, 0.9));
            calibrationPoints.Left.Add(new Point(0.5, 0.1));
            calibrationPoints.Left.Add(new Point(0.5, 0.9));
            calibrationPoints.Left.Add(new Point(0.9, 0.1));
            calibrationPoints.Left.Add(new Point(0.9, 0.5));
            calibrationPoints.Left.Add(new Point(0.9, 0.9));
            calibrationPoints.Left.Add(new Point(0.5, 0.5));

            calibrationPoints.Right = new List<Point>();
            calibrationPoints.Right.Add(new Point(0.1, 0.1));
            calibrationPoints.Right.Add(new Point(0.1, 0.5));
            calibrationPoints.Right.Add(new Point(0.1, 0.9));
            calibrationPoints.Right.Add(new Point(0.5, 0.1));
            calibrationPoints.Right.Add(new Point(0.5, 0.5));
            calibrationPoints.Right.Add(new Point(0.5, 0.9));
            calibrationPoints.Right.Add(new Point(0.9, 0.1));
            calibrationPoints.Right.Add(new Point(0.9, 0.5)); 
            calibrationPoints.Right.Add(new Point(0.9, 0.9));

            currentPointLeft = calibrationPoints.Left.GetEnumerator();
            currentPointRight = calibrationPoints.Right.GetEnumerator();

            gui = new CalibrationGUI();
            gui.CalibrationCanceled += this.CalibrationCanceledEventHandler;

            gui.Show();

            trackerController.tracker.StartCalibrationAsync(CalibrationStarted);
        }

        private void LeftTargetReadyEventHandler(object sender, EventArgs e)
        {
            trackerController.tracker.AddCalibrationPointAsync(new Point2D(currentPointLeft.Current.X, currentPointLeft.Current.Y), PointAddedLeft);
        }

        private void RightTargetReadyEventHandler(object sender, EventArgs e)
        {
            trackerController.tracker.AddCalibrationPointAsync(new Point2D(currentPointRight.Current.X, currentPointRight.Current.Y), PointAddedRight);
        }

        private void CalibrationStarted(ErrorCode errorCode)
        {
            Debug.WriteLine("Calibration Started: " + errorCode.ToString());
            InvokeInGUI(new Action(delegate() { StartLeft(); }));
        }

        private void StartLeft()
        {
            gui.ShowMessage("Cover your right eye and follow the white circle!");

            gui.TargetSet += this.LeftTargetReadyEventHandler;

            if (currentPointLeft.MoveNext())
                gui.SetTarget(currentPointLeft.Current);
        }

        private void NextLeft()
        {
            if (currentPointLeft.MoveNext())
                gui.SetTarget(currentPointLeft.Current);
            else
                StartRight();
        }

        private void StartRight()
        {
            gui.TargetSet -= this.LeftTargetReadyEventHandler;
            gui.ShowMessage("Cover your left eye and follow the white circle!");
            
            gui.TargetSet += this.RightTargetReadyEventHandler;

            if (currentPointRight.MoveNext())
                gui.SetTarget(currentPointRight.Current);
        }

        private void NextRight()
        {
            if (currentPointRight.MoveNext())
                gui.SetTarget(currentPointRight.Current);
            else
                Compute();
        }

        private void Compute()
        {
            trackerController.tracker.ComputeAndSetCalibrationAsync(CalibrationComputed);
        }

        private void CalibrationComputed(ErrorCode errorCode)
        {
            Debug.WriteLine("Calibration Compute: " + errorCode.ToString());
            trackerController.tracker.StopCalibrationAsync(CalibrationStopped);
        }


        private void PointAddedLeft(ErrorCode errorCode)
        {
            Debug.WriteLine("Point Add (Left): " + errorCode.ToString());
            InvokeInGUI(new Action(delegate() { NextLeft(); }));
        }

        private void PointAddedRight(ErrorCode errorCode)
        {
            Debug.WriteLine("Point Add (Right): " + errorCode.ToString());
            InvokeInGUI(new Action(delegate() { NextRight(); }));
        }

        private void InvokeInGUI(Action action)
        {
            gui.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, action);
        }

        public void CalibrationCanceledEventHandler(object sender, EventArgs e)
        {
            trackerController.tracker.StopCalibrationAsync(CalibrationStopped);
        }

        private void CalibrationStopped(ErrorCode errorCode)
        {
            Debug.WriteLine("Calibration Stop: " + errorCode.ToString());
            InvokeInGUI(new Action(delegate() { gui.Close(); }));
        }
    }
}
