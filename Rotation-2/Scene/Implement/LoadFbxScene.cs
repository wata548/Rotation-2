using Rotation.Light;

namespace Rotation.Scene;

public class LoadFbxScene: IScene {

	private List<Object> _objs = new();
	private List<SpotLight> _lights = new();
	public IEnumerable<IDrawable> Objs => _objs;
	public IEnumerable<ILight> Lights => _lights;
	public string OtherData { get; }
	public float _speed = 360; 
    
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
			Pos = new(0, -3, -5),
			Scale = scale * Vector.One,
			Rotation = Quaternion.Euler(0, 0, 0),
			Mesh = mesh
		});
		_lights.Add(new() {
			Color = new Color("f7f499") * 4,
			Pos = pSetting.CameraPos + Vector.Left * +20,
			Strength = 50,
		});
		_lights.Add(new() {
			Color = new Color("a4c8f7") * 4,
			Pos = pSetting.CameraPos + Vector.Left * -20,
			Strength = 50,
		});
		_lights.Add(new() {
			Color = new Color("f2afca") * 4,
			Pos = pSetting.CameraPos + Vector.Up * 9,
			Strength = 50,
		});
        
	}
    
	public void Update(Setting pSetting) {
		var q1 = Quaternion.Euler(0, _speed * Program.Logic.DeltaTime, 0);
		_objs[0].Rotation = q1 * _objs[0].Rotation;
		Triangle();
	}

	private void Triangle() {
		var middle = _lights.Aggregate(Vector.Zero, (sum, light) => sum + light.Pos) / 3;
		var q = Quaternion.Euler(0, 0, 40 * Program.Logic.DeltaTime);
		foreach (var light in _lights) {
			light.Pos = q.Rotate(light.Pos - middle);
		}
	}
}