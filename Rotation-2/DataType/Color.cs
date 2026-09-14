using System.Globalization;

namespace Rotation;

public struct Color(float pR = 1, float pG = 1, float pB = 1) {
	public float R { get; set; } = pR;
	public float G { get; set; } = pG;
	public float B { get; set; } = pB;

	public byte ByteR => (byte)(Math.Clamp(R, 0, 1) * 255);
	public byte ByteG => (byte)(Math.Clamp(G, 0, 1) * 255);
	public byte ByteB => (byte)(Math.Clamp(B, 0, 1) * 255);

	public override int GetHashCode() {
		return HashCode.Combine(R, G, B);
	}

	public bool Equals(Color pRhs) {
		var diff = MathF.Abs(R - pRhs.R) 
		           + MathF.Abs(G - pRhs.G) 
		           + MathF.Abs(B - pRhs.B);
		return diff <= 1e-5;		
	}
	
	public override bool Equals( object? pObj) {
		return pObj is Color rhs && Equals(rhs);
	}
	
	public Color(string pHex): this(
		int.Parse(pHex[0..2], NumberStyles.HexNumber) / 255f,
		int.Parse(pHex[2..4], NumberStyles.HexNumber) / 255f,
		int.Parse(pHex[4..6], NumberStyles.HexNumber) / 255f
	){}

	public static bool operator ==(Color pLhs, Color pRhs) =>
		pLhs.Equals(pRhs);

	public static bool operator !=(Color pLhs, Color pRhs) =>
		!(pLhs == pRhs);

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

	public Color Map(Func<float, float> pFunc) => new(pFunc(R), pFunc(G), pFunc(B));

	public Color Map(Func<float, float, float> pFunc, Color pRhs) => new(
		pFunc(R, pRhs.R),
		pFunc(G, pRhs.G),
		pFunc(B, pRhs.B)
	);
	public Color Screen(Color pRhs) => 1 - (1 - this) * (1 - pRhs);

	public Color Overlay(Color pRhs) {

		return Map(Impl, pRhs);
		float Impl(float pA, float pB) {
			if (pA < 0.5f) return 2 * pA * pB;
			return 1 - (1 - pA) * (1 - pB) * 2;
		}
	}
	 public Color HardLight(Color pRhs) {
		 return Map(Impl, pRhs);
		 float Impl(float pA, float pB) {
			 if (pB < 0.5f) return 2 * pA * pB;
			 return 1 - (1 - pA) * (1 - pB) * 2;
		 }
	}
	public Color SoftLight(Color pRhs) {
		return Map(Impl, pRhs);
		float Impl(float pA, float pB) {
			if (pB < 0.5f) return pA - (1 - 2 * pB) * pA * (1 - pA);
			return pA + (2 * pB - 1) * (MathF.Sqrt(pA) - pA);
		}
	}
}