namespace Rotation.Scene;

public class LoadFbxScene: IScene {

    private List<ObjBase> _objs = new();
    public IEnumerable<IDrawable> Objs => _objs; 
    public string OtherData { get; }
    public float _speed = 360; 
    
    public LoadFbxScene() {
        Console.Write("Enter speed(360): ");
        if (!float.TryParse(Console.ReadLine(), out _speed)) _speed = 360;
        Console.Write("Enter target file(test.fbx): ");
        var targetFile = Console.ReadLine();
        targetFile = string.IsNullOrWhiteSpace(targetFile) ? "Models/test.fbx" : "Models/"+targetFile;
        targetFile += targetFile.Contains('.') ? "" : ".fbx";
        Console.Write("Enter scale(0.03): ");
        if(!float.TryParse(Console.ReadLine(), out var scale)) scale = 0.03f;
        
        
        Console.Clear();
        Console.Write("NOW!, PLEASE ZOOM OUT QUICKLY!!!");
        
        _objs.Add(new FbxObj(targetFile) {
            Pos = new(0, -3, -3),
            Scale = scale * Vector.One,
            Rotation = Quaternion.Euler(0, 0, 0)
        });
    }
    
    public void Update(Setting pSetting) {
		var q1 = Quaternion.Euler(0, _speed * Program.Logic.DeltaTime, 0);
        _objs[0].Rotation = q1 * _objs[0].Rotation;
    }
}