namespace Roation;

public class LoadFbxScene: IScene {

    private List<ObjBase> _objs = new();
    public IEnumerable<IDrawable> Objs => _objs; 
    public string OtherData { get; }

    public LoadFbxScene() {
        Console.Write("Enter target file(test.fbx): ");
        var targetFile = Console.ReadLine();
        targetFile = string.IsNullOrWhiteSpace(targetFile) ? "Models/test.fbx" : "Models/"+targetFile;
        targetFile += targetFile.Contains('.') ? "" : ".fbx";
        
        /*
        _objs.Add(new TriangleHone {
            Pos = new(0, 4, -5),
            Scale = 2 * Vector.One
        });
        _objs.Add(new TriangleHone() {
            Pos = new(-5, 3, -4),
            Scale = 3 * Vector.One
        });
        _objs.Add(new Crystal {
            Pos = new(3, -2, -2),
            Scale = 4 * Vector.One		
        });
        */
        _objs.Add(new FbxObj(targetFile) {
            Pos = new(0, -3, -3),
            Scale = 0.03f * new Vector(1, 1, 1),
            Rotation = Quaternion.Euler(0, 0, 0)
        });
    }
    
    public void Update(Setting pSetting) {
		var q1 = Quaternion.Euler(0, 10, 0);
        _objs[0].Rotation = q1 * _objs[0].Rotation;
    }
}