namespace Rotation.UV;

public class UVMap {
	public record struct UVCoord(float U, float V) {
		public static UVCoord operator +(UVCoord pLhs, UVCoord pRhs) =>
			new(pLhs.U + pRhs.U, pLhs.V + pRhs.V);
		public static UVCoord operator -(UVCoord pLhs, UVCoord pRhs) =>
			new(pLhs.U - pRhs.U, pLhs.V - pRhs.V);
		public static UVCoord operator *(float pLhs, UVCoord pRhs) =>
			new(pLhs * pRhs.U, pLhs * pRhs.V);
	}

	public void AddCoords(IEnumerable<UVCoord> pCoords) =>
		_coords.AddRange(pCoords);

	public void SetDets(IReadOnlyList<TriangleIdx> pTriangleIndices) {
		_dets = pTriangleIndices.Select(
			triIdx => {
				var u = _coords[triIdx.B] - _coords[triIdx.A];
				var v = _coords[triIdx.C] - _coords[triIdx.A];
				return u.U * v.V - u.V * v.U;
			}).ToList();
	}

	public UVCoord GetVertex(int pIdx) => _coords[pIdx];
	
	public UVCoord Get(TriangleIdx pIdx, float pU, float pV) {
		var a = _coords[pIdx.A];
		var b = _coords[pIdx.B];
		var c = _coords[pIdx.C];
		var coord = pU * (b - a) + pV * (c - a);
		var x = coord.U % 1;
		if (x < 0) x += 1;
		var y = coord.V % 1;
		if (y < 0) y += 1;
		return new(x,y);
	}
	
	private readonly List<UVCoord> _coords = new();
	private List<float> _dets = new();
}