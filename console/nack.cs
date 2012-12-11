
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NDesk.Options;

namespace Nack
{
    class Program
    {
        static Dictionary<string, string> ignore_dirs = Fold(new string[]{
        ".bzr"              , "Bazaar",
        ".cdv"              , "Codeville",
        "~.dep"             , "Interface Builder",
        "~.dot"             , "Interface Builder",
        "~.nib"             , "Interface Builder",
        "~.plst"            , "Interface Builder",
        ".git"              , "Git",
        ".hg"               , "Mercurial",
        ".pc"               , "quilt",
        ".svn"              , "Subversion",
        "_MTN"                , "Monotone",
        "blib"                , "Perl module building",
        "CVS"                 , "CVS",
        "RCS"                 , "RCS",
        "SCCS"                , "SCCS",
        "_darcs"              , "darcs",
        "_sgbak"              , "Vault/Fortress",
        "autom4te.cache"    , "autoconf",
        "cover_db"          , "Devel::Cover",
        "_build"              , "Module::Build",
        });

        private static Dictionary<string, T> Fold<T>(T[] p)
        {
            var dic = new Dictionary<string, T>();
            for (int i = 0; i < p.Length; i += 2)
            {
                dic[p[i].ToString()] = p[i + 1];
            }
            return dic;
        }

        Dictionary<string, object> mappings = Fold<Object>(new object[]{
        "actionscript", qw(" as mxml "),
        "ada", qw(" ada adb ads "),
        "asm", qw(" asm s "),
        "batch", qw(" bat cmd "),
        "binary", q("Binary files, as defined by Perl\"s -B op (default: off)"),
        "cc", qw(" c h xs "),
        "cfmx", qw(" cfc cfm cfml "),
        "clojure", qw(" clj "),
        "cpp", qw(" cpp cc cxx m hpp hh h hxx "),
        "csharp", qw(" cs "),
        "css", qw(" css "),
        "delphi", qw(" pas int dfm nfm dof dpk dproj groupproj bdsgroup bdsproj "),
        "elisp", qw(" el "),
        "erlang", qw(" erl hrl "),
        "fortran", qw(" f f77 f90 f95 f03 for ftn fpp "),
        "go", qw(" go "),
        "groovy", qw(" groovy gtmpl gpp grunit "),
        "haskell", qw(" hs lhs "),
        "hh", qw(" h "),
        "html", qw(" htm html shtml xhtml "),
        "java", qw(" java properties "),
        "js", qw(" js "),
        "jsp", qw(" jsp jspx jhtm jhtml "),
        "lisp", qw(" lisp lsp "),
        "lua", qw(" lua "),
        "make", q("Makefiles (including *.mk and *.mak)"),
        "mason", qw(" mas mhtml mpl mtxt "),
        "objc", qw(" m h "),
        "objcpp", qw(" mm h "),
        "ocaml", qw(" ml mli "),
        "parrot", qw(" pir pasm pmc ops pod pg tg "),
        "perl", qw(" pl pm pm6 pod t psgi "),
        "php", qw(" php phpt php3 php4 php5 phtml"),
        "plone", qw(" pt cpt metadata cpy py "),
        "python", qw(" py "),
        "rake", q("Rakefiles"),
        "ruby", qw(" rb rhtml rjs rxml erb rake spec "),
        "scala", qw(" scala "),
        "scheme", qw(" scm ss "),
        "shell", qw(" sh bash csh tcsh ksh zsh "),
        "skipped", q("Files, but not directories, normally skipped by ack (default: off)"),
        "smalltalk", qw(" st "),
        "sql", qw(" sql ctl "),
        "tcl", qw(" tcl itcl itk "),
        "tex", qw(" tex cls sty "),
        "text", q("Text files, as defined by Perl\"s -T op (default: off)"),
        "tt", qw(" tt tt2 ttml "),
        "vb", qw(" bas cls frm ctl vb resx "),
        "verilog", qw(" v vh sv "),
        "vhdl", qw(" vhd vhdl "),
        "vim", qw(" vim "),
        "yaml", qw(" yaml yml "),
        "xml", qw(" xml dtd xsl xslt ent "),
    });
        private static Action O(Action a) { return a; }
        static Dictionary<string, object> opt = new Dictionary<string, object>();
        static Dictionary<string, object> ENV = new Dictionary<string, object>();
        private static Action<string> Opt(string p)
        {
            return (v) => { if (!string.IsNullOrEmpty(v)) opt[p] = true; };
        }
        private static Action<string> Env(string p)
        {
            return (v) => {if (!string.IsNullOrEmpty(v)) ENV[p] = true; };
        }
        private static Action<string> OptV(string p)
        {
            return (v) => { if (!string.IsNullOrEmpty(v)) opt[p] = v; };
        }
        private static Action<string> EnvV(string p)
        {
            return (v) => { if (!string.IsNullOrEmpty(v)) ENV[p] = v; };
        }
        private static Action<string> Sub(Action<string> s)
        {
            return s;
        }
        private static Action Sub(Action s)
        {
            return s;
        }
        private static string remove_dir_sep(string s)
        {
            throw new NotImplementedException("remove_dir_sep");
            return s;
        }
        private void print_version_statement()
        {
            throw new NotImplementedException();
        }
        private void show_help(string v)
        {
            throw new NotImplementedException();
        }
        private void show_help_types()
        {
            throw new NotImplementedException();
        }
        
        private static string q(string p)
        {
            return p;
        }

        private static Regex word = new Regex("\\w*");
        private static string[] qw(string p)
        {
            return word.Split(p);
        }

        static void Main(string[] args)
        {
            var p = new Program();
            OptionSet getopt_specs = p.Options();

            var parsed = getopt_specs.Parse(args);
        }

        private OptionSet Options()
        {
            OptionSet getopt_specs = new OptionSet() {
{"1", Sub((v)=> { opt["1"] = opt["m"] = true; }) },
{"A|after-context=", OptV("after_context") },
{"B|before-context=", OptV("before_context") },
{"C|context:", Sub((v)=> { var val = v; opt["before_context"] = opt["after_context"] = (string.IsNullOrEmpty(val)?(object)2: val );}) },       
{"a|all-types", Opt("all") },
{"break!", Opt("break") },
{"c", Opt("count") },
{"color|colour!", Opt("color") },
{"color-match=", EnvV("ACK_COLOR_MATCH") },
{"color-filename=", EnvV("ACK_COLOR_FILENAME") },
{"color-lineno=", EnvV("ACK_COLOR_LINENO") },
{"column!", Opt("column") },
{"count", Opt("count") },
{"env!", Sub((a)=>{})},// # ignore this option, it is handled beforehand                                                },
{"f", Opt("f") },
{"flush", Opt("flush") },
{"follow!", Opt("follow") },
{"g=", Sub((v)=> { opt["G"] = v; opt["f"] = 1; }) },
{"G=", OptV("G") },
{"group!", Sub((v)=>{ opt["heading"] = opt["break"] = v; }) },
{"heading!", Opt("heading") },
{"h|no-filename", Opt("h") },
{"H|with-filename", Opt("H") },
{"i|ignore-case", Opt("i") },
{"invert-file-match", Opt("invert_file_match") },
{"lines=", Sub((v)=> { var val = v; throw new NotImplementedException("lines"); /*push @{$opt{lines}}, $val*/ }) },
{"l|files-with-matches", Opt("l") },
{"L|files-without-matches", Sub((v)=> { opt["l"] = opt["v"] = 1; }) },
{"m|max-count=", OptV("m") },
{"match=", OptV("regex") },
{"n|no-recurse", Opt("n") },
{"o", Sub(v=> { opt["output"] = "$&"; }) },
{"output=", OptV("output") },
{"pager=", OptV("pager") },
{"nopager", Sub(v=> { opt["pager"] = false; }) },
{"passthru", Opt("passthru") },
{"print0", Opt("print0") },
{"Q|literal", Opt("Q") },
{"r|R|recurse", Sub(v=> { opt["n"] = 0; }) },
{"show-types", Opt("show_types") },
{"smart-case!", Opt("smart_case") },
{"sort-files", Opt("sort_files") },
{"u|unrestricted", Opt("u") },
{"v|invert-match", Opt("v") },
{"w|word-regexp", Opt("w") },
{"ignore-dirs=", Sub(v=>{ var dir = remove_dir_sep( v); ignore_dirs[dir] = "--ignore-dirs"; }) },
{"noignore-dirs=", Sub(v=> { var dir = remove_dir_sep( v); ignore_dirs.Remove(dir); }) },

{"version", Sub(v=> { print_version_statement(); Environment.Exit(0); }) },
{"help|?:", Sub(v=> { show_help(v); Environment.Exit(0); }) },
{"help-types", Sub(v=> { show_help_types(); Environment.Exit(0); }) },
{"man", Sub(v=> {                                                                                                   
            /*require Pod::Usage;                                                                               
            Pod::Usage::pod2usage({                                                                             
                -verbose => 2,                                                                                  
                -exitval => 0,                                                                                  
            });*/                                                                                                 
    throw new NotImplementedException();
}) },
                                                                                                                
{"type=", Sub(v=> {                                                                                                 
/*            # Whatever --type=xxx they specify, set it manually in the hash                                     
            my $dummy = shift;                                                                                  
            my $type = shift;                                                                                   
            my $wanted = ($type =~ s/^no//) ? 0 : 1; # must not be undef later                                  
                                                                                                                
            if ( exists $type_wanted{ $type } ) {
                $type_wanted{ $type } = $wanted;
            }
            else {
                App::Ack::die( qq{Unknown --type "$type"} );
            }*/
            throw new NotImplementedException();
        })}
        }; //# type sub
            return getopt_specs;
        }

        public string[] Files()
        {
            return null;
        }
    }
}
