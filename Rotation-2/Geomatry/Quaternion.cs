namespace Roation;

public struct Quaternion(float pW, Vector pV) {
	public float W { get; private set; } = pW;
	public Vector V { get; private set; } = pV;

	public Quaternion() : this(1, Vector.Zero){}
	
	public static Quaternion GenerateWithAxisAndAngle(float pTheta, Vector pAxis, bool pNormalized = false) {
		var half = pTheta * MathF.PI / 360f;
		if (!pNormalized) pAxis = pAxis.Normalized;
		return new(MathF.Cos(half), MathF.Sin(half) * pAxis);
	}

	public static Quaternion Euler(float pX, float pY, float pZ) =>
		GenerateWithAxisAndAngle(pZ, Vector.Forward)
		* GenerateWithAxisAndAngle(pY, Vector.Up)
		* GenerateWithAxisAndAngle(pX, Vector.Right);

	public Vector Rotate(Vector pP) {
		var temp = this * pP;
		var temp2 = Flip();
		var temp3 = temp * temp2; 
		return (this * pP * Flip()).V;
	} 
	
	public static Quaternion operator *(Quaternion pLhs, Quaternion pRhs) =>
		new(pLhs.W * pRhs.W - pLhs.V.Dot(pRhs.V),
			pLhs.V * pRhs.W + pLhs.W * pRhs.V + pLhs.V.Cross(pRhs.V)
		);

	public static Quaternion operator *(Quaternion pLhs, Vector pRhs) =>
		new(-pLhs.V.Dot(pRhs),
			pLhs.W * pRhs + pLhs.V.Cross(pRhs)
		);

	public Quaternion Flip() => new(W, -V);
}