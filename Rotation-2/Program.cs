namespace Roation;

public class Program {
	public static Logic Logic;
	
	public static void Main() {
		Console.Write("Frame? (60): ");
		if(!int.TryParse(Console.ReadLine()??"", out var frame)) frame = 60; 
		Console.Write("Ascii? (y / N): ");
		var ascii = (Console.ReadLine()??"").Contains('y');
		Console.Write("Isolate? (Y / n): ");
		var isolate = !(Console.ReadLine()??"").Contains('n');
		
		var setting = new Setting(
			new(8,8),
			Vector.Zero, 
			//new(250, 213, 27), yellow
			new Color(),
			frame,
			50, 
			Fog: 0.1f,
			FOV: 109,
			Isolate: isolate,
			UseColor: !ascii,
			FillContext: false,
			ZBufferShading: false
		);
		
		var scene = new LoadFbxScene();
		Logic = new Logic(setting, scene);	
		
		Logic.StartDataLoop();
		Logic.StartRenderLoop().Wait();
		
	}
}