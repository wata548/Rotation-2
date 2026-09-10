using Rotation.Light;

namespace Rotation;

public interface IScene {
    IEnumerable<IDrawable> Objs { get; }
    IEnumerable<ILight> Lights { get; }
    string OtherData { get; }
    void Update(Setting pSetting);
}