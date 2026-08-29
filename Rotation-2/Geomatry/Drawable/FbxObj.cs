using Assimp;

namespace Roation;

public class FbxObj: ObjBase {

    public FbxObj(string pPath): base(true) {
        var importer = new AssimpContext();
        var file = importer.ImportFile(pPath,
            PostProcessSteps.Triangulate 
            | PostProcessSteps.JoinIdenticalVertices
        );
        _dv = file.Meshes.SelectMany(mesh => mesh.Vertices.Select(v => new Vector(v.X, v.Y, v.Z)));
        var list = new List<TriangleIdx>();
        var num = 0;
        foreach (var mesh in file.Meshes) {
            var indices = mesh.GetIndices();
            if (indices.Length % 3 != 0) throw new ArgumentException("indices count is strange");
            for (int i = 0; i < (indices?.Length ?? 0) - 2; i+=3) {
                list.Add(new(num + indices[i], num + indices[i + 1], num + indices[i + 2]));   
            }
            
            num += mesh.VertexCount;
        }
        

        _dt = list;
        Initializer();
    }

    private readonly IEnumerable<Vector> _dv;
    private readonly IEnumerable<TriangleIdx> _dt;

    protected override IEnumerable<Vector> _defaultVertices => _dv;
    
    protected override IEnumerable<TriangleIdx> _triangleIndies => _dt;
}