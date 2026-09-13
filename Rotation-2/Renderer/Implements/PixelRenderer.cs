using System.Text;

namespace Rotation;

public class PixelRenderer(Setting pSetting, string pBrightString = " .;-=+*#%@"): IRenderer {
	protected readonly Setting _setting = pSetting;
	protected readonly string _brightnessString = pBrightString;
	private readonly int _screenSize = (int)pSetting.ScreenSize.X * (int)pSetting.ScreenSize.Y;

	protected virtual string GetPixel(StringBuilder pBuilder, Color pColor, float pPower) {
		var colorString = $"2;{pColor.ByteR};{pColor.ByteG};{pColor.ByteB}m";
		pBuilder.Append("\x1b[48;");
		pBuilder.Append(colorString);
		return "  ";
	}
	
    public StringBuilder GetResultBuilder(IScene pScene, PointInfo[] pPointInfo, Color[] pColors) {
	    var result = new StringBuilder();
		Color prev = new(0,0,0);
		var curPixel = "  ";
		result.Append("|");
		for (int i = 0; i < _screenSize; i++) {
    				
			if (i != 0 && i % _setting.ScreenSize.X == 0) {
				prev = new(0,0,0);
				result.Append("\x1b[48;2;0;0;0m\x1b[38;2;255;255;255m");
				result.Append("|\n|\x1b[38;2;0;0;0m");	
			}
			Color value = new(0,0, 0);
			var strength = 0f;
			if (pPointInfo[i].ZInv > 0) {
				if (!_setting.ZBufferShading && pPointInfo[i].Triangle != null) {
					var color = _setting.DefaultColor;
					var point = pPointInfo[i].Triangle!.GetPoint(pPointInfo[i].U, pPointInfo[i].V);
					strength  = 1 + _setting.Fog * point.Z;
					strength = Math.Clamp(strength, 0, 1);

					var obj = pPointInfo[i].Object!;
					foreach (var light in pScene.Lights) {
						if (_setting.CastShadow) {
							var ray = new Ray.Ray(light.Pos, point - light.Pos);
							var rayResult = obj.Mesh!.BVH.RayCasting(obj.Mesh, obj, ray);
							if(rayResult.Ratio < 1 - 1e-4)
								continue;	
						}
						
						if(!light.CalcColor(pPointInfo[i].Triangle!, point, out var lightColor)) continue;
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
					strength = Math.Clamp(pPointInfo[i].ZInv, 0, 1);
					value = new(strength, strength, strength);
				}
			}
    				
			if (value != prev) {
				prev = value;
				curPixel = GetPixel(result, value, _brightnessString.Length * strength);
			}
    
			result.Append(curPixel);
		}
		result.Append("\n\x1b[38;2;255;255;255m");
		return result;
    }
}