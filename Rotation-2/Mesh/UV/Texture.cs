using System.Drawing;
using System.Drawing.Imaging;

namespace Rotation.UV;

public interface ITexture {
	public Color GetPixel(TriangleIdx pIdx, float pU, float pV);
	public void SwapIndex(int pLhs, int pRhs);
}

public class Texture: ITexture {
	private readonly int _width;
	private readonly int _height;
	private readonly Color[] _map;
	private readonly List<UVCoord> _coords;

	public record struct UVCoord(float X, float Y) {
		public static UVCoord operator +(UVCoord pLhs, UVCoord pRhs) =>
			new(pLhs.X + pRhs.X, pLhs.Y + pRhs.Y);
		public static UVCoord operator -(UVCoord pLhs, UVCoord pRhs) =>
			new(pLhs.X - pRhs.X, pLhs.Y - pRhs.Y);
		public static UVCoord operator *(float pLhs, UVCoord pRhs) =>
			new(pLhs * pRhs.X, pLhs * pRhs.Y);
	}

	public void AddCoords(IEnumerable<UVCoord> pCoords) {
		_coords.AddRange(pCoords);
	}
	
	public Texture(List<UVCoord> pCoords, byte[] pBytes, int pTargetWidth) {
		_coords = pCoords;
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
	
	public Color GetPixel(TriangleIdx pIdx, float pU, float pV) {
		var a = _coords[pIdx.A];
		var b = _coords[pIdx.B];
		var c = _coords[pIdx.C];
		var coord = a + pU * (b - a) + pV * (c - a);
		var x = (int)MathF.Round((_width - 1) * coord.X);
		var y = (int)MathF.Round((_height - 1) * (1 - coord.Y));
		return _map[x + _width * y];
	}

	public void SwapIndex(int pLhs, int pRhs) {
		(_map[pLhs], _map[pRhs]) = (_map[pRhs], _map[pLhs]);
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