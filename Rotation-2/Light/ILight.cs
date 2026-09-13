namespace Rotation.Light;

public interface ILight: ITransform {
	Color Color { get; set; }
	float Strength { get; set; }
	bool CalcColor(Triangle pTriangle, Vector pPos, out Color color);
}