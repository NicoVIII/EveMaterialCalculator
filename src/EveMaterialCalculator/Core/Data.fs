namespace EveMaterialCalculator.Core

open FSharp.Data
open ICSharpCode.SharpZipLib.BZip2
open System.IO

[<RequireQualifiedAccess>]
module Data =
    let baseUrl = "https://www.fuzzwork.co.uk/dump/latest"
    let basePath = "./sde"

    let necessaryCSVs =
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

            use outputFile = new FileStream(outputFile, FileMode.Create)

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

    let ensureDownloadData () =
        necessaryCSVs
        |> List.choose (fun csv ->
            let csvPath = Path.GetFullPath($"%s{basePath}/%s{csv}.csv")

            if not (File.Exists csvPath) then
                let url = $"%s{baseUrl}/%s{csv}.csv.bz2"
                let archivePath = $"%s{csvPath}.bz2"

                async {
#if DEBUG
                    printfn $"Download %s{csvPath}..."
#endif
                    do! downloadFile url archivePath
#if DEBUG
                    printfn $"Extract %s{csvPath}..."
#endif
                    do! extractCsv archivePath csvPath
#if DEBUG
                    printfn $"Finished work for %s{csvPath}"
#endif
                }
                |> Some
            else
#if DEBUG
                printfn $"Skip download of %s{csvPath}"
#endif
                None)
        |> Async.Sequential
        |> Async.Ignore

    let prepare () =
        async {
            do! ensureDownloadData ()

            let types =
                InvTypes.loadRows ()
                |> Seq.map (fun row -> TypeID.create row.TypeID, row)
                |> Map.ofSeq

            let typeNameIdMap =
                types
                |> Map.toSeq
                |> Seq.map (fun (id, row) -> row.TypeName, id)
                |> Map.ofSeq

            let materialMap =
                IndustryActivityMaterials.loadRows ()
                |> Seq.filter (fun row -> row.ActivityID <> 8)
                |> Seq.map (fun row ->
                    {
                        typ = row.TypeID |> TypeID.create
                        quantity = row.Quantity
                        materialType = row.MaterialTypeID |> TypeID.create
                    })
                |> Seq.groupBy (fun material -> material.typ)
                |> Map.ofSeq

            let productMap =
                IndustryActivityProducts.loadRows ()
                |> Seq.map (fun row ->
                    let product =
                        {
                            typ = TypeID.create row.TypeID
                            product = TypeID.create row.ProductTypeID
                            quantity = row.Quantity
                        }

                    product.product, product)
                |> Map.ofSeq

            return
                {
                    materialMap = materialMap
                    productMap = productMap
                    typeNameIdMap = typeNameIdMap
                    types = types
                }
        }
