using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Reflection;

namespace find
{
    class find
    {
        static IEnumerable<string> Search(string root, Func<string, Type, bool> onsearch, int? maxdepth = null, string searchexpr=null)
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
                    foreach (string file in paths.Where(f => onsearch(f, Type.File)))
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

        static void Main(string[] args)
        {
            var p = new find();
            OptionSet getopt_specs = p.Options();

            var parsed = getopt_specs.Parse(args);
            var path = args.FirstOrDefault();
            //Console.WriteLine(String.Join(", ", opt.Select(o => o.Key + "=" + o.Value)));
            int? maxdepth = null;
            Func<string, Type, bool> onsearch = (string s, Type t) => true;
            string searchexpr = null;
            foreach (var option in opt)
            {
                switch (option.Key)
                {
                    case "name":
                    case "iname":
                        {
                            searchexpr = (string)option.Value;
                            break;
                        }
                    case "regex":
                        {
                            var _onsearch = onsearch;
                            var regex = new Regex((string)option.Value, RegexOptions.Compiled);
                            onsearch = (string s, Type t) => _onsearch(s, t) && regex.IsMatch(s);
                            break;
                        }
                    case "iregex":
                        {
                            var _onsearch = onsearch;
                            var regex = new Regex((string)option.Value, RegexOptions.Compiled|RegexOptions.IgnoreCase);
                            onsearch = (string s, Type t) => _onsearch(s, t) && regex.IsMatch(s);
                            break;
                        }
                    case "type":
                        {
                            var _onsearch = onsearch;
                            var type = (Type)option.Value;
                            onsearch = (string s, Type t) => _onsearch(s, t) && t==type;
                            break;
                        }
                    case "depth":
                        maxdepth = (int)option.Value;
                        break;
                    case "size":
                        {
                            var _onsearch = onsearch;
                            var size = (long)option.Value;
                            onsearch = (string s, Type t) => _onsearch(s, t) && t == Type.File && new FileInfo(s).Length>size;
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
            }
            var files = Search(path, onsearch, maxdepth, searchexpr);
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }
        }
        enum Type
        {
            Unknown = 0,
            File = 1,
            Directory = 2

        }
        private static Action O(Action a) { return a; }
        static Dictionary<string, object> opt = new Dictionary<string, object>();
        static Dictionary<string, object> ENV = new Dictionary<string, object>();
        private static Action<string> Opt(string p)
        {
            return (v) => { if (!string.IsNullOrEmpty(v)) opt[p] = true; };
        }
        private static Action<string> Env(string p)
        {
            return (v) => { if (!string.IsNullOrEmpty(v)) ENV[p] = true; };
        }
        private static Action<string> OptV(string p)
        {
            return (v) => { if (!string.IsNullOrEmpty(v)) opt[p] = v; };
        }
        private static Action<string> OptV<T>(string p)
        {
            return (v) => { if (!string.IsNullOrEmpty(v)) opt[p] = TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(v); };
        }
        private static Action<string> OptSub(string p, Func<string, object> sub)
        {
            return (v) => { if (!string.IsNullOrEmpty(v)) opt[p] = sub(v); };
        }
        
        private OptionSet Options()
        {
            OptionSet getopt_specs = new OptionSet() {
{"d|depth=", OptV<int>("depth") },
{"name=", OptV("name") },//
{"type=", OptSub("type",(v)=>{
    switch (v)
	{
    case "f":
        return Type.File;
    case "d":
        return Type.Directory;
	default:
        return Type.Unknown;
	} }) },
{"size=",OptSub("size",(v)=>GetSize(v))},
{"h|help", OptV("help") },
{"maxdepth=", OptV("maxdepth")},
{"mindepth=", OptV("mindepth")},
{"version", OptV("version")},
            };
            return getopt_specs;
        }

        private object GetSize(string v)
        {
            int factor = 512;
            switch (v.Last())
            {
                case 'c'://'c'    for bytes
                    factor = 1; break;
                case 'w'://'w'    for two-byte words
                    factor = 2; break;
                case 'k'://'k'    for Kilobytes (units of 1024 bytes)
                    factor = 1024; break;
                case 'M'://'M'    for Megabytes (units of 1048576 bytes)
                    factor = 1048576; break;
                case 'G'://'G'    for Gigabytes (units of 1073741824 bytes)
                    factor = 1073741824; break;
                case 'b'://'b'    for 512-byte blocks (this is the default if no suffix  is used)
                    factor = 512; break;
            }
            if (!Char.IsNumber(v.Last())) 
            {
                v = v.Substring(0, v.Length - 1);
            }
            return factor * Int64.Parse(v);
        }

    }
}
