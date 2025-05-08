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
}