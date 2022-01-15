open Fake.IO

open RunHelpers
open RunHelpers.BasicShortcuts

[<RequireQualifiedAccess>]
module Config =
    let projectName = "EveMaterialCalculator.AvaloniaApp"

    let mainProject = $"./src/EveMaterialCalculator/AvaloniaApp/%s{projectName}.fsproj"

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

    let publish () =
        let commonArgs =
            [
                "-v"
                "minimal"
                "-c"
                "Release"
                "-o"
                Config.packPath
                "/p:SelfContained=true"
                "/p:PublishSingleFile=true"
                "/p:PublishTrimmed=true"
                "/p:TrimMode=Link"
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
                     "/p:PublishReadyToRun=true"
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
        | [ "publish" ] -> Command.publish ()
        | _ ->
            let msg =
                [
                    "Usage: dotnet run [<command>]"
                    "Look up available commands in run.fs"
                ]

            Error(1, msg)
    |> ProcessResult.wrapUp
