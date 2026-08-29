namespace Roation;

public interface IScene {
    IEnumerable<ObjBase> Objs { get; }
    string OtherData { get; }
    void Update(Setting pSetting);
}