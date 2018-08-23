﻿
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Library
{
    internal class InternalsVisibleTo
    {
#if DEBUG
        public const string ToUnitTests = "UnitTests";
        public const string ToPerformanceTests = "SafeOrbit.PerformanceTests";
        public const string ToIntegrationTests = "IntegrationTests";
        public const string ToDynamicProxyGenAssembly2 = "DynamicProxyGenAssembly2";
#else
        public const string ToUnitTests = "UnitTests, PublicKey=00240000048000009400000006020000002400005253413100040000010001005bc5f29f3c74f543ea44b37448fc2bc7bedf2aa3f751fa20bf96132f15728c00e811e813ad74c65b3c3d9f6bbe565e3bee562a2aec40f3fec2e62646ae3fd6b018269d66a002e34bf35ab253ccd29a8e8c32b3d7fd4f89a71f36a2d365dbec47a024dac8d78893db2dbdaf520769f5c9bf0727eaba877b6e40dcd9b976a3faa3";
        public const string ToPerformanceTests = "SafeOrbit.PerformanceTests, PublicKey=002400000480000094000000060200000024000052534131000400000100010093766cb44eb55f438844d86a7dd3fa35ed7d1979499f503a4ee040bbd90bd6020285d089d27fc69159545e7b8b5bec9d87dc496d6a0d9d76cc7f3a7950758b6278b1230f79276888d41806e9e587affa8f0799473f9322593ea0f8284c766e2b570bf5c007e1038fa7aff1b317a34e291f8241f3e61bf0c3118cf21feac28e8b";
        public const string ToIntegrationTests = "IntegrationTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ed2abddceaf54f71888719a32545c0ac1dbf44a87d7644092caadc0494b0ff0ebc7e32999b09db5bc7804e526639fca6abeb48102ddffd90a118ae28ac7e994110085ef2b76f9c2135e5b7ac3d48be5c7d0fea003c7e7d407aaeabc6b4e2fe9b0b5b5ec3bd05702f6fbbb505c0705dc77a6eac75b2dcc8ba7c92f3ada640f588";
        /// <summary>
        ///     Constant to use when making assembly internals visible to proxy types generated by DynamicProxy. Required when
        ///     proxying internal types.
        /// </summary>
        public const string ToDynamicProxyGenAssembly2 =
            "DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7";
#endif
    }
}
