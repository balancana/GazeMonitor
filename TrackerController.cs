using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.Gaze.Core;
using System.Diagnostics;
using System.Threading;


namespace GazeMonitor
{
    class TrackerController
    {
        public IEyeTracker tracker;
        private Thread eventLoop;
        public bool trackingStarted = false;

        public void Initialize() 
        {
            Uri url = new EyeTrackerCoreLibrary().GetConnectedEyeTracker();

            if (url == null)
            {
                throw new ApplicationException("No eye tracker found, check cable!");
            }
            else
            {
                tracker = new EyeTracker(url);
                eventLoop = CreateAndRunEventLoopThread(tracker);
                tracker.Connect();
            }
        }

        private Thread CreateAndRunEventLoopThread(IEyeTracker tracker)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    tracker.RunEventLoop();
                }
                catch (EyeTrackerException ex)
                {
                    Debug.WriteLine("An error occurred in the eye tracker event loop: " + ex.Message);
                }

                Debug.WriteLine("Leaving the event loop.");
            });

            thread.Start();
            return thread;
        }

        public void Dispose()
        {
            if (tracker != null)
            {
                tracker.Disconnect();
                if (eventLoop != null)
                {
                    tracker.BreakEventLoop();
                    eventLoop.Join();
                }
                tracker.Dispose();
            }
        }

        public void StopTracking()
        {
            try
            {
                tracker.StopTrackingAsync(TrackingStopped);
            }
            catch (EyeTrackerException e)
            {
                Debug.WriteLine(e.ErrorCode);
            }      
        }

        private void TrackingStopped(ErrorCode errorCode)
        {
            Debug.WriteLine("Tracking Stopped: " + errorCode.ToString());
            trackingStarted = false;
        }

        public void StartTracking()
        {
            tracker.StartTrackingAsync(TrackingStarted);
        }

        private void TrackingStarted(ErrorCode errorCode)
        {
            Debug.WriteLine("Tracking Started: " + errorCode.ToString());
            trackingStarted = true;
        }

        public void SetCalibration(byte[] data)
        {
            Calibration calibration = new Calibration(data);
            tracker.SetCalibrationAsync(calibration, CalibrationSet);
        }

        private void CalibrationSet(ErrorCode errorCode)
        {
            Debug.WriteLine("Calibration Set: " + errorCode.ToString());
        }

        public byte[] GetCalibration()
        {
            Calibration calibration = tracker.GetCalibration();
            return calibration.GetData();
        }
    }
}
