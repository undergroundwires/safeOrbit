﻿#if !NETSTANDARD1_6
using System;
using System.Security.Permissions;
using System.Runtime.Serialization;
#endif

namespace SafeOrbit.Exceptions
{
    /// <summary>
    ///     This type of exception is thrown when trying to modify an object with only read only access.
    /// </summary>
    /// <remarks>
    ///     TODO: rename this class.
    /// </remarks>
#if !NETSTANDARD1_6
    [Serializable]
#endif
    public class ReadOnlyAccessForbiddenException : SafeOrbitException
    {
        public ReadOnlyAccessForbiddenException(string message) : base(message)
        {
        }

#if !NETSTANDARD1_6
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public ReadOnlyAccessForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}