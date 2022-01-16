open Fake.IO

open RunHelpers
open RunHelpers.BasicShortcuts

[<RequireQualifiedAccess>]
module Config =
    let projectName = "EveMaterialCalculator.AvaloniaApp"

    let mainProject = $"./src/EveMaterialCalculator/AvaloniaApp/%s{projectName}.fsproj"

    let testProject =
        $"./tests/EveMaterialCalculator.Core/EveMaterialCalculator.Core.Tests.fsproj"

    let artifactName = "EveMaterialCalculator"

    let packPath = "./deploy"

module Task =
    let restore () =
        job {
            Template.DotNet.toolRestore ()
            Template.DotNet.restore Config.mainProject
        }

    let build () =
        dotnet [ "build"
                 Config.mainProject
                 "--no-restore" ]

    let run () =
        dotnet [ "run"
                 "--project"
                 Config.mainProject ]

    let runTest () =
        dotnet [ "run"
                 "--project"
                 Config.testProject ]

    let publish () =
        let commonArgs =
            [
                "-v"
                "minimal"
                "-c"
                "Release"
                "-o"
                Config.packPath
                "--self-contained"
                "/p:PublishSingleFile=true"
                "/p:PublishTrimmed=true"
                "/p:PublishReadyToRun=true"
                "/p:EnableCompressionInSingleFile=true"
                "/p:IncludeNativeLibrariesForSelfExtract=true"
                "/p:DebugType=None"
                Config.mainProject
            ]

        Shell.mkdir Config.packPath
        Shell.cleanDir Config.packPath

        job {
            // Linux
            dotnet [ "publish"
                     "-r"
                     "linux-x64"
                     yield! commonArgs ]

            Shell.mv
                $"%s{Config.packPath}/%s{Config.projectName}"
                $"%s{Config.packPath}/%s{Config.artifactName}-linux-x64"

            // Windows
            dotnet [ "publish"
                     "-r"
                     "win-x64"
                     yield! commonArgs ]

            Shell.mv
                $"{Config.packPath}/%s{Config.projectName}.exe"
                $"{Config.packPath}/%s{Config.artifactName}-win-x64.exe"
        }

module Command =
    let restore () = Task.restore ()

    let build () =
        job {
            restore ()
            Task.build ()
        }

    let run () =
        job {
            restore ()
            Task.run ()
        }

    let test () =
        job {
            restore ()
            Task.runTest ()
        }

    let publish () =
        job {
            restore ()
            Task.publish ()
        }

[<EntryPoint>]
let main args =
    args
    |> List.ofArray
    |> function
        | [ "restore" ] -> Command.restore ()
        | [ "build" ] -> Command.build ()
        | []
        | [ "run" ] -> Command.run ()
        | [ "test" ] -> Command.test ()
        | [ "publish" ] -> Command.publish ()
        | _ ->
            let msg =
                [
                    "Usage: dotnet run [<command>]"
                    "Look up available commands in run.fs"
                ]

            Error(1, msg)
    |> ProcessResult.wrapUp
