namespace EveMaterialCalculator.Core

module Program =
    [<EntryPoint>]
    let main _ =
        let types =
            InvTypes.loadRows ()
            |> Seq.map (fun row -> TypeID.create row.TypeID, row)
            |> Map.ofSeq

        let materialMap =
            IndustryActivityMaterials.loadRows ()
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
                    }

                product.product, product)
            |> Map.ofSeq

        let rec getBasicMaterials quantity searchedId =
            Map.tryFind searchedId materialMap
            |> function
                | Some materials ->
                    Seq.collect (fun material -> getBasicMaterials material.quantity material.materialType) materials
                | None ->
                    // If we didn't find it at materials we look for a matching product
                    Map.tryFind searchedId productMap
                    |> function
                        | Some product -> getBasicMaterials 1 product.typ
                        | None -> seq { searchedId, quantity }
            |> Seq.groupBy fst
            |> Seq.map (fun (id, data) -> id, Seq.sumBy snd data)

        TypeID.create 28606 // Orca
        |> getBasicMaterials 1
        |> Seq.sortByDescending snd
        |> Seq.iter (fun (id, quantity) ->
            let typ = Map.find id types
            printfn "%8i: %s" quantity typ.TypeName)

        0
