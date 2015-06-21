using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Vector3D
{
    private double x;
    private double y;
    private double z;

    public Vector3D(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3D(Tobii.Gaze.Core.Point3D point3D1, Tobii.Gaze.Core.Point3D point3D2)
    {
        this.x = point3D2.X - point3D1.X;
        this.y = point3D2.Y - point3D1.Y;
        this.z = point3D2.Z - point3D1.Z;
    }

    public static double Angle(Vector3D vector1, Vector3D vector2)
    {
        double dotProduct = DotProduct(vector1, vector2);
        double normx = vector1.Norm * vector2.Norm;
        return Math.Acos(dotProduct / normx);
    }

    public static double DotProduct(Vector3D vector1, Vector3D vector2)
    {
        return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z;
    }

    public static Vector3D operator +(Vector3D v1, Vector3D v2)
    {
        return new Vector3D(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static Vector3D operator *(double scalar, Vector3D v)
    {
        return new Vector3D(scalar * v.x, scalar * v.y, scalar * v.z);
    }

    public double Norm
    {
        get { return Math.Sqrt(NormSquared); }
    }

    public double NormSquared
    {
        get { return x * x + y * y + z * z; }
    }

    public Tobii.Gaze.Core.Point3D ToPoint3D
    {
        get { return new Tobii.Gaze.Core.Point3D(x, y, z); }
    }
}

