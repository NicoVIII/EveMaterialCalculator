namespace EveMaterialCalculator.Core

[<RequireQualifiedAccess>]
module Analyse =
    let getBasicMaterials preparedData (neededQuantity: int) searchedId =
        let rec helper history preparedData neededQuantity searchedId =
            Map.tryFind searchedId preparedData.productions
            |> function
                | Some production ->
                    production.materials
                    |> Seq.collect (fun material ->
                        let neededQuantity =
                            float (neededQuantity * int64 material.needed)
                            / float production.producedQuantity
                            |> ceil
                            |> int64

                        helper (searchedId :: history) preparedData neededQuantity material.typ)
                | None -> seq { searchedId, neededQuantity, seq { history } }
            |> Seq.groupBy (fun (x, _, _) -> x)
            |> Seq.map (fun (id, data) ->
                let quantity = Seq.sumBy (fun (_, quantity, _) -> quantity) data
                let histories = Seq.collect (fun (_, _, histories) -> histories) data
                id, quantity, histories)

        helper [] preparedData (int64 neededQuantity) searchedId
