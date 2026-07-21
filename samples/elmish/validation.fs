модуль Elmish.Validation

 // Form Validation ішінде Elmish, by Zaid Ajaj

ашық System
ашық Fable.Core
ашық Browser.Types
ашық Elmish
ашық Elmish.React
ашық Fable.React
ашық Fable.React.Props

түрі LoginResult =
    | Success бастап token:string
    | UsernameDoesNotExist
    | PasswordIncorrect
    | LoginError бастап errorMsg:string

түрі LoginInfo =
    { Username : string
      Password : string }

түрі Msg =
    | Login
    | ChangeUsername бастап string
    | ChangePassword бастап string
    | LoginSuccess бастап adminSecureToken: string
    | LoginFailed бастап error:string
    | UpdateValidationErrors

түрі State = {
    LoggingIn: bool
    InputUsername: string
    UsernameValidationErrors: string list
    PasswordValidationErrors: string list
    InputPassword: string
    HasTriedToLogin: bool
    LoginError: string option
}


модуль Http =
    болсын жеке loginAsync (info: LoginInfo) =
        async {
            // simulate server word
            do! Async.Sleep 1500
            return LoginResult.Success "my-secure-access-token"
        }

    болсын login (info: LoginInfo) =

        болсын successHandler = функция
            | Success token -> LoginSuccess token
            | UsernameDoesNotExist -> LoginFailed "Username does not exist"
            | PasswordIncorrect -> LoginFailed "The password you entered is incorrect"
            | LoginError error -> LoginFailed error

        Cmd.OfAsync.either loginAsync info
            successHandler
            (функ ex -> LoginFailed "Unknown error occured while logging you ішінде")


болсын init() =
    { InputUsername = ""
      InputPassword = ""
      UsernameValidationErrors =  [ ]
      PasswordValidationErrors =  [ ]
      HasTriedToLogin = false
      LoginError = None
      LoggingIn = false }, Cmd.none


болсын validateInput (state: State) =
  болсын usernameRules =
    [ String.IsNullOrWhiteSpace(state.InputUsername), "Field 'Username' cannot be empty"
      state.InputUsername.Trim().Length < 5, "Field 'Username' must at least have 5 characters" ]
  болсын passwordRules =
    [ String.IsNullOrWhiteSpace(state.InputPassword), "Field 'Password' cannot be empty"
      state.InputPassword.Trim().Length < 5, "Field 'Password' must at least have 5 characters" ]
  болсын usernameValidationErrors =
      usernameRules
      |> List.filter fst
      |> List.map snd
  болсын passwordValidationErrors =
      passwordRules
      |> List.filter fst
      |> List.map snd

  usernameValidationErrors, passwordValidationErrors


болсын update msg (state: State) =
    сәйкестік msg с
    | ChangeUsername name ->
        болсын nextState = { state с InputUsername = name }
        nextState, Cmd.ofMsg UpdateValidationErrors

    | ChangePassword pass ->
        болсын nextState = { state с InputPassword = pass }
        nextState, Cmd.ofMsg UpdateValidationErrors

    | UpdateValidationErrors ->
        болсын usernameErrors, passwordErrors = validateInput state
        болсын nextState =
            { state с UsernameValidationErrors = usernameErrors
                         PasswordValidationErrors = passwordErrors }
        nextState, Cmd.none

    | Login ->
        болсын state = { state с HasTriedToLogin = true }
        болсын usernameErrors, passwordErrors =
           validateInput state
        болсын startLogin =
            List.isEmpty usernameErrors
         && List.isEmpty passwordErrors

        егер not startLogin содан state, Cmd.none
        басқа
          болсын nextState = { state с LoggingIn = true }
          болсын credentials = {
              Username = state.InputUsername
              Password = state.InputPassword
          }

          nextState, Http.login credentials

    | LoginSuccess token ->
        болсын nextState = { state с LoggingIn = false }
        nextState, Cmd.none

    | LoginFailed error ->
        болсын nextState =
            { state с
                LoginError = Some error
                LoggingIn = false }

        nextState, Cmd.none

түрі InputType = Text | Password

болсын textInput inputLabel initial inputType (onChange: string -> unit) =
  болсын inputType = сәйкестік inputType с
                  | Text -> "input"
                  | Password -> "password"
  div
    [ Class "form-group" ]
    [ input [ Class "form-control form-control-lg"
              Type inputType
              DefaultValue initial
              Placeholder inputLabel
              OnChange (функ e ->
                болсын el = e.target :?> HTMLInputElement
                onChange el.value) ] ]

болсын loginFormStyle =
  Style [ Width "400px"
          MarginTop "70px"
          TextAlign TextAlignOptions.Center ]

болсын cardBlockStyle =
  Style [ Padding "30px"
          TextAlign TextAlignOptions.Left
          BorderRadius 10 ]

болсын errorMessagesIfAny triedLogin = функция
  | [ ] -> None
  | _ when triedLogin = false -> None
  | errors ->
    болсын errorStyle = Style [ Color "crimson"; FontSize 12 ]
    ul [ ]
       [ үшін error ішінде errors ->
          li [ errorStyle ] [ str error ] ] |> Some

болсын appIcon =
  img [ Src "https://zaid-ajaj.github.io/elmish-login-flow-validation/img/fable_logo.png"
        Style [ Height 80; Width 100 ] ]

болсын render (state: State) dispatch =

    болсын loginBtnContent =
      егер state.LoggingIn содан i [ Class "fas fa-circle-notch fa-spin" ] []
      басқа str "Login"

    болсын validationRules =
      [ state.InputUsername.Trim().Length >= 5
        state.InputPassword.Trim().Length >= 5 ]

    болсын canLogin = Seq.forall id validationRules

    болсын btnClass =
      егер canLogin
      содан "btn btn-success btn-lg"
      басқа "btn btn-info btn-lg"
    div
      [ Class "container" ; loginFormStyle ]
      [ div
         [ Class "card" ]
         [ div
             [ Class "card-block"; cardBlockStyle ]
             [ div
                [ Style [ TextAlign TextAlignOptions.Center ] ]
                [ appIcon ]
               br []
               textInput "Username" state.InputUsername Text (ChangeUsername >> dispatch)
               ofOption (errorMessagesIfAny state.HasTriedToLogin state.UsernameValidationErrors)
               textInput "Password" state.InputPassword Password (ChangePassword >> dispatch)
               ofOption (errorMessagesIfAny state.HasTriedToLogin state.PasswordValidationErrors)
               div
                [ Style [ TextAlign TextAlignOptions.Center ] ]
                [ button
                    [ Class btnClass
                      OnClick (функ e -> dispatch Login) ]
                    [ loginBtnContent ] ] ] ] ]


Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
|> Program.run
