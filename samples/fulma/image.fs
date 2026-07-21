// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Image

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Fable.React
ашық Fable.React.Props
ашық Fulma

болсын fixedInteractive () =
    div [ Class "block" ]
        [ Image.image [ Image.Is64x64 ]
            [ img [ Src "https://dummyimage.com/64x64/7a7a7a/fff" ] ]
          br [ ]
          Image.image [ Image.Is128x128 ]
            [ img [ Src "https://dummyimage.com/128x128/7a7a7a/fff" ] ] ]

болсын responsiveInteractive () =
    div [ Class "block" ]
        [ Image.image [ Image.Is2by1 ]
            [ img [ Src "https://dummyimage.com/640x320/7a7a7a/fff" ] ] ]

div [] [
    Card.card [] [Card.content [] [fixedInteractive()] ]
    Card.card [] [Card.content [] [responsiveInteractive()] ]
] |> mountById "elmish-app"
