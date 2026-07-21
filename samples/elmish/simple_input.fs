модуль Elmish.SimpleInput

(**
Minimal application showing how to use Elmish
You can find more info about Emish architecture and samples at https://elmish.github.io/
*)

ашық Fable.Core.JsInterop
ашық Fable.React
ашық Fable.React.Props
ашық Elmish
ашық Elmish.React

// MODEL

түрі Model =
    { Value : string }

түрі Msg =
    | ChangeValue бастап string

болсын init () = { Value = "" }, Cmd.none

// UPDATE

болсын update (msg:Msg) (model:Model) =
    сәйкестік msg с
    | ChangeValue newValue ->
        { model с Value = newValue }, Cmd.none

// VIEW (rendered с React)

болсын view model dispatch =
    div [ Class "main-container" ]
        [ input [ Class "input"
                  Value model.Value
                  OnChange (функ ev -> ev.target?value |> string |> ChangeValue |> dispatch) ]
          span [ ]
            [ str "Hello, "
              str model.Value
              str "!" ] ]

// App
Program.mkProgram init update view
|> Program.withConsoleTrace
|> Program.withReactSynchronous "elmish-app"
|> Program.run
