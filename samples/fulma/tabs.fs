// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Tabs

ашық Fable.React
ашық Fulma

болсын basic () =
    Tabs.tabs [ ]
        [ Tabs.tab [ Tabs.Tab.IsActive true ]
            [ a [ ] [ str "Fable" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Elmish" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Bulma" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Hink" ] ] ]

болсын alignment () =
    Tabs.tabs [ Tabs.IsCentered ]
        [ Tabs.tab [ Tabs.Tab.IsActive true ]
            [ a [ ] [ str "Fable" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Elmish" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Bulma" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Hink" ] ] ]

болсын size () =
    Tabs.tabs [ Tabs.Size IsLarge ]
        [ Tabs.tab [ Tabs.Tab.IsActive true ]
            [ a [ ] [ str "Fable" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Elmish" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Bulma" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Hink" ] ] ]

болсын styles () =
    Tabs.tabs [ Tabs.IsFullWidth
                Tabs.IsBoxed ]
        [ Tabs.tab [ Tabs.Tab.IsActive true ]
            [ a [ ] [ str "Fable" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Elmish" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Bulma" ] ]
          Tabs.tab [ ]
            [ a [ ] [ str "Hink" ] ] ]

div [] [
    Card.card [] [Card.content [] [basic()] ]
    Card.card [] [Card.content [] [alignment()] ]
    Card.card [] [Card.content [] [size()] ]
    Card.card [] [Card.content [] [styles()] ]
] |> mountById "elmish-app"
