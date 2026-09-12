namespace Rotation.Ray;

public partial class BVH {
    private IReadOnlyList<AABBNode> _hirearchy;

    public RayResult RayCasting(IMesh pMesh, ITransform pTransform, Ray pRay) {
        var localRay = pRay.ApplyTransform(pTransform);
        var stack = new Stack<int>();
        var result = new RayResult(default, 2);
        stack.Push(0);
        while (stack.Count > 0) {
            var node = _hirearchy[stack.Pop()];
            if(!localRay.VsAABB(node.AABB, result.Ratio)) 
                continue;

            if (!node.IsLeaf) {
                stack.Push(node.LeftIdx);
                stack.Push(node.RightIdx);
                continue;
            }

            for (int i = 0; i < node.TriangleCnt; i++) {
                var triangle = pMesh.TriangleIndies[node.TriangleIdx + i];
                var vsTriangle = localRay.VsTriangle(
                    pMesh.Vertices,
                    triangle,
                    out var t
                );
                if (vsTriangle && t < result.Ratio) {
                    result = new(triangle, t);
                }
            }
        }
        return result;
    }
}