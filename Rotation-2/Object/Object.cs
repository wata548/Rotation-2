using Rotation.Ray;

namespace Rotation;

public class Object: ITransform {
	//==================================================Properties	
	public Vector Pos {
		get;
		set {
			field = value;
			_needUpdate = true;
		}
	} = default;

	public Vector Scale {
		get;
		set {
			field = value;
			_needUpdate = true;
		}
	} = Vector.One;

	public Quaternion Rotation {
		get;
		set {
			field = value;
			_needUpdate = true;
		}
	} = new();

	public IMesh? Mesh {
		get;
		set {
			field = value;
			OnChangeMesh();
			_needUpdate = true;
			
		}
	} = null;

	public IEnumerable<Triangle> Triangles {
		get {
			VertexPositionUpdate();
			return _triangles;
		}
	}
	//==================================================Fields	
	private bool _needUpdate = false;

	private IEnumerable<Triangle> _triangles = [];
	private readonly List<Wrapper<Vector>> _vertices = [];
	
	
	//==================================================Methods
	public Vector GetVertex(int pIdx) => _vertices[pIdx].V; 
	
	private void OnChangeMesh() {
		if (Mesh == null) return;
		var comp = -1;
		while (comp != 0) {
			comp = _vertices.Count.CompareTo(Mesh.Vertices.Count);
			switch (comp) {
				case 0: break;
				case 1: _vertices.RemoveAt(_vertices.Count - 1);
					break;
				case -1: _vertices.Add(new(new()));
					break;
			}
		}
		MeshUpdate();
	}
	private void MeshUpdate() {
		if (Mesh == null) return;
		Mesh.UV?.SetDets(Mesh.TriangleIndies);	
		_triangles = Mesh.TriangleIndies.Select((idxs, idx) => new Triangle(
			idx, 
			[
				_vertices[idxs.A],
				_vertices[idxs.B],
				_vertices[idxs.C]
			])
		).ToList();
	}
	
	private void VertexPositionUpdate() {
		if (Mesh == null) return;
		if (!_needUpdate) return;
		
		_needUpdate = false;
		
		foreach (var (p, v) in _vertices.Zip(Mesh.Vertices)) {
			p.V = this.ApplyTransform(v);
		}

		foreach (var triangle in _triangles) {
			triangle.NormalUpdate();
		}
	}

	public RayResult RayCasting(Ray.Ray pRay) {
		if (Mesh == null) return new(null, 2);
		var localRay = pRay.ApplyTransform(this);
		var stack = new Stack<int>();
		var result = new RayResult(default, 2);
		stack.Push(0);
		while (stack.Count > 0) {
			var node = Mesh.BVH.Hierarchy[stack.Pop()];
			if(!localRay.VsAABB(node.AABB, result.Ratio)) 
				continue;
    
			if (!node.IsLeaf) {
				stack.Push(node.LeftIdx);
				stack.Push(node.RightIdx);
				continue;
			}
    
			for (int i = 0; i < node.TriangleCnt; i++) {
				var triangle = Mesh.TriangleIndies[node.TriangleIdx + i];
				var vsTriangle = localRay.VsTriangle(
					Mesh.Vertices,
					triangle,
					out var t
				);
				if (vsTriangle && t < result.Ratio) {
					result = new(triangle, t);
				}
			}
		}
		return result;
	}
	
	public void Update() {
		if(Mesh?.NeedUpdate() ?? false) 
			MeshUpdate();
	}
}