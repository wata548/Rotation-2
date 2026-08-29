using System.Diagnostics;

namespace Roation;

public class Program {
	public static void Main() {
		Console.Write("Frame? (60): ");
		if(!int.TryParse(Console.ReadLine()??"", out var frame)) frame = 60; 
		Console.Write("Isolate? (Y / n): ");
		var isolate = !(Console.ReadLine()??"").Contains('n');
		
		var setting = new Setting(
			new(15, 15),
			new(), 
			frame,
			10, 
			9,
			Fog: 0.1f,
			FOV: 109,
			Isolate: isolate,
			FillContext: true
		);
		var scene = new TestScene();
		var logic = new Logic(setting, scene);	
		
		while (true) {
			logic.Update();
		}
	}
}