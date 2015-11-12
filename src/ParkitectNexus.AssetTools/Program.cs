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
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using ParkitectNexus.AssetMagic;
using ParkitectNexus.AssetMagic.Readers;
using ParkitectNexus.AssetMagic.Writers;
using ParkitectNexus.AssetTools.OptionSets;

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
                    ProcesssBlueprint(subOptions as FileProcessingSubOptions);
                    break;
                case "savegame":
                    ProcessSavegame(subOptions as FileProcessingSubOptions);
                    break;
                case "blueprint-convert":
                    ConvertBlueprint(subOptions as BlueprintConvertSubOptions);
                    break;
                default:
                    Console.WriteLine(options.GetUsage(null));
                    Environment.Exit(Parser.DefaultExitCodeFail);
                    break;
            }
        }

        private static void AssertFileOfType(string input, string extension, bool isRaw)
        {
            if (extension == null) throw new ArgumentNullException(nameof(extension));

            if (string.IsNullOrWhiteSpace(input) || (!isRaw && !File.Exists(input)))
            {
                Console.WriteLine("specified path does not exist");
                Environment.Exit(1);
            }
            if (!isRaw && Path.GetExtension(input) != extension)
            {
                Console.WriteLine("invalid file type");
                Environment.Exit(1);
            }
        }

        private static void ConvertBlueprint(BlueprintConvertSubOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            AssertFileOfType(options.Input, ".png", options.IsRaw);

            try
            {

                using (
                    var bitmap = options.IsRaw
                        ? new Bitmap(new MemoryStream(Convert.FromBase64String(options.Input)))
                        : (Bitmap) Image.FromFile(options.Input))
                {
                    var blueprintReader = new BlueprintReader();
                    var blueprint = blueprintReader.Read(bitmap);

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


                        if (!string.IsNullOrWhiteSpace(options.Font) && (options.DrawText?.Any() ?? false))
                        {
                            Font prototypeFont;

                            PrivateFontCollection fonts = null;
                            FontFamily family = null;

                            if (options.Font.EndsWith(".ttf"))
                            {
                                using (var file = File.OpenRead(options.Font))
                                    family = FontTools.LoadFontFamily(file, out fonts);

                                prototypeFont = new Font(family, 10, options.FontStyle);
                            }
                            else
                            {
                                prototypeFont = new Font(options.Font, 10, options.FontStyle);
                            }
                            
                            foreach (var text in options.DrawText)
                            {
                                var split = text.Split(new[] {' '}, 2);

                                if (split.Length != 2)
                                    continue;

                                var meta = split[0].Split(',');

                                int x, y;
                                float size;
                                if (meta.Length < 4 || !int.TryParse(meta[0], out x) || !int.TryParse(meta[1], out y) ||
                                    !float.TryParse(meta[3], out size))
                                    continue;

                                using (var font = new Font(prototypeFont.FontFamily, size, prototypeFont.Style))
                                    g.DrawString(split[1], font, new SolidBrush(GetColorFromString(meta[2])), x, y);
                            }

                            prototypeFont.Dispose();
                            family?.Dispose();
                            fonts?.Dispose();
                        }
                    }

                    var blueprintWriter = new BlueprintWriter();
                    blueprintWriter.Write(blueprint, bitmap);

                    if (string.IsNullOrWhiteSpace(options.Output))
                    {
                        using (var stream = new MemoryStream())
                        {
                            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            Console.WriteLine(Convert.ToBase64String(stream.ToArray()));
                        }
                    }
                    else
                    {
                        bitmap.Save(options.Output, ImageFormat.Png);
                    }
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("invalid input");
                Environment.Exit(1);
            }
            catch (InvalidBlueprintException)
            {
                Console.WriteLine("invalid blueprint");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.GetType() + ": " + e.Message + "\n" + e.StackTrace);
                Environment.Exit(1);
            }
        }

        private static Color GetColorFromString(string text)
        {
            try
            {
                   return Color.FromArgb(
                        Convert.ToInt32(
                            text.Substring(text.StartsWith("#") ? 1 : 0), 16));
            }
            catch
            {
                return Color.Black;
            }
        }

        private static void ProcesssBlueprint(FileProcessingSubOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            AssertFileOfType(options.Input, ".png", options.IsRaw);

            try
            {
                using (var bitmap = options.IsRaw
                        ? new Bitmap(new MemoryStream(Convert.FromBase64String(options.Input)))
                        : (Bitmap)Image.FromFile(options.Input))
                {
                    var blueprintReader = new BlueprintReader();
                    var blueprint = blueprintReader.Read(bitmap);

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
            }
            catch (FormatException)
            {
                Console.WriteLine("invalid input");
                Environment.Exit(1);
            }
            catch (InvalidBlueprintException)
            {
                Console.WriteLine("invalid blueprint");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.GetType() + ": " + e.Message + "\n" + e.StackTrace);
                Environment.Exit(1);
            }
        }

        private static void ProcessSavegame(FileProcessingSubOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            AssertFileOfType(options.Input, ".txt", options.IsRaw);

            try
            {
                using (
                    var stream = options.IsRaw
                        ? new MemoryStream(Encoding.UTF8.GetBytes(options.Input))
                        : (Stream) File.OpenRead(options.Input))
                {
                    var savegameReader = new SavegameReader();
                    var savegame = savegameReader.Deserialize(stream);

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
            catch (FormatException)
            {
                Console.WriteLine("invalid input");
                Environment.Exit(1);
            }
            catch (InvalidSavegameException)
            {
                Console.WriteLine("invalid savegame");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.GetType() + ": " + e.Message + "\n" + e.StackTrace);
                Environment.Exit(1);
            }
        }
    }
}