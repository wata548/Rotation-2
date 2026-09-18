using Rotation.Ray;
using Rotation.UV;

namespace Rotation;

public interface IMesh {
    UVMap? UV { get; }
    Texture? Normal { get; }
    Texture? Texture { get; }
    BVH BVH { get; }
    IReadOnlyList<Vector> Vertices { get; }
    IReadOnlyList<TriangleIdx> TriangleIndies { get; }
    bool NeedUpdate();
}

public class Mesh(IReadOnlyList<Vector> pVertices, List<TriangleIdx> pTriangleIndies, UVMap? pUV = null, Texture? pTexture = null, Texture? pNormal = null): IMesh {
    
    private bool _needUpdate = false;
    public UVMap? UV { get; } = pUV;
    public Texture? Texture { get; } = pTexture;
    public Texture? Normal { get; } = pNormal;
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
