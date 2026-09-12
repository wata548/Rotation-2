using Rotation.Ray;

namespace Rotation;


public interface IMesh {
    IReadOnlyList<Vector> Vertices { get; }
    IEnumerable<TriangleIdx> TriangleIndies { get; }
}

public class Mesh(IReadOnlyList<Vector> pVertices, List<TriangleIdx> pTriangleIndies): IMesh {
    public IReadOnlyList<Vector> Vertices { get; } = pVertices;
    public IEnumerable<TriangleIdx> TriangleIndies { get; } = pTriangleIndies;
}
public class CollisionMesh: IMesh {
    public IReadOnlyList<Vector> Vertices { get; }
    public IEnumerable<TriangleIdx> TriangleIndies { get; }
    public readonly BVH BVH;

    public CollisionMesh(IReadOnlyList<Vector> pVertices, List<TriangleIdx> pTriangleIndies) {
        BVH = new(pVertices, pTriangleIndies);
        Vertices = pVertices;
        TriangleIndies = pTriangleIndies;
    }
} 
