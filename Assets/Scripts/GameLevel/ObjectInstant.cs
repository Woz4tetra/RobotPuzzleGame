using Unity.VisualScripting;
using UnityEngine;
public class ObjectInstant
{
    public Matrix4x4 pose;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public ObjectInstant()
    {
        pose = Matrix4x4.identity;
        velocity = Vector3.zero;
        angularVelocity = Vector3.zero;
    }

    public ObjectInstant(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
    {
        pose = Matrix4x4.TRS(position, rotation, Vector3.one);
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;
    }

    /**
        interpolation is 0..1
        instant1 is at time 0
        instant2 is at time 1
    */
    public static ObjectInstant Slerp(ObjectInstant instant1, ObjectInstant instant2, float interpolation)
    {
        return new ObjectInstant
        {
            pose = Matrix4x4.TRS(
                PositionSlerp(instant1, instant2, interpolation),
                QuaternionSlerp(instant1, instant2, interpolation),
                Vector3.one
            ),
            velocity = VelocitySlerp(instant1, instant2, interpolation),
            angularVelocity = AngularVelocitySlerp(instant1, instant2, interpolation)
        };
    }

    private static Vector3 PositionSlerp(ObjectInstant instant1, ObjectInstant instant2, float interpolation)
    {
        Vector3 pose1 = instant1.pose.GetT();
        Vector3 pose2 = instant2.pose.GetT();
        return Vector3.Slerp(pose1, pose2, interpolation);
    }

    private static Quaternion QuaternionSlerp(ObjectInstant instant1, ObjectInstant instant2, float interpolation)
    {
        Quaternion quat1 = instant1.pose.GetR();
        Quaternion quat2 = instant2.pose.GetR();
        return Quaternion.Slerp(quat1, quat2, interpolation);
    }

    private static Vector3 VelocitySlerp(ObjectInstant instant1, ObjectInstant instant2, float interpolation)
    {
        Vector3 vel1 = instant1.velocity;
        Vector3 vel2 = instant2.velocity;
        return Vector3.Slerp(vel1, vel2, interpolation);
    }

    private static Vector3 AngularVelocitySlerp(ObjectInstant instant1, ObjectInstant instant2, float interpolation)
    {
        Vector3 angVel1 = instant1.angularVelocity;
        Vector3 angVel2 = instant2.angularVelocity;
        return Vector3.Slerp(angVel1, angVel2, interpolation);
    }

}
