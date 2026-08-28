namespace Roation;

public record Setting(
	Vector ScreenRange,
	Vector Origin,
	int CoordDetail = 4,
	int TriangleDetail = 100,
	float FOV = 95, 
	bool Isolate = false,
	bool FillContext = false
) {
	public readonly float Term = 1f / TriangleDetail;
	public readonly Vector ScreenSize = ScreenRange * CoordDetail;
	public readonly Vector OriginDelta = Origin - ScreenRange / 2;
	public readonly Vector ViewDirection = new(0, 0, -1);
	public readonly float CameraDistance = ScreenRange.X / 2f / MathF.Tan(FOV / 2 * MathF.PI / 180f);
	public readonly Vector CameraPos = new(Origin.X, Origin.Y, Origin.Z + 
		/*CameraDistance*/ScreenRange.X / 2f / MathF.Tan(FOV / 2 * MathF.PI / 180f));
}