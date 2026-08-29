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
	
    public Logic(Setting pSetting, IScene pScene) {
        Setting = pSetting;
        Scene = pScene;
        _streamWriter = new StreamWriter(new BufferedStream(Console.OpenStandardOutput()));
        _render = new();
        _stopWatch = new();
    }
	
    public void Update() {
        var term = (int)MathF.Ceiling(1000f / Setting.Frame);
        _stopWatch.Restart();
    
        Scene.Update(Setting);
        var result = _render.Show(Setting, Scene.Objs);
        Console.SetCursorPosition(0,0);
        _streamWriter.Write(result);
        _streamWriter.WriteLine(
            $"""
             {Scene.OtherData}
             Frame: {MathF.Min(1000f / _stopWatch.ElapsedMilliseconds, Setting.Frame) :F}
             DeltaTime: {DeltaTime}
             PlayTime: {Playtime}
             """);
        _streamWriter.Flush();
    			
        var deltaMilliSec = (int)Math.Max(term - _stopWatch.ElapsedMilliseconds, 0);
        Playtime += DeltaTime;
        DeltaTime = deltaMilliSec / 1000f;
        Thread.Sleep(deltaMilliSec);
    }	
}