// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Breadcrumb

ашық Fable.React
ашық Fable.React.Props
ашық Fulma

болсын basic () =
    Breadcrumb.breadcrumb [ ]
        [ Breadcrumb.item [ ]
            [ a [ ] [ str "F#" ] ]
          Breadcrumb.item [ ]
            [ a [ ] [ str "Fable" ] ]
          Breadcrumb.item [ Breadcrumb.Item.IsActive true ]
            [ a [ ] [ str "Fable.React" ] ] ]

болсын alignmentCenter () =
    Breadcrumb.breadcrumb [ Breadcrumb.IsCentered ]
        [ Breadcrumb.item [ ]
            [ a [ ] [ str "F#" ] ]
          Breadcrumb.item [ ]
            [ a [ ] [ str "Fable" ] ]
          Breadcrumb.item [ Breadcrumb.Item.IsActive true ]
            [ a [ ] [ str "Elmish" ] ] ]

болсын icons () =
    Breadcrumb.breadcrumb [ ]
        [ Breadcrumb.item [ ]
            [ a [ ]
                [ Icon.icon [ Icon.Size IsSmall ]
                    [ i [ Class "fa fa-home" ] [ ] ]
                  str "F#" ] ]
          Breadcrumb.item [ ]
            [ a [ ]
                [ Icon.icon [ Icon.Size IsSmall ]
                    [ i [ Class "fa fa-book" ] [ ] ]
                  str "Fable" ] ]
          Breadcrumb.item [ Breadcrumb.Item.IsActive true ]
            [ a [ ]
                [ Icon.icon [ Icon.Size IsSmall ]
                    [ i [ Class "fa fa-thumbs-up" ] [ ] ]
                  str "Elmish" ] ] ]

болсын size () =
    Breadcrumb.breadcrumb [ Breadcrumb.Size IsLarge ]
        [ Breadcrumb.item [ ]
            [ a [ ] [ str "F#" ] ]
          Breadcrumb.item [ ]
            [ a [ ] [ str "Fable" ] ]
          Breadcrumb.item [ Breadcrumb.Item.IsActive true ]
            [ a [ ] [ str "Elmish" ] ] ]

болсын separator () =
    Breadcrumb.breadcrumb [ Breadcrumb.HasSucceedsSeparator ]
        [ Breadcrumb.item [ ]
            [ a [ ] [ str "F#" ] ]
          Breadcrumb.item [ ]
            [ a [ ] [ str "Fable" ] ]
          Breadcrumb.item [ Breadcrumb.Item.IsActive true ]
            [ a [ ] [ str "Elmish" ] ] ]

div [] [
    Card.card [] [Card.content [] [basic()] ]
    Card.card [] [Card.content [] [alignmentCenter()] ]
    Card.card [] [Card.content [] [icons()] ]
    Card.card [] [Card.content [] [size()] ]
    Card.card [] [Card.content [] [separator()] ]
] |> mountById "elmish-app"
