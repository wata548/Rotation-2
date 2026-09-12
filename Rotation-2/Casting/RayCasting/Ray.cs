namespace Rotation.Ray;

public record Ray(Vector Pos, Vector Direction) {
    public Ray ApplyTransform(ITransform pTransform) {
        var end = pTransform.CancelTransform(Pos + Direction);
        var start = pTransform.CancelTransform(Pos);
        return new(start, end - start);
    }

    public bool VsTriangle(IReadOnlyList<Vector> pVertices, TriangleIdx pIdx, out float t) {
	    var a = pVertices[pIdx.A];
	    var b = pVertices[pIdx.B];
	    var c = pVertices[pIdx.C];
	    var up = (b - a).Cross(c - a);
	    var denominator = up.Dot(Direction);
	    if (denominator <= 1e-5) {
		    t = 2;
		    return false;
	    }
	    t = up.Dot(a - Pos) / denominator;
	    if (t is < 0 or > 1) {
		    t = 2;
		    return false;
	    }
	    var point = Pos + t * Direction;
	    var s1 = MathF.Sign(CCW(b - a, point - a));
	    var s2 = MathF.Sign(CCW(c - b, point - b));
	    var s3 = MathF.Sign(CCW(a - c, point - c));
	    return (s1 >= 0 && s2 >= 0 && s3 >= 0)
	           || (s1 <= 0 && s2 <= 0 && s3 <= 0);

	    float CCW(Vector pA, Vector pB) =>
		    pA.Cross(pB).Dot(up);
    } 
	
    public bool VsAABB(AABB pAABB, float pMin) {
	    var minT = 0f;
	    var maxT = 1f;
	    var min = pAABB.Min - Pos;
	    var max = pAABB.Max - Pos;
	    if (Direction.X == 0) {
		    if(min.X * max.X > 0) 
			    return false;
	    }
	    else {
		    var minX = min.X / Direction.X;
		    var maxX = max.X / Direction.X;
		    if (Direction.X < 0) {
			    (minX, maxX) = (maxX, minX);
		    }
    
		    minT = float.Max(minT, minX);
		    maxT = float.Min(maxT, maxX);
	    }
	    if (Direction.Y == 0) {
		    if(min.Y * max.Y > 0) 
			    return false;
	    }
	    else {
		    var minY = min.Y / Direction.Y;
		    var maxY = max.Y / Direction.Y;
		    if (Direction.Y < 0) {
			    (minY, maxY) = (maxY, minY);
		    }
        
		    minT = float.Max(minT, minY);
		    maxT = float.Min(maxT, maxY);
	    }
	    if (Direction.Z == 0) {
		    if(min.Z * max.Z > 0) 
			    return false;
	    }
	    else {
		    var minZ = min.Z / Direction.Z;
		    var maxZ = max.Z / Direction.Z;
		    if (Direction.Z < 0) {
			    (minZ, maxZ) = (maxZ, minZ);
		    }
        
		    minT = float.Max(minT, minZ);
		    maxT = float.Min(maxT, maxZ);
	    }
	    return minT <= maxT && minT >= pMin;
    }
    	
}