namespace Rotation.Light;

public class SpotLight: ILight {
	
	public Vector Pos { get; set; }
	public Vector Scale { get; set; }
	public Quaternion Rotation { get; set; }
	public Color Color { get; set; }
	public float Strength { get; set; }
	private float SqStrength => Strength * Strength;


	public bool CalcColor(Vector pNormal, Vector pPos, out Color color) {
		var diff = pPos - Pos;
		var strength = diff.SqDistance;
		if (strength > SqStrength) {
			color = default;
			return false;
		}
		strength = MathF.Sqrt(strength);
		var dot = -pNormal.Dot(diff.Normalized);
		var power = float.Max(dot * (1 - strength / Strength), 0);
		power = PostProcessing(power);
		
		color = Color * power;
		return true;
	}

	protected virtual float PostProcessing(float pV) => pV;
}