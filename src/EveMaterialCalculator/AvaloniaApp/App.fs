namespace EveMaterialCalculator.AvaloniaApp

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open Elmish
open System

open EveMaterialCalculator.Core

module App =
    type ReadyState =
        {
            data: PreparedData
            materials: (TypeID * int64 * TypeID list seq) seq
            search: string
        }

    module ReadyState =
        let init preparedData =
            {
                data = preparedData
                materials = Seq.empty
                search = ""
            }

    type State =
        | DataPending
        | DataReady of ReadyState

    module State =
        let map mapper state =
            match state with
            | DataPending -> failwith "State invalid!"
            | DataReady state -> DataReady(mapper state)

    type Msg =
        | DataPreparationFinished of PreparedData
        | SearchChanged of string
        | CalculateBasicMaterials of TypeID

    let init () =
        let state = DataPending

        // At first, we download
        let cmd = Cmd.OfAsync.either Data.prepare () DataPreparationFinished raise

        state, cmd

    let update msg state =
        match msg with
        | DataPreparationFinished data -> data |> ReadyState.init |> DataReady, Cmd.none
        | SearchChanged search ->
            State.map
                (fun ready ->
                    { ready with
                        materials = Seq.empty
                        search = search
                    })
                state,
            Cmd.none
        | CalculateBasicMaterials id ->
            State.map
                (fun ready ->
                    let materials = Analyse.getBasicMaterials ready.data 1 id
                    { ready with materials = materials })
                state,
            Cmd.none

    let renderDataPending () =
        [
            TextBlock.create [
                TextBlock.text "Downloading data from SDE..."

                TextBlock.fontSize 36.
                TextBlock.horizontalAlignment HorizontalAlignment.Center
                TextBlock.verticalAlignment VerticalAlignment.Center
            ]
            :> IView
        ]

    let renderDataReady state dispatch =
        [
            StackPanel.create [
                StackPanel.children [
                    TextBox.create [
                        TextBox.text state.search
                        TextBox.onTextChanged (SearchChanged >> dispatch)
                    ]
                    let typeId = Map.tryFind state.search state.data.typeNameIdMap

                    match typeId with
                    | Some typeId ->
                        Button.create [
                            Button.content "Calculate basic materials"
                            Button.onClick ((fun _ -> CalculateBasicMaterials typeId |> dispatch), OnChangeOf typeId)
                        ]
                    | None -> ()

                    if not (Seq.isEmpty state.materials) then
                        StackPanel.create [
                            StackPanel.margin (10., 10., 10., 10.)
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.fontWeight FontWeight.Bold
                                    TextBlock.fontSize 14.
                                    TextBlock.margin (0., 0., 0., 5.)
                                    TextBlock.text "Materials:"
                                ]
                                for (typeID, quantity, histories) in
                                    state.materials
                                    |> Seq.sortByDescending (fun (_, x, _) -> x) do
                                    let typ = Map.find typeID state.data.types

                                    let histories =
                                        let limit = 15

                                        histories
                                        |> Seq.truncate limit
                                        |> Seq.map (fun history ->
                                            let limit = 4

                                            history
                                            |> List.truncate limit
                                            |> List.rev
                                            |> List.map (fun typeId -> (state.data.types.Item typeId).TypeName)
                                            // Append ... if list was truncated
                                            |> match List.length history with
                                               | x when x > limit -> List.append [ "..." ]
                                               | _ -> id
                                            |> String.concat " > ")
                                        // Append ... if list was truncated
                                        |> match Seq.length histories with
                                           | x when x > limit -> (fun x -> Seq.append x [ "..." ])
                                           | _ -> id
                                        |> String.concat "\n"

                                    let quantityString =
                                        String
                                            .Format("{0:#,#.###}", (quantity: int64))
                                            .PadLeft(20)

                                    TextBlock.create [
                                        TextBlock.text $"%s{quantityString}: %s{typ.TypeName}"
                                        TextBlock.tip histories
                                        TextBlock.fontFamily Config.monospaceFont
                                    ]
                            ]
                        ]
                ]
            ]
            :> IView
        ]

    let render (state: State) dispatch =
        DockPanel.create [
            DockPanel.children [
                yield!
                    match state with
                    | DataPending -> renderDataPending ()
                    | DataReady state -> renderDataReady state dispatch
            ]
        ]
        :> IView
