using System.Text;
using System.Threading.Channels;

namespace Roation;

public class Render {
	public readonly Channel<string> Outputs = Channel.CreateBounded<string>(5);
	private readonly int _screenSize;
	private readonly Setting _setting;
	private readonly float[] _zBuffer;
	private readonly float[] _darkness;
	private const string DarknessString = " .;-=+*#%@";

	public Render(Setting pSetting) {
		_setting = pSetting;
		_screenSize = (int)(pSetting.ScreenSize.X * pSetting.ScreenSize.Y);
		_zBuffer = new float[_screenSize];
		_darkness = new float[_screenSize];
	}
	
	public async Task Update(params IEnumerable<IDrawable> pObjs) {
		Array.Fill(_darkness, 0);
		Array.Fill(_zBuffer, 0);
		var renderedTriangle = 0;
		foreach (var obj in pObjs) {
			foreach (var triangle in obj.GetTriangles()) {
				if (!_setting.DoubleFace && !triangle.IsVisible(_setting)) continue;
				renderedTriangle++;

				var darkness = triangle.Normal.Normalized.Dot(_setting.ViewDirection);
				Fill(triangle, 0, 1, darkness);
				Fill(triangle, 1, 0, darkness);
				for (float i = 0; i < 1; i += _setting.Term) {
					for (float j = 0; j < 1; j += _setting.Term) {
						if (i + j > 1) break;
						Fill(triangle, i,j, darkness);
					}
				}	
			}
		}

		await SaveResult();
		return;
		
		void Fill(Triangle pTriangle, float pU, float pV, float pDarkness) {
			var point = pTriangle.GetPoint(pU, pV);
			point.Y *= -1;
			var z = -point.Z;
			
			if (!_setting.Isolate) {
				var ratio = _setting.CameraDistance / (_setting.CameraDistance - point.Z);
				point *= ratio;	
			}
			
			point = ((point - _setting.OriginDelta) * _setting.CoordDetail).Map(MathF.Round);
			if (point.X < 0 || point.X >= _setting.ScreenSize.X
				|| point.Y < 0 || point.Y >= _setting.ScreenSize.Y)
				return;
			var coord = (int)point.X + (int)(_setting.ScreenSize.X * point.Y);
			var zInv = 1f / (z + float.Epsilon);
			if (_zBuffer[coord] > zInv) return;
			_zBuffer[coord] = zInv;
			_darkness[coord] = pDarkness;
		}
		async Task SaveResult() {
			var result = new StringBuilder();
			result.Clear();
			var prev = 0f;
			var curPixel = "  ";
			result.Append("|");
			for (int i = 0; i < _screenSize; i++) {
				
				if (i != 0 && i % _setting.ScreenSize.X == 0) {
					prev = 0;
					if (_setting.UseColor) {
						result.Append("\x1b[48;2;0;0;0m\x1b[38;2;255;255;255m|\n|");
						if (_setting.FillContext) result.Append("\x1b[38;2;0;0;0m");	
					}
					else result.Append("|\n|");
				}
				var value = 0f;
				
				if (_darkness[i] < 0) {
					if (!_setting.ZBufferShading) {
						value = -_darkness[i] * (1 - _setting.Fog / _zBuffer[i]);
						value = Math.Clamp(value, 0, 1);
					}
					else
						value = Math.Clamp(_zBuffer[i], 0, 1);
				}
				
				if (Math.Abs(value - prev) > 1e-5f) {
					var r = (int)(_setting.Color.R * value);
					var g = (int)(_setting.Color.G * value);
					var b = (int)(_setting.Color.B * value);
					if (_setting.UseColor) {
						result.Append($"\x1b[48;2;{r};{g};{b}m");
						if (_setting.FillContext) result.Append($"\x1b[38;2;{r};{g};{b}m");	
					}
					prev = value;
					if (!_setting.FillContext) curPixel = "  ";
					else if (_setting.UseColor) curPixel = value == 0 ? "  " : $"{(int)(100 * value):d02}";
					else curPixel = new string(DarknessString[(int)(DarknessString.Length * value)], 2);
				}

				result.Append(curPixel);
			}
			result.Append("\n");
			if(_setting.UseColor) result.Append("\x1b[38;2;255;255;255m");
			result.AppendLine($"Calculated triangle count: {renderedTriangle}");
			await Outputs.Writer.WriteAsync(result.ToString());
		}
	}
}