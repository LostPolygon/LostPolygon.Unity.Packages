namespace LostPolygon.Unity.Utility.Editor {
    internal interface IMemberHandle<out TMemberInfo> {
        bool Valid { get; }
        TMemberInfo MemberInfo { get; }
    }
}
