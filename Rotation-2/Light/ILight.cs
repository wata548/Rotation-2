namespace Rotation.Light;

public interface ILight: ITransform {
	Color Color { get; set; }
	Color CalcColor(Triangle pTriangle, Vector pPos);
}