using Rotation.Ray;
using Rotation.UV;

namespace Rotation;

public interface IMesh {
    ITexture? Texture { get; }
    BVH BVH { get; }
    IReadOnlyList<Vector> Vertices { get; }
    IReadOnlyList<TriangleIdx> TriangleIndies { get; }
}

public class Mesh(IReadOnlyList<Vector> pVertices, List<TriangleIdx> pTriangleIndies, ITexture? pTexture = null): IMesh {
    public ITexture? Texture { get; } = pTexture;
    public IReadOnlyList<Vector> Vertices { get; } = pVertices;
    public IReadOnlyList<TriangleIdx> TriangleIndies => _triangleIdxes;
    private readonly List<TriangleIdx> _triangleIdxes = pTriangleIndies;
    public BVH BVH => _bvh ??= new BVH(Vertices, _triangleIdxes);
    private BVH? _bvh = null;
}
