// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Dropdown

ашық Fable.React
ашық Fable.React.Props
ашық Fulma

болсын basic () =
    Dropdown.dropdown [ Dropdown.IsHoverable ]
        [ div [ ]
            [ Button.button [ ]
                [ span [ ]
                    [ str "Dropdown" ]
                  Icon.icon [ Icon.Size IsSmall ] [ i [ Class "fas fa-angle-down" ] [] ] ] ]
          Dropdown.menu [ ]
            [ Dropdown.content [ ]
                [ Dropdown.Item.a [ ] [ str "Item n°1" ]
                  Dropdown.Item.a [ ] [ str "Item n°2" ]
                  Dropdown.Item.a [ Dropdown.Item.IsActive true ] [ str "Item n°3" ]
                  Dropdown.Item.a [ ] [ str "Item n°4" ]
                  Dropdown.divider [ ]
                  Dropdown.Item.a [ ] [ str "Item n°5" ] ] ] ]

div [] [
    Card.card [] [Card.content [] [basic()] ]
] |> mountById "elmish-app"
