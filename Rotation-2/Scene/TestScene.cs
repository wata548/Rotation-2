namespace Roation;

public class TestScene: IScene {

    private List<ObjBase> _objs = new();
    public IEnumerable<ObjBase> Objs => _objs; 
    public string OtherData { get; }

    public TestScene() {
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
        _objs.Add(new FbxObj("test.fbx") {
            Pos = new(0, 0, -9),
            Scale = 1f * Vector.One,
            Rotation = Quaternion.Euler(-90, 0, 0)
        });
    }
    
    public void Update(Setting pSetting) {
		var q1 = Quaternion.Euler(0, 10, 0);
        _objs[0].Rotation = q1 * _objs[0].Rotation;
    }
}