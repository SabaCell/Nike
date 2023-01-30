using System;

namespace Nike.Framework.Domain.EventSourcing.Equality{

public static class BitConverterUtil
{
    public static int SingleToInt32Bits(float value)
    {
        return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
    }
}
}