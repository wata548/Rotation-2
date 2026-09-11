namespace Rotation;

public partial class Render {
	private record PointInfo (
		float ZInv, float U, float V, Triangle? Triangle
	){}
}
