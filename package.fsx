#r "nuget: Fake.Tools.Git"
#r "nuget: Fake.DotNet.Cli"
open System
open System.IO
open Fake.Tools
open Fake.DotNet

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let check (r : Fake.Core.ProcessResult) =
    if not r.OK then
        failwithf "Command failed with exit code %d" r.ExitCode

try Directory.Delete("repo", true) |> ignore
with _ -> ()

Git.Repository.clone Environment.CurrentDirectory  "https://github.com/wo80/Triangle.NET.git" "repo"



File.WriteAllLines("repo/paket.dependencies", [
    "source https://api.nuget.org/v3/index.json"
    "nuget Aardvark.Build ~> 1.0.22"
])
File.WriteAllLines("repo/src/Triangle/paket.references", [
    "Aardvark.Build"
])

File.WriteAllLines("repo/src/Triangle/paket.template", [
    "type project"
    "id Unofficial.Triangle.NET"
    "authors aardvark platform team"
    "owners aardvark platform team"
    "licenseExpression MIT"
    "description \"Triangle.NET is a 2D quality mesh generator and Delaunay triangulator.\""
])

File.Copy("RELEASE_NOTES.md", "repo/RELEASE_NOTES.md")


let options (o : DotNet.Options) =
    { o with WorkingDirectory = "repo" }


DotNet.exec options "paket" "install"
|> check

Threading.Thread.Sleep(500)

DotNet.exec options "aardpack" "src/Triangle/Triangle.csproj --notag --norelease"
|> check








