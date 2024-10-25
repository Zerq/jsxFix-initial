using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text.RegularExpressions;

class Program
{


    private static void doDirectory(DirectoryInfo sourceDirectory, string outputPath, int depth)
    {

        var sourceTsxFiles = sourceDirectory.GetFiles("*.tsx");

        foreach (var file in sourceTsxFiles)
        {
            var source = file.Name.Replace(".tsx", ".js");
            var outputFilePath = Path.Combine(outputPath, source);


            var depthModifier = "./";
            if (depth > 0)
            {
                depthModifier = "";

                for (var i = 0; i < depth; i++)
                {
                    depthModifier = "../" + depthModifier;
                }

            }

            var map = outputFilePath + ".map";
 

            var text = System.IO.File.ReadAllText(outputFilePath)
            .Replace(
                @"import { jsx as _jsx } from ""react/jsx-runtime"";",
                $@"import {{ jsx as _jsx }} from ""{depthModifier}react/jsx-runtime.js"";"
            ).Replace(
                @"import { jsx as _jsx, jsxs as _jsxs } from ""react/jsx-runtime"";",
                $@"import {{ jsx as _jsx, jsxs as _jsxs }} from ""{depthModifier}react/jsx-runtime.js"";"
            ).Replace("{ className:", "{ class:");

  







            fixMap(map, depth);



            System.IO.File.WriteAllText(outputFilePath, text);
        }

        foreach (var subfolder in sourceDirectory.GetDirectories())
        {
            var outputSubFolder = Path.Combine(outputPath, subfolder.Name);

            if (Directory.Exists(outputSubFolder))
            {
                doDirectory(subfolder, outputSubFolder, depth + 1);
            }


        }

    }

    private static void fixMap(string mapPath, int depth)
    {
        if (File.Exists(mapPath) && depth > 0)
        {
            var txt = File.ReadAllText(mapPath);
            var replacer = "src/";
            for (var i = 0; i < depth; i++)
            {

                replacer = "../" + replacer;

            }


            var oldTxt = "\"sourceRoot\":\"src/\"";
            var newTxt = $"\"sourceRoot\":\"{replacer}\"";


            txt = txt.Replace(oldTxt, newTxt);
            File.WriteAllText(mapPath, txt);

        }

    }

    public static void Main(String[] args)
    {

        if (args.Count() < 2)
        {
            Console.WriteLine(@"JsxFix <source directory> <outputDirectory> (relative directories start with ""./"" )");
        }

        if (args.Count() == 2)
        {

            if (args[0].StartsWith("."))
            {
                var current = System.IO.Directory.GetCurrentDirectory();
                args[0] = Path.Combine(current, args[0].Replace("./", ""));
            }

            if (args[1].StartsWith("."))
            {
                var current = System.IO.Directory.GetCurrentDirectory();
                args[1] = Path.Combine(current, args[1].Replace("./", ""));
            }

        }


        if (args.Count() == 2 && System.IO.Path.Exists(args[0]) && System.IO.Path.Exists(args[1]))
        {

            var sourceDirectory = new System.IO.DirectoryInfo(args[0]);
            var outputPath = args[1];
            doDirectory(sourceDirectory, outputPath, 0);



        }
    }
}

//import { jsx as _jsx } from "react/jsx-runtime;;
//import { jsx as _jsx } from "./react/jsx-runtime.js";