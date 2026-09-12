namespace Rotation.Ray;

public partial class BVH {
    private enum Axis{X,Y,Z}
    
    public BVH(IReadOnlyList<Vector> pVertices, List<TriangleIdx> pTriangleIndies) {
        var centers = pTriangleIndies.Select(Center)
            .ToList();
        
        var aabbs = new List<AABBNode> {
            new() { TriangleCnt= pTriangleIndies.Count }
        };
        if (aabbs[0].IsLeaf) throw new NotImplementedException("Hmm.... triangle count is very few");

        var stack = new Stack<int>();
        stack.Push(0);
        
        while (stack.Count > 0) {
            var idx = stack.Pop();
            var node = aabbs[idx];
            var triangleCnt = aabbs[idx].TriangleCnt;
            
            var term = (int)MathF.Floor(MathF.Sqrt(triangleCnt));
            var bestCenter = Vector.Zero;
            var bestAxis = Axis.X;
            var minHeuristic = float.MaxValue;
            for (int i = 0; i < triangleCnt; i += term) {
                
                var center = centers[node.TriangleIdx + i];
                for (var axis = Axis.X; axis <= Axis.Z; axis++) {
                    var temp = SurfaceAreaHeuristic(center, axis, node, term);
                    if (minHeuristic > temp) {
                        minHeuristic = temp;
                        bestAxis = axis;
                        bestCenter = center;
                    }    
                }
            }

            aabbs[idx].LeftIdx = aabbs.Count;
            var start = node.TriangleIdx;
            var end = node.TriangleIdx + triangleCnt - 1;
            var smaller = new AABBNode();
            var bigger = new AABBNode();
            
            var pos = GetAxis(bestCenter, bestAxis);
            var findFront = false;
            while (start < end) {
                if (!findFront) {
                    if (pos < GetAxis(centers[start], bestAxis)) {
                        findFront = true;
                    }
                    else {
                        smaller.AABB.Expand(pVertices, pTriangleIndies[start]);
                        smaller.TriangleCnt++;
                        start++;
                    }
                    continue;
                }
                if (pos >= GetAxis(centers[end], bestAxis)) {
                    (pTriangleIndies[start], pTriangleIndies[end]) =
                                            (pTriangleIndies[end], pTriangleIndies[start]);
                    (centers[start], centers[end]) = (centers[end], centers[start]);
                    smaller.AABB.Expand(pVertices, pTriangleIndies[start]);
                    smaller.TriangleCnt++;
                    start++;
                    findFront = false;
                    
                }
                bigger.AABB.Expand(pVertices, pTriangleIndies[end]);
                bigger.TriangleCnt++;
                end--;
            }

            if (start == end) {
                bigger.AABB.Expand(pVertices, pTriangleIndies[end]);
                bigger.TriangleCnt++;
            }
            
            
            aabbs.Add(smaller);
            if (!aabbs[^1].IsLeaf)
                stack.Push(aabbs.Count - 1);
            aabbs.Add(bigger);
            if(!aabbs[^1].IsLeaf)
                stack.Push(aabbs.Count - 1);
        }

        aabbs[0].AABB = new(aabbs[1].AABB, aabbs[2].AABB);
        _hirearchy = aabbs;

        Vector Center(TriangleIdx pIdx) {
            var a = pVertices[pIdx.A];
            var b = pVertices[pIdx.B];
            var c = pVertices[pIdx.C];
            return (a + b + c) / 3;
        }

        float GetAxis(Vector pPos, Axis pAxis) =>
            pAxis switch {
                Axis.X => pPos.X,
                Axis.Y => pPos.Y,
                Axis.Z => pPos.Z,
                _ => throw new AggregateException()
            };
        
        float SurfaceAreaHeuristic(Vector pPos, Axis pAxis, AABBNode pNode, int pTerm) {
            var triangleCnt = pNode.TriangleCnt;
            var big = new AABB();
            var small = new AABB();
            var bigCnt = 0;
            var smallCnt = 0;
            var pos = GetAxis(pPos, pAxis);
            for (int i = 0; i < triangleCnt; i += pTerm) {
                if (pos < GetAxis(centers[pNode.TriangleIdx + i], pAxis)) {
                    bigCnt++;
                    big.Expand(pVertices, pTriangleIndies[pNode.TriangleIdx + i]);
                }
                else {
                    smallCnt++;
                    small.Expand(pVertices, pTriangleIndies[pNode.TriangleIdx + i]);
                }
            }

            if (bigCnt == 0 || smallCnt == 0) return float.MaxValue;
            var sum = new AABB(big, small);
            var sumVolume = sum.SurfaceArea();
            return bigCnt * big.SurfaceArea() / sumVolume + smallCnt * small.SurfaceArea() / sumVolume;
        }
    }
}