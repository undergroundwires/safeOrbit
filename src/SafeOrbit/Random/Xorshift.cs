﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Random
{
    /// <summary>
    /// This random alghoritm is around 29x times faster than <see cref="System.Random"/>.
    /// This alghoritm has no crypthological value and is predictable.
    /// </summary>
    public class Xorshift : RandomNumberGenerator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe void GetBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            int offset = 0, offsetEnd = data.Length;
            uint x = 548787455, y = 842502087, z = 3579807591, w = 273326509;
            fixed (byte* pbytes = data)
            {
                uint* pbuf = (uint*)(pbytes + offset);
                uint* pend = (uint*)(pbytes + offsetEnd);
                while (pbuf < pend)
                {
                    uint tx = x ^ (x << 11);
                    uint ty = y ^ (y << 11);
                    uint tz = z ^ (z << 11);
                    uint tw = w ^ (w << 11);
                    *(pbuf++) = x = w ^ (w >> 19) ^ (tx ^ (tx >> 8));
                    *(pbuf++) = y = x ^ (x >> 19) ^ (ty ^ (ty >> 8));
                    *(pbuf++) = z = y ^ (y >> 19) ^ (tz ^ (tz >> 8));
                    *(pbuf++) = w = z ^ (z >> 19) ^ (tw ^ (tw >> 8));
                }
            }
        }
    }
}
