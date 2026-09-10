namespace Rotation;

public class Object: IDrawable, ITransform {
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

	public Mesh? Mesh {
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
		_triangles = Mesh.TriangleIndies.Select(idxs => new Triangle(
			_vertices[idxs.A],
			_vertices[idxs.B],
			_vertices[idxs.C])
		).ToList();
		
	}
	
	private void VertexPositionUpdate() {
		if (Mesh == null) return;
		if (!_needUpdate) return;
		
		_needUpdate = false;
		
		foreach (var (p, v) in _vertices.Zip(Mesh.Vertices)) {
			p.V = Rotation.Rotate(v) * Scale + Pos;
		}

		foreach (var triangle in _triangles) {
			triangle.NormalRecalculate();
		}
	}
}