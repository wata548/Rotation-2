using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rotation.Input;

public static class Device {
	[DllImport("Dlls/Input.dll")]
	public static extern bool KeyDown(int c);
	
	[DllImport("Dlls/Input.dll")]
	private static extern float GetMouseX();
	[DllImport("Dlls/Input.dll")]
	private static extern float GetMouseY();

	public static Vector MousePos =>
		new(GetMouseX(), GetMouseY());

	public static Vector MouseDelta {
		get {
			UpdateMouseDelta();
			return _mouseDelta;
		}
	}
	private static Vector _prevMousePos;
	private static Vector _mouseDelta;

	private static void UpdateMouseDelta() {
		_mouseDelta = MousePos - _prevMousePos;
		_prevMousePos = MousePos;
	}

	public static Quaternion SimpleRotate(float pCoefficient, Quaternion pQ) {
		var delta = MouseDelta;
		var dr = Quaternion.Euler(-pCoefficient * delta.SwapedXY());
		return dr * pQ;
	}
	
	public static Vector SimpleMove() {
		var result = Vector.Zero;
		if (KeyDown('W')) result += Vector.Forward;
		if (KeyDown('S')) result += Vector.Backward;
		if (KeyDown('A')) result += Vector.Left;
		if (KeyDown('D')) result += Vector.Right;
		if (KeyDown('Q')) result += Vector.Up;
		if (KeyDown('E')) result += Vector.Down;
		return result;
	}
}