using System.Runtime.Serialization;

namespace M5x.Chassis.Mh.Core;

/// <summary> A marker for types that can copy themselves to another type </summary>
public interface ICopyable<out T>
{
    [IgnoreDataMember] T Copy { get; }
}