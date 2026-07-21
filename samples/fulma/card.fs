// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Card

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Fable.React
ашық Fable.React.Props
ашық Fulma

болсын basic () =
    Card.card [ ]
        [ Card.header [ ]
            [ Card.Header.title [ ]
                [ str "Component" ]
              Card.Header.icon [ ]
                [ i [ Class "fa fa-angle-down" ] [ ] ] ]
          Card.content [ ]
            [ Content.content [ ]
                [ str "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus nec iaculis mauris." ] ]
          Card.footer [ ]
            [ Card.Footer.div [ ]
                [ str "Save" ]
              Card.Footer.div [ ]
                [ str "Edit" ]
              Card.Footer.div [ ]
                [ str "Delete" ] ] ]

болсын centered () =
    Card.card [ ]
        [ Card.header [ ]
            [ Card.Header.title [ Card.Header.Title.IsCentered ]
                [ str "Component" ]
              Card.Header.icon [ ]
                [ i [ Class "fa fa-angle-down" ] [ ] ] ]
          Card.content [ ]
            [ Content.content [ ]
                [ str "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus nec iaculis mauris." ] ]
          Card.footer [ ]
            [ Card.Footer.div [ ]
                [ str "Save" ]
              Card.Footer.div [ ]
                [ str "Edit" ]
              Card.Footer.div [ ]
                [ str "Delete" ] ] ]

div [] [
    Card.card [] [Card.content [] [basic()] ]
    Card.card [] [Card.content [] [centered()] ]
] |> mountById "elmish-app"
