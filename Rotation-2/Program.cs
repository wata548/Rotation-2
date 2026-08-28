namespace Roation;

public class Program {
	public static void Main() {
		Console.Write("Isolate? (Y / n): ");
		var isolate = !(Console.ReadLine()??"").Contains('n');
		var setting = new Setting(
			new(15, 15),
			new(), 
			8, 
			18,
			FOV: 60,
			Isolate: isolate,
			FillContext: true
		);
		
		var sw = new StreamWriter(new BufferedStream(Console.OpenStandardOutput()));
		var render = new Render();
		var triangleHone = new TriangleHone(4, new(3, 4, -2));
		var cube1 = new Cube(5, new(2, 2, -2));
		var cube2 = new Cube(5, new(-2, 2, -2));
		var sphere = new Crystal(4, new(3, -2, -2));
		/*var test = new Triangle([
			new(new(1, 0, -2)),
			new(new(0, 3, -2)),
			new(new(-2, 0, -2)),
		]);*/
		var q1 = Quaternion.Euler(2, 7, 4);
		var q2 = Quaternion.Euler(5, 7, 2);
		var q3 = Quaternion.Euler(2, 3, 7);
		while (true) {
			cube1.Rotation *= q1;
			cube2.Rotate(2, 7, 3);
			sphere.Rotation *= q2;
			triangleHone.Rotation *= q3;
			var result = render.Show(setting, triangleHone, sphere, cube1, cube2);
			Console.Clear();
			sw.Write(result);
			sw.Flush();
			Thread.Sleep(30);
		}
	}
}