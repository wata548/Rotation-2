namespace Rotation.Scene;

public class Rotation: IScene {
    private List<ObjBase> _objs = new();
    public IEnumerable<IDrawable> Objs => _objs;
    public string OtherData => "";

    public Rotation() {
        _objs.Add(new Cube {
            Pos = new(4, 4, -5),
            Scale = Vector.One * 5
        });
        /*_objs.Add(new Cube {
            Pos = new(-3, 0, -2)
        });*/
    }

    private Vector Sum = Vector.Zero;
    public void Update(Setting pSetting) {
        _objs[0].Rotation *= Quaternion.Euler(5, 7, 0);
        var delta = new Vector(5, 7, 0);
        //_objs[1].Rotate(Sum += delta);
    }
}