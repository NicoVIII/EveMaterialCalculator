namespace EveMaterialCalculator.Core

[<RequireQualifiedAccess>]
module Analyse =
    let rec getBasicMaterials history preparedData quantity searchedId =
        Map.tryFind searchedId preparedData.materialMap
        |> function
            | Some materials ->
                materials
                |> Seq.collect (fun material ->
                    let quantity = material.quantity * quantity
                    getBasicMaterials (searchedId :: history) preparedData quantity material.materialType)
            | None ->
                // If we didn't find it at materials we look for a matching product
                Map.tryFind searchedId preparedData.productMap
                |> function
                    | Some product ->
                        let neededQuantity = quantity / product.quantity
                        getBasicMaterials history preparedData neededQuantity product.typ
                    | None -> seq { searchedId, quantity, seq { history } }
        |> Seq.groupBy (fun (x, _, _) -> x)
        |> Seq.map (fun (id, data) ->
            let quantity = Seq.sumBy (fun (_, quantity, _) -> quantity) data
            let histories = Seq.collect (fun (_, _, histories) -> histories) data
            id, quantity, histories)
