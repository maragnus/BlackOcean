using System.Numerics;

namespace BlackOcean.Simulation;

public static class Extensions
{
    public static JQuaternion ToJQuaternion(this Quaternion q) => new(q.X, q.Y, q.Z, q.W);
    public static JVector ToJVector(this Vector3 v) => new(v.X, v.Y, v.Z);

    public static void Transform(this IPhysics physics, JVector position, JQuaternion orientation)
    {
        physics.Position = position;
        physics.Orientation = orientation;
    }

    public static JQuaternion LookAt(JVector source, JVector target, JVector up)
    {
        // Calculate the forward direction from source to target
        var forward = target - source;
        forward.Normalize();

        // Calculate the right vector (orthogonal to up and forward)
        var right = JVector.Cross(up, forward);
        right.Normalize();

        // Recalculate the up vector to ensure orthogonality
        var adjustedUp = JVector.Cross(forward, right);

        // Build the rotation matrix
        var rotationMatrix = new JMatrix(
            right.X, right.Y, right.Z,
            adjustedUp.X, adjustedUp.Y, adjustedUp.Z,
            forward.X, forward.Y, forward.Z);

        // Convert the rotation matrix to a quaternion
        return JQuaternion.CreateFromMatrix(rotationMatrix);
    }
    
    public static void LookAt(this IPhysics physics, JVector target, JVector up) => 
        physics.Orientation = LookAt(physics.Position, target, up);
}