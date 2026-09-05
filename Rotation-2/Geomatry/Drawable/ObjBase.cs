using Microsoft.VisualBasic.CompilerServices;

namespace Rotation;

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

	private IReadOnlyList<Triangle>? Triangles { get; set; } = null;
	private IList<Wrapper<Vector>>? Vertices { get; set; } = null;
	protected abstract IEnumerable<Vector> _defaultVertices { get; }
	protected abstract IEnumerable<TriangleIdx> _triangleIndies { get; }
	private bool _initialized = false;

	protected void Initializer() {
		_initialized = true;
		Vertices = _defaultVertices.Select(p => new Wrapper<Vector>(p))
        			.ToList();
        		
		Triangles = _triangleIndies.Select(idxs => new Triangle(
			Vertices[idxs.A],
			Vertices[idxs.B],
			Vertices[idxs.C])
		).ToList();
	}
	protected ObjBase(bool pSkip = false) {
		if(!pSkip) Initializer();
	}

	private void Refresh() {
		if (!_initialized) throw new Exception("Need to call Initializer()");
		if (!_needUpdate) return;
		_needUpdate = false;
		foreach (var (p, v) in Vertices!.Zip(_defaultVertices)) {
			p.V = Rotation.Rotate(v) * Scale + Pos;
		}

		foreach (var triangle in Triangles!) {
			triangle.NormalRecalculate();
		}
	}

	public IEnumerable<Triangle> GetTriangles() {
		Refresh();
		return Triangles!;
	}

	[Obsolete]
	public void Rotate(Vector pD) => Rotate(pD.X, pD.Y, pD.Z);
	
	[Obsolete]
	public void Rotate(float pX, float pY, float pZ) {
		if (!_initialized) throw new Exception("Need to call Initializer()");
		foreach (var (p, v) in Vertices!.Zip(_defaultVertices)) {
			var t = v;
			t.EulerRotateX(pX);
			t.EulerRotateY(pY);
			t.EulerRotateZ(pZ);
			p.V = t * Scale + Pos;
		}	
	}
}