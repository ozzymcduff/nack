using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Reflection;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Matcher = System.Func<string, find.find.Type, bool>;

namespace find
{
    public class find
    {
        static IEnumerable<string> Search(string root, Matcher onsearch, long? maxdepth = null, string searchexpr=null)
        {
            var dirs = new Queue<Tuple<string, int>>();
            dirs.Enqueue(new Tuple<string, int>(root, 0));
            while (dirs.Count > 0)
            {
                var dir = dirs.Dequeue();
                if (maxdepth != null && maxdepth.Value < dir.Item2)
                    continue;
                // files
                string[] paths = null;
                try
                {
                    paths = string.IsNullOrEmpty(searchexpr)
                        ? Directory.GetFiles(dir.Item1)
                        : Directory.GetFiles(dir.Item1, searchexpr);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (paths != null && paths.Length > 0)
                {
                    foreach (string file in paths.Where(f => onsearch(f,Type.File)))
                    {
                        yield return file;
                    }
                }

                // sub-directories
                paths = null;
                try
                {
                    paths = Directory.GetDirectories(dir.Item1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (paths != null && paths.Length > 0)
                {
                    foreach (string subDir in paths)
                    {
                        if (onsearch(subDir, Type.Directory))
                        {
                            yield return subDir;
                        }

                        dirs.Enqueue(new Tuple<string, int>(subDir, dir.Item2 + 1));
                    }
                }
            }
        }
        public static bool debug = false;
        static void Main(string[] args)
        {
            var p = new find();
            
            var path = args.FirstOrDefault();
            var s = new MemoryStream();
            var tobeparsed = String.Join(" ", args.Skip(1));
            if (debug) Console.WriteLine(tobeparsed);
            
            var cli= FindEval.Parse(tobeparsed);
            //Console.WriteLine(cli.onsearch.ToString());
            //Func<string, Type, bool> onsearch = (string s, Type t) => true;
            /*string searchexpr = null;
            foreach (var option in opt)
            {
                switch (option.Key)
                {
                    case "type":
                        {
                            var _onsearch = onsearch;
                            var type = (Type)option.Value;
                            onsearch = (string s, Type t) => _onsearch(s, t) && t==type;
                            break;
                        }
                    case "version":
                        {
                            var assembly = typeof(find).Assembly;
                            var name = assembly.GetName();
                            Console.WriteLine(name.Name);
                            var descriptionAttribute = assembly
                                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                                .OfType<AssemblyDescriptionAttribute>()
                                .FirstOrDefault();

                            if (descriptionAttribute != null)
                                Console.WriteLine(descriptionAttribute.Description);
                            Console.WriteLine(name.Version);
                            return;
                        }
                    default:
                        throw new NotImplementedException(option.Key);
                        break;
                }
            }*/
            var files = Search(path, cli.onsearch, cli.maxdepth, cli.searchexpr);
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }
        }
        public enum Type
        {
            Unknown = 0,
            File = 1,
            Directory = 2
        }
    }
    public static class FindExtensions
    {
        public static bool IsFile(this find.Type self) { return self == find.Type.File; }
        public static bool IsDirectory(this find.Type self) { return self == find.Type.Directory; }
    }
}
