using Assimp;
using Rotation.UV;

namespace Rotation;

public class FbxLoader {
	public IMesh Load(string pPath, bool pLoadTexture = false) {
		var importer = new AssimpContext();
		var scene = importer.ImportFile(pPath,
			PostProcessSteps.Triangulate 
			| PostProcessSteps.JoinIdenticalVertices
		);
		var vList = new List<Vector>();
		var tList = new List<TriangleIdx>();
		var num = 0;
		var stack = new Stack<(Node, Matrix4x4)>();
		UVMap uv = new();
		Texture? texture = null;
		Texture? normal = null;
		stack.Push((scene.RootNode, Matrix4x4.Identity));
		var sum = 0;
		while (stack.Count > 0) {
			var (node, transform) = stack.Pop();
			var world = transform * node.Transform;
			if (node.HasMeshes) {
				var mesh = scene.Meshes[node.MeshIndices[0]];
				var mat = scene.Materials[mesh.MaterialIndex];

				if (pLoadTexture) {
					
					if (mat.GetMaterialTexture(TextureType.Normals, 0, out var normalSlot)) {
						normal = Load(pPath, scene, normalSlot, 200);
					}

					if (mat.GetMaterialTexture(TextureType.Diffuse, 0, out var diffuseSlot)) {
						texture = Load(pPath, scene, diffuseSlot, 200);
					}
					var coords = mesh.TextureCoordinateChannels[0]
						.Select(coord => new UVMap.UVCoord(coord.X, coord.Y));
					uv.AddCoords(coords);
				
				}
				var indices = mesh.GetIndices()!;
				if (indices.Length % 3 != 0) throw new ArgumentException("indices count is strange");
				vList.AddRange(mesh.Vertices.Select(v => {
					var worldPos = world * v;
					return new Vector(worldPos.X, worldPos.Y, worldPos.Z);
				}));
				for (int i = 0; i < indices.Length - 2; i+=3) {
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

		return new Mesh(vList, tList, uv, texture, normal);
	}

	private Texture Load(string pPath, Assimp.Scene pScene, TextureSlot pTex, int pDetail) {
		if(pTex.FilePath.StartsWith('*')) {
			var idx = int.Parse(pTex.FilePath[1..]);
			return new(pScene.Textures[idx].CompressedData, pDetail);
		}

		var path = Path.Combine(Path.GetDirectoryName(pPath) ?? ".", pTex.FilePath);
		return new(File.ReadAllBytes(path), pDetail);
	}
}