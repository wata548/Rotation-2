namespace Rotation.Light;

public class SpotLight: ILight {
	
	public Vector Pos { get; set; }
	public Vector Scale { get; set; }
	public Quaternion Rotation { get; set; }
	public Color Color { get; set; }
	public float Strength { get; set; }
	private float SqStrength => Strength * Strength;


	public Color CalcColor(Triangle pTriangle, Vector pPos) {
		var diff = pPos - Pos;
		var strength = diff.SqDistance;
		if (strength > SqStrength) return new Color(0,0,0);
		strength = MathF.Sqrt(strength);
		var dot = -pTriangle.Normal.Dot(diff.Normalized);
		return Color * (dot * Ease(1 - strength / Strength));
	}

	private float Ease(float pV) => pV; //MathF.Sin(MathF.PI / 2 * pV);
}