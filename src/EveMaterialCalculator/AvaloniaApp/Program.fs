namespace EveMaterialCalculator.AvaloniaApp

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Diagnostics
open Avalonia.FuncUI.Elmish
open Elmish
open Avalonia.FuncUI
open Avalonia.FuncUI.Components.Hosts
open System

module Program =
    type EveMaterialCalculatorWindow() =
        inherit HostWindow()

        let mutable customHandler = []

        let removeAllCustomHandler () =
            customHandler
            |> List.iter (fun (x: IDisposable) -> x.Dispose())

    type MainWindow() as this =
        inherit EveMaterialCalculatorWindow()

        do
            base.Title <- "EveMaterialCalculator"
            base.Width <- 1024.0
            base.Height <- 660.0

#if DEBUG
            DevTools.Attach(this, Config.devToolGesture)
            |> ignore
#endif

            Program.mkProgram App.init App.update App.render
            |> Program.withHost this
#if DEBUG
            |> Program.withConsoleTrace
#endif
            |> Program.runWith ()

    type EveMaterialCalculatorApplication() =
        inherit Application()

        override this.Initialize() =
            this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
            this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"

        override this.OnFrameworkInitializationCompleted() =
            match this.ApplicationLifetime with
            | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
                desktopLifetime.MainWindow <- MainWindow()
            | _ -> ()

    [<EntryPoint>]
    let main (args: string []) : int =
        AppBuilder
            .Configure<EveMaterialCalculatorApplication>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
