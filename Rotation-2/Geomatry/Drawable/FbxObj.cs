using Assimp;

namespace Roation;

public class FbxObj: ObjBase {

    public FbxObj(string pPath): base(true) {
        var importer = new AssimpContext();
        var file = importer.ImportFile(pPath,
            PostProcessSteps.Triangulate 
            | PostProcessSteps.JoinIdenticalVertices
        );
        var vList = new List<Vector>();
        var tList = new List<TriangleIdx>();
        var num = 0;
        var stack = new Stack<(Node, Matrix4x4)>();
        stack.Push((file.RootNode, Matrix4x4.Identity));
        while (stack.Count > 0) {
	        var (node, transform) = stack.Pop();
	        var world = transform * node.Transform;
            if (node.HasMeshes) {
                var mesh = file.Meshes[node.MeshIndices[0]];
                var indices = mesh.GetIndices();
                if (indices.Length % 3 != 0) throw new ArgumentException("indices count is strange");
                vList.AddRange(mesh.Vertices.Select(v => {
                    var worldPos = world * v;
                    return new Vector(worldPos.X, worldPos.Y, worldPos.Z);
                }));
                for (int i = 0; i < (indices?.Length ?? 0) - 2; i+=3) {
                    tList.Add(new(num + indices[i], num + indices[i + 1], num + indices[i + 2]));   
                }
                num += mesh.VertexCount;
            }
	        if (node.HasChildren) {
		        foreach (var child in node.Children) {
			        stack.Push((child, transform));
		        }
	        }
        }

        _dv = vList;
        _dt = tList;
        Initializer();
    }

    private readonly IEnumerable<Vector> _dv;
    private readonly IEnumerable<TriangleIdx> _dt;

    protected override IEnumerable<Vector> _defaultVertices => _dv;
    
    protected override IEnumerable<TriangleIdx> _triangleIndies => _dt;
}