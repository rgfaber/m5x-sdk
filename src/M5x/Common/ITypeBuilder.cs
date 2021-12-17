using System;

namespace M5x.Common;

internal interface ITypeBuilder
{
    Type GenerateType(string className);
}