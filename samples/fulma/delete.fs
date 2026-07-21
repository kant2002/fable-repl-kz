// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Delete

ашық Fable.React
ашық Fable.React.Props
ашық Fulma

болсын demoInteractive () =
    div [ Class "block" ]
        [ Delete.delete
            [ Delete.Size IsSmall ] [ ]
          Delete.delete
            [ ] [ ]
          Delete.delete
            [ Delete.Size IsMedium ] [ ]
          Delete.delete
            [ Delete.Size IsLarge ] [ ] ]

div [] [
    Card.card [] [Card.content [] [demoInteractive()] ]
] |> mountById "elmish-app"
