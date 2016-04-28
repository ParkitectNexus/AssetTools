// ParkitectNexus.AssetTools
// Copyright 2016 Tim Potze
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace ParkitectNexus.AssetTools
{
    internal class FontTools
    {
        public static FontFamily LoadFontFamily(byte[] buffer, out PrivateFontCollection fontCollection)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                fontCollection = new PrivateFontCollection();
                fontCollection.AddMemoryFont(ptr, buffer.Length);
                return fontCollection.Families[0];
            }
            finally
            {
                handle.Free();
            }
        }

        public static FontFamily LoadFontFamily(Stream stream, out PrivateFontCollection fontCollection)
        {
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return LoadFontFamily(buffer, out fontCollection);
        }
    }
}