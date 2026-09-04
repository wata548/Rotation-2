namespace Rotation;

public class Crystal: ObjBase {
	private static readonly IEnumerable<Vector> DefaultPoints = [
		new Vector(-0.5f, 0, -0.288f) * 0.612f,
		new Vector(0.5f, 0, -0.288f) * 0.612f,
		new Vector(0f, 0, 0.577f) * 0.612f,
		new Vector(0f, 0.816f, 0) * 0.612f,
		new Vector(0f, -0.816f, 0) * 0.612f,
	];
	private static readonly IEnumerable<TriangleIdx> TriangleIndies = [
		new(0,1,4),
		new(1,2,4),
		new(2,0,4),
		new(1,0,3),
		new(2,1,3),
		new(0,2,3),
	];

	protected override IEnumerable<Vector> _defaultVertices => DefaultPoints;
	protected override IEnumerable<TriangleIdx> _triangleIndies => TriangleIndies;
}