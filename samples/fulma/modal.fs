// More info about Fulma at https://mangelmaxime.github.io/Fulma/
модуль Fulma.Modal

ашық Fable.React
ашық Fable.React.Props
ашық Fulma
ашық Fable.Core
ашық Fable.Import

болсын content =
    Content.content [ ]
        [ h1 [ ] [str "Hello World"]
          p [ ]
            [ str "Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                  Nulla accumsan, metus ultrices eleifend gravida, nulla nunc varius lectus
                  , nec rutrum justo nibh eu lectus. Ut vulputate semper dui. Fusce erat odio
                  , sollicitudin vel erat vel, interdum mattis neque." ]
          h2 [ ] [str "Second level" ]
          p [ ]
            [ str "Curabitur accumsan turpis pharetra "
              strong [ ] [str "augue tincidunt" ]
              str "blandit. Quisque condimentum maximus mi
                  , sit amet commodo arcu rutrum id. Proin pretium urna vel cursus venenatis.
                  Suspendisse potenti. Etiam mattis sem rhoncus lacus dapibus facilisis.
                  Donec at dignissim dui. Ut et neque nisl." ]
          ul [ ]
             [ li [ ] [str "In fermentum leo eu lectus mollis, quis dictum mi aliquet." ]
               li [ ] [str "Morbi eu nulla lobortis, lobortis est ішінде, fringilla felis." ]
               li [ ] [str "Aliquam nec felis ішінде sapien venenatis viverra fermentum nec lectus." ]
               li [ ] [str "Ut non enim metus."] ]
          p [ ] [str "Sed sagittis enim ac tortor maximus rutrum.
                     Nulla facilisi. Donec mattis vulputate risus ішінде luctus.
                     Maecenas vestibulum interdum commodo." ] ]

болсын basicModal isActive closeDisplay =
    Modal.modal [ Modal.IsActive isActive ]
        [ Modal.background [ Props [ OnClick closeDisplay ] ] [ ]
          Modal.content [ ]
            [ Box.box' [ ]
                [ content ] ]
          Modal.close [ Modal.Close.Size IsLarge
                        Modal.Close.OnClick closeDisplay ] [ ] ]

болсын closeDisplay = ignore

болсын basicModalCode () =
    Modal.modal [ Modal.IsActive true ]
        [ Modal.background [ Props [ OnClick closeDisplay ] ] [ ]
          Modal.content [ ]
            [ Box.box' [ ]
                [ content ] ]
          Modal.close [ Modal.Close.Size IsLarge
                        Modal.Close.OnClick closeDisplay ] [ ] ]

болсын cardModal isActive closeDisplay =
    Modal.modal [ Modal.IsActive isActive ]
        [ Modal.background [ Props [ OnClick closeDisplay ] ] [ ]
          Modal.Card.card [ ]
            [ Modal.Card.head [ ]
                [ Modal.Card.title [ ]
                    [ str "Modal title" ]
                  Delete.delete [ Delete.OnClick closeDisplay ] [ ] ]
              Modal.Card.body [ ]
                [ content ]
              Modal.Card.foot [ ]
                [ Button.button [ Button.Color IsSuccess ]
                    [ str "Save changes" ]
                  Button.button [ ]
                    [ str "Cancel" ] ] ] ]

болсын cardModalCode () =
    Modal.modal [ Modal.IsActive true ]
        [ Modal.background [ Props [ OnClick closeDisplay ] ] [ ]
          Modal.Card.card [ ]
            [ Modal.Card.head [ ]
                [ Modal.Card.title [ ]
                    [ str "Modal title" ]
                  Delete.delete [ Delete.OnClick closeDisplay ] [ ] ]
              Modal.Card.body [ ]
                [ content ]
              Modal.Card.foot [ ]
                [ Button.button [ Button.Color IsSuccess ]
                    [ str "Save changes" ]
                  Button.button [ ]
                    [ str "Cancel" ] ] ] ]

div [] [
    Card.card [] [Card.content [] [basicModalCode()] ]
    Card.card [] [Card.content [] [cardModalCode()] ]
] |> mountById "elmish-app"
