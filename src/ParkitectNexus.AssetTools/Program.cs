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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using ParkitectNexus.AssetMagic;
using ParkitectNexus.AssetMagic.Readers;
using ParkitectNexus.AssetMagic.Writers;
using ParkitectNexus.AssetTools.OptionSets;

namespace ParkitectNexus.AssetTools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string verb = null;
            object subOptions = null;

            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options,
                (v, o) =>
                {
                    verb = v;
                    subOptions = o;
                }))
            {
                Environment.Exit(Parser.DefaultExitCodeFail);
            }

            switch (verb)
            {
                case "blueprint":
                    ProcesssBlueprint(subOptions as ProcessBlueprintSubOptions);
                    break;
                case "savegame":
                    ProcessSavegame(subOptions as ProcessSavegameSubOptions);
                    break;
                default:
                    Console.WriteLine(options.GetUsage(null));
                    Environment.Exit(Parser.DefaultExitCodeFail);
                    break;
            }
        }

        private static void AssertFileOfType(string path, string extension)
        {
            if (extension == null) throw new ArgumentNullException(nameof(extension));

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine("specified path does not exist");
                Environment.Exit(1);
            }
            if (Path.GetExtension(path) != extension)
            {
                Console.WriteLine("invalid file type");
                Environment.Exit(1);
            }
        }

        private static void ProcesssBlueprint(ProcessBlueprintSubOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            AssertFileOfType(options.Input, ".png");

            try
            {
                using (var bitmap = (Bitmap) Image.FromFile(options.Input))
                {
                    var blueprintReader = new BlueprintReader();
                    var blueprint = blueprintReader.Read(bitmap);

                    if (options.DumpData)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(new BlueprintDump(blueprint),
                            new JsonSerializerSettings
                            {
                                ContractResolver = new DumpDataResolver(
                                    "Type",
                                    "Data",
                                    "IsEmpty",
                                    "Id",
                                    "ContentTypes",
                                    "Position",
                                    "Rotation",
                                    "CarColors",
                                    "TrackColors",
                                    "TrackId",
                                    "StationControllers"
                                    )
                            }));
                    }

                    if (options.Output != null)
                    {
                        using (var g = Graphics.FromImage(bitmap))
                        {
                            // Remove default logo.
                            if (!string.IsNullOrWhiteSpace(options.Background))
                                using (var bg = Image.FromFile(options.Background))
                                {
                                    var path = new GraphicsPath();
                                    path.AddPolygon(new[]
                                    {
                                        new Point(0, 437),
                                        new Point(175, 437),
                                        new Point(175, 437),
                                        new Point(200, 465),
                                        new Point(200, 511),
                                        new Point(0, 511)
                                    });

                                    g.SetClip(path);
                                    g.DrawImage(bg, new Rectangle(new Point(), bg.Size));
                                }

                            // Replace with custom logo.
                            if (!string.IsNullOrWhiteSpace(options.Logo))
                                using (var logo = Image.FromFile(options.Logo))
                                {
                                    g.SetClip(new Rectangle(new Point(), bitmap.Size));
                                    g.DrawImage(logo,
                                        new Rectangle(new Point(options.LogoX, options.LogoY),
                                            new Size(options.LogoWidth, options.LogoHeight)));
                                }
						}

						var blueprintWriter = new BlueprintWriter();
						blueprintWriter.Write(blueprint, bitmap);

						bitmap.Save(options.Output);
                    }
                }
            }
            catch (InvalidBlueprintException)
            {
                Console.WriteLine("invalid blueprint");
                Environment.Exit(1);
            }
        }

        private static void ProcessSavegame(ProcessSavegameSubOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            AssertFileOfType(options.Input, ".txt");

            try
            {
                using (var stream = File.OpenRead(options.Input))
                {
                    var savegameReader = new SavegameReader();
                    var savegame = savegameReader.Deserialize(stream);

                    if (options.DumpData)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(new SavegameDump(savegame),
                            new JsonSerializerSettings
                            {
                                ContractResolver = new DumpDataResolver(
                                    "Type",
                                    "Data",
                                    "IsEmpty",
                                    "Id",
                                    "JobAgency",
                                    "Patches",
                                    "Zones",
                                    "Transactions"
                                    )
                            }));
                    }
                }
            }
            catch (InvalidSavegameException)
            {
                Console.WriteLine("invalid savegame");
                Environment.Exit(1);
            }
        }
    }
}