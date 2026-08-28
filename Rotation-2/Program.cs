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
			8, 
			35,
			Fog: 0.1f,
			FOV: 109,
			Isolate: isolate,
			FillContext: false
		);
		
		var sw = new StreamWriter(new BufferedStream(Console.OpenStandardOutput()));
		var render = new Render();
		var triangleHone = new TriangleHone {
			Pos = new(0, 4, -5),
			Scale = 2 * Vector.One
		};
		var cube1 = new Cube() {
			Pos = new(5, 3, -4),
			Scale = 3 * Vector.One
		};
		var cube2 = new TriangleHone() {
			Pos = new(-5, 3, -4),
			Scale = 3 * Vector.One
		};
		var sphere = new Crystal {
				Pos = new(3, -2, -2),
    			Scale = 4 * Vector.One		
		};
		var q1 = Quaternion.Euler(2, 7, 4);
		var q2 = Quaternion.Euler(5, 7, 2);
		var q3 = Quaternion.Euler(2, 3, 7);

		var delta = new Vector(60, 90);
		var num = Vector.Zero;
		var term = (int)MathF.Ceiling(1000f / setting.Frame);
		var stopWatch = new Stopwatch();
		var deltaTime = 0f;
		while (true) {
			stopWatch.Restart();
			num += delta * deltaTime;
			cube1.Rotation = Quaternion.Euler(num);
			cube2.Rotate(num.X, num.Y, num.Z);
			
			//need to find apply deltaTime
			sphere.Rotation *= q2;
			triangleHone.Rotation *= q3;
			
			var result = render.Show(setting, triangleHone, sphere, cube1, cube2);
			Console.SetCursorPosition(0,0);
			sw.Write(result);
			sw.WriteLine($"Rotation: {num}");
			sw.Write($"Frame: {MathF.Min(1000f / stopWatch.ElapsedMilliseconds, setting.Frame) :F}");
			sw.Flush();
			var deltaMilliSec = (int)Math.Max(term - stopWatch.ElapsedMilliseconds, 0);
			deltaTime = deltaMilliSec / 1000f;
			Thread.Sleep(deltaMilliSec);
		}
	}
}