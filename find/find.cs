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

namespace find
{
    public class find
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
        public static bool debug = false;
        static void Main(string[] args)
        {
            var p = new find();
            
            var path = args.FirstOrDefault();
            var s = new MemoryStream();
            var tobeparsed = String.Join(" ", args.Skip(1));
            if (debug) Console.WriteLine(tobeparsed);
            var w = new StreamWriter(s);
            
            w.Write(tobeparsed);
            
            w.Flush();
            
            s.Position = 0;
            ANTLRInputStream input = new ANTLRInputStream(s);
            // Create an ExprLexer that feeds from that stream
            FindLexer lexer = new FindLexer(input);
            // Create a stream of tokens fed by the lexer
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            // Create a parser that feeds off the token stream
            FindParser parser = new FindParser(tokens);
            
            var c= parser.CommandLine();
            //Console.WriteLine("c:"+(c==null?"null":c.Text));
            var cstream = new CommonTreeNodeStream(c);
            var eval = new FindEval(cstream);
            var cli= eval.CommandLine();
            //Console.WriteLine(cli.onsearch.ToString());
            //Func<string, Type, bool> onsearch = (string s, Type t) => true;
            /*string searchexpr = null;
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
