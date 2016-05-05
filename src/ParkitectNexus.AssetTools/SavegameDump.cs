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
using ParkitectNexus.AssetMagic.Data.Savegames;

namespace ParkitectNexus.AssetTools
{
    public class SavegameDump
    {
        public SavegameDump(ISavegame savegame)
        {
            if (savegame == null) throw new ArgumentNullException(nameof(savegame));
           
            var h = savegame.Header;
            Header = new SavegameHeader {
                ActiveMods = h.ActiveMods,
                GuestCount = h.GuestCount,
                Money = h.Money,
                Name = h.Name,
                ParkDate = h.ParkDate,
                ParkRating = h.ParkRating,
                Screenshot = h.Screenshot,
                TimePlayed = h.TimePlayed,
                Date = h.Date,
                GameVersion = h.GameVersion,
                GameVersionName = h.GameVersionName,
                SavegameVersion = h.SavegameVersion,
                Type = h.Type
            };

            var p = savegame.Park;
            Park = new Park {
                Guid = p.Guid,
                Id = p.Id,
                ParkInfo = p.ParkInfo,
                ParkName = p.ParkName,
                Patches = p.Patches,
                SendGuestsHome = p.SendGuestsHome,
                Settings = p.Settings,
                SpawnedAtTime = p.SpawnedAtTime,
                XSize = p.XSize,
                YSize = p.YSize,
                ZSize = p.ZSize,
                Type = p.Type
            };
            GuestCount = (int)h.GuestCount;
        }

        public SavegameHeader Header { get; set; }

        public Park Park { get; set; }

        public int GuestCount { get; set; }
    }
}