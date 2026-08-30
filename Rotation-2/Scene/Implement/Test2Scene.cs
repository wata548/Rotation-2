namespace Roation;

public class Test2Scene: IScene {

    private List<Triangle> _objs = new();
    public IEnumerable<IDrawable> Objs => _objs; 
    public string OtherData { get; }

    public Test2Scene() {
        _objs.Add(new(
                new(new(4, 3, -4)),
                new(new(-1, 4, -2)),
                new(new(1, 2, -2))
            )
        );
        
        _objs.Add(new(
                new(new(1, 4, -3)),
                new(new(2, 2, -3)),
                new(new(3, 4, -3))
            )
        );
    }
    
    public void Update(Setting pSetting) { }
}