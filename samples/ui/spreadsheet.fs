модуль SpreadSheet

// Build your own Excel 365 ішінде an hour с F# by Tomas Petricek!
// Watch the video бастап the talk here: https://www.youtube.com/watch?v=Bnm71YEt_lI

модуль Elmish =

    ашық System
    ашық Fable.Core
    ашық Browser
    ашық Browser.Types

    // ------------------------------------------------------------------------------------------------
    // Virtual Dom bindings
    // ------------------------------------------------------------------------------------------------

    түрі IVirtualdom =
        abstract h: arg1: string * arg2: obj * arg3: obj[] -> obj
        abstract diff: tree1:obj * tree2:obj -> obj
        abstract patch: node:obj * patches:obj -> Node
        abstract create: e:obj -> Node

    [<Global("virtualDom")>]
    болсын Virtualdom: IVirtualdom = jsNative

    // ------------------------------------------------------------------------------------------------
    // F# representation бастап DOM and rendering using VirtualDom
    // ------------------------------------------------------------------------------------------------

    түрі DomAttribute =
        | EventHandler бастап (Event -> unit)
        | Attribute бастап string
        | Property бастап string

    түрі DomNode =
        | Text бастап string
        | Element бастап tag:string * attributes:(string * DomAttribute)[] * children : DomNode[]

    болсын createTree tag args children =
        болсын attrs = ResizeArray<_>()
        болсын props = ResizeArray<_>()
        үшін k, v ішінде args жасау
            сәйкестік k, v с
            | "style", Attribute v
            | "style", Property v ->
                    болсын args = v.Split(';') |> Array.map (функ a ->
                        болсын sep = a.IndexOf(':')
                        егер sep > 0 содан a.Substring(0, sep), box (a.Substring(sep+1))
                        басқа a, box "" )
                    props.Add ("style", JsInterop.createObj args)
            | "class", Attribute v
            | "class", Property v ->
                    attrs.Add (k, box v)
            | k, Attribute v ->
                    attrs.Add (k, box v)
            | k, Property v ->
                    props.Add (k, box v)
            | k, EventHandler f ->
                    props.Add (k, box f)
        болсын attrs = JsInterop.createObj attrs
        болсын props = JsInterop.createObj (Seq.append ["attributes", attrs] props)
        болсын elem = Virtualdom.h(tag, props, children)
        elem

    болсын rec render node =
        сәйкестік node с
        | Text(s) ->
                box s
        | Element(tag, attrs, children) ->
                createTree tag attrs (Array.map render children)

    // ------------------------------------------------------------------------------------------------
    // Helpers үшін dynamic property access & үшін creating HTML elements
    // ------------------------------------------------------------------------------------------------

    түрі Dynamic() =
        [<Emit("$0[$1]")>]
        статикалық мүшесі (?) (d:Dynamic, s:string) : Dynamic = jsNative

    болсын text s = Text(s)
    болсын (=>) k v = k, Property(v)
    болсын (=!>) k f = k, EventHandler(функ e -> f e)

    түрі El() =
        статикалық мүшесі (?) (_:El, n:string) = функ a b ->
            Element(n, Array.ofList a, Array.ofList b)

    болсын h = El()

    // ------------------------------------------------------------------------------------------------
    // Entry point - create event and update on trigger
    // ------------------------------------------------------------------------------------------------

    түрі Cmd<'Msg> = (('Msg -> unit) -> unit) list

    түрі SingleObservable<'T>() =
        болсын mutable listener: IObserver<'T> option = None
        мүшесі _.Trigger v =
            сәйкестік listener с
            | Some lis -> lis.OnNext v
            | None -> ()
        interface IObservable<'T> с
            мүшесі _.Subscribe w =
                listener <- Some w
                { жаңа IDisposable с
                    мүшесі _.Dispose() = () }

    болсын app id (init: unit -> 'Model * Cmd<'Msg>) update view =
        болсын event = жаңа Event<'Msg>()
        болсын trigger e = event.Trigger(e)
        болсын model, cmds = init()
        болсын mutable state = model
        болсын mutable tree = view state trigger |> render
        болсын mutable container = Virtualdom.create(tree)
        document.getElementById(id).appendChild(container) |> ignore

        болсын handleEvent evt =
            болсын model, cmds = update evt state
            болсын newTree = view model trigger |> render
            болсын patches = Virtualdom.diff(tree, newTree)
            container <- Virtualdom.patch(container, patches)
            tree <- newTree
            state <- model
            үшін cmd ішінде cmds жасау
                cmd trigger

        event.Publish.Add(handleEvent)
        үшін cmd ішінде cmds жасау
            cmd trigger

модуль Parsec =
    түрі ParseStream<'T> = int * list<'T>
    түрі Parser<'T, 'R> = Parser бастап (ParseStream<'T> -> option<ParseStream<'T> * 'R>)

    /// Returned by the `slot` функция to create a parser slot that is filled later
    түрі ParserSetter<'T, 'R> =
      { Set : Parser<'T, 'R> -> unit }

    /// Ignore the result бастап the parser
    болсын ignore (Parser p) = Parser(функ input ->
      p input |> Option.map (функ (i, r) -> i, ()))

    /// Creates a delayed parser whose actual parser is set later
    болсын slot () =
      болсын mutable slot = None
      { Set = функ (Parser p) -> slot <- Some p },
      Parser(функ input ->
        сәйкестік slot с
        | Some slot -> slot input
        | None -> failwith "Slot not initialized")

    /// If the input matches the specified prefix, produce the specified result
    болсын prefix (prefix:list<'C>) result = Parser(функ (offset, input) ->
      болсын rec loop (word:list<'C>) input =
        сәйкестік word, input с
        | c::word, i::input when c = i -> loop word input
        | [], input -> Some(input)
        | _ -> None

      сәйкестік loop prefix input с
      | Some(input) -> Some((offset+List.length prefix, input), result)
      | _ -> None)

    /// Parser that succeeds when either бастап the two arguments succeed
    болсын (<|>) (Parser p1) (Parser p2) = Parser(функ input ->
      сәйкестік p1 input с
      | Some(input, res) -> Some(input, res)
      | _ -> p2 input)

    /// Run two parsers ішінде sequence and return the result as a tuple
    болсын (<*>) (Parser p1) (Parser p2) = Parser(функ input ->
      сәйкестік p1 input с
      | Some(input, res1) ->
          сәйкестік p2 input с
          | Some(input, res2) -> Some(input, (res1, res2))
          | _ -> None
      | _ -> None)

    /// Transforms the result бастап the parser using the specified функция
    болсын map f (Parser p) = Parser(функ input ->
      p input |> Option.map (функ (input, res) -> input, f res))

    /// Run two parsers ішінде sequence and return the result бастап the second one
    болсын (<*>>) p1 p2 = p1 <*> p2 |> map snd

    /// Run two parsers ішінде sequence and return the result бастап the first one
    болсын (<<*>) p1 p2 = p1 <*> p2 |> map fst

    /// Succeed without consuming input
    болсын unit res = Parser(функ input -> Some(input, res))

    /// Parse using the first parser and содан call a функция to produce
    /// next parser and parse the rest бастап the input с the next parser
    болсын bind f (Parser p) = Parser(функ input ->
      сәйкестік p input с
      | Some(input, res) ->
          болсын (Parser g) = f res
          сәйкестік g input с
          | Some(input, res) -> Some(input, res)
          | _ -> None
      | _ -> None)

    /// Parser that tries to use a specified parser, but returns None егер it fails
    болсын optional (Parser p) = Parser(функ input ->
      сәйкестік p input с
      | None -> Some(input, None)
      | Some(input, res) -> Some(input, Some res) )

    /// Parser that succeeds егер the input matches a predicate
    болсын pred p = Parser(функция
      | offs, c::input when p c -> Some((offs+1, input), c)
      | _ -> None)

    /// Parser that succeeds егер the predicate returns Some value
    болсын choose p = Parser(функция
      | offs, c::input -> p c |> Option.map (функ c -> (offs + 1, input), c)
      | _ -> None)

    /// Parse zero or more repetitions using the specified parser
    болсын zeroOrMore (Parser p) =
      болсын rec loop acc input =
        сәйкестік p input с
        | Some(input, res) -> loop (res::acc) input
        | _ -> Some(input, List.rev acc)
      Parser(loop [])

    /// Parse one or more repetitions using the specified parser
    болсын oneOrMore p =
      (p <*> (zeroOrMore p))
      |> map (функ (c, cs) -> c::cs)


    болсын anySpace = zeroOrMore (pred (функ t -> t = ' '))

    болсын char tok = pred (функ t -> t = tok)

    болсын separated sep p =
      p <*> zeroOrMore (sep <*> p)
      |> map (функ (a1, args) -> a1::(List.map snd args))

    болсын separatedThen sep p1 p2 =
      p1 <*> zeroOrMore (sep <*> p2)
      |> map (функ (a1, args) -> a1::(List.map snd args))

    болсын separatedOrEmpty sep p =
      optional (separated sep p)
      |> map (функ l -> defaultArg l [])

    болсын number = pred (функ t -> t <= '9' && t >= '0')

    болсын integer = oneOrMore number |> map (функ nums ->
      nums |> List.fold (функ res n -> res * 10 + (int n - int '0')) 0)

    болсын letter = pred (функ t ->
      (t <= 'Z' && t >= 'A') || (t <= 'z' && t >= 'a'))

    болсын run (Parser(f)) input =
      сәйкестік f (0, List.ofSeq input) с
      | Some((i, _), res) when i = Seq.length input -> Some res
      | _ -> None

модуль Evaluator =
    ашық Parsec

    // ----------------------------------------------------------------------------
    // DOMAIN MODEL
    // ----------------------------------------------------------------------------

    түрі Position = char * int

    түрі Expr =
      | Reference бастап Position
      | Number бастап int
      | Binary бастап Expr * char * Expr

    // ----------------------------------------------------------------------------
    // PARSER
    // ----------------------------------------------------------------------------

    // Basics: operators (+, -, *, /), cell reference (e.g. A10), number (e.g. 123)
    болсын operator = char '+' <|> char '-' <|> char '*' <|> char '/'
    болсын reference = letter <*> integer |> map Reference
    болсын number = integer |> map Number

    // Nested operator uses need to be parethesized, үшін example (1 + (3 * 4)).
    // <expr> is a binary operator without parentheses, number, reference or
    // nested brackets, while <term> is always bracketed or primitive. We need
    // to use `expr` recursively, which is handled via mutable slots.
    болсын exprSetter, expr = slot ()
    болсын brack = char '(' <*>> anySpace <*>> expr <<*> anySpace <<*> char ')'
    болсын term = number <|> reference <|> brack
    болсын binary = term <<*> anySpace <*> operator <<*> anySpace <*> term |> map (функ ((l,op), r) -> Binary(l, op, r))
    болсын exprAux = binary <|> term
    exprSetter.Set exprAux

    // Formula starts с `=` followed by expression
    // Equation you can write ішінде a cell is either number or a formula
    болсын formula = char '=' <*>> anySpace <*>> expr
    болсын equation = anySpace <*>> (formula <|> number) <<*> anySpace

    // Run the parser on a given input
    болсын parse input = run equation input

    // ----------------------------------------------------------------------------
    // EVALUATOR
    // ----------------------------------------------------------------------------

    болсын rec evaluate visited (cells:Map<Position, string>) expr =
      сәйкестік expr с
      | Number num ->
          Some num

      | Binary(l, op, r) ->
          болсын ops = dict [ '+', (+); '-', (-); '*', (*); '/', (/) ]
          evaluate visited cells l |> Option.bind (функ l ->
            evaluate visited cells r |> Option.map (функ r ->
              ops.[op] l r ))

      | Reference pos when Set.contains pos visited ->
          None

      | Reference pos ->
          cells.TryFind pos |> Option.bind (функ value ->
            parse value |> Option.bind (функ parsed ->
              evaluate (Set.add pos visited) cells parsed))

ашық Elmish
ашық Evaluator

// ----------------------------------------------------------------------------
// DOMAIN MODEL
// ----------------------------------------------------------------------------

түрі Event =
  | UpdateValue бастап Position * string
  | StartEdit бастап Position

түрі State =
  { Rows : int list
    Active : Position option
    Cols : char list
    Cells : Map<Position, string> }

түрі Movement =
    | MoveTo бастап Position
    | Invalid

түрі Direction = Up | Down | Left | Right

болсын KeyDirection : Map<string, Direction> = Map.ofList [
  ("ArrowLeft", Left)
  ("ArrowUp", Up)
  ("ArrowRight", Right)
  ("ArrowDown", Down)
]

// ----------------------------------------------------------------------------
// EVENT HANDLING
// ----------------------------------------------------------------------------

болсын update msg state =
  сәйкестік msg с
  | StartEdit(pos) ->
      { state с Active = Some pos }, []

  | UpdateValue(pos, value) ->
      болсын newCells =
          егер value = ""
              содан Map.remove pos state.Cells
              басқа Map.add pos value state.Cells
      { state с Cells = newCells }, []

// ----------------------------------------------------------------------------
// RENDERING
// ----------------------------------------------------------------------------

болсын getDirection (ke: Browser.Types.KeyboardEvent) : Option<Direction> =
    Map.tryFind ke.key KeyDirection

болсын getPosition ((col, row): Position) (direction: Direction) : Position =
    сәйкестік direction с
    | Up -> (col, row - 1)
    | Down -> (col, row + 1)
    | Left -> (char((int col) - 1), row)
    | Right -> (char((int col) + 1), row)

болсын getMovement (state: State) (direction: Direction) : Movement =
    сәйкестік state.Active с
    | None -> Invalid
    | (Some position) ->
        болсын (col, row) = getPosition position direction
        егер List.contains col state.Cols && List.contains row state.Rows
            содан MoveTo (col, row)
            басқа Invalid

болсын getKeyPressEvent state trigger = функ (ke: Browser.Types.Event) ->
    сәйкестік getDirection (ke :?> _) с
    | None -> ()
    | Some direction ->
        сәйкестік getMovement state direction с
        | Invalid -> ()
        | MoveTo position -> trigger(StartEdit(position))

болсын renderEditor (trigger:Event -> unit) pos state value =
  h?td [ "class" => "selected" ] [
    h?input [
      "autofocus" => "true"
      "onkeydown" =!> (getKeyPressEvent state trigger)
      "oninput" =!> (функ e -> trigger (UpdateValue (pos, (e.target :?> Browser.Types.HTMLInputElement).value)))
      "value" => value ] []
  ]

болсын renderView trigger pos (value:option<_>) =
  h?td
    [ "style" => (егер value.IsNone содан "background:#ffb0b0" басқа "background:white")
      "onclick" =!> (функ _ -> trigger(StartEdit(pos)) ) ]
    [ Text (Option.defaultValue "#ERR" value) ]

болсын renderCell trigger pos state =
  болсын value = Map.tryFind pos state.Cells
  егер state.Active = Some pos содан
    renderEditor trigger pos state (Option.defaultValue "" value)
  басқа
    болсын value =
      сәйкестік value с
      | Some value ->
          parse value |> Option.bind (evaluate Set.empty state.Cells) |> Option.map string
      | _ -> Some ""
    renderView trigger pos value

болсын view state trigger =
  болсын empty = h?td [] []
  болсын header htext = h?th [] [Text htext]
  болсын headers = state.Cols |> List.map (функ h -> header (string h))
  болсын headers = empty::headers

  болсын row cells = h?tr [] cells
  болсын cells n =
    болсын cells = state.Cols |> List.map (функ h -> renderCell trigger (h, n) state)
    header (string n) :: cells
  болсын rows = state.Rows |> List.map (функ r -> h?tr [] (cells r))

  h?table [] [
    h?tr [] headers
    h?tbody [] rows
  ]

// ----------------------------------------------------------------------------
// ENTRY POINT
// ----------------------------------------------------------------------------

болсын initial () =
  { Cols = ['A' .. 'K']
    Rows = [1 .. 15]
    Active = None
    Cells = Map.empty },
  []

app "main" initial update view
