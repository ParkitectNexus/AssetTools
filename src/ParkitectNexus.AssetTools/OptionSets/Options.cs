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
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace ParkitectNexus.AssetTools.OptionSets
{
    public class Options
    {
        [VerbOption("blueprint", HelpText = "Extract information from a blueprint")]
        public FileProcessingSubOptions BlueprintVerb { get; set; }

        [VerbOption("savegame", HelpText = "Extract information from a savegame")]
        public FileProcessingSubOptions SavegameVerb { get; set; }

        [VerbOption("blueprint-convert", HelpText = "Convert design of a blueprint")]
        public BlueprintConvertSubOptions BlueprintConvertVerb { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            var help = new HelpText
            {
                Heading = new HeadingInfo(name.Name, name.Version.ToString()),
                Copyright = new CopyrightInfo("ParkitectNexus", DateTime.Now.Year),
                AdditionalNewLineAfterOption = false,
                MaximumDisplayWidth = 80,
                AddDashesToOption = verb != null
            };
            help.AddPreOptionsLine(" ");
            help.AddPreOptionsLine($"usage: {name.Name} {verb ?? "<command>"}");

            var optionsInstanceByName = verb == null
                ? null
                : GetType()
                    .GetProperties()
                    .FirstOrDefault(p => p.GetCustomAttribute<VerbOptionAttribute>()?.LongName == verb)
                    .GetValue(this);

            help.AddOptions(optionsInstanceByName ?? this);
            return help;
        }
    }
}