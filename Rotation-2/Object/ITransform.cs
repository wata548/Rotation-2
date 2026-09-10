namespace Rotation;

public interface ITransform {
	Vector Pos { get; set; }
	Vector Scale { get; set; }
	Quaternion Rotation { get; set; }
}