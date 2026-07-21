// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Box

ашық Fable.React
ашық Fable.React.Props
ашық Fulma

болсын basic () =
    div [ Class "block" ]
        [ Box.box' [ ]
            [ str "Lorem ipsum dolor sit amet, consectetur adipisicing elit
                   , sed жасау eiusmod tempor incididunt ut labore et dolore magna aliqua."] ]

div [] [
    Card.card [] [Card.content [] [basic()] ]
] |> mountById "elmish-app"
