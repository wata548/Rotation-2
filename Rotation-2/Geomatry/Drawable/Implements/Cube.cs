using System.Collections;

namespace Rotation;

public class Cube: ObjBase {
	private static readonly IEnumerable<Vector> DefaultPoints = [
		new(-0.5f, -0.5f, -0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(-0.5f, 0.5f, -0.5f),
		new(0.5f, 0.5f, -0.5f),
		new(-0.5f, -0.5f, 0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(-0.5f, 0.5f, 0.5f),
		new(0.5f, 0.5f, 0.5f),
	];

	private static readonly IEnumerable<TriangleIdx> TriangleIndies = [
		new (0, 2, 1),
		new (2, 3, 1),
		new (0, 1, 4),
		new (1, 5, 4),
		new (2, 6, 3),
		new (3, 6, 7),
		new (1, 3, 5),
		new (3, 7, 5),
		new (0, 4, 2),
		new (2, 4, 6),
		new (4, 5, 6),
		new (5, 7, 6),
	];

	protected override IEnumerable<Vector> _defaultVertices => DefaultPoints;
	protected override IEnumerable<TriangleIdx> _triangleIndies => TriangleIndies;
}