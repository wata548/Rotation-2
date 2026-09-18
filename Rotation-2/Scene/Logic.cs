using System.Diagnostics;

namespace Rotation;

public class Logic {
	public Setting Setting { get; set; }
	public IScene Scene { get; set; }
	public float DeltaTime { get; private set; } = 0;
	public float Playtime { get; private set; } = 0;
	private readonly StreamWriter _streamWriter;
	private readonly Render _render;
	private readonly Stopwatch _stopWatch;
	private readonly CancellationTokenSource _cancelDataLoop = new();
	public readonly CancellationTokenSource CancelRenderLoop = new();
	
	public Logic(Setting pSetting, IScene pScene) {
		Setting = pSetting;
		Scene = pScene;
		_streamWriter = new StreamWriter(new BufferedStream(Console.OpenStandardOutput()));
		_stopWatch = new();
		_render = pSetting switch {
			{ Ascii: true } => new(pSetting, new AsciiRenderer(pSetting)),
			{ Ascii: false } => new(pSetting, new PixelRenderer(pSetting)),
		};
	}

	public Task StartRenderLoop() =>
		Task.Run(() => RenderLoop(CancelRenderLoop.Token), CancelRenderLoop.Token);
	public Task StartDataLoop() =>
		Task.Run(() => DataLoop(_cancelDataLoop.Token), _cancelDataLoop.Token);
	
	private async Task RenderLoop(CancellationToken pToken) {
		try {
			await foreach (var context in _render.Outputs.Reader.ReadAllAsync(pToken)) {
				Console.SetCursorPosition(0, 0);
				await _streamWriter.WriteAsync(context);
				await _streamWriter.WriteLineAsync(
					$"""
					 {Scene.OtherData}                 
					 Frame: {1f / DeltaTime:F}            
					 DeltaTime: {DeltaTime}           
					 PlayTime: {Playtime}           
					 """);
				await _streamWriter.FlushAsync(pToken);
			}
		}
		catch(Exception pError) {
			Console.WriteLine(pError);
		}
	}

	private async Task DataLoop(CancellationToken pToken) {
		try {
			var term = (int)MathF.Ceiling(1000f / Setting.Frame);
			while (true) {
				if (pToken.IsCancellationRequested) break;
				_stopWatch.Restart();
				Scene.Update(Setting);
				foreach (var obj in Scene.Objs)
					obj.Update();

				_render.Update(Scene);
				await _render.SaveResult(Scene);

				var used = (int)_stopWatch.ElapsedMilliseconds;
				var remain = term - used;
				if (remain < 0) remain = 0;
				else used += remain;

				Playtime += DeltaTime;
				DeltaTime = (used + remain) / 1000f;
				Thread.Sleep(remain);
			}

			return;
		}
		catch (Exception pError) {
			Console.WriteLine(pError);
		}
	}
}