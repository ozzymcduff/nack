require 'rubygems'
require 'albacore'
require 'fileutils'

include FileUtils

task :default => [:all]

desc "Rebuild solution"
msbuild :build do |msb, args|
    msb.properties :configuration => :Debug
    msb.verbosity = :minimal
    msb.targets :Rebuild
    msb.solution = "Nack.sln"
end

desc "Nuget"
namespace :nuget do
    desc "Get"
    task :get do 
        sh ".\\.nuget\\NuGet.exe "
    end
    desc "Install"
    task :install do |t, args|
        #package = args[:package]
        sh ".\\.nuget\\NuGet.exe install .\\Tests\\packages.config -OutputDirectory .\\packages\\ "
    end

end

desc "Run everything!"
task :all => [:build]

desc "test using nunit console"
nunit :test => :build do |nunit|
    nunit.command = "packages/NUnit.Runners.2.6.2/tools/nunit-console.exe"
    nunit.assemblies "Tests/bin/Debug/Tests.dll"
end
@standard_classpaths = [#"%CLASSPATH%",
    "./packages_manual/antlr-3.4-complete-no-antlrv2.jar"]
def classpath
    return @standard_classpaths.join(File::PATH_SEPARATOR)
end
def render_grammar(grammar, output)
    sh "java -cp #{classpath} org.antlr.Tool #{grammar} -o #{output}"
end
def render_grammar2(grammar, output)
    sh ".\\packages_manual\\antlr-dotnet-tool-3.4.1.9004\\Antlr3.exe -fo #{output} #{grammar} "
end
task :findg do 
    render_grammar("./find/Find.g", "./find/") 
end
task :findevalg => [:findg] do 
    render_grammar("./find/FindEval.g", "./find/")
end
task :searchexprg do 
    render_grammar2(".\\find\\SearchExpr.g", ".\\find\\")
end
task :antlr => [:findevalg,:searchexprg] 
