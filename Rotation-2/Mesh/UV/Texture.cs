using System.Drawing;
using System.Drawing.Imaging;
namespace Rotation.UV;

public class Texture{
	protected readonly int _width;
	protected readonly int _height;
	protected readonly Color[] _map;

	public Texture(byte[] pBytes, int pTargetWidth) {
		using (var image = Image.FromStream(new MemoryStream(pBytes))) {
			using (var bitmap = (Bitmap)image) {
				var data = bitmap.LockBits(new(0, 0, image.Width, image.Height),
					ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
				var scale = pTargetWidth < bitmap.Width
					? bitmap.Width / pTargetWidth + (bitmap.Width % pTargetWidth == 0 ? 0 : 1)
					: 1;
				_width = bitmap.Width / scale + (bitmap.Width % scale == 0 ? 0 : 1);
				_height = bitmap.Height / scale + (bitmap.Height % scale == 0 ? 0 : 1);
				_map = new Color[_width * _height];
				unsafe {
					byte* ptr = (byte*)data.Scan0;
					for (int y = 0, i = 0; y < bitmap.Height; y += scale, i++) {
						for (int x = 0, j = 0; x < bitmap.Width; x += scale, j++) {
							var color = ptr + y * data.Stride + x * 3;
							_map[i * _width + j] = new(
								color[2] / 255f,
								color[1] / 255f,
								color[0] / 255f
							);
						}
					}
				}
			}
		}
	}

	public Color GetPixel(UVMap pUV, TriangleIdx pIdx, float pU, float pV) {
		var coord = pUV.Get(pIdx, pU, pV);
		var x = (int)MathF.Round((_width - 1) * coord.X);
		var y = (int)MathF.Round((_height - 1) * (1 - coord.Y));
		return _map[x + _width * y];
	}
	public void Save(string pName) {
		using Bitmap bitmap = new(_width, _height);
		
		BitmapData data = bitmap.LockBits(
			new Rectangle(0, 0, _width, _height),
			ImageLockMode.WriteOnly,
			PixelFormat.Format24bppRgb);

		unsafe
		{
			byte* ptr = (byte*)data.Scan0;

			for (int y = 0; y < _height; y++)
			{
				byte* row = ptr + y * data.Stride;

				for (int x = 0; x < _width; x++)
				{
					var c = _map[x + _width * y];
					row[x * 3 + 2] = c.ByteR;
					row[x * 3 + 1] = c.ByteG;
					row[x * 3 + 0] = c.ByteB;
				}
			}
		}

		bitmap.UnlockBits(data);
		bitmap.Save($"{pName}.png");
	}
	}