using System.Text;

namespace Rotation;


public interface IRenderer {
    StringBuilder GetResultBuilder(IScene pScene, PointInfo[] pPointInfo, Color[] pColors); }