namespace Rotation;

public class D20: ObjBase {
    private const float A = 0.26286556f;
    private const float B = 0.42532540f;
    
    private static readonly IEnumerable<Vector> DefaultPoints = [
        new(-A,  B,  0f),
        new( A,  B,  0f),
        new(-A, -B,  0f),
        new( A, -B,  0f),
        new( 0f, -A,  B),
        new( 0f,  A,  B),
        new( 0f, -A, -B),
        new( 0f,  A, -B),
        new( B,  0f, -A),
        new( B,  0f,  A),
        new(-B,  0f, -A),
        new(-B,  0f,  A),
    ];
    
    private static readonly IEnumerable<TriangleIdx> TriangleIndies = [
        // top fan around vertex 0
        new (0, 11, 5),
        new (0, 5, 1),
        new (0, 1, 7),
        new (0, 7, 10),
        new (0, 10, 11),
        // upper band
        new (1, 5, 9),
        new (5, 11, 4),
        new (11, 10, 2),
        new (10, 7, 6),
        new (7, 1, 8),
        // bottom fan around vertex 3
        new (3, 9, 4),
        new (3, 4, 2),
        new (3, 2, 6),
        new (3, 6, 8),
        new (3, 8, 9),
        // lower band
        new (4, 9, 5),
        new (2, 4, 11),
        new (6, 2, 10),
        new (8, 6, 7),
        new (9, 8, 1),
    ];

    protected override IEnumerable<Vector> _defaultVertices => DefaultPoints;
    protected override IEnumerable<TriangleIdx> _triangleIndies => TriangleIndies;
}