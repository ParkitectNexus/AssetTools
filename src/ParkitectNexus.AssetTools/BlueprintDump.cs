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
using ParkitectNexus.AssetMagic;
using ParkitectNexus.AssetMagic.Elements;

namespace ParkitectNexus.AssetTools
{
    public class BlueprintDump
    {
        private readonly IBlueprint _blueprint;

        public BlueprintDump(IBlueprint blueprint)
        {
            if (blueprint == null) throw new ArgumentNullException(nameof(blueprint));
            _blueprint = blueprint;
        }

        public IBlueprintHeader Header => _blueprint.Header;
        public ICoaster Coaster => _blueprint.Coaster;
    }
}