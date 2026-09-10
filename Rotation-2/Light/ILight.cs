namespace Rotation.Light;

public interface ILight: ITransform {
	Color Color { get; set; }
	float GetPower(Triangle pTriangle, Vector pPos);
}