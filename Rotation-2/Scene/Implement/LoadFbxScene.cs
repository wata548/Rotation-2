using Rotation.Light;
using Rotation.Ray;
using Rotation.UV;

namespace Rotation.Scene;

public class LoadFbxScene: IScene {

	private List<Object> _objs = new();
	private List<ILight> _lights = new();
	public IEnumerable<Object> Objs => _objs;
	public IEnumerable<ILight> Lights => _lights;
	public string OtherData => "";
	private readonly float _speed;
	private readonly Vector Pos;
	private const float MoveSpeed = 1;
	private const float MoveTerm = 2;
	
	public LoadFbxScene(Setting pSetting) {
		Console.Write("Enter target file(test.fbx): ");
		var targetFile = Console.ReadLine(); 
		targetFile = string.IsNullOrWhiteSpace(targetFile) ? "Models/test.fbx" : "Models/"+targetFile;
		targetFile += targetFile.Contains('.') ? "" : ".fbx";
        
		Console.Write("Enter speed(360): ");
		if (!float.TryParse(Console.ReadLine(), out _speed)) _speed = 360;
		Console.Write("Enter scale(0.03): ");
		if(!float.TryParse(Console.ReadLine(), out var scale)) scale = 0.03f;
        
        
		Console.Clear();
		Console.Write("NOW!, PLEASE ZOOM OUT QUICKLY!!!");
		var loader = new FbxLoader();
		var mesh = loader.Load(targetFile, true);
		_objs.Add( new Object {
			Pos = new(0, -6, -4),
			Scale = scale * Vector.One,
			Mesh = mesh,
			Rotation = Quaternion.Euler(0, 0, 0)
		});

		var existWall = File.Exists("Models/normal.fbx");
		var wall = existWall
			? loader.Load("Models/normal.fbx", true)
			: Sample.Sample.Cube();
		var size = existWall
			? new Vector(0.1f, 0.1f, 0.003f)
			: new Vector(30, 30, 1);
			
		_objs.Add( new Object {
			Pos = new(0, 0, -8),
			Scale = size,
			Rotation = Quaternion.Euler(0, 0, 0),
			Mesh = wall
		});
		Pos = _objs[0].Pos;
		_lights.Add(new SpotLight() {
			Color = new Color("ffffff") * 3,
			Pos = pSetting.CameraPos + Vector.Up * 8,
			Strength = 50
		});
	}
    
	public void Update(Setting pSetting) {
		var q1 = Quaternion.Euler(0, _speed * Program.Logic.DeltaTime, 0);
		_objs[0].Rotation = q1 * _objs[0].Rotation;
		_objs[0].Pos = Pos + Vector.Up * (MathF.Sin(MoveSpeed * Single.Pi * Program.Logic.Playtime) * 0.5f * MoveTerm);
		var delta = new Vector(MathF.Cos(MoveSpeed * Single.Pi * Program.Logic.Playtime),
			MathF.Sin(MoveSpeed * Single.Pi * Program.Logic.Playtime)) * 0.5f * MoveTerm * 10;
		_lights[0].Pos = pSetting.CameraPos + Vector.Up * 8 + delta;
	}
}