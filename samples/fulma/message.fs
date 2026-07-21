// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Message

ашық Fable.React
ашық Fulma

болсын loremText =
    "Donec fermentum interdum elit, ішінде congue justo maximus congue. Mauris tincidunt ultricies lacus, vel pulvinar diam luctus et. In vel tellus vitae dolor efficitur pulvinar eu non tortor. Nunc eget augue id nisl bibendum congue vitae vitae purus. Phasellus pharetra nunc at justo dictum rutrum. Nullam diam diam, tincidunt id interdum a, rutrum ac lorem."

болсын basic () =
    Message.message [ ]
        [ Message.header [ ]
            [ str "Nunc finibus ligula et semper suscipit"
              Delete.delete [ ]
                [ ] ]
          Message.body [ ]
            [ str loremText ] ]

болсын color () =
    div [ ]
        [ Message.message [ Message.Color IsInfo ]
            [ Message.header [ ]
                [ str "Nunc finibus ligula et semper suscipit"
                  Delete.delete [ ]
                    [ ] ]
              Message.body [ ]
                [ str loremText ] ]
          Message.message [ Message.Color IsDanger ]
            [ Message.header [ ]
                [ str "Nunc finibus ligula et semper suscipit"
                  Delete.delete [ ]
                    [ ] ]
              Message.body [ ]
                [ str loremText ] ] ]

болсын sizes () =
    Message.message [ Message.Size IsSmall ]
        [ Message.header [ ]
            [ str "Nunc finibus ligula et semper suscipit"
              Delete.delete [ ]
                [ ] ]
          Message.body [ ]
            [ str loremText ] ]

болсын bodyOnly () =
    Message.message [ ]
        [ Message.body [ ]
            [ str loremText ] ]

div [] [
    Card.card [] [Card.content [] [basic()] ]
    Card.card [] [Card.content [] [color()] ]
    Card.card [] [Card.content [] [sizes()] ]
    Card.card [] [Card.content [] [bodyOnly()] ]
] |> mountById "elmish-app"
