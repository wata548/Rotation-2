namespace Rotation.Ray;

public partial class BVH {
    private class AABBNode {
        public const int MaxLeafTriangleCnt = 4;
        public int TriangleCnt { get; set; }
        public int TriangleIdx { get; set; }
        public int LeftIdx { get; set; }
        public int RightIdx => LeftIdx + 1;
        public AABB AABB { get; set; } = new();
        public bool IsLeaf => TriangleCnt <= MaxLeafTriangleCnt;
    }
}