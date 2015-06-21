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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace GazeMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CalibrationGUI : Window
    {

        private Point currentTargetNormalized;
        private Storyboard storyboard;

        public delegate void TargetSetEventHandler(object sender, EventArgs e);
        public delegate void CalibrationCaneledEventHandler(object sender, EventArgs e);

        public event TargetSetEventHandler TargetSet;
        public event CalibrationCaneledEventHandler CalibrationCanceled;

        public CalibrationGUI()
        {
            bloatedSize = 25;
            shrinkedSize = 5;

            bloatDuration = TimeSpan.FromSeconds(0.5);
            transitionSpeed = 1000;
            shrinkDuration = TimeSpan.FromSeconds(0.5);

            InitializeComponent();
        }

        public void SetTarget(Point targetNormalized)
        {
            currentTargetNormalized = targetNormalized;
            Bloat();
        }

        private Point Denormalize(Point pointNormalized)
        {
            return new Point(canvas.ActualWidth * pointNormalized.X, canvas.ActualHeight * pointNormalized.Y);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            this.Close();
            if (CalibrationCanceled != null)
                CalibrationCanceled(this, e);
        }

        private void Target_Shrinked(object sender, EventArgs e)
        {
            if (TargetSet != null)
                TargetSet(this, e);
        }

        private void Target_InPlace(object sender, EventArgs e)
        {
            Shrink();
        }

        private void Target_Bloated(object sender, EventArgs e)
        {
            GoToPoint();
        }

        private double GetDistance(Point p, Point q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }

        private void GoToPoint()
        {
            Point targetPoint = Denormalize(currentTargetNormalized);
            double seconds = GetDistance(circleGeometry.Center, targetPoint) / transitionSpeed;
            TimeSpan transitionDuration = TimeSpan.FromSeconds(seconds);
            AnimateTransition(targetPoint, transitionDuration, Target_InPlace);
        }

        private void Bloat()
        {
            AnimateSize(bloatedSize, bloatDuration, new EventHandler(Target_Bloated));
        }

        private void Shrink()
        {
            AnimateSize(shrinkedSize, shrinkDuration, new EventHandler(Target_Shrinked));
        }

        private void AnimateTransition(Point point, TimeSpan duration, EventHandler completed)
        {
            PointAnimation pointAnimation = new PointAnimation();
            pointAnimation.From = circleGeometry.Center;
            pointAnimation.To = point;  
            pointAnimation.Duration = duration;

            storyboard = new Storyboard();
            storyboard.Children.Add(pointAnimation);

            Storyboard.SetTargetName(pointAnimation, "circleGeometry");
            Storyboard.SetTargetProperty(pointAnimation, new PropertyPath(EllipseGeometry.CenterProperty));

            storyboard.Completed += completed;
            storyboard.Begin(this);
        }

        private void AnimateSize(double targetSize, TimeSpan duration, EventHandler completed)
        {
            DoubleAnimation sizeAnimationX = new DoubleAnimation();
            sizeAnimationX.From = circleGeometry.RadiusX;
            sizeAnimationX.To = targetSize;
            sizeAnimationX.Duration = duration;

            DoubleAnimation sizeAnimationY = new DoubleAnimation();
            sizeAnimationY.From = circleGeometry.RadiusY;
            sizeAnimationY.To = targetSize;
            sizeAnimationY.Duration = duration;

            storyboard = new Storyboard();
            storyboard.Children.Add(sizeAnimationX);
            storyboard.Children.Add(sizeAnimationY);

            Storyboard.SetTargetName(sizeAnimationX, "circleGeometry");
            Storyboard.SetTargetName(sizeAnimationY, "circleGeometry");

            Storyboard.SetTargetProperty(sizeAnimationX, new PropertyPath(EllipseGeometry.RadiusXProperty));
            Storyboard.SetTargetProperty(sizeAnimationY, new PropertyPath(EllipseGeometry.RadiusYProperty));

            storyboard.Completed += completed;
            storyboard.Begin(this);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            circleGeometry.Center = new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public double transitionSpeed { get; set; }
        public TimeSpan shrinkDuration { get; set; }
        public TimeSpan bloatDuration { get; set; }

        public double shrinkedSize { get; set; }
        public double bloatedSize { get; set; }
    }
}
