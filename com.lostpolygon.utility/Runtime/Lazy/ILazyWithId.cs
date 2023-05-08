namespace LostPolygon.Unity.Utility {
    public interface ILazyWithId<out TId, out TValue> {
        TId Id { get; }
        TValue Value { get; }
    }
}
