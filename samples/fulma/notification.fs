// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Notification

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Fable.React
ашық Fable.React.Props
ашық Fulma

болсын basic () =
    Notification.notification [ ]
        [ str "I am a notification" ]

болсын color () =
    Notification.notification [ Notification.Color IsSuccess ]
        [ str "I am a notification с some colors" ]

болсын withCross () =
    Notification.notification [ Notification.Color IsDanger ]
        [ Notification.delete [ ] [ ]
          str "I am a notification с some colors and a delete button" ]

div [] [
    Card.card [] [Card.content [] [basic()] ]
    Card.card [] [Card.content [] [color()] ]
    Card.card [] [Card.content [] [withCross()] ]
] |> mountById "elmish-app"
