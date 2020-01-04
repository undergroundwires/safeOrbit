﻿#if !NETSTANDARD1_6 && !NETSTANDARD2_0
using System;
using System.Security.Cryptography;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector
{
    /// <summary>
    ///     .NET Framework implementation of <see cref="IByteArrayProtector" />.
    ///     Wrapper for <see cref="ProtectedMemory" />.
    /// </summary>
    /// <remarks>
    ///     .NET offers <see cref="ProtectedMemory" /> and <see cref="ProtectedData" /> classes.
    ///     The main difference between is that if the computer is rebooted between calls to Protect and Unprotect,
    ///     you won't be able to decrypt the data with <see cref="ProtectedMemory" />.
    /// </remarks>
    /// <seealso cref="IByteArrayProtector" />
    public partial class MemoryProtector : IByteArrayProtector
    {
        public int BlockSizeInBytes => 16;

        public void Protect(byte[] userData)
        {
            EnsureParameter(userData);
            if (userData == null) throw new ArgumentNullException(nameof(userData));
            ProtectedMemory.Protect(userData, MemoryProtectionScope.SameProcess);
        }

        public void Unprotect(byte[] encryptedData)
        {
            EnsureParameter(encryptedData);
            ProtectedMemory.Unprotect(encryptedData, MemoryProtectionScope.SameProcess);
        }
    }
}
#endif