namespace Rotation;

public record Setting(
	Vector ScreenRange,
	Color DefaultColor,
	float Frame = 20,
	int CoordDetail = 4,
	float Fog = 0.05f,
	bool Isolate = false,
	bool Ascii = false,
	bool ZBufferShading = false,
	bool ApplyNormalMap = false,
	bool CastShadow = false,
	LightProcessType LightProcessType = LightProcessType.Screen
) {
	public readonly Vector ScreenSize = ScreenRange * CoordDetail;
	public required Camera Camera { get; set; }
	public readonly Vector ScreenDelta = ScreenRange / 2;
	public Vector ViewDirection => Camera.Rotation.Rotate(Vector.Forward) * CameraDistance;
	public float CameraDistance => ScreenRange.X / 2f / MathF.Tan(Camera.FOV / 2 * MathF.PI / 180f);
}

public enum LightProcessType {
	Screen, HardLight, SoftLight, Overlay
}