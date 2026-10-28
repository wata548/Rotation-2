namespace Rotation;

public class Camera: ITransform {
	public Vector Pos { get; set; } = default;
	public Vector Scale { get; set; } = default;
	public Quaternion Rotation { get; set; } = new();
	public float FOV { get; set; } = 95;
}