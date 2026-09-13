using Rotation.Light;
using Rotation.Ray;

namespace Rotation.Scene;

public class LoadFbxScene: IScene {

	private List<Object> _objs = new();
	private List<SpotLight> _lights = new();
	public IEnumerable<Object> Objs => _objs;
	public IEnumerable<ILight> Lights => _lights;
	public string OtherData => "";
	private readonly float _speed;
	private const float LightTerm = 8;
	private readonly Vector Middle;
	
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
		var mesh = loader.Load(targetFile);
		_objs.Add( new Object {
			Pos = new(0, -6, -4),
			Scale = scale * Vector.One,
			Mesh = mesh
		});
		_objs.Add( new Object {
			Pos = new(0, 0, -8),
			Scale = new(30, 30, 1),
			Rotation = Quaternion.Euler(0, 0, 0),
			Mesh = Sample.Sample.Cube()
		});
		
		_lights.Add(new() {
			Color = new Color("ffff00") * 3,
			Pos = pSetting.CameraPos + Vector.Up * 8,
			Strength = 50
		});
		Middle = new Vector(0, LightTerm * float.Sqrt(3) / 3, 0) + pSetting.CameraPos;

	}
    
	public void Update(Setting pSetting) {
		var q1 = Quaternion.Euler(0, _speed * Program.Logic.DeltaTime, 0);
		_objs[0].Rotation = q1 * _objs[0].Rotation;
	}
}