namespace EveMaterialCalculator.AvaloniaApp

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

module App =
    type State = unit

    type Msg = | Void

    let init () : State = ()

    let update msg (state: State) : State = ()

    let render (state: State) dispatch =
        DockPanel.create [
            DockPanel.children [
                TextBlock.create [
                    TextBlock.text "Hallo Welt!"
                ]
            ]
        ]
        :> IView
