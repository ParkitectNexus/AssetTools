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
using System.Linq;
using ParkitectNexus.AssetMagic;
using ParkitectNexus.AssetMagic.Data.Blueprints;
using ParkitectNexus.AssetMagic.Data.Coasters;

namespace ParkitectNexus.AssetTools
{
    public class BlueprintDump
    {
        public BlueprintDump(IBlueprint blueprint)
        {
            if (blueprint == null) throw new ArgumentNullException(nameof(blueprint));
            Header = blueprint.Header;

            if (Header.TrackedRideTypes == null)
                Header.TrackedRideTypes = Header.Types;

            Coaster = blueprint.Coasters.FirstOrDefault();
        }

        public BlueprintHeader Header { get; set; }
        public Coaster Coaster { get; set; }
    }
}