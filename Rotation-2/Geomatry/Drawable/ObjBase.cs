namespace Roation;

public abstract class ObjBase: IDrawable {
	protected record struct TriangleIdx(int A, int B, int C);

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

	private bool _needUpdate = false; 
	
	protected readonly IReadOnlyList<Triangle> _triangles;
	protected readonly IList<Wrapper<Vector>> _vertices;
	protected abstract IEnumerable<Vector> _defaultVertices { get; }
	protected abstract IEnumerable<TriangleIdx> _triangleIndies { get; }
	protected ObjBase() {
		_vertices = _defaultVertices.Select(p => new Wrapper<Vector>(p))
			.ToList();
		
		_triangles = _triangleIndies.Select(idxs => new Triangle(
			_vertices[idxs.A],
			_vertices[idxs.B],
			_vertices[idxs.C])
		).ToList();
	}

	private void Refresh() {
		if (!_needUpdate) return;
		_needUpdate = false;
		foreach (var (p, v) in _vertices.Zip(_defaultVertices)) {
			p.V = Rotation.Rotate(v) * Scale + Pos;
		}
	}

	public IEnumerable<Triangle> GetTriangles() {
		Refresh();
		return _triangles;
	}
	
	[Obsolete]
	public void Rotate(float pX, float pY, float pZ) {
		foreach (var (p, v) in _vertices.Zip(_defaultVertices)) {
			var t = v;
			t.EulerRotateX(pX);
			t.EulerRotateY(pY);
			t.EulerRotateZ(pZ);
			p.V = t * Scale + Pos;
		}	
	}
}