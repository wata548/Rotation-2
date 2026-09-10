namespace Rotation.Light;

public class SpotLight: ILight {
	
	public Vector Pos { get; set; }
	public Vector Scale { get; set; }
	public Quaternion Rotation { get; set; }
	public Color Color { get; set; }
	public float Strength { get; set; }
	private float SqStrength => Strength * Strength;


	public float GetPower(Triangle pTriangle, Vector pPos) {
		var diff = pPos - Pos;
		var strength = diff.SqDistance;
		if (strength > SqStrength) return 0;
		strength = MathF.Sqrt(strength);
		var dot = diff.Normalized.Dot(pTriangle.Normal);
		return dot * Ease(strength / Strength);
	}

	private float Ease(float pV) => 1 - MathF.Pow(1 - pV, 5);
}