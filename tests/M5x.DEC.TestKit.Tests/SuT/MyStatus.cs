using System;

namespace M5x.DEC.TestKit.Tests.SuT;

[Flags]
public enum MyStatus
{
    Unknown = 0,
    Pending = 1,
    Initialized = 2
}