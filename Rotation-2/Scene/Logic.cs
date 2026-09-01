using System.Diagnostics;

namespace Roation;

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
		_render = new(pSetting);
		_stopWatch = new();
	}

	public Task StartRenderLoop() =>
		Task.Run(() => RenderLoop(CancelRenderLoop.Token), CancelRenderLoop.Token);
	public Task StartDataLoop() =>
		Task.Run(() => DataLoop(_cancelDataLoop.Token), _cancelDataLoop.Token);
	
	private async Task RenderLoop(CancellationToken pToken) {
		await foreach (var context in _render.Outputs.Reader.ReadAllAsync(pToken)) {
			Console.SetCursorPosition(0,0);
			await _streamWriter.WriteAsync(context);
			await _streamWriter.WriteLineAsync(
				$"""
				 {Scene.OtherData}
				 Frame: {1f / DeltaTime :F}
				 DeltaTime: {DeltaTime}
				 PlayTime: {Playtime}
				 """);
			await _streamWriter.FlushAsync(pToken);	
		}
	}

	private async Task DataLoop(CancellationToken pToken) {
		var term = (int)MathF.Ceiling(1000f / Setting.Frame);
		while (true) {
			if (pToken.IsCancellationRequested) break;
			_stopWatch.Restart();
			Scene.Update(Setting);
			await _render.Update(Scene.Objs);

			var used = (int)_stopWatch.ElapsedMilliseconds;
			var remain = term - used;
			if (remain < 0) remain = 0;
			else used += remain;
			
			Playtime += DeltaTime;
			DeltaTime = used / 1000f;
			Thread.Sleep(remain);
		}
		return;
	}
}