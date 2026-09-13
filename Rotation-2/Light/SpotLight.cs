namespace Rotation.Light;

public class SpotLight: ILight {
	
	public Vector Pos { get; set; }
	public Vector Scale { get; set; }
	public Quaternion Rotation { get; set; }
	public Color Color { get; set; }
	public float Strength { get; set; }
	private float SqStrength => Strength * Strength;


	public bool CalcColor(Triangle pTriangle, Vector pPos, out Color color) {
		var diff = pPos - Pos;
		var strength = diff.SqDistance;
		if (strength > SqStrength) {
			color = default;
			return false;
		}
		strength = MathF.Sqrt(strength);
		var dot = -pTriangle.Normal.Dot(diff.Normalized);
		var power = float.Max(dot * Ease(1 - strength / Strength), 0);
		color = (Color * power).Map(f => Math.Clamp(f, 0, 1));
		return true;
	}

	private float Ease(float pV) => pV; //MathF.Sin(MathF.PI / 2 * pV);
}