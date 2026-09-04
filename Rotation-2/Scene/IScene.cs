namespace Rotation;

public interface IScene {
    IEnumerable<IDrawable> Objs { get; }
    string OtherData { get; }
    void Update(Setting pSetting);
}