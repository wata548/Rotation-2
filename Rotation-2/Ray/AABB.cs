namespace Rotation.Ray;

public class AABB(Vector pMin, Vector pMax) {
	public Vector Min { get; } = pMin;
	public Vector Max { get; } = pMax;

	public bool IsIn(Ray pRay, ITransform pTransform) {
		return true;
	}
		
}