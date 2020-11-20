using System;

namespace GatewayBranch.Extensions
{
    public static class HexExtensions
    {
        readonly static char[] HexdumpTable;
        static HexExtensions()
        {
            HexdumpTable = new char[1024];
            char[] array = "0123456789ABCDEF".ToCharArray();
            for (int i = 0; i < 256; i++)
            {
                HexdumpTable[i << 1] = array[((uint)i >> 4) & 0xF];
                HexdumpTable[(i << 1) + 1] = array[i & 0xF];
            }
        }

        public static string ToHexString(this byte[] array)
        {
            return DoHexDump(array, 0, array.Length);
        }

        static string DoHexDump(byte[] array, int fromIndex, int length)
        {
            if (length == 0)
            {
                return "";
            }
            int num = fromIndex + length;
            char[] array2 = new char[(length * 3) - 1];
            int num2 = fromIndex;
            int num3 = 0;
            while (num2 < num)
            {
                if (num3 != 0)
                    array2[num3++] = ' ';
                Array.Copy(HexdumpTable, (array[num2] & 0xFF) << 1, array2, num3, 2);
                num2++;
                num3 += 2;
            }
            return new string(array2);
        }
    }
}