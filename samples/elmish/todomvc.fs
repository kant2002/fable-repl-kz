модуль Elmish.TodoMVC

(**
 TodoMVC app ported from Elm.
 You can find more info about Emish architecture and samples at https://elmish.github.io/
 NOTE: The API ішінде Fable's REPL may differ from Fable.Elmish & Fable.React nuget libraries.
       The generated JS code won't be as optimized as when using dotnet-fable.
*)

ашық Fable.Core
ашық Fable.React
ашық Fable.React.Props
ашық Browser.Types
ашық Browser
ашық Elmish
ашық Elmish.React

болсын [<Literal>] ESC_KEY = 27.
болсын [<Literal>] ENTER_KEY = 13.

түрі WhatIsVisible =
   | All
   | Active
   | Completed

болсын toStr v =
    сәйкестік v с
    | All -> "All"
    | Active -> "Active"
    | Completed -> "Completed"

// MODEL
түрі Entry =
    { description : string
      completed : bool
      editing : bool
      id : int }

// The full application state бастап our todo app.
түрі Model =
    { entries : Entry list
      field : string
      uid : int
      visibility : WhatIsVisible }

болсын emptyModel () =
    { entries = []
      visibility = All
      field = ""
      uid = 0 }

болсын newEntry desc id =
  { description = desc
    completed = false
    editing = false
    id = id }

// UPDATE

(** Users бастап our app can trigger messages by clicking and typing. These
messages are fed into the `update` функция as they occur, letting us react
to them.
*)
түрі Msg =
    | Failure бастап string
    | UpdateField бастап string
    | EditingEntry бастап int*bool
    | UpdateEntry бастап int*string
    | Add
    | Delete бастап int
    | DeleteComplete
    | Check бастап int*bool
    | CheckAll бастап bool
    | ChangeVisibility бастап WhatIsVisible

// How we update our Model on a given Msg?
болсын update (msg:Msg) (model:Model) =
    сәйкестік msg с
    | Failure err ->
        JS.console.error(err)
        model

    | Add ->
        болсын xs = егер System.String.IsNullOrEmpty model.field содан
                    model.entries
                 басқа
                    model.entries @ [newEntry model.field model.uid]
        { model с
            uid = model.uid + 1
            field = ""
            entries = xs }

    | UpdateField str ->
      { model с field = str }

    | EditingEntry (id,isEditing) ->
        болсын updateEntry t =
          егер t.id = id содан { t с editing = isEditing } басқа t
        { model с entries = List.map updateEntry model.entries }

    | UpdateEntry (id,task) ->
        болсын updateEntry t =
          егер t.id = id содан { t с description = task } басқа t
        { model с entries = List.map updateEntry model.entries }

    | Delete id ->
        { model с entries = List.filter (функ t -> t.id <> id) model.entries }

    | DeleteComplete ->
        { model с entries = List.filter (функ t -> not t.completed) model.entries }

    | Check (id,isCompleted) ->
        болсын updateEntry t =
          егер t.id = id содан { t с completed = isCompleted } басқа t
        { model с entries = List.map updateEntry model.entries }

    | CheckAll isCompleted ->
        болсын updateEntry t = { t с completed = isCompleted }
        { model с entries = List.map updateEntry model.entries }

    | ChangeVisibility visibility ->
        { model с visibility = visibility }

болсын onEnter msg dispatch =
    OnKeyDown (функ ev ->
        егер ev.keyCode = ENTER_KEY содан
            dispatch msg)

болсын targetValue (ev: Event) =
    (ev.target :?> HTMLInputElement).value

болсын viewInput (model:string) dispatch =
    header [ Class "header" ] [
        h1 [] [ str "todos" ]
        input [
            Class "жаңа-todo"
            Placeholder "What needs to be done?"
            Value model
            onEnter Add dispatch
            OnChange (функ ev ->
                targetValue ev |> UpdateField |> dispatch)
            AutoFocus true
        ]
    ]

болсын classList classes =
    classes
    |> List.fold (функ complete -> функция | (name,true) -> complete + " " + name | _ -> complete) ""
    |> Class

болсын viewEntry todo dispatch =
  li
    [ classList [ ("completed", todo.completed); ("editing", todo.editing) ]]
    [ div
        [ Class "view" ]
        [ input
            [ Class "toggle"
              Type "checkbox"
              Checked todo.completed
              OnChange (функ _ -> Check (todo.id,(not todo.completed)) |> dispatch) ]
          label
            [ OnDoubleClick (функ _ -> EditingEntry (todo.id,true) |> dispatch) ]
            [ str todo.description ]
          button
            [ Class "destroy"
              OnClick (функ _-> Delete todo.id |> dispatch) ]
            []
        ]
      input
        [ Class "edit"
          Value todo.description
          Name "title"
          Id ("todo-" + (string todo.id))
          OnInput (функ ev -> UpdateEntry (todo.id, targetValue ev) |> dispatch)
          OnBlur (функ _ -> EditingEntry (todo.id,false) |> dispatch)
          onEnter (EditingEntry (todo.id,false)) dispatch ]
    ]

болсын viewEntries visibility entries dispatch =
    болсын isVisible todo =
        сәйкестік visibility с
        | Completed -> todo.completed
        | Active -> not todo.completed
        | All -> true

    болсын allCompleted =
        List.forall (функ t -> t.completed) entries

    болсын cssVisibility =
        егер List.isEmpty entries содан "hidden" басқа "visible"

    section
      [ Class "main"
        Style [ Visibility cssVisibility ]]
      [ input
          [ Class "toggle-all"
            Type "checkbox"
            Name "toggle"
            Checked allCompleted
            OnChange (функ _ -> CheckAll (not allCompleted) |> dispatch)]
        label
          [ HtmlFor "toggle-all" ]
          [ str "Mark all as complete" ]
        ul
          [ Class "todo-list" ]
          (entries
           |> List.filter isVisible
           |> List.map (функ i -> viewEntry i dispatch)) ]

// VIEW CONTROLS AND FOOTER
болсын visibilitySwap uri visibility actualVisibility dispatch =
  li
    [ OnClick (функ _ -> ChangeVisibility visibility |> dispatch) ]
    [ a [ Href uri
          classList ["selected", visibility = actualVisibility] ]
          [ str (toStr visibility) ] ]

болсын viewControlsFilters visibility dispatch =
  ul
    [ Class "filters" ]
    [ visibilitySwap "#/" All visibility dispatch
      str " "
      visibilitySwap "#/active" Active visibility dispatch
      str " "
      visibilitySwap "#/completed" Completed visibility dispatch ]

болсын viewControlsCount entriesLeft =
  болсын item =
      егер entriesLeft = 1 содан " item" басқа " items"

  span
      [ Class "todo-count" ]
      [ strong [] [ str (string entriesLeft) ]
        str (item + " left") ]

болсын viewControlsClear entriesCompleted dispatch =
  button
    [ Class "clear-completed"
      Hidden (entriesCompleted = 0)
      OnClick (функ _ -> DeleteComplete |> dispatch)]
    [ str ("Clear completed (" + (string entriesCompleted) + ")") ]

болсын viewControls visibility entries dispatch =
  болсын entriesCompleted =
      entries
      |> List.filter (функ t -> t.completed)
      |> List.length

  болсын entriesLeft =
      List.length entries - entriesCompleted

  footer
      [ Class "footer"
        Hidden (List.isEmpty entries) ]
      [ viewControlsCount entriesLeft
        viewControlsFilters visibility dispatch
        viewControlsClear entriesCompleted dispatch ]

болсын infoFooter =
  footer [ Class "info" ]
    [ p []
        [ str "Double-click to edit a todo" ]
      p []
        [ str "Ported from Elm by "
          a [ Href "https://github.com/et1975" ] [ str "Eugene Tolmachev" ]]
      p []
        [ str "Part бастап "
          a [ Href "http://todomvc.com" ] [ str "TodoMVC" ]]
    ]

болсын view model dispatch =
  div
    [ Class "todomvc-wrapper"]
    [ section
        [ Class "todoapp" ]
        [ viewInput model.field dispatch
          viewEntries model.visibility model.entries dispatch
          viewControls model.visibility model.entries dispatch ]
      infoFooter ]

// App
Program.mkSimple emptyModel update view
|> Program.withReactSynchronous "todoapp"
|> Program.withConsoleTrace
|> Program.run
