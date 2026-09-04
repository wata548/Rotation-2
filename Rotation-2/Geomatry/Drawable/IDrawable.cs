using System.Collections.Generic;

namespace Rotation;

public interface IDrawable {
	IEnumerable<Triangle> GetTriangles();
}