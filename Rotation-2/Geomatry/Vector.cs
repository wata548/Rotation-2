namespace Roation;

public struct Vector(float pX = 0,float pY = 0,float pZ = 0) {
	public float X { get; set; } = pX;
	public float Y { get; set; } = pY;
	public float Z { get; set; } = pZ;

	public override string ToString() => $"({X}, {Y}, {Z})";

	public static readonly Vector Zero = new(0, 0, 0);
	public static readonly Vector One = new(1, 1, 1);
	public static readonly Vector Left = new(-1, 0, 0);
	public static readonly Vector Right = new(1, 0, 0);
	public static readonly Vector Up = new(0, 1, 0);
	public static readonly Vector Down = new(0, -1, 0);
	public static readonly Vector Backward = new(0, 0, 1);
	public static readonly Vector Forward = new(0, 0, -1);
	
#region Operators

	public readonly float SqrDistance => X * X + Y * Y + Z * Z;
	public readonly float Distance => MathF.Sqrt(SqrDistance);
	public readonly Vector Normalized {
		get {
			var dis = Distance;
			return new Vector(X, Y, Z) / dis;
		}
	}
	public readonly float Dot(Vector pOther) => X * pOther.X + Y * pOther.Y + Z * pOther.Z;
	public readonly Vector Cross(Vector pOther) => new(
		Y * pOther.Z - Z * pOther.Y,
		Z * pOther.X - X * pOther.Z,
		X * pOther.Y - Y * pOther.X
	);
	
	public void EulerRotateX(float pDelta) {
		var rad = pDelta * MathF.PI / 180f;
		var (cos, sin) = (MathF.Cos(rad), MathF.Sin(rad));
		(Y, Z) = (cos * Y - sin * Z, cos * Z + sin * Y);
	}
	public void EulerRotateZ(float pDelta) {
		var rad = pDelta * MathF.PI / 180f;
		var (cos, sin) = (MathF.Cos(rad), MathF.Sin(rad));
		(X, Y) = (cos * X - sin * Y, cos * Y + sin * X);
	}
	public void EulerRotateY(float pDelta) {
		var rad = pDelta * MathF.PI / 180f;
		var (cos, sin) = (MathF.Cos(rad), MathF.Sin(rad));
		(X, Z) = (cos * X - sin * Z, cos * Z + sin * X);
	}
	public void Mapped(Func<float, float> pFunc) => (X, Y, Z) = (pFunc(X), pFunc(Y), pFunc(Z));
	public readonly Vector Map(Func<float, float> pFunc) => new(pFunc(X), pFunc(Y), pFunc(Z));
	public void SwapXY() => (X, Y) = (Y, X);
	public void SwapXZ() => (X, Z) = (Z, X);
	public void SwapYZ() => (Y, Z) = (Z, Y);
	public Vector SwapedXY() => new(Y, X, Z);
	public Vector SwapedXZ() => new(Z, Y, X);
	public Vector SwapedYZ() => new(X, Z, Y);
	public static Vector operator -(Vector lhs) => new(-lhs.X, -lhs.Y, -lhs.Z);
	public static Vector operator +(Vector lhs, Vector rhs) =>
		new(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
	public static Vector operator -(Vector lhs, Vector rhs) =>
		new(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
	public static Vector operator *(Vector lhs, Vector rhs) =>
		new(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z);
	public static Vector operator /(Vector lhs, Vector rhs) =>
		new(lhs.X / rhs.X, lhs.Y / rhs.Y, lhs.Z / rhs.Z);
	public static Vector operator +(float lhs, Vector rhs) =>
		new(lhs + rhs.X, lhs + rhs.Y, lhs + rhs.Z);
	public static Vector operator +(Vector lhs, float rhs) =>
		new(lhs.X + rhs, lhs.Y + rhs, lhs.Z + rhs);
	public static Vector operator -(float lhs, Vector rhs) =>
		new(lhs - rhs.X, lhs - rhs.Y, lhs - rhs.Z);
	public static Vector operator -(Vector lhs, float rhs) =>
		new(lhs.X - rhs, lhs.Y - rhs, lhs.Z - rhs);
	public static Vector operator *(float lhs, Vector rhs) =>
		new(lhs * rhs.X, lhs * rhs.Y, lhs * rhs.Z);
	public static Vector operator *(Vector lhs, float rhs) =>
		new(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
	public static Vector operator /(float lhs, Vector rhs) =>
		new(lhs / rhs.X, lhs / rhs.Y, lhs / rhs.Z);
	public static Vector operator /(Vector lhs, float rhs) =>
		new(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);

	public static bool operator ==(Vector lhs, Vector rhs) =>
		(lhs.X, lhs.Y, lhs.Z) == (rhs.X, rhs.Y, rhs.Z);

	public static bool operator !=(Vector lhs, Vector rhs) => !(lhs == rhs);

#endregion
}