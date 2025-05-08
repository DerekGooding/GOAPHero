namespace GOAPHero.Core;
public readonly struct Vector3(float X, float Y, float Z)
{
    public float X { get; } = X;
    public float Y { get; } = Y;
    public float Z { get; } = Z;

    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3 operator *(Vector3 a, float scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);
    public static Vector3 operator /(Vector3 a, float scalar) => new(a.X / scalar, a.Y / scalar, a.Z / scalar);

        /// <summary>
    /// Calculates the distance between two points.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>The distance between the points.</returns>
    public static float Distance(Vector3 a, Vector3 b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return (float)Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
    }
}