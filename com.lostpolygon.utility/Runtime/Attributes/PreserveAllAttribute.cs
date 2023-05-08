using System;
using UnityEngine.Scripting;

namespace LostPolygon.Unity.Utility {
    /// <summary>
    /// Prevents stripping of any members of a type and of its nested types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PreserveAllAttribute : PreserveAttribute {
    }
}
