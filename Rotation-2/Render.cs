using System.Text;

namespace Roation;

public class Render {
	private const int _colorCnt = 255;
	private float[]? _zBuffer = null;
	private float[]? _faceNormal = null;
	
	public string Show(Setting pSetting, params IEnumerable<IDrawable> pObjs) {
		var size = (int)(pSetting.ScreenSize.X * pSetting.ScreenSize.Y);
		_zBuffer ??= new float[size];
		_faceNormal ??= new float[size];
		Array.Fill(_zBuffer, 0);
		Array.Fill(_faceNormal, 0);
		var skipped = 0;
		foreach (var obj in pObjs) {
			foreach (var triangle in obj.GetTriangles()) {
				if (!pSetting.DoubleFace && !triangle.IsVisible(pSetting)) {
					skipped++;
					continue;
				}

				var darkness = triangle.Normal.Normalized.Dot(pSetting.ViewDirection);
				Fill(triangle, 0, 1, darkness);
				Fill(triangle, 1, 0, darkness);
				for (float i = 0; i < 1; i += pSetting.Term) {
					for (float j = 0; j < 1; j += pSetting.Term) {
						if (i + j > 1) break;
						Fill(triangle, i,j, darkness);
					}
				}	
			}
		}

		return GetResult();	

		string GetResult() {
			var prev = 0f;
			var result = new StringBuilder();
			result.Append("|");
			for (int i = 0; i < size; i++) {
				
				if (i != 0 && i % pSetting.ScreenSize.X == 0) {
					prev = 0;
					result.Append($"\x1b[48;2;0;0;0m\x1b[38;2;255;255;255m|\n|");
					if (pSetting.FillContext) result.Append("\x1b[38;2;0;0;0m");
				}
				var value = 0f;
				
				if (_faceNormal[i] < 0) {
					if (!pSetting.ZBufferShading) {
						value = -_faceNormal[i] * (1 - pSetting.Fog / _zBuffer[i]);
						value = Math.Clamp(value, 0, 1);
					}
					else
						value = Math.Clamp(_zBuffer[i], 0, 1);
				}
				
				if (Math.Abs(value - prev) > 1e-5f) {
					var r = (int)(pSetting.Color.R * value);
					var g = (int)(pSetting.Color.G * value);
					var b = (int)(pSetting.Color.B * value);
					result.Append($"\x1b[48;2;{r};{g};{b}m");
					if (pSetting.FillContext) result.Append($"\x1b[38;2;{r};{g};{b}m");
					prev = value;
				}

				result.Append(pSetting.FillContext
					? value == 0 ? "  " : $"{(int)(100 * value):d02}"
					: "  ");
			}
			result.Append("\n\x1b[38;2;255;255;255m");
			result.AppendLine($"Skipped triangle cnt: {skipped}");
			return result.ToString();
		}
		void Fill(Triangle pTriangle, float pU, float pV, float pDarkness) {
			var point = pTriangle.GetPoint(pU, pV);
			point.Y *= -1;
			var z = -point.Z;
			
			if (!pSetting.Isolate) {
				var ratio = pSetting.CameraDistance / (pSetting.CameraDistance - point.Z);
				point *= ratio;	
			}
			
			point = ((point - pSetting.OriginDelta) * pSetting.CoordDetail).Map(MathF.Round);
			if (point.X < 0 || point.X >= pSetting.ScreenSize.X
				|| point.Y < 0 || point.Y >= pSetting.ScreenSize.Y)
				return;
			var coord = (int)point.X + (int)(pSetting.ScreenSize.X * point.Y);
			var zInv = 1f / (z + float.Epsilon);
			if (_zBuffer[coord] > zInv) return;
			_zBuffer[coord] = zInv;
			_faceNormal[coord] = pDarkness;
		}
	}
}