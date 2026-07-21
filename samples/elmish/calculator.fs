модуль Elmish.Calculator

(**
 Calculator sample, by Zaid Ajaj.
 You can find more info about Emish architecture and samples at https://elmish.github.io/
*)

ашық Fable.React
ашық Fable.React.Props
ашық Elmish
ашық Elmish.React

// Types
түрі Input =
    | Const бастап int
    | Plus
    | Minus
    | Times
    | Div
    | Clear
    | Equals

түрі Model =  InputStack бастап Input list

түрі Messages = PushInput бастап Input

// State

/// Active pattern that matches с an operation
болсын (|Operation|_|) = функция
    | Plus -> Some Plus
    | Minus -> Some Minus
    | Times -> Some Times
    | Div -> Some Div
    | _ -> None

/// Given a model, calculate the answer
болсын solve (state : Input list) =
  сәйкестік state с
  | [Const x; Operation op; Const y] ->
      сәйкестік op с
      | Plus -> Some (x + y)
      | Minus -> Some (x - y)
      | Times -> Some (x * y)
      | Div when y = 0 -> None // division by zero not allowed
      | Div -> Some (x / y)
      | _ -> None
  | _ -> None


/// Given two integers, append the second on the first
/// concatInts 3 5 -> 35
/// concatInts 1 1 -> 11
болсын concatInts x y = x * 10 + y * (sign x)

болсын initialState() : Model = InputStack []

/// Given the input message and the state бастап the app, calculate the next state. This is known as the update функция
болсын update (PushInput input) (InputStack xs)  =
    егер input = Clear содан InputStack []
    басқа
    сәйкестік xs с
    | [] ->
        сәйкестік input с
        | Minus -> InputStack [Minus]
        | Operation op -> InputStack [ ]
        | Equals -> InputStack []
        | _ -> InputStack [input]
    | [Minus] ->
        сәйкестік input с
        | Const x -> InputStack [ Const (-x) ]
        | _ -> InputStack xs
    | [Const x] ->
        сәйкестік input с
        | Const y -> InputStack [Const (concatInts x y)]
        | Operation op -> InputStack [Const x; op]
        | _ -> InputStack xs
    | [Const x; Operation op] ->
        сәйкестік input с
        | Const y -> InputStack [Const x; op; Const y] // push Const y to stack
        | Minus when op = Minus -> InputStack [Const x; Plus] // Minus Minus = Plus
        | Minus -> InputStack [Const x; op; Minus]
        | Operation otherOp -> InputStack [Const x; otherOp] // replace op с otherOp
        | _ -> InputStack xs // жасау nothing
    | [Const x; Operation op; Minus] ->
        сәйкестік input с
        | Const y -> InputStack [Const x; op; Const (-y)]
        | _ -> InputStack xs
    | [Const x; Operation op; Const y] ->
        сәйкестік input с
        | Const y' -> InputStack [Const x; op; Const (concatInts y y')]
        | Equals ->
            сәйкестік solve xs с
            | Some answer -> InputStack [Const answer]
            | None -> InputStack xs
        | Operation op ->
            сәйкестік solve xs с
            | Some answer -> InputStack [Const answer; op]
            | None -> InputStack xs
        | _ -> InputStack xs
    | _ -> InputStack xs

// View
болсын inputToString = функция
    | Plus -> "+"
    | Minus -> "-"
    | Times -> "*"
    | Div -> "/"
    | Equals -> "="
    | Clear -> "CE"
    | Const n -> string n

болсын modelToString (InputStack xs) =
    xs
    |> Seq.map inputToString
    |> String.concat " "

болсын digitBtn n dispatch =
    болсын message = PushInput (Const n)
    div
      [ Class "calculator-button is-digit"; OnClick (функ _ -> dispatch message) ]
      [ str (string n) ]

болсын operationBtn input dispatch =
    болсын message = PushInput input
    div
        [ Class "calculator-button is-op"; OnClick (функ _ -> dispatch message) ]
        [ str (inputToString input) ]

болсын tableRow xs = tr [] [ үшін x ішінде xs -> td [] [x] ]

болсын view model dispatch =
    болсын digit n = digitBtn n dispatch
    болсын opBtn op = operationBtn op dispatch
    div
      [ Class "calculator" ]
      [ h2
          [ Style [ Height 40; MarginLeft 20 ] ]
          [ str (modelToString model) ]
        br []
        table []
            [ tableRow [digit 1; digit 2; digit 3; opBtn Plus]
              tableRow [digit 4; digit 5; digit 6; opBtn Minus]
              tableRow [digit 7; digit 8; digit 9; opBtn Times]
              tableRow [opBtn Input.Clear; digit 0; opBtn Equals; opBtn Div] ] ]

// App
Program.mkSimple initialState update view
|> Program.withReactSynchronous "elmish-app"
|> Program.run
