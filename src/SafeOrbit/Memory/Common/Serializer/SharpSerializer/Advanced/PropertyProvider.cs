﻿
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeInfo = SafeOrbit.Memory.Serialization.SerializationServices.Serializing.TypeInfo;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Advanced
{
    /// <summary>
    ///     Provides properties to serialize from source object.
    /// </summary>
    /// <remarks>
    ///     Its methods <see cref="GetAllProperties" /> and <see cref="IgnoreProperty" /> can be overwritten in an inherited
    ///     class to customize its functionality.
    /// </remarks>
    internal class PropertyProvider
    {
#if !PORTABLE
        [ThreadStatic]
#endif
        private static PropertyCache _cache;

        private static PropertyCache Cache => _cache ?? (_cache = new PropertyCache());

        /// <summary>
        ///     Gives all properties back which:
        ///     - are public
        ///     - are not static
        ///     - does not contain ExcludeFromSerializationAttribute
        ///     - have their set and get accessors
        ///     - are not indexers
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public IList<PropertyInfo> GetProperties(TypeInfo typeInfo)
        {
            // Search in cache
            var propertyInfos = Cache.TryGetPropertyInfos(typeInfo.Type);
            if (propertyInfos != null)
                return propertyInfos;

            // Creating infos
            var properties = GetAllProperties(typeInfo.Type);
            var result = properties
                .Where(property => !IgnoreProperty(typeInfo, property))
                .ToList();

            // adding result to Cache
            Cache.Add(typeInfo.Type, result);

            return result;
        }

        /// <summary>
        ///     Should the property be removed from serialization?
        /// </summary>
        /// <param name="info"></param>
        /// <param name="property"></param>
        /// <returns>
        ///     true if the property:
        ///     - is in the PropertiesToIgnore,
        ///     - contains ExcludeFromSerializationAttribute,
        ///     - does not have it's set or get accessor
        ///     - is indexer
        /// </returns>
        protected virtual bool IgnoreProperty(TypeInfo info, PropertyInfo property)
        {
            if (!property.CanRead || !property.CanWrite)
                return true;

            var indexParameters = property.GetIndexParameters(); // remove indexer
            if (indexParameters.Length > 0)
                return true;

            return false;
        }

        /// <summary>
        ///     Gives all properties back which:
        ///     - are public
        ///     - are not static (instance properties)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual PropertyInfo[] GetAllProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}