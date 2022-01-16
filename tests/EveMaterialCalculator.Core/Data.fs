module EveMaterialCalculator.Core.Tests.Data

open Expecto
open Expecto.ExpectoFsCheck

open EveMaterialCalculator.Core

let preparedData = Data.prepare () |> Async.RunSynchronously

[<Tests>]
let tests = testList "Test list" []
