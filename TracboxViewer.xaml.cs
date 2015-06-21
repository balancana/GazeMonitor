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
using System.Windows.Media.Animation;
using Tobii.Gaze.Core;


namespace GazeMonitor
{

    public partial class TracboxViewer : Window
    {
        private Ellipse lastDrawedRight;
        private Ellipse lastDrawedLeft;
        private double rectangleLeft;
        private double rectangleTop;
        private double rectangleSize;

        public TracboxViewer()
        {
            InitializeComponent();
        }

        private void DrawTrackbox()
        {
            rectangleSize = canvas.ActualHeight / 2;
            Rectangle trackbox = new Rectangle() { Height = rectangleSize , Width = rectangleSize };
            trackbox.Stroke = Brushes.White;
            rectangleLeft = (canvas.ActualWidth - rectangleSize) / 2;
            rectangleTop = (canvas.ActualHeight - rectangleSize) / 2;
            Canvas.SetLeft(trackbox, rectangleLeft);
            Canvas.SetTop(trackbox, rectangleTop);
            canvas.Children.Add(trackbox);

            DrawCircle(new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2), 25, false, Brushes.White);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }

        internal void Start()
        {
            Show();
            DrawTrackbox();
        }

        internal void EyeTrackerGazeData(object sender, GazeDataEventArgs e)
        {
            Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() { DisplayGaze(e.GazeData); }));
        }

        internal void EyeTrackerError(object sender, EyeTrackerErrorEventArgs e)
        {
            Close();
        }

        private void DisplayGaze(GazeData data)
        {
            if (this.IsLoaded)
            {
                if (lastDrawedLeft != null)
                    canvas.Children.Remove(lastDrawedLeft);
                if (lastDrawedRight != null)
                    canvas.Children.Remove(lastDrawedRight);
                switch (data.TrackingStatus)
                {
                    case TrackingStatus.BothEyesTracked:
                        DisplayEyeLeft(data.Left);
                        DisplayEyeRight(data.Right);
                        break;
                    case TrackingStatus.OnlyLeftEyeTracked:
                    case TrackingStatus.OneEyeTrackedProbablyLeft:
                        DisplayEyeLeft(data.Left);
                        break;
                    case TrackingStatus.OnlyRightEyeTracked:
                    case TrackingStatus.OneEyeTrackedProbablyRight:
                        DisplayEyeRight(data.Right);
                        break;
                }
            }
        }

        private void DisplayEyeRight(GazeDataEye gazeDataEye)
        {
            Point p = new Point(1 - gazeDataEye.EyePositionInTrackBoxNormalized.X, gazeDataEye.EyePositionInTrackBoxNormalized.Y);
            lastDrawedRight = DrawCircle(Denormalize(p), (1 - gazeDataEye.EyePositionInTrackBoxNormalized.Z) * 50, true, Brushes.Red);
        }

        private void DisplayEyeLeft(GazeDataEye gazeDataEye)
        {
            Point p = new Point(1 - gazeDataEye.EyePositionInTrackBoxNormalized.X, gazeDataEye.EyePositionInTrackBoxNormalized.Y);
            lastDrawedLeft = DrawCircle(Denormalize(p), (1 - gazeDataEye.EyePositionInTrackBoxNormalized.Z) * 50, true, Brushes.Green);
        }


        private Ellipse DrawCircle(Point center, double radius, bool filled, Brush brush)
        {
            if (radius > 0)
            {
                Ellipse circle = new Ellipse() { Height = radius * 2, Width = radius * 2 };
                circle.Stroke = brush;
                if (filled)
                    circle.Fill = brush;
                Canvas.SetLeft(circle, center.X - radius);
                Canvas.SetTop(circle, center.Y - radius);
                canvas.Children.Add(circle);
                return circle;
            }
            return null;
        }

        private Point Denormalize(Point p)
        {
            return new Point(p.X * rectangleSize + rectangleLeft, p.Y * rectangleSize + rectangleTop);
        }
    }
}
