namespace EveMaterialCalculator.Core

open FSharp.Data
open System.IO

[<Measure>]
type typeID

type TypeID = int<typeID>

module TypeID =
    let inline create value : TypeID = value * 1<typeID>

type Material =
    {
        typ: TypeID
        quantity: int
        materialType: TypeID
    }

type Product =
    {
        typ: TypeID
        product: TypeID
        quantity: int
    }

type InvTypes = CsvProvider<"../../../sde-static/invTypes.csv", IgnoreErrors=true>

module InvTypes =
    let load () =
        "./sde/invTypes.csv"
        |> Path.GetFullPath
        |> InvTypes.Load

    let loadRows () = (load ()).Rows

type IndustryActivityMaterials = CsvProvider<"../../../sde-static/industryActivityMaterials.csv">

module IndustryActivityMaterials =
    let load () =
        "./sde/industryActivityMaterials.csv"
        |> Path.GetFullPath
        |> IndustryActivityMaterials.Load

    let loadRows () = (load ()).Rows

type IndustryActivityProducts = CsvProvider<"../../../sde-static/industryActivityProducts.csv">

module IndustryActivityProducts =
    let load () =
        "./sde/industryActivityProducts.csv"
        |> Path.GetFullPath
        |> IndustryActivityProducts.Load

    let loadRows () = (load ()).Rows

type PreparedData =
    {
        materialMap: Map<TypeID, Material seq>
        productMap: Map<TypeID, Product>
        typeNameIdMap: Map<string, TypeID>
        types: Map<TypeID, InvTypes.Row>
    }
