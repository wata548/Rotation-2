namespace Rotation.Ray;

public struct AABB {
	public Vector Min { get; private set; }
	public Vector Max { get; private set; }

	public AABB(Vector pMin, Vector pMax) {
		Min = pMin;
		Max = pMax;
	}

	public AABB() {
		Min = Vector.One * float.MaxValue;
		Max = Vector.One * float.MinValue;
	}
	
	public AABB(IReadOnlyList<Vector> pVertices, TriangleIdx pIdx) {
		var a = pVertices[pIdx.A];
		var b = pVertices[pIdx.B];
		var c = pVertices[pIdx.C];
		Min = new(
			ExNumber.Min(a.X, b.X, c.X),
			ExNumber.Min(a.Y, b.Y, c.Y),
			ExNumber.Min(a.Z, b.Z, c.Z)
		);
		Max = new(
			ExNumber.Max(a.X, b.X, c.X),
			ExNumber.Max(a.Y, b.Y, c.Y),
			ExNumber.Max(a.Z, b.Z, c.Z)
		);
	}

	public AABB(AABB pLhs, AABB pRhs) {
		Min = new(
			float.Min(pLhs.Min.X, pRhs.Min.X),
			float.Min(pLhs.Min.Y, pRhs.Min.Y),
			float.Min(pLhs.Min.Z, pRhs.Min.Z)
		);
		Max = new(
			float.Max(pLhs.Max.X, pRhs.Max.X),
			float.Max(pLhs.Max.Y, pRhs.Max.Y),
			float.Max(pLhs.Max.Z, pRhs.Max.Z)
		);
	}
	
	public void Expand(IReadOnlyList<Vector> pVertices, TriangleIdx pIdx) {
			var a = pVertices[pIdx.A];
    		var b = pVertices[pIdx.B];
    		var c = pVertices[pIdx.C];
    		Min = new(
    			ExNumber.Min(Min.X , a.X, b.X, c.X),
    			ExNumber.Min(Min.Y, a.Y, b.Y, c.Y),
    			ExNumber.Min(Min.Z, a.Z, b.Z, c.Z)
    		);
    		Max = new(
    			ExNumber.Max(Max.X, a.X, b.X, c.X),
    			ExNumber.Max(Max.Y, a.Y, b.Y, c.Y),
    			ExNumber.Max(Max.Z, a.Z, b.Z, c.Z)
    		);	
	}

	public float SurfaceArea() {
		var lenght = Max - Min;
		return 2 * (lenght.X * lenght.Y + lenght.Y * lenght.Z + lenght.X * lenght.Z);
	}
}