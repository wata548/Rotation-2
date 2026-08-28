using System.Text;

namespace Roation;

public class Render {
	private const int _startColor = 233;
	private const int _endColor = 255;
	private const int _colorCnt = _endColor - _startColor;
	
	public string Show(Setting pSetting, params IEnumerable<IDrawable> pObjs) {
		var size = (int)(pSetting.ScreenSize.X * pSetting.ScreenSize.Y);
		var buffer = new float[size];
		var skipped = 0;
		foreach (var obj in pObjs) {
			foreach (var triangle in obj.GetTriangles()) {
				if (!triangle.IsVisible(pSetting)) {
					skipped++;
					continue;
				}
				
				Fill(triangle, 0, 1);
				Fill(triangle, 1, 0);
				for (float i = 0; i < 1; i += pSetting.Term) {
					for (float j = 0; j < 1; j += pSetting.Term) {
						if (i + j > 1) break;
						Fill(triangle, i,j);
					}
				}	
			}
		}

		return GetResult();	

		string GetResult() {
			var prev = -1;
			var result = new StringBuilder();
			result.Append("|");
			for (int i = 0; i < size; i++) {
				if (i != 0 && i % pSetting.ScreenSize.X == 0) {
					prev = -1;
					result.Append($"\x1b[48;5;232m\x1b[38;5;255m|\n|");
					if (pSetting.FillContext) result.Append("\x1b[38;5;232m");
				}
				var value = -1;
				if (buffer[i] != 0) {
					value = (int)MathF.Round(Math.Clamp(buffer[i], 0, 1) * _colorCnt);
				}
				if (value != prev) {
					result.Append($"\x1b[48;5;{_startColor + value}m");
					if (pSetting.FillContext) result.Append($"\x1b[38;5;{_startColor + value}m");
					prev = value;
				}

				result.Append(pSetting.FillContext
					? value == -1 ? "  " : $"{value:d02}"
					: "  ");
			}

			result.AppendLine($"\n\x1b[38;5;255mSkipped triangle cnt: {skipped}");
			return result.ToString();
		}
		void Fill(Triangle pTriangle, float pU, float pV) {
			var point = pTriangle.GetPoint(pU, pV);
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
			buffer[coord] = MathF.Max(buffer[coord], 1f / (z + float.Epsilon));
		}
	}
}