namespace Rotation.Ray;

public partial class BVH {
    private class AABBNode {
        public int TriangleCnt { get; set; }
        public int LeftIdx { get; set; }
        public int RightIdx => LeftIdx + 1;
        public AABB AABB { get; set; } = new();
        public TriangleIdx? Triangle { get; set; }
    }

    private record DevideRequset(int NodeIdx, int TriangleIdx);
    private enum Axis{X,Y,Z}
    
    public BVH(IReadOnlyList<Vector> pVertices, List<TriangleIdx> pTriangleIndies) {
        var centers = pTriangleIndies.Select(Center)
            .ToList();
        
        var aabbs = new List<AABBNode> {
            new() { TriangleCnt= pTriangleIndies.Count }
        };

        var stack = new Stack<DevideRequset>();
        stack.Push(new(0, 0));
        
        while (stack.Count > 0) {
            var req = stack.Pop();
            var triangleCnt = aabbs[req.NodeIdx].TriangleCnt;
            
            //register triangle
            if (triangleCnt == 1) {
                aabbs[req.NodeIdx].Triangle = pTriangleIndies[req.TriangleIdx];
                continue;
            }
            
            //generate two children
            var term = (int)MathF.Floor(MathF.Sqrt(triangleCnt));
            var bestCenter = Vector.Zero;
            var bestAxis = Axis.X;
            var minHeuristic = float.MaxValue;
            for (int i = 0; i < triangleCnt; i += term) {
                
                var center = centers[req.TriangleIdx + i];
                for (var axis = Axis.X; axis <= Axis.Z; axis++) {
                    var temp = SurfaceAreaHeuristic(center, axis, req, term);
                    if (minHeuristic > temp) {
                        minHeuristic = temp;
                        bestAxis = axis;
                        bestCenter = center;
                    }    
                }
            }

            aabbs[req.NodeIdx].LeftIdx = aabbs.Count;
            var start = req.TriangleIdx;
            var end = req.TriangleIdx + triangleCnt - 1;
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
            
            
            stack.Push(new(aabbs.Count, req.TriangleIdx));
            aabbs.Add(smaller);
            stack.Push(new(aabbs.Count, start));
            aabbs.Add(bigger);
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
        
        float SurfaceAreaHeuristic(Vector pPos, Axis pAxis, DevideRequset pReq, int pTerm) {
            var triangleCnt = aabbs[pReq.NodeIdx].TriangleCnt;
            var big = new AABB();
            var small = new AABB();
            var bigCnt = 0;
            var smallCnt = 0;
            var pos = GetAxis(pPos, pAxis);
            for (int i = 0; i < triangleCnt; i += pTerm) {
                if (pos < GetAxis(centers[pReq.TriangleIdx + i], pAxis)) {
                    bigCnt++;
                    big.Expand(pVertices, pTriangleIndies[pReq.TriangleIdx + i]);
                }
                else {
                    smallCnt++;
                    small.Expand(pVertices, pTriangleIndies[pReq.TriangleIdx + i]);
                }
            }

            if (bigCnt == 0 || smallCnt == 0) return float.MaxValue;
            var sum = new AABB(big, small);
            var sumVolume = sum.SurfaceArea();
            return bigCnt * big.SurfaceArea() / sumVolume + smallCnt * small.SurfaceArea() / sumVolume;
        }
    }
}