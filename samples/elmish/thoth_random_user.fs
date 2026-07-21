модуль Thoth.RandomUser

(**
Small application showing how to use:
- Thoth.Json (https://mangelmaxime.github.io/Thoth/json/v3.html)
- Promise and Fetch APIs
*)

ашық System
ашық Fable.Core
ашық Fable.React
ашық Fable.React.Props
ашық Elmish
ашық Elmish.React
ашық Thoth.Json

// MODEL
түрі Gender =
    | Male
    | Female

    статикалық мүшесі Decoder =
        Decode.string
        |> Decode.andThen (
            функция
            | "male" -> Decode.succeed Male
            | "female" -> Decode.succeed Female
            | invalid -> "`" + invalid + "` isn't a valid value үшін Gender"
                            |> Decode.fail
        )

түрі User =
    { Gender : Gender
      FullName : string
      Email : string
      CellPhone : string
      OfficePhone : string
      Age : int
      Birthday : DateTime
      Picture : string }

    статикалық мүшесі Decoder =
        // When using Thoth.Json, you are not forced to жасау a 1 to 1
        // mapping between the JSON format and your types
        // For example, ішінде the next decoder we will access deep information
        // and store it at the "root" бастап our түрі
        Decode.object (функ get ->
            // In object decoder, we can execute any F#
            // So үшін example, we can use temporary variables
            болсын firstname = get.Required.At [ "name"; "first" ] Decode.string
            болсын lastname = get.Required.At [ "name"; "last" ] Decode.string

            { Gender = get.Required.Field "gender" Gender.Decoder
              FullName = firstname + " " + lastname
              Email = get.Required.Field "email" Decode.string
              CellPhone = get.Required.Field "cell" Decode.string
              OfficePhone = get.Required.Field "phone" Decode.string
              Age = get.Required.At [ "dob"; "age" ] Decode.int
              Birthday = get.Required.At [ "dob"; "date" ] Decode.datetime
              Picture = get.Required.At [ "picture"; "large" ] Decode.string }
        )

түрі Model =
    /// Loading state
    /// If user is None, содан it's the initial loading
    | Loading бастап User option
    /// Loaded state
    | Loaded бастап User
    /// If last request results ішінде an error
    | Errored

түрі Msg =
    | FetchRandomUser
    | FetchResponse бастап Result<User, string>
    | FetchError бастап exn

/// At first, we have no user to display
болсын init () = Loading None, Cmd.ofMsg FetchRandomUser

// UPDATE

болсын жеке getRandomUser () = promise {
    // We add a delay бастап 300ms so the button animation is more visible
    do! Promise.sleep 300
    let! response = Fetch.fetch "https://randomuser.me/api/" []
    let! responseText = response.text()
    болсын resultDecoder = Decode.field "results" (Decode.index 0 User.Decoder)
    return Decode.fromString resultDecoder responseText
}
болсын update (msg:Msg) (model:Model) =
    сәйкестік msg с
    | FetchRandomUser ->
        болсын newModel =
            сәйкестік model с
            // If we have a current user
            // we keep it while waiting the жаңа user
            | Loaded user ->
                Loading (Some user)
            | _ -> Loading None

        newModel, Cmd.OfPromise.either getRandomUser () FetchResponse FetchError

    // We got a response and decoding succeded
    | FetchResponse (Ok user) ->
        Loaded user, Cmd.none

    // We got a response and decoding failed
    | FetchResponse (Error msg) ->
        JS.console.error msg
        Errored, Cmd.none

    // An error occured, when fetching the жаңа user
    | FetchError error ->
        JS.console.error error.Message
        Errored, Cmd.none

// VIEW (rendered с React)

болсын кіріктірілген жеке renderInfo iconClass value =
    болсын iconClass = "fa " + iconClass
    div [ ]
        [ span [ Class "icon" ]
            [ i [ Class iconClass ]
                [ ] ]
          str " "
          str value ]

болсын кіріктірілген жеке viewMessage color msg =
    div [ Class ("message " + color) ]
        [ div [ Class "message-body" ]
            [ str msg ] ]

болсын жеке viewLoading =
    viewMessage "is-info" "Waiting the server response..."

болсын жеке viewErrored =
    viewMessage "is-danger" "An error occured, please check the console үшін more information."

болсын жеке viewUser (user : User) =
    болсын birthday =
        user.Birthday.ToShortDateString()

    div [ Class "card is-avatar" ]
        [ div [ Class "card-image" ]
            [ figure [ Class "image is-128x128" ]
                [ img [ Class "is-rounded"
                        Src user.Picture ] ] ]
          div [ Class "card-content" ]
            [ div [ Class "content has-text-centered" ]
                [ div [ Class "has-text-weight-semibold is-size-5" ]
                    [ str user.FullName ]
                  div [ Class "is-italic" ]
                    [ str birthday ] ]
              div [ Class "content" ]
                [ renderInfo "fa-phone" user.CellPhone
                  renderInfo "fa-phone" user.OfficePhone
                  renderInfo "fa-envelope" user.Email ] ] ]

болсын жеке viewGenerateButton isLoading dispatch =
    болсын buttonClass =
        егер isLoading содан
            " is-loading"
        басқа
            ""
        |> (+) "button is-primary "

    div [ Class "has-text-centered" ]
        [ div [ Class buttonClass
                OnClick (функ _ ->
                    dispatch FetchRandomUser
                ) ]
            [ str "Generate a жаңа user" ] ]

болсын жеке center child =
    div [ Class "columns is-mobile" ]
        [ div [ Class "column is-3" ] [ ]
          div [ Class "column" ] [ child ]
          div [ Class "column is-3" ] [ ] ]

болсын view model dispatch =
    болсын (isLoading, content) =
        сәйкестік model с
        | Loading None ->
            true, viewLoading
        | Loading (Some user) ->
            true, viewUser user
        | Loaded user ->
            false, viewUser user
        | Errored ->
            false, viewErrored

    section [ Class "hero is-fullheight" ]
        [ div [ Class "hero-body" ]
            [ div [ Class "container" ]
                [ center (viewGenerateButton isLoading dispatch)
                  center content ] ] ]

// App
Program.mkProgram init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.run
