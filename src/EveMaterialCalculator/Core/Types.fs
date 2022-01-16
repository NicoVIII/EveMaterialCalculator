namespace EveMaterialCalculator.Core

open FSharp.Data
open System.IO

[<Measure>]
type typeID

type TypeID = int<typeID>

module TypeID =
    let inline create value : TypeID = value * 1<typeID>

type Material = { typ: TypeID; needed: int }

module Material =
    let inline create typ needed : Material =
        {
            typ = TypeID.create typ
            needed = needed
        }

type Production =
    {
        producedQuantity: int
        materials: Material seq
    }

module Production =
    let inline create producedQuantity materials : Production =
        {
            producedQuantity = producedQuantity
            materials = materials
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

type PlanetarySchematicsTypeMap = CsvProvider<"../../../sde-static/planetSchematicsTypeMap.csv">

module PlanetarySchematicsTypeMap =
    let load () =
        "./sde/planetSchematicsTypeMap.csv"
        |> Path.GetFullPath
        |> PlanetarySchematicsTypeMap.Load

    let loadRows () = (load ()).Rows

type PreparedData =
    {
        // Contains blueprint crafting and planetary interaction
        productions: Map<TypeID, Production>
        typeNameIdMap: Map<string, TypeID>
        types: Map<TypeID, InvTypes.Row>
    }
