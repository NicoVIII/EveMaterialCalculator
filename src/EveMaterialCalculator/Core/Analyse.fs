namespace EveMaterialCalculator.Core

[<RequireQualifiedAccess>]
module Analyse =
    let rec getBasicMaterials preparedData quantity searchedId =
        Map.tryFind searchedId preparedData.materialMap
        |> function
            | Some materials ->
                Seq.collect
                    (fun material -> getBasicMaterials preparedData material.quantity material.materialType)
                    materials
            | None ->
                // If we didn't find it at materials we look for a matching product
                Map.tryFind searchedId preparedData.productMap
                |> function
                    | Some product -> getBasicMaterials preparedData 1 product.typ
                    | None -> seq { searchedId, quantity }
        |> Seq.groupBy fst
        |> Seq.map (fun (id, data) -> id, Seq.sumBy snd data)
