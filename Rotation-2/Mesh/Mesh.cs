namespace Rotation;
public record Mesh(IReadOnlyList<Vector> Vertices, IEnumerable<TriangleIdx> TriangleIndies) { }