using System.Text;
using System.Threading.Channels;

namespace Rotation;

public class Render {
	public readonly Channel<string> Outputs = Channel.CreateBounded<string>(5);
	private readonly IRenderer _renderer;
	private readonly Setting _setting;
	private readonly PointInfo[] _pointInfo;
	private readonly Color[] _colors;
	private int _renderedTriangleCnt = 0;

	public Render(Setting pSetting, IRenderer pRenderer) {
		_setting = pSetting;
		_renderer = pRenderer;
		var screenSize = (int)pSetting.ScreenSize.X * (int)pSetting.ScreenSize.Y;
		_pointInfo = new PointInfo[screenSize];
		_colors = new Color[screenSize];
	}
	
	public void Update(IScene pScene) {
		Array.Fill(_colors, new(0, 0,0));
		Array.Fill(_pointInfo, new(null, 0, 0,0,null));
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
				
				Fill(obj, triangle, 0, 1);
				Fill(obj, triangle, 1, 0);
				for (float i = 0; i < 1; i += uTerm) {
					for (float j = 0; j < 1; j += vTerm) {
						if (i + j > 1) break;
						Fill(obj, triangle, i,j);
					}
				}	
			}
		}
		return;

		Vector ToCameraSpace(Vector pPos) {
			var ratio = _setting.CameraDistance / (_setting.CameraDistance - pPos.Z);
			return pPos * ratio;	
		}

		void Fill(Object pObject, Triangle pTriangle, float pU, float pV) {
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
			_pointInfo[coord] = new(pObject, zInv, pU, pV, pTriangle);
		}
	}
	
	public async Task SaveResult(IScene pScene) {
		var builder = _renderer.GetResultBuilder(pScene, _pointInfo, _colors);
		builder.AppendLine($"Calculated triangle count: {_renderedTriangleCnt}");
		await Outputs.Writer.WriteAsync(builder.ToString());
	}
}