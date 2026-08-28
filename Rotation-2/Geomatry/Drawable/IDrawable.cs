using System.Collections.Generic;

namespace Roation;

public interface IDrawable {
	IEnumerable<Triangle> GetTriangles();
}