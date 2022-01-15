namespace EveMaterialCalculator.AvaloniaApp

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Elmish

open EveMaterialCalculator.Core

module App =
    type ReadyState = unit

    type State =
        | DataPending
        | DataReady of ReadyState

    type Msg = | DownloadFinished

    let init () =
        let state = DataPending

        // At first, we download
        let cmd =
            Cmd.OfAsync.either DataDownload.start () (fun _ -> DownloadFinished) raise

        state, cmd

    let update msg state =
        match msg with
        | DownloadFinished -> DataReady(), Cmd.none

    let renderDataPending () =
        TextBlock.create [
            TextBlock.text "Downloading data from SDE..."

            TextBlock.fontSize 36.
            TextBlock.horizontalAlignment HorizontalAlignment.Center
            TextBlock.verticalAlignment VerticalAlignment.Center
        ]

    let renderDataReady state dispatch =
        TextBlock.create [
            TextBlock.text "Data ready!"

            TextBlock.fontSize 36.
            TextBlock.horizontalAlignment HorizontalAlignment.Center
            TextBlock.verticalAlignment VerticalAlignment.Center
        ]

    let render (state: State) dispatch =
        DockPanel.create [
            DockPanel.children [
                match state with
                | DataPending -> renderDataPending ()
                | DataReady state -> renderDataReady state dispatch
            ]
        ]
        :> IView
