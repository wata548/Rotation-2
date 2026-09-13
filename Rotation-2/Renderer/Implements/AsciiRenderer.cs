using System.Text;

namespace Rotation;

public class AsciiRenderer(Setting pSetting, string pBrightString = " .;-=+*#%@") : PixelRenderer(pSetting, pBrightString) {
	protected override string GetPixel(StringBuilder pBuilder, Color pColor, float pPower) {
		pBuilder.Append("\x1b[38;");
		pBuilder.Append($"2;{pColor.ByteR};{pColor.ByteG};{pColor.ByteB}m");
		return new string(_brightnessString[(int)pPower], 2);
	}
}