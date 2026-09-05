using System.Text;
using System.Threading.Channels;

namespace Rotation;

public class Render {
	public readonly Channel<string> Outputs = Channel.CreateBounded<string>(5);
	private readonly int _screenSize;
	private readonly Setting _setting;
	private readonly float[] _zBuffer;
	private readonly float[] _brightness;
	private const string BrightnessString = " .;-=+*#%@";
	private int _renderedTriangleCnt = 0;

	public Render(Setting pSetting) {
		_setting = pSetting;
		_screenSize = (int)(pSetting.ScreenSize.X * pSetting.ScreenSize.Y);
		_zBuffer = new float[_screenSize];
		_brightness = new float[_screenSize];
	}
	
	public void Update(params IEnumerable<IDrawable> pObjs) {
		Array.Fill(_brightness, 0);
		Array.Fill(_zBuffer, 0);
		_renderedTriangleCnt = 0;
		foreach (var obj in pObjs) {
			foreach (var triangle in obj.GetTriangles()) {
				
				var brightness = triangle.Brightness(_setting);
				if (brightness < 1e-5) continue;
				_renderedTriangleCnt++;

				//count == 1 / (u term, v term)
				var uTerm = 1f / (triangle.U.Distance * _setting.CoordDetail);
				var vTerm = 1f / (triangle.V.Distance * _setting.CoordDetail);
				
				Fill(triangle, 0, 1, brightness);
				Fill(triangle, 1, 0, brightness);
				for (float i = 0; i < 1; i += uTerm) {
					for (float j = 0; j < 1; j += vTerm) {
						if (i + j > 1) break;
						Fill(triangle, i,j, brightness);
					}
				}	
			}
		}
		return;
		
		void Fill(Triangle pTriangle, float pU, float pV, float pDarkness) {
			var point = pTriangle.GetPoint(pU, pV);
			var fixedPoint = point; 
			fixedPoint.Y *= -1;
			var z = -fixedPoint.Z;
			
			if (!_setting.Isolate) {
				var ratio = _setting.CameraDistance / (_setting.CameraDistance - fixedPoint.Z);
				fixedPoint *= ratio;	
			}
			
			fixedPoint = ((fixedPoint - _setting.OriginDelta) * _setting.CoordDetail).Map(MathF.Round);
			if (fixedPoint.X < 0 || fixedPoint.X >= _setting.ScreenSize.X
				|| fixedPoint.Y < 0 || fixedPoint.Y >= _setting.ScreenSize.Y)
				return;
			var coord = (int)fixedPoint.X + (int)(_setting.ScreenSize.X * fixedPoint.Y);
			var zInv = 1f / (z + 1e-6f);
			if (_zBuffer[coord] > zInv) return;
			_zBuffer[coord] = zInv;
			
			if (_setting.Isolate)
				_brightness[coord] = pDarkness;
			else //need to recalculate brightness (it need each point position)
				_brightness[coord] = -pTriangle.Normal.Dot((point - _setting.CameraPos).Normalized);
		}
		
	}
	public async Task SaveResult() {
    			var result = new StringBuilder();
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
    				
    				if (_brightness[i] > 0) {
    					if (!_setting.ZBufferShading) {
    						value = _brightness[i] * (1 - _setting.Fog / _zBuffer[i]);
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
    					else if (_setting.UseColor) curPixel = value == 0 ? "  " : $"{(int)(100 * (value - 1e-5f)):d02}";
    					else curPixel = new string(BrightnessString[(int)(BrightnessString.Length * value)], 2);
    				}
    
    				result.Append(curPixel);
    			}
    			result.Append("\n");
    			if(_setting.UseColor) result.Append("\x1b[38;2;255;255;255m");
    			result.AppendLine($"Calculated triangle count: {_renderedTriangleCnt}");
    			await Outputs.Writer.WriteAsync(result.ToString());
    		}
}