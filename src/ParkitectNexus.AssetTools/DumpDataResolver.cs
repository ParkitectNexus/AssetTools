// ParkitectNexus.AssetTools
// Copyright 2015 Tim Potze
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

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ParkitectNexus.AssetTools
{
    public class DumpDataResolver : DefaultContractResolver
    {
        private readonly string[] _excludedProperties;

        public DumpDataResolver(params string[] excludedProperties)
        {
            if (excludedProperties == null) throw new ArgumentNullException(nameof(excludedProperties));
            _excludedProperties = excludedProperties;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return
                base.CreateProperties(type, memberSerialization)
                    .Where(p => !_excludedProperties.Contains(p.PropertyName))
                    .ToList();
        }
    }
}