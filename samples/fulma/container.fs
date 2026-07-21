// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Container

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Fable.React
ашық Fable.React.Props
ашық Fulma

болсын basic () =
    Container.container [ Container.IsFluid ]
        [ Content.content [ ]
            [ h1 [ ] [str "Hello World"]
              p [ ]
                [ str "Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                      Nulla accumsan, metus ultrices eleifend gravida, nulla nunc varius lectus
                      , nec rutrum justo nibh eu lectus. Ut vulputate semper dui. Fusce erat odio
                      , sollicitudin vel erat vel, interdum mattis neque." ] ] ]

div [] [
    Card.card [] [Card.content [] [basic()] ]
] |> mountById "elmish-app"
