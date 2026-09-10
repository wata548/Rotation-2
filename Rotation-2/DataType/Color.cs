namespace Rotation;

public struct Color(float pR = 1, float pG = 1, float pB = 1) {
	public float R { get; set; } = pR;
	public float G { get; set; } = pG;
	public float B { get; set; } = pB;

	public int ByteR => (int)(Math.Clamp(R, 0, 1) * 255);
	public int ByteG => (int)(Math.Clamp(G, 0, 1) * 255);
	public int ByteB => (int)(Math.Clamp(B, 0, 1) * 255);
	public static bool operator ==(Color pLhs, Color pRhs) {
		var diff = MathF.Abs(pLhs.R - pRhs.R) +
			MathF.Abs(pLhs.G - pRhs.G) +
			MathF.Abs(pLhs.B - pRhs.B);
		return diff <= 1e-5;
	}

	public static bool operator !=(Color pLhs, Color pRhs) {
		return !(pLhs == pRhs);
	}

	public static Color operator +(Color pLhs, Color pRhs) {
		return new(
			pLhs.R + pRhs.R,
			pLhs.G + pRhs.G,
			pLhs.B + pRhs.B
		);
	}
	public static Color operator +(float pLhs, Color pRhs) {
		return new(
			pLhs + pRhs.R, 
			pLhs + pRhs.G,
			pLhs + pRhs.B
		);
	}
	public static Color operator +(Color pLhs, float pRhs) {
		return new(
			pLhs.R + pRhs, 
			pLhs.G + pRhs,
			pLhs.B + pRhs
		);
	}
	public static Color operator -(Color pLhs, Color pRhs) {
		return new(
			pLhs.R - pRhs.R,
			pLhs.G - pRhs.G,
			pLhs.B - pRhs.B
		);
	}
	public static Color operator -(float pLhs, Color pRhs) {
		return new(
			pLhs - pRhs.R, 
			pLhs - pRhs.G,
			pLhs - pRhs.B
		);
	}
	public static Color operator -(Color pLhs, float pRhs) {
		return new(
			pLhs.R - pRhs, 
			pLhs.G - pRhs,
			pLhs.B - pRhs
		);
	}
		
	public static Color operator *(Color pLhs, float pRhs) {
		return new(
			pLhs.R * pRhs,
			pLhs.G * pRhs,
			pLhs.B * pRhs
		);
	}
	public static Color operator *(Color pLhs, Color pRhs) {
		return new(
			pLhs.R * pRhs.R,
			pLhs.G * pRhs.G,
			pLhs.B * pRhs.B
		);
	}
}