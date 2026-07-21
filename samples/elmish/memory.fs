модуль Elmish.Memory

(**
 Classic Memory game, by Zaid Ajaj.
 You can find more info about Emish architecture and samples at https://elmish.github.io/
*)

ашық Fable.React
ашық Fable.React.Props
ашық Browser
ашық Elmish
ашық Elmish.React

// Types
түрі Card = {
    Id : int
    ImgUrl : string
    Selected : bool
    MatchFound : bool
}

түрі Model = {
    Cards : Card list
    FirstSelection : int option
    SecondSelection : int option
}

түрі Actions =
    | SelectCard бастап int
    | StartNewGame
    | NoOp

// State
болсын random = жаңа System.Random()

болсын origin =
    // Sample is running ішінде an iframe, so get the location бастап parent
    болсын topLocation = window.top.location
    topLocation.origin + topLocation.pathname

болсын getCards() =
    болсын images = [ "violin"; "electric-guitar"; "headphones"; "piano"; "saxophone";  "trumpet";"turntable";"bass-guitar" ]
    images
    |> List.append images
    |> List.sortBy (функ img -> random.Next())
    |> List.map (sprintf "%simg/memory/%s.png" origin)
    |> List.mapi (функ index img -> { Id = index; ImgUrl = img; Selected = false; MatchFound = false})

болсын initialModel() = {
    Cards = getCards()
    FirstSelection = None
    SecondSelection = None
}

болсын cardsEqual id1 id2 (cards: Card list) =
    болсын card1 = cards |> List.find (функ c -> c.Id = id1)
    болсын card2 = cards |> List.find (функ c -> c.Id = id2)
    card1.ImgUrl = card2.ImgUrl

болсын cardSelected id (cards: Card list) =
    болсын card = List.find (функ c -> c.Id = id) cards
    card.Selected

болсын gameCleared (model: Model) =
    List.forall (функ card -> card.MatchFound) model.Cards

болсын update action model  =
    сәйкестік action с
    | StartNewGame -> initialModel()
    | SelectCard index ->
        сәйкестік model.FirstSelection, model.SecondSelection с
        | None, None ->
            болсын cards =
                model.Cards
                |> List.map (функ card ->
                    егер card.Id = index
                    содан { card с Selected = true }
                    басқа card)
            { model с Cards = cards; FirstSelection = Some index }
        | Some id, None when id = index -> model
        | Some id, None when cardsEqual id index (model.Cards) ->
            болсын cards =
                model.Cards
                |> List.map (функ card ->
                    егер card.Id = index || card.Id = id
                    содан { card с Selected = true; MatchFound = true }
                    басқа card)
            { model с Cards = cards; FirstSelection = None; SecondSelection = None }
        | Some id, None when id <> index ->
            болсын cards =
                model.Cards
                |> List.map (функ card ->
                    егер card.Id = index
                    содан { card с Selected = true }
                    басқа card)
            { Cards = cards; FirstSelection = Some id; SecondSelection = Some index }
        | Some id, Some id' when cardsEqual id' index (model.Cards) ->
            болсын cards =
                model.Cards
                |> List.map (функ card ->
                    егер (card.Id = id && not card.MatchFound)
                    содан { card с Selected = false }
                    басегер (card.Id = id' || card.Id = index)
                    содан { card с Selected = true; MatchFound = true }
                    басқа card)
            { model с Cards = cards; FirstSelection = None; SecondSelection = None }
        | Some id, Some id' ->
            болсын cards =
                model.Cards
                 |> List.map (функ card ->
                      егер (card.Id = id || card.Id = id') && not card.MatchFound
                      содан { card с Selected = false }
                      басегер card.Id = index
                      содан { card с Selected = true }
                      басқа card
                 )
            { Cards = cards; FirstSelection = Some index; SecondSelection = None }
        | _, _ -> failwith "Cannot happen :)"
    | NoOp -> model

// View
болсын cardClicked (card: Card) dispatch  =
   егер not (card.MatchFound) && not (card.Selected)
   содан dispatch (SelectCard card.Id)
   басқа dispatch (NoOp)

болсын viewCard (card: Card) dispatch =
    div
      [ classList [ "card-container", true; "сәйкестік-found", card.MatchFound]
        OnClick (функ _ -> cardClicked card dispatch) ]
      [ img [ Src (егер card.Selected содан card.ImgUrl басқа origin + "img/memory/fable.jpg") ] ]

болсын view model dispatch =
    егер gameCleared model содан
        h1
          [ Class "winner centered"
            Style [ Padding 20; Width "500px" ]
            OnClick (функ _ -> dispatch StartNewGame ) ]
          [ str "You win, Click me to play again" ]
    басқа
        div [ Class "container centered"
              Style [ Width "500px" ] ]
            [ үшін card ішінде model.Cards -> viewCard card dispatch ]

// App
Program.mkSimple initialModel update view
|> Program.withReactSynchronous "elmish-app"
|> Program.run
