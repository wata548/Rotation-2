using System.Text;
using System.Threading.Channels;

namespace Rotation;

public partial class Render {
	public readonly Channel<string> Outputs = Channel.CreateBounded<string>(5);
	private readonly int _screenSize;
	private readonly Setting _setting;
	private readonly PointInfo[] _pointInfo;
	private readonly Color[] _colors;
	private const string BrightnessString = " .;-=+*#%@";
	private int _renderedTriangleCnt = 0;

	public Render(Setting pSetting) {
		_setting = pSetting;
		_screenSize = (int)pSetting.ScreenSize.X * (int)pSetting.ScreenSize.Y;
		_pointInfo = new PointInfo[_screenSize];
		_colors = new Color[_screenSize];
	}
	
	public void Update(IScene pScene) {
		Array.Fill(_colors, new(0, 0,0));
		Array.Fill(_pointInfo, new(0, 0,0,null));
		_renderedTriangleCnt = 0;
		foreach (var obj in pScene.Objs) {
			foreach (var triangle in obj.Triangles) {
				
				var isBackFace = -triangle.Normal.Dot(_setting.Isolate
					? _setting.ViewDirection
					: (triangle.Middle - _setting.CameraPos).Normalized
				);
				
				if (isBackFace < 1e-5) continue;
				_renderedTriangleCnt++;
				
				//count == 1 / (u term, v term)
				var uTerm = 1f / (triangle.U.Distance * _setting.CoordDetail);
				var vTerm = 1f / (triangle.V.Distance * _setting.CoordDetail);
				
				Fill(triangle, 0, 1);
				Fill(triangle, 1, 0);
				for (float i = 0; i < 1; i += uTerm) {
					for (float j = 0; j < 1; j += vTerm) {
						if (i + j > 1) break;
						Fill(triangle, i,j);
					}
				}	
			}
		}
		return;

		Vector ToCameraSpace(Vector pPos) {
			var ratio = _setting.CameraDistance / (_setting.CameraDistance - pPos.Z);
			return pPos * ratio;	
		}

		void Fill(Triangle pTriangle, float pU, float pV) {
			var point = pTriangle.GetPoint(pU, pV);
			var fixedPoint = point; 
			fixedPoint.Y *= -1;
			var z = -fixedPoint.Z;
			
			if (!_setting.Isolate)
				fixedPoint = ToCameraSpace(fixedPoint);
			
			fixedPoint = ((fixedPoint - _setting.OriginDelta) * _setting.CoordDetail).Map(MathF.Round);
			if (fixedPoint.X < 0 || fixedPoint.X >= _setting.ScreenSize.X
				|| fixedPoint.Y < 0 || fixedPoint.Y >= _setting.ScreenSize.Y)
				return;
			var coord = (int)fixedPoint.X + (int)(_setting.ScreenSize.X * fixedPoint.Y);
			var zInv = 1f / (z + 1e-6f);
			if (_pointInfo[coord].ZInv > zInv) return;
			_pointInfo[coord] = new(zInv, pU, pV, pTriangle);
		}
	}
	
	public async Task SaveResult(IScene pScene) {
		var result = new StringBuilder();
		Color prev = new(0,0,0);
		var curPixel = "  ";
		result.Append("|");
		for (int i = 0; i < _screenSize; i++) {
    				
			if (i != 0 && i % _setting.ScreenSize.X == 0) {
				prev = new(0,0,0);
				if (_setting.UseColor) {
					result.Append("\x1b[48;2;0;0;0m\x1b[38;2;255;255;255m|\n|");
					if (_setting.FillContext) result.Append("\x1b[38;2;0;0;0m");	
				}
				else result.Append("|\n|");
			}
			Color value = new(0,0, 0);
			var strength = 0f;
			if (_pointInfo[i].ZInv > 0) {
				if (!_setting.ZBufferShading && _pointInfo[i].Triangle != null) {
					strength  = 1 - _setting.Fog / _pointInfo[i].ZInv;
					strength = Math.Clamp(strength, 0, 1);
					var color = _setting.DefaultColor;
					var point = _pointInfo[i].Triangle!.GetPoint(_pointInfo[i].U, _pointInfo[i].V);
					foreach (var light in pScene.Lights) {
						var lightColor = light.CalcColor(_pointInfo[i].Triangle!, point);
						color = _setting.LightProcessType switch {
							LightProcessType.Screen => color.Screen(lightColor),
							LightProcessType.Overlay => color.Overlay(lightColor),
							LightProcessType.SoftLight => color.SoftLight(lightColor),
							LightProcessType.HardLight => color.HardLight(lightColor),
							_ => color
						};
					}
					value = color * strength;
				}
				else {
					strength = Math.Clamp(_pointInfo[i].ZInv, 0, 1);
					value = new(strength, strength, strength);
				}
			}
    				
			if (value != prev) {
				if (_setting.UseColor) {
					var colorString = $"2;{value.ByteR};{value.ByteG};{value.ByteB}m";
					result.Append("\x1b[48;");
					result.Append(colorString);
					if (_setting.FillContext) {
						result.Append("\x1b[38;");
						result.Append(colorString);
					}	
				}
				prev = value;
				if (!_setting.FillContext) curPixel = "  ";
				else curPixel = new string(BrightnessString[(int)(BrightnessString.Length * strength)], 2);
			}
    
			result.Append(curPixel);
		}
		result.Append("\n");
		if(_setting.UseColor) result.Append("\x1b[38;2;255;255;255m");
		result.AppendLine($"Calculated triangle count: {_renderedTriangleCnt}");
		await Outputs.Writer.WriteAsync(result.ToString());
	}
}