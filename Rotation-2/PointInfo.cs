namespace Rotation;

public partial class Render {
	private record PointInfo (
		Object? Object, float ZInv, float U, float V, Triangle? Triangle
	){}
}
