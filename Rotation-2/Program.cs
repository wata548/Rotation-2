using System.Diagnostics;
using Assimp;

namespace Roation;

public class Program {
	public static void Main() {
		Console.Write("Frame? (60): ");
		if(!int.TryParse(Console.ReadLine()??"", out var frame)) frame = 60; 
		Console.Write("Isolate? (Y / n): ");
		var isolate = !(Console.ReadLine()??"").Contains('n');
		
		var setting = new Setting(
			new(15, 15),
			Vector.Zero, 
			//new(250, 213, 27), yellow
			new Color(),
			frame,
			10, 
			50,
			Fog: 0.1f,
			FOV: 109,
			Isolate: isolate,
			FillContext: true, 
			ZBufferShading: true
		);
		
		var scene = new Rotation();
		var logic = new Logic(setting, scene);	
		
		while (true) {
			logic.Update();
		}
	}
}