namespace Rotation.Scene;

public class Test2Scene: IScene {

    private List<ObjBase> _objs = new();
    public IEnumerable<IDrawable> Objs => _objs;
    public string OtherData => "";

    public Test2Scene() {
        _objs.Add(new Cube() {
           Pos = new(0, -5, -7) ,
           Scale = new(10f,1f,10f),
           Rotation = Quaternion.Euler(0, 0, 0)
        });
    }

    public void Update(Setting pSetting) {

        _objs[0].Rotation = _objs[0].Rotation.Rotate(Quaternion.Euler(0, 30 * Program.Logic.DeltaTime, 0));
    }
}