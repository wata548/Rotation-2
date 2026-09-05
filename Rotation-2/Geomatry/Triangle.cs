
namespace Rotation;

public class Triangle: IDrawable {
	public Triangle(params Wrapper<Vector>[] pVertices) {
		if (pVertices.Length < 3) throw new ArgumentOutOfRangeException("pVertices count must over 3");
		A = pVertices[0];
		B = pVertices[1];
		C = pVertices[2];
		NormalRecalculate();
	}
	
	public Wrapper<Vector> A { get; set; }
	public Wrapper<Vector> B { get; set; }
	public Wrapper<Vector> C { get; set; }
	public Vector Normal { get; set; }
	public Vector U => B.V - A.V;
	public Vector V => C.V - A.V;
	public Vector Middle => (A.V + B.V + C.V) / 3f;

	public void NormalRecalculate() => 
		Normal = U.Cross(V).Normalized;
	
	public float Brightness(Setting pSetting) =>
		-Normal.Dot(pSetting.Isolate
			? pSetting.ViewDirection
			: (Middle - pSetting.CameraPos).Normalized
		);
	
	public Vector GetPoint(float pU, float pV) =>
		(1 - pU - pV) * A.V + pU * B.V + pV * C.V;

	public IEnumerable<Triangle> GetTriangles() => [this];

	/*
	public void RotateX(float pDelta) {
		var middle = Middle;
		A.V.RotateX(middle, pDelta);
		B.V.RotateX(middle, pDelta);
		C.V.RotateX(middle, pDelta);
	}
	public void RotateY(float pDelta) {
		var middle = Middle;
		A.V.RotateY(middle, pDelta);
		B.V.RotateY(middle, pDelta);
		C.V.RotateY(middle, pDelta);
	}
	public void RotateZ(float pDelta) {
		var middle = Middle;
		A.V.RotateZ(middle, pDelta);
		B.V.RotateZ(middle, pDelta);
		C.V.RotateZ(middle, pDelta);
	}

	public void Rotate(float pX, float pY, float pZ) {
		RotateX(pX);
		RotateY(pY);
		RotateZ(pZ);
	}*/
}