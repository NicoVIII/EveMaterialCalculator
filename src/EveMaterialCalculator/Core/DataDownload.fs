namespace EveMaterialCalculator.Core

open FSharp.Data
open ICSharpCode.SharpZipLib.BZip2
open System.IO

module DataDownload =
    let baseUrl = "https://www.fuzzwork.co.uk/dump/latest"
    let basePath = "./sde"

    let necessaryCsvs =
        [
            "invTypes"
            "industryActivityMaterials"
            "industryActivityProducts"
        ]

    let downloadFile url outputFile =
        async {
            let! request =
                Http.AsyncRequestStream(
                    url,
                    headers =
                        [
                            // Without a valid User-Agent we get a 403
                            "User-Agent", "EveMaterialCalculator/1.0"
                        ]
                )

            Directory.CreateDirectory(Path.GetDirectoryName(outputFile: string))
            |> ignore

            use outputFile =
                new FileStream(outputFile, FileMode.Create)

            do!
                request.ResponseStream.CopyToAsync(outputFile)
                |> Async.AwaitTask
        }

    let extractCsv inPath outPath =
        async {
            let inStream = File.OpenRead(inPath)
            use bz2Stream = new BZip2InputStream(inStream)
            let outStream = File.OpenWrite(outPath)

            do!
                bz2Stream.CopyToAsync(outStream)
                |> Async.AwaitTask

            outStream.Close()
            bz2Stream.Close()
            inStream.Close()

            // Delete inFile
            if inPath <> outPath then
                File.Delete inPath
        }

    let start () =
        List.choose
            (fun csv ->
                let csvPath =
                    Path.GetFullPath($"%s{basePath}/%s{csv}.csv")

                if not (File.Exists csvPath) then
                    let url = $"%s{baseUrl}/%s{csv}.csv.bz2"
                    let archivePath = $"%s{csvPath}.bz2"

                    async {
                        printfn $"Download %s{csvPath}..."
                        do! downloadFile url archivePath
                        printfn $"Extract %s{csvPath}..."
                        do! extractCsv archivePath csvPath
                        printfn $"Finished work for %s{csvPath}"
                    }
                    |> Some
                else
                    printfn $"Skip download of %s{csvPath}"
                    None)
            necessaryCsvs
        |> Async.Sequential
        |> Async.RunSynchronously
        |> ignore
