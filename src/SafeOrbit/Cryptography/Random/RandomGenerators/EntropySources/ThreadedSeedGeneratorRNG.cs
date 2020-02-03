﻿using System;
using System.Security.Cryptography;
using System.Threading;
using SafeOrbit.Cryptography.Random.RandomGenerators.Crypto.Prng;
using SafeOrbit.Memory;
using SafeOrbit.Threading;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <inheritdoc />
    /// <summary>
    ///     Starts a thread in a tight increment loop, while another thread samples the variable being incremented.
    ///     Entropy is generated by the OS thread scheduler, not knowing how many times the first thread will loop in
    ///     the period of time the second thread loops once.
    /// </summary>
    /// <remarks>
    ///     A simple wrapper around BouncyCastle <see cref="ThreadedSeedGenerator"/>.
    ///     It is good to use <see cref="ThreadedSeedGeneratorRng"/> as one of the entropy sources, but not all by itself,
    ///     because thread scheduling is deterministically controlled by your OS, and easily influenced by outsiders.
    /// </remarks>
    public sealed class ThreadedSeedGeneratorRng : RandomNumberGenerator
    {
        /// <summary>
        ///     The amounts of bytes available for read that this instance of <see cref="ThreadedSeedGeneratorRng"/> will always try to fill up to.
        /// </summary>
        public int MaxPoolSize { get; }


        private readonly object _fifoStreamLock = new object();
        private readonly SafeMemoryStream _bytes = new SafeMemoryStream();
        private readonly AutoResetEvent _poolFullAre = new AutoResetEvent(false);
        private readonly ThreadedSeedGenerator _threadedSeedGenerator = new ThreadedSeedGenerator();
        
        private int _disposed = IntCondition.False;

        public ThreadedSeedGeneratorRng()
        {
            MaxPoolSize = 4096;
            var mainThread = new Thread(MainThreadLoop)
            {
                IsBackground = true // Don't prevent application from dying if it wants to.
            };
            mainThread.Start();
        }

        /// <inheritdoc cref="ThrowIfDisposed"/>
        public override void GetBytes(byte[] data)
        {
            this.ThrowIfDisposed();
            if (Read(data, 0, data.Length) != data.Length)
                throw new CryptographicException("Failed to return requested number of bytes");
        }


#if !NETSTANDARD1_6
        /// <inheritdoc cref="ThrowIfDisposed"/>
        public override void GetNonZeroBytes(byte[] data)
        {
            this.ThrowIfDisposed();
            var offset = 0;
            while (offset < data.Length)
            {
                var newBytes = new byte[data.Length - offset];
                if (Read(newBytes, 0, newBytes.Length) != newBytes.Length)
                    throw new CryptographicException("Failed to return requested number of bytes");
                foreach (var newByte in newBytes)
                {
                    if (newByte == 0) continue;
                    data[offset] = newByte;
                    offset++;
                }

            }
        }
#endif

        /// <summary>
        ///     When overridden in a derived class, releases the unmanaged resources used by the
        ///     <see cref="System.Security.Cryptography.RandomNumberGenerator" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     true to release both managed and unmanaged resources; false to release only unmanaged
        ///     resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref _disposed, IntCondition.True) == IntCondition.True)
                return;
            if (disposing)
            {
                _poolFullAre?.Dispose();
                lock (_fifoStreamLock)
                {
                    _bytes?.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc cref="ThrowIfDisposed"/>
        private int Read(byte[] buffer, int offset, int count)
        {
            this.ThrowIfDisposed();
            try
            {
                var pos = offset;
                lock (_fifoStreamLock)
                {
                    while (pos < count)
                    {
                        var readCount = _bytes.Length; // All the available bytes
                        if (pos + readCount >= count)
                            readCount = count - pos; // Don't try to read more than we need
                        if (readCount > 0)
                        {
                            var bytesRead = _bytes.Read(buffer, pos, (int)readCount);
                            pos += bytesRead;
                        }

                        if (pos < count)
                            Thread.Sleep(1);
                    }
                    return count;
                }
            }
            finally
            {
                _poolFullAre.Set();
            }
        }


        private void MainThreadLoop()
        {
            try
            {
                // ThreadedSeedGenerator performs better with large hunks of data, but if we just use MaxPoolSize, then the whole
                // thing gets drained before it starts refilling.  In effect, the pool will drain by one unit of byteCount, before it
                // starts refilling, and likewise, there will be zero bytes available until at least one unit of byteCount becomes
                // available.  So there's a balancing act happening here... Faster throughput versus faster response time...
                // Divide by 8 seems to be a reasonable compromise between the two.
                var byteCount = MaxPoolSize / 8;
                while (true)
                {
                    if (_bytes.Length >= MaxPoolSize)
                    {
                        _poolFullAre.WaitOne();
                        continue; // The only time we ever quit is on the terminate signal ... interrupt signal ... whatever.  OS kills the thread.
                    }
                    var newBytes = new byte[byteCount];
                    // By my measurements, estimated entropy returned by ThreadedSeedGenerator is approx 0.6 or 0.7 bits per bit
                    // when fast=false, and 0.5 when fast=true.  Occasionally we see measurements between 0.4 and 0.5. So round this 
                    // down to 0.125, and just generate 8x as much data as you need. And mix it.
                    for (var i = 0; i < 8; i++)
                    {
                        var maskBytes = _threadedSeedGenerator.GenerateSeed(byteCount, true);
                        for (var j = 0; j < newBytes.Length; j++)
                            newBytes[j] ^= maskBytes[j];
                        Array.Clear(maskBytes, 0, maskBytes.Length);
                    }
                    _bytes.Write(newBytes, 0, newBytes.Length);
                }
            }
            catch
            {
                if (!IsDisposed())
                    throw;
                // Swallow any exceptions if we get disposed in the middle of doing stuff
                // TODO: Gracefully terminate instead.
            }
        }

        ~ThreadedSeedGeneratorRng()
        {
            Dispose(false);
        }

        /// <exception cref="ObjectDisposedException"><see cref="ThreadedSeedGeneratorRng"/> instance is disposed</exception>
        private void ThrowIfDisposed()
        {
            if (IsDisposed())
                throw new ObjectDisposedException(GetType().Name);
        }
        private bool IsDisposed()
        {
#if !NETSTANDARD1_6
            return Thread.VolatileRead(ref this._disposed) == IntCondition.True;
#else
            return this._disposed == IntCondition.True;
#endif
        }
    }
}