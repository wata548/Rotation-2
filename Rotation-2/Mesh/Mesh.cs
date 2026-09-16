using Rotation.Ray;
using Rotation.UV;

namespace Rotation;

public interface IMesh {
    ITexture? Texture { get; }
    BVH BVH { get; }
    IReadOnlyList<Vector> Vertices { get; }
    IReadOnlyList<TriangleIdx> TriangleIndies { get; }
    bool NeedUpdate();
}

public class Mesh(IReadOnlyList<Vector> pVertices, List<TriangleIdx> pTriangleIndies, ITexture? pTexture = null): IMesh {
    private bool _needUpdate = false;
    public ITexture? Texture { get; } = pTexture;
    public IReadOnlyList<Vector> Vertices { get; } = pVertices;
    public IReadOnlyList<TriangleIdx> TriangleIndies => _triangleIdxes;
    private readonly List<TriangleIdx> _triangleIdxes = pTriangleIndies;
    public BVH BVH {
        get {
            if (field == null) {
                _needUpdate = true;
                field = new BVH(Vertices, _triangleIdxes, Texture);
            }
            return field;
        }
    }

    public bool NeedUpdate() {
        var result = _needUpdate;
        _needUpdate = false;
        return result;
    }
}
