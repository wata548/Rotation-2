namespace Roation;

public class TriangleHone: ObjBase {
	private static readonly IEnumerable<Vector> DefaultPoints = [
		new(-0.5f, -0.5f, -0.5f),
		new(-0.5f, -0.5f, 0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(0, 0.5f, 0),
	];
	private static readonly IEnumerable<TriangleIdx> TriangleIndies = [
		new(0,2,1),
		new(1,2,3),
		new(0,1,4),
		new(1,3,4),
		new(3,2,4),
		new(2,0,4),
	];

	protected override IEnumerable<Vector> _defaultVertices => DefaultPoints;
	protected override IEnumerable<TriangleIdx> _triangleIndies => TriangleIndies;
}