модуль Sudoku

ашық System.Collections.Generic
ашық Fable.React
ашық Fable.React.Props
ашық Elmish
ашық Elmish.React

түрі Box = int
түрі Sudoku = Box array array

болсын rows = id
болсын cols (sudoku:Sudoku) =
    sudoku
    |> Array.mapi (функ a row -> row |> Array.mapi (функ b cell -> sudoku.[b].[a]))

болсын getBoxIndex count row col =
   болсын n = row/count
   болсын m = col/count
   n * count + m

болсын boxes (sudoku:Sudoku) =
    болсын l = sudoku |> Array.length
    болсын d = float l |> System.Math.Sqrt |> int
    болсын list = жаңа List<_>()
    үшін a ішінде 0..l - 1 жасау
        list.Add(жаңа List<_>())

    үшін a ішінде 0..(l - 1) жасау
        үшін b ішінде 0..(l - 1) жасау
            list.[getBoxIndex d a b].Add(sudoku.[a].[b])

    list
      |> Seq.map Seq.toArray

болсын toSudoku x : Sudoku =
    x
    |> Seq.map Seq.toArray
    |> Seq.toArray

болсын allUnique numbers =
    болсын set = жаңа HashSet<_>()
    numbers
    |> Seq.filter ((<>) 0)
    |> Seq.forall set.Add

болсын solvable sudoku =
    rows sudoku
    |> Seq.append (cols sudoku)
    |> Seq.append (boxes sudoku)
    |> Seq.forall allUnique

болсын replaceAtPos (x:Sudoku) row col newValue :Sudoku =
    [| үшін a ішінде 0..(Array.length x - 1) ->
        [| үшін b ішінде 0..(Array.length x - 1) ->
            егер a = row && b = col содан newValue басқа x.[a].[b] |] |]

болсын rec substitute row col (x:Sudoku) =
    болсын a,b = егер col >= Array.length x содан row+1,0 басқа row,col
    егер a >= Array.length x содан seq { yield x } басқа
    егер x.[a].[b] = 0 содан
        [1..Array.length x]
            |> Seq.map (replaceAtPos x a b)
            |> Seq.filter solvable
            |> Seq.collect (substitute a (b+1))
     басқа substitute a (b+1) x

болсын getFirstSolution = substitute 0 0 >> Seq.head

болсын puzzle =
    [[0; 0; 8;  3; 0; 0;  6; 0; 0]
     [0; 0; 4;  0; 0; 0;  0; 1; 0]
     [6; 7; 0;  0; 8; 0;  0; 0; 0]

     [0; 1; 6;  4; 3; 0;  0; 0; 0]
     [0; 0; 0;  7; 9; 0;  0; 2; 0]
     [0; 9; 0;  0; 0; 0;  4; 0; 1]

     [0; 0; 0;  9; 1; 0;  0; 0; 5]
     [0; 0; 3;  0; 5; 0;  0; 0; 2]
     [0; 5; 0;  0; 0; 0;  0; 7; 4]]
    |> toSudoku

болсын init() = puzzle

түрі Model = Sudoku

түрі Msg =
| Reset
| Solve

болсын update (msg:Msg) (model:Model) =
    сәйкестік msg с
    | Reset -> puzzle
    | Solve -> getFirstSolution model

болсын tableRow xs = tr [] [ үшін x ішінде xs -> td [] [x] ]


болсын view (model:Model) dispatch =
    div
      []
      [ div
          [ Class "calc" ]
          [ table []
                [ үшін row ішінде model ->
                    tableRow [
                        үшін n ішінде row ->
                            div [ Class "digit" ] [
                                str (егер n = 0 содан "" басқа string n) ] ] ]
          ]
        br []
        div
          [ Class "controls" ]
          [ div
              [ Class "op-button"
                OnClick (функ _ -> dispatch Reset) ]
              [ str "Reset" ]
            div
              [ Class "op-button"
                OnClick (функ _ -> dispatch Solve) ]
              [ str "Solve" ]]]

// App
Program.mkSimple init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.run
