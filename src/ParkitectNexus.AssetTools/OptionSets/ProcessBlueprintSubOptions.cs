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

using CommandLine;

namespace ParkitectNexus.AssetTools.OptionSets
{
    public class ProcessBlueprintSubOptions : FileProcessingSubOptions
    {
        [Option("output", HelpText = "The output path")]
        public string Output { get; set; }

        [Option("background", HelpText = "Path to the background image", DefaultValue = "blueprint_bg.png")]
        public string Background { get; set; }

        [Option("logo", HelpText = "Path to the background image", DefaultValue = "logo.png")]
        public string Logo { get; set; }

        [Option("logo-x", HelpText = "target X-position of logo", DefaultValue = 14)]
        public int LogoX { get; set; }

        [Option("logo-y", HelpText = "target y-position of logo", DefaultValue = 418)]
        public int LogoY { get; set; }

        [Option("logo-width", HelpText = "target width of logo", DefaultValue = 160)]
        public int LogoWidth { get; set; }

        [Option("logo-height", HelpText = "target height of logo", DefaultValue = 80)]
        public int LogoHeight { get; set; }
    }
}