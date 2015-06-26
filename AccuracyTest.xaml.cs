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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Tobii.Gaze.Core;

namespace GazeMonitor
{
    public partial class AccuracyTest : Window
    {

        public AccuracyTest()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        internal void Start()
        {
            Show();
            DrawTargets();
        }

        private void DrawTargets()
        {
            double w = canvas.ActualWidth; double h = canvas.ActualHeight;

            double r = w / 30; double m = 2 * r;
            Brush b = Brushes.White;
            DrawCircle(new Point(m, m), r, false, b);
            DrawCircle(new Point(m, h / 2), r, false, b);
            DrawCircle(new Point(m, h - m), r, false, b);

            DrawCircle(new Point(w / 2, m), r, false, b);
            DrawCircle(new Point(w / 2, h / 2), r, false, b);
            DrawCircle(new Point(w / 2, h - m), r, false, b);

            DrawCircle(new Point(w - m, m), r, false, b);
            DrawCircle(new Point(w - m, h / 2), r, false, b);
            DrawCircle(new Point(w - m, h - m), r, false, b);

            r /= 10;
            DrawCircle(new Point(m, m), r, true, b);
            DrawCircle(new Point(m, h / 2), r, true, b);
            DrawCircle(new Point(m, h - m), r, true, b);

            DrawCircle(new Point(w / 2, m), r, true, b);
            DrawCircle(new Point(w / 2, h / 2), r, true, b);
            DrawCircle(new Point(w / 2, h - m), r, true, b);

            DrawCircle(new Point(w - m, m), r, true, b);
            DrawCircle(new Point(w - m, h / 2), r, true, b);
            DrawCircle(new Point(w - m, h - m), r, true, b);
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
                switch (data.TrackingStatus)
                {
                    case TrackingStatus.BothEyesTracked:
                        DisplayGazeLeft(data.Left);
                        DisplayGazeRight(data.Right);
                        break;
                    case TrackingStatus.OnlyLeftEyeTracked:
                    case TrackingStatus.OneEyeTrackedProbablyLeft:
                        DisplayGazeLeft(data.Left);
                        break;
                    case TrackingStatus.OnlyRightEyeTracked:
                    case TrackingStatus.OneEyeTrackedProbablyRight:
                        DisplayGazeRight(data.Right);
                        break;
                }
            }
        }

        private void DisplayGazeRight(GazeDataEye gazeDataEye)
        {
            Point p = new Point(gazeDataEye.GazePointOnDisplayNormalized.X, gazeDataEye.GazePointOnDisplayNormalized.Y);
            var circle = DrawCircle(Denormalize(p), 5, true, Brushes.Red);
            circle.Loaded += new RoutedEventHandler(CircleLoaded);
        }

        private void DisplayGazeLeft(GazeDataEye gazeDataEye)
        {
            Point p = new Point(gazeDataEye.GazePointOnDisplayNormalized.X, gazeDataEye.GazePointOnDisplayNormalized.Y);
            var circle = DrawCircle(Denormalize(p), 5, true, Brushes.Green);
            circle.Loaded += new RoutedEventHandler(CircleLoaded);
        }

        private void CircleLoaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeAnimation = new DoubleAnimation();
            fadeAnimation.From = 1.0;
            fadeAnimation.To = 0.0;
            fadeAnimation.Duration = TimeSpan.FromSeconds(.5);

            Storyboard fadeStoryboard = new Storyboard();
            fadeStoryboard.Children.Add(fadeAnimation);
            Storyboard.SetTarget(fadeAnimation, (Ellipse) sender);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(Ellipse.OpacityProperty));

            fadeStoryboard.Begin();
        }

        private Ellipse DrawCircle(Point center, double radius, bool filled, Brush brush)
        {
            Ellipse circle = new Ellipse() { Height = radius*2, Width = radius*2 };
            circle.Stroke = brush;
            if (filled)
                circle.Fill = brush;
            Canvas.SetLeft(circle, center.X - radius);
            Canvas.SetTop(circle, center.Y - radius);
            canvas.Children.Add(circle);
            return circle;
        }

        private Point Normalize(Point p)
        {
            return new Point(p.X / canvas.ActualWidth, p.Y / canvas.ActualHeight);
        }

        private Point Denormalize(Point p)
        {
            return new Point(p.X * canvas.ActualWidth, p.Y * canvas.ActualHeight);
        }

    }
}
