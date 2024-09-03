using System;
using System.Collections.Generic;
using System.Linq;

namespace Rikitav.IO.ExtensibleFirmware.BootService
{
    internal static class BS_ValueFormatHelper
    {
        public static string GetLoadOptionNameFromIndex(ushort OptIndex)
            => "Boot" + OptIndex.ToString("X").PadLeft(4, '0');

        public static ushort GetIndexFromLoadOptionName(string OptName)
        {
            // Format check
            if (!OptName.StartsWith("Boot") || OptName.Length != 8)
                throw new FormatException("The boot option name is not formatted correctly. The name must be equal to \"Boot####\", where \"####\" is the option index");

            // Parsing
            return ushort.Parse(OptName.Substring("Boot".Length, 4));
        }

        public static ushort? GetFirstFreeLoadOptionName(ushort[] loadOptionsOrder)
        {
            for (ushort i = 0; i < 256; i++)
            {
                if (!loadOptionsOrder.Contains(i))
                    return i;
            }

            return null;
        }

        public static T[] MoveToFront<T>(T[] array, Predicate<T> match)
        {
            T[] newArray = new T[array.Length];
            int idx = Array.FindIndex(array, match);

            if (idx == -1)
                throw new KeyNotFoundException(); // matching value not found

            if (idx == 0) // move only if not already in front
                return array;

            bool moved = false;
            for (int i = 0; i < array.Length; i++)
            {
                if (i == idx)
                {
                    newArray[0] = array[i];
                    moved = true;
                }
                else
                {
                    newArray[i + (moved ? 0 : 1)] = array[i];
                }
            }

            return newArray;
        }
    }
}
