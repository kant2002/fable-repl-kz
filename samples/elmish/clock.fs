модуль Elmish.Clock

(**
 Timer as a source бастап events с an SVG clock, by Zaid Ajaj.
 You can find more info about Emish architecture and samples at https://elmish.github.io/
*)

ашық System
ашық Fable.React
ашық Fable.React.Props
ашық Browser
ашық Elmish
ашық Elmish.React
түрі SVG = SVGAttr

// Types
түрі Model = CurrentTime бастап DateTime
түрі Messages = Tick бастап DateTime

// State
болсын initialState() =
    CurrentTime DateTime.Now, Cmd.none

болсын update (Tick next) (CurrentTime _time) =
    CurrentTime next, Cmd.none

болсын timerTick dispatch =
    window.setInterval(функ _ ->
        dispatch (Tick DateTime.Now)
    , 1000) |> ignore

// View
түрі Time =
    | Hour бастап int
    | Minute бастап int
    | Second бастап int

болсын clockHand time color width length =
    болсын clockPercentage =
        сәйкестік time с
        | Hour n -> (float n) / 12.0
        | Second n -> (float n) / 60.0
        | Minute n -> (float n) / 60.0
    болсын angle = 2.0 * Math.PI * clockPercentage
    болсын handX = (50.0 + length * cos (angle - Math.PI / 2.0))
    болсын handY = (50.0 + length * sin (angle - Math.PI / 2.0))
    line [ X1 "50"
           Y1 "50"
           X2 handX
           Y2 handY
           // Qualify these props to avoid name collision с CSSProp
           SVG.Stroke color
           SVG.StrokeWidth width ] []

болсын handTop n color length fullRound =
    болсын revolution = float n
    болсын angle = 2.0 * Math.PI * (revolution / fullRound)
    болсын handX = (50.0 + length * cos (angle - Math.PI / 2.0))
    болсын handY = (50.0 + length * sin (angle - Math.PI / 2.0))
    circle [ Cx handX
             Cy handY
             R "2"
             SVG.Fill color ] []

болсын view (CurrentTime time) dispatch =
    svg
      [ ViewBox "0 0 100 100"
        SVG.Width "350px" ]
      [ circle
          [ Cx "50"
            Cy "50"
            R "45"
            SVG.Fill "#0B79CE" ] []
        // Hours
        clockHand (Hour time.Hour) "lightgreen" "2" 25.0
        handTop time.Hour "lightgreen" 25.0 12.0
        // Minutes
        clockHand (Minute time.Minute) "white" "2" 35.0
        handTop time.Minute "white" 35.0 60.0
        // Seconds
        clockHand (Second time.Second) "#023963" "1" 40.0
        handTop time.Second "#023963" 40.0 60.0
        // circle ішінде the center
        circle
          [ Cx "50"
            Cy "50"
            R "3"
            SVG.Fill "#0B79CE"
            SVG.Stroke "#023963"
            SVG.StrokeWidth 1.0 ] []
      ]

// App
Program.mkProgram initialState update view
|> Program.withSubscription (функ _ -> Cmd.ofSub timerTick)
|> Program.withReactSynchronous "elmish-app"
|> Program.run
