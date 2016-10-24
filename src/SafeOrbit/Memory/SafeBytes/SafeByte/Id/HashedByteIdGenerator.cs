﻿using System.Collections.Generic;
using System.Linq;
using SafeOrbit.Exceptions;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Utilities;

namespace SafeOrbit.Memory.SafeBytesServices.Id
{
    using System;
    using Hash;
    using Random;
    /// <summary>
    /// Creates a unique <see cref="int" /> values for each <see cref="byte"/>.
    /// The class is stateless but session-based. <see cref="SessionSalt"/> is different values each time the application is loaded.
    /// </summary>
    /// <seealso cref="SessionSalt"/>
    /// <seealso cref="IByteIdGenerator" />
    internal class HashedByteIdGenerator : IByteIdGenerator
    {
        private bool _isSaltEncrypted;
        private const int SaltLength = 16;
        public byte[] SessionSalt => _sessionSalt = _sessionSalt ?? GenerateSessionSalt();
        private static byte[] _sessionSalt;
        private readonly IFastHasher _fastHasher;
        private readonly ISafeRandom _safeRandom;
        private readonly IByteArrayProtector _memoryProtector;
        /// <exception cref="MemoryInjectionException">If the object has been changed outside of <see cref="LibraryManagement.Factory"/>.</exception>
        public HashedByteIdGenerator() : this(
            LibraryManagement.Factory.Get<IFastHasher>(),
            LibraryManagement.Factory.Get<ISafeRandom>(),
            LibraryManagement.Factory.Get<IByteArrayProtector>())
        {

        }
        internal HashedByteIdGenerator(IFastHasher fastHasher, ISafeRandom safeRandom, IByteArrayProtector memoryProtector)
        {
            if (fastHasher == null) throw new ArgumentNullException(nameof(fastHasher));
            if (safeRandom == null) throw new ArgumentNullException(nameof(safeRandom));
            if (memoryProtector == null) throw new ArgumentNullException(nameof(memoryProtector));
            _fastHasher = fastHasher;
            _safeRandom = safeRandom;
            _memoryProtector = memoryProtector;
        }
        /// <summary>
        /// Clears the session salt. A new session salt will created lazily upon request. Please keep in my that requesting
        /// a new session salt will break all of the SafeOrbit instances and should never be used at all.
        /// </summary>
        internal static void ClearSessionSalt() => _sessionSalt = null;
        public int Generate(byte b) => GenerateInternal(b, SessionSalt, ref _isSaltEncrypted);
        private int GenerateInternal(byte b, byte[] salt, ref bool isSaltEncrypted, bool doNotEncrypt = false)
        {
            var byteBuffer = new byte[salt.Length + 1];
            try
            {
                //Decrypt salt
                if (isSaltEncrypted)
                {
                    _memoryProtector.Unprotect(salt);
                    isSaltEncrypted = false;
                }
                //Append salt + byte
                Array.Copy(salt, byteBuffer, salt.Length);
                byteBuffer[salt.Length] = b;
                //Hash it
                var result = _fastHasher.ComputeFast(byteBuffer);
                return result;
            }
            finally
            {
                Array.Clear(byteBuffer, 0, byteBuffer.Length);
                //Encrypt the salt
                if (!isSaltEncrypted && !doNotEncrypt)
                {
                    _memoryProtector.Protect(salt);
                    isSaltEncrypted = true;
                }
            }
        }

        /// <summary>
        /// Recursive method that generates and returns a validated session salt.
        /// </summary>
        /// <seealso cref="IsSaltValid"/>
        private byte[] GenerateSessionSalt()
        {
            var salt = _safeRandom.GetBytes(SaltLength);
            if (IsSaltValid(salt))
            {
                _memoryProtector.Protect(salt);
                _isSaltEncrypted = true;
                return salt;
            }
            return GenerateSessionSalt(); //try with a new random value
        }
        /// <summary>
        /// Determines if all byte id's will have different result with the given salt.
        /// </summary>
        private bool IsSaltValid(byte[] salt)
        {
            const int totalBytes = 256;
            //generate all possible bytes
            var byteHashes = GetHashesForAllBytes(salt, false);
            //check if they're all unique.
            var totalUniqueHashes = byteHashes.Distinct().Count();
            return totalUniqueHashes == totalBytes;
        }

        private int[] GetHashesForAllBytes(byte[] salt, bool isEncrypted)
        {
            const int totalBytes = 256;
            var byteHashes = new int[totalBytes];
            //Fast.For(0, totalBytes, (i) => byteHashes[i] = this.GenerateInternal((byte)i, salt, ref isEncrypted));
            for (var i = 0; i < 256; i++) byteHashes[i] = GenerateInternal((byte)i, salt, ref isEncrypted, true);
            return byteHashes;
        }
    }
}
