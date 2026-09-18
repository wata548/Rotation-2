namespace Rotation.UV;

public class Normal {
	private Texture _tex;

	public Vector GetVector(TriangleIdx pTriangleIdx, Object pObj, Vector pNormal) {
		var a = pObj.GetVertex(pTriangleIdx.A);
		var ba = pObj.GetVertex(pTriangleIdx.B) - a;
		var ca = pObj.GetVertex(pTriangleIdx.C) - a;
		var ua = pObj.Mesh!.UV!.GetVertex(pTriangleIdx.A);
		var uba = pObj.Mesh!.UV!.GetVertex(pTriangleIdx.B) - ua;
		var uca = pObj.Mesh!.UV!.GetVertex(pTriangleIdx.C) - ua;
		var det = uba.X * uca.Y - uba.Y * uca.X;
		var t = (ba * uca.X - ca * uba.X) / det;
		var b = (ba * uca.Y - ca * uba.Y) / det;
		return default;
	} 
	
}