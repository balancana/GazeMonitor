using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Tobii.Gaze.Core;


namespace GazeMonitor
{

    public partial class CalibrationPlot : Window
    {
        byte[] calibrationData;

        public CalibrationPlot(byte[] calibrationData)
        {
            this.calibrationData = calibrationData;
            InitializeComponent();
        }

        private void DrawCircle(Point center, double radius, bool filled, Brush brush)
        {
            Ellipse circle = new Ellipse() { Height = radius * 2, Width = radius * 2 };
            circle.Stroke = brush;
            if (filled)
                circle.Fill = brush;
            Canvas.SetLeft(circle, center.X - radius);
            Canvas.SetTop(circle, center.Y - radius);
            canvas.Children.Add(circle);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var calibration = new Calibration(calibrationData);
            var points = calibration.GetCalibrationPointDataItems();

            foreach (var p in points)
            {
                DrawCircle(Denormalize(new Point(p.TruePosition.X, p.TruePosition.Y)), 10, false, Brushes.Black);
                if(p.LeftStatus == CalibrationPointStatus.CalibrationPointValidAndUsedInCalibration)
                    DrawCircle(Denormalize(new Point(p.LeftMapPosition.X, p.LeftMapPosition.Y)), 5, true, Brushes.Green);
                if (p.RightStatus == CalibrationPointStatus.CalibrationPointValidAndUsedInCalibration)
                    DrawCircle(Denormalize(new Point(p.RightMapPosition.X, p.RightMapPosition.Y)), 5, true, Brushes.Red);
            }
        }

        private Point Denormalize(Point p)
        {
            return new Point(p.X * canvas.ActualWidth, p.Y * canvas.ActualHeight);
        }
    }
}
