namespace Rotation;

public static class ExTransform {
    
    public static Vector CancelTransform(this ITransform pTransform, Vector pPos) =>
        pTransform.Rotation.Flip().Rotate((pPos - pTransform.Pos) / pTransform.Scale);
    public static Vector ApplyTransform(this ITransform pTransform, Vector pPos) =>
        pTransform.Rotation.Rotate(pPos) * pTransform.Scale + pTransform.Pos;
	
}