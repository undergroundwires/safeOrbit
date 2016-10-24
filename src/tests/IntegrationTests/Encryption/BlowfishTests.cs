﻿using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SafeOrbit.Encryption;
using SafeOrbit.Memory;
using SafeOrbit.Tests.Cases;
using SafeOrbit.Tests;
using SafeOrbit.Vectors;

namespace SafeOrbit.IntegrationTests.Encryption
{
    [TestFixture]
    public class BlowfishTests : TestsFor<IFastEncryptor>
    {
        protected override IFastEncryptor GetSut()
        {
            return new BlowfishEncryptor();
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Encrypt_returnsDifferentValueThanInput(byte[] input, byte[] key)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var output = sut.Encrypt(input, key);
            //Assert
            Assert.IsFalse(input.SequenceEqual(output));
        }


        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ThreeDifferentByteArrays16Length))]
        public void Encrypt_WhenDifferentKeysGiven_returnsDifferentValues(byte[] input, byte[] key, byte[] differentKey)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var output = sut.Encrypt(input, key);
            var outputFromDifferentKey = sut.Encrypt(input, differentKey);
            //Assert
            Assert.IsFalse(output.SequenceEqual(outputFromDifferentKey));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Decrypt_Ecb_WhenDecryptedWithEncryptionKey_returnsInput(byte[] input, byte[] key)
        {
            //Arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Ecb); 
            var encrypted = sut.Encrypt(input, key);
            //Act
            var output = sut.Decrypt(encrypted, key);
            //Assert
            Assert.IsTrue(input.SequenceEqual(output));
        }
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Decrypt_Cbc_WhenDecryptedWithEncryptionKey_returnsInput(byte[] input, byte[] key)
        {
            //Arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Cbc);
            var encrypted = sut.Encrypt(input, key);
            //Act
            var output = sut.Decrypt(encrypted, key);
            //Assert
            Assert.IsTrue(input.SequenceEqual(output));
        }

        [Test]
        [TestCaseSource(typeof(BlowfishVectors), nameof(BlowfishVectors.Vectors))]
        public void Encrypt_Ecb_ReturnsVector(byte[] keyBytes, byte[] clearBytes, byte[] cipherBytes)
        {
            //arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Ecb);
            var expected = cipherBytes;
            //act
            var actual = sut.Encrypt(clearBytes, keyBytes);
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        public void Encrypt_KeySizeIs0_CanEncrypt()
        {
            throw new System.NotImplementedException();
        }
        
    }
}