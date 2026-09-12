using Rotation.Ray;

namespace Rotation;


public interface IMesh {
    IReadOnlyList<Vector> Vertices { get; }
    IReadOnlyList<TriangleIdx> TriangleIndies { get; }
}

public class Mesh(IReadOnlyList<Vector> pVertices, List<TriangleIdx> pTriangleIndies): IMesh {
    public IReadOnlyList<Vector> Vertices { get; } = pVertices;
    public IReadOnlyList<TriangleIdx> TriangleIndies => _triangleIdxes;
    private readonly List<TriangleIdx> _triangleIdxes = pTriangleIndies;
    public BVH BVH => _bvh ??= new BVH(Vertices, _triangleIdxes);
    private BVH? _bvh = null;
}
