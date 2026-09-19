namespace Rotation.Ray;

public partial class BVH {
    public record struct AABBNode(int TriangleCnt, int TriangleIdx, int LeftIdx, AABB AABB) {
        public const int MaxLeafTriangleCnt = 3;
        public int RightIdx => LeftIdx + 1;
        public bool IsLeaf => TriangleCnt <= MaxLeafTriangleCnt;
    }
}