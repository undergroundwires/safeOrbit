


<img align="left" src="https://raw.githubusercontent.com/undergroundwires/SafeOrbit/master/docs/img/logo/logo_60x60.png"> 

# **SafeOrbit** - Protect your memory in .NET

[![NuGet Status](https://img.shields.io/nuget/v/SafeOrbit.svg?style=flat)](https://www.nuget.org/packages/SafeOrbit/) ![Build status](https://github.com/undergroundwires/SafeOrbit/workflows/Build%20&%20test/badge.svg) [![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/undergroundwires/SafeOrbit/issues)

## What

**SafeOrbit** is an advanced [**memory protection**](#memory-security) library with easy to use classes.

* Protects your strings in memory while allowing you to securely compare & modify them with [SafeString](https://github.com/undergroundwires/SafeOrbit/wiki/SafeString).
* Protects your binary data with [SafeBytes](https://github.com/undergroundwires/SafeOrbit/wiki/SafeBytes).
* Anti injection module safeguards your application against memory injections and timing attacks using [SafeObject](https://github.com/undergroundwires/SafeOrbit/wiki/SafeObject), [SafeContainer](https://github.com/undergroundwires/SafeOrbit/wiki/SafeObject) (injection aware DI container) and [more](https://github.com/undergroundwires/SafeOrbit/wiki).
* Leverages high performance and secure algorithms for [encryption, hashing and random](#cryptography) in interfaces that makes it much hard to screw up.

## Why

* You want to secure strings in memory and modify & compare them without revealing them in memory.
* You want to take advantage of security best-practices without having any cryptology knowledge.
* You want to use high-performance algorithms in .NET such as `Murmur32` hashing and `Blowfish` encryption.
* You do not trust OS generated crypto randoms and want direct access to entropy hashes or non-OS PNRG seeded by them.

## Want to say thanks? :beer:

Hit the :star: star :star: button

## Contribute

Feel free to contribute by joining the coding process or opening [issues](https://github.com/undergroundwires/safeOrbit/issues). [Read more on wiki](https://github.com/undergroundwires/SafeOrbit/wiki/Contribute).

## License

[This project is MIT Licensed](LICENSE). It means that you're free to use **SafeOrbit** freely in any application, copy, and modify its code.

> It must not be required to be secret, and it must be able to fall into the hands of the enemy without inconvenience.
> -[Auguste Kerckhoffs](https://en.wikipedia.org/wiki/Kerckhoffs%27s_principle)

# Quick Documentation

[Visit wiki for full documentation](https://github.com/undergroundwires/SafeOrbit/wiki)

## Memory security

### SafeString <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/SafeBytes)</sub></sup>

* `SafeString` represents an encrypted string that guarantees to not leak your data in the memory while allowing modifications and comparisons.
* It has more advantages over `System.Security.SecureString` because of the security design of the **SafeOrbit**.

#### SafeString vs [System.Security.SecureString](https://msdn.microsoft.com/en-us/library/system.security.securestring(v=vs.110).aspx)

|                              | SecureString | SafeString |
|-----------------------------:|:------------:|:----------:|
|  Supports multiple encodings |       ✖     |     ✔      |
|      Safely character insert |       ✖     |     ✔      |
|      Safely character remove |       ✖     |     ✔      |
|                Safely equals |       ✖     |     ✔      |
|              Safely retrieve |       ✖     |     ✔      |
|      Reveal only single char |       ✖     |     ✔      |
|         Unlimited characters |       ✖     |     ✔      |
|     Timing attack protection |       ✖     |     ✔      |

### SafeBytes <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/SafeBytes)</sub></sup>

* `SafeBytes` is protected sequence of bytes in memory.
* It's a lower level module used by `SafeString`.
* You can hide any data from the memory, then modify and compare them safely without revealing the bytes.

## Detect injections

* You can detect injections for any of your `.NET` class including their
  * the state (data in the memory)
  * code that's loaded in memory
* Internal protection for `SafeOrbit` library be **enabled as default**.
  * You can disable it to gain more performance [by changing SafeOrbit's security settings](https://github.com/undergroundwires/SafeOrbit/wiki/Library-settings#change-security-settings).

### SafeObject <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/SafeObject)</sub></sup>

An object that can detect memory injections to itself.

```C#
    var safeObject = new SafeObject<Customer>();
    // Each change to the object's state or code must be using ApplyChanges
    safeObject.ApplyChanges((customer) => customer.SensitiveInfo = "I'm protected!");
    // Retrieve safe data
    var safeInfo = safeObject.Object.SensitiveInfo; // returns "I'm protected!" or alerts if any injection is detected
```

### SafeContainer <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/SafeContainer)</sub></sup>

* **`SafeContainer`** is a dependency container that detects and notifies injections to its instances.
* It's security mode can be changed dynamically.

### InjectionDetector <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/InjectionDetector)</sub></sup>

* A service that's consumed by `SafeContainer` and `SafeObject`.
* Lowest level of the injection detection and alerting mechanism.

## Cryptography

### Encryption <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/Encryption)</sub></sup>

Supported:

* Asynchronous encryption using [cryptostream](https://msdn.microsoft.com/en-us/library/hh472379(v=vs.110).aspx)s.
* `ISafeEncryptor` a.k.a. **AES-256**
  * Considered as one of the strongest encryption algorithms.
  * Easy-to-use interface using best-practices such as PBKDF2 key derivation, random IV, salt and PKCS7 padding.
* `IFastEncryptor` a.k.a. **Blowfish**
  * Considered as one of the fastest encryption algorithms.
  * ECB & CBC (with IV) implementation that passes the vector tests.

### Hashers <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/Hashers)</sub></sup>

Supported :

* `ISafeHasher` a.k.a. **SHA512** for higher security.
* `IFastHasher` a.k.a. **MurmurHash (Murmur32)** for better performance, it should be seeded and salted.

### Random <sub><sup>[(wiki)](https://github.com/undergroundwires/SafeOrbit/wiki/Random)</sub></sup>

> What if your OS crypto random has in any way been undermined (for example, by a nefarious government agency, or simple incompetence)?

`SafeOrbit` guarantees not to reduce the strength of your crypto random. It has the ability to improve the strength of your crypto random:

* `SafeRandom` combines different entropy sources
* `FastRandom` is a simple wrapper around a PRNG, which uses `SafeRandom` for seed material.

## Speed up

* **For better performance**, it's **highly recommended** to start the application early in your application start with `SafeOrbitCore.Current.StartEarlyAsync();`.

* Memory injection is enabled as default.
  * It provides self security on client side applications, but on a protected server disabling the memory injection for more performance is recommended. [Read more on wiki](https://github.com/undergroundwires/SafeOrbit/wiki/Library-settings#change-security-settings).
