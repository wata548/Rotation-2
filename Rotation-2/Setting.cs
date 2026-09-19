namespace Rotation;

public record Setting(
	Vector ScreenRange,
	Vector Origin,
	Color DefaultColor,
	float Frame = 20,
	int CoordDetail = 4,
	float FOV = 95,
	float Fog = 0.05f,
	bool Isolate = false,
	bool Ascii = false,
	bool ZBufferShading = false,
	bool ApplyNormalMap = false,
	bool CastShadow = false,
	LightProcessType LightProcessType = LightProcessType.Screen
) {
	public readonly Vector ScreenSize = ScreenRange * CoordDetail;
	public readonly Vector OriginDelta = Origin - ScreenRange / 2;
	public readonly Vector ViewDirection = new(0, 0, -1);
	public readonly float CameraDistance = ScreenRange.X / 2f / MathF.Tan(FOV / 2 * MathF.PI / 180f);
	public readonly Vector CameraPos = new(Origin.X, Origin.Y, Origin.Z + 
		/*CameraDistance*/ScreenRange.X / 2f / MathF.Tan(FOV / 2 * MathF.PI / 180f));
}

public enum LightProcessType {
	Screen, HardLight, SoftLight, Overlay
}