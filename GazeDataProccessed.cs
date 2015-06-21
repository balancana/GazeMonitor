using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.Gaze.Core;


namespace GazeMonitor
{
    class GazeDataProccessed
    {
        public double angleAtDistance;
        public double angleAtScreen;
        public String typeAtDistance;
        public String typeAtScreen;

        public GazeDataProccessed(GazeData gazeData)
        {
            if (gazeData.TrackingStatus == TrackingStatus.BothEyesTracked)
            {
                Vector3D gazeVectorRight = new Vector3D(gazeData.Right.EyePositionFromEyeTrackerMM, gazeData.Right.GazePointFromEyeTrackerMM);
                Vector3D gazeVectorLeft = new Vector3D(gazeData.Left.EyePositionFromEyeTrackerMM, gazeData.Left.GazePointFromEyeTrackerMM);

                angleAtDistance = Vector3D.Angle(gazeVectorLeft, gazeVectorRight);
                angleAtDistance = 180 * angleAtDistance / Math.PI;

                Vector3D eyeAxis = new Vector3D(gazeData.Left.EyePositionFromEyeTrackerMM, gazeData.Right.EyePositionFromEyeTrackerMM);

                if (Vector3D.Angle(eyeAxis, gazeVectorRight) > Vector3D.Angle(eyeAxis, gazeVectorLeft))
                    typeAtDistance = "eso";
                else if (Vector3D.Angle(eyeAxis, gazeVectorRight) < Vector3D.Angle(eyeAxis, gazeVectorLeft))
                    typeAtDistance = "exo";

                Vector3D gazeVectorLeftIdeal = new Vector3D(gazeData.Left.EyePositionFromEyeTrackerMM, gazeData.Right.GazePointFromEyeTrackerMM);
                angleAtScreen = Vector3D.Angle(gazeVectorLeft, gazeVectorLeftIdeal);
                angleAtScreen = 180 * angleAtScreen / Math.PI;

                if (Vector3D.Angle(eyeAxis, gazeVectorLeftIdeal) > Vector3D.Angle(eyeAxis, gazeVectorLeft))
                    typeAtScreen = "eso";
                else if (Vector3D.Angle(eyeAxis, gazeVectorRight) < Vector3D.Angle(eyeAxis, gazeVectorLeft))
                    typeAtScreen = "exo";
            }
        }
    }
}
