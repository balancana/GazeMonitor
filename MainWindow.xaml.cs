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
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace GazeMonitor
{

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private TrackerController trackerController;
        private AccuracyTest accuracyTest;
        private TracboxViewer tracboxViewer;

        public MainWindow()
        {
            trackerController = new TrackerController();
            InitializeComponent();
        }


        private void New_Calibration(object sender, RoutedEventArgs e)
        {
            new CalibrationController(this, trackerController);
        }

        private void Open_Calibration(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog filePicker = new Microsoft.Win32.OpenFileDialog();

            filePicker.DefaultExt = ".clb";
            filePicker.Filter = "Calibration Files (*.clb)|*.clb";
            var result = filePicker.ShowDialog();

            if (result == true)
            {
                byte[] data = File.ReadAllBytes(filePicker.FileName);
                trackerController.SetCalibration(data);
            }
        }

        private void Save_Calibration(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog filePicker = new Microsoft.Win32.SaveFileDialog();

            filePicker.DefaultExt = ".clb";
            filePicker.Filter = "Calibration Files (*.clb)|*.clb";
            var result = filePicker.ShowDialog();

            if (result == true)
            {
                byte[] data = trackerController.GetCalibration();
                File.WriteAllBytes(filePicker.FileName, data);
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AccuracyTest(object sender, RoutedEventArgs e)
        {
            accuracyTest = new AccuracyTest();
            trackerController.tracker.GazeData += accuracyTest.EyeTrackerGazeData;
            trackerController.tracker.EyeTrackerError += accuracyTest.EyeTrackerError;

            accuracyTest.Closed += accuracyTest_Closed;
            accuracyTest.Start();
        }

        private void TracboxViewer(object sender, RoutedEventArgs e)
        {
            tracboxViewer = new TracboxViewer();
            trackerController.tracker.GazeData += tracboxViewer.EyeTrackerGazeData;
            trackerController.tracker.EyeTrackerError += tracboxViewer.EyeTrackerError;

            tracboxViewer.Closed += tracboxViewer_Closed;
            tracboxViewer.Start();
        }

        private void tracboxViewer_Closed(object sender, EventArgs e)
        {
            trackerController.tracker.GazeData -= tracboxViewer.EyeTrackerGazeData;
            trackerController.tracker.EyeTrackerError -= tracboxViewer.EyeTrackerError;
        }

        private void accuracyTest_Closed(object sender, EventArgs e)
        {
            trackerController.tracker.GazeData -= accuracyTest.EyeTrackerGazeData;
            trackerController.tracker.EyeTrackerError -= accuracyTest.EyeTrackerError;
        }

        private void CalibrationPlot(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog filePicker = new Microsoft.Win32.OpenFileDialog();

            filePicker.DefaultExt = ".clb";
            filePicker.Filter = "Calibration Files (*.clb)|*.clb";
            var result = filePicker.ShowDialog();

            if (result == true)
            {
                byte[] data = File.ReadAllBytes(filePicker.FileName);
                new CalibrationPlot(data).Show();
            }
        }


        private void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                trackerController.Initialize();
                trackerController.tracker.EyeTrackerError += this.EyeTrackerError;
                trackerController.tracker.GazeData += this.GazeData;
            }
            catch(Exception ex)
            {
                ShowErrorAndQuit(ex.Message);
            }
        }

        private void GazeData(object sender, Tobii.Gaze.Core.GazeDataEventArgs e)
        {
            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() { ProcessData(e.GazeData); }));
        }

        private void ProcessData(Tobii.Gaze.Core.GazeData gazeData)
        {
            GazeDataProccessed gazeDataProccessed = new GazeDataProccessed(gazeData);

            if (gazeData.TrackingStatus == Tobii.Gaze.Core.TrackingStatus.BothEyesTracked)
            {
                this.angleAtDistance.Content = Decimal.Round((decimal) gazeDataProccessed.angleAtDistance, 1, MidpointRounding.AwayFromZero) + "° " + gazeDataProccessed.typeAtDistance;
                this.angleAtScreen.Content = Decimal.Round((decimal)gazeDataProccessed.angleAtScreen, 1, MidpointRounding.AwayFromZero) + "° " + gazeDataProccessed.typeAtScreen;
            }
        }

        private void EyeTrackerError(object sender, Tobii.Gaze.Core.EyeTrackerErrorEventArgs e)
        {
            Debug.WriteLine(e.ErrorCode); 
        }

        private void ShowErrorAndQuit(String error)
        {
            string caption = "Error";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Error;
            MessageBox.Show(error, caption, button, icon);
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            trackerController.Dispose();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (trackerController.trackingStarted) 
            {
                trackerController.StopTracking();
                Start_Button.Content = "Start";
            }
            else
            {
                trackerController.StartTracking();
                Start_Button.Content = "Stop";
            }
        }

    }
}
