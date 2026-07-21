// Fractal playground by Mark Pattison (Twitter @mark_pattison)
// Source code available ішінде Github: https://github.com/markpattison/FableFractal

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

модуль WebGLHelper =

  ашық Browser.Types
  ашық Fable.Core.JsInterop

  // Shorthand
  түрі GL = WebGLRenderingContext

  болсын getWebGLContext (canvas: HTMLCanvasElement) =
      болсын getContext ctxString =
          canvas.getContext(ctxString, createObj [ "premultipliedAlpha" ==> false ]) |> unbox<WebGLRenderingContext>

      болсын webgl = getContext "webgl"

      // If we have webgl = null ішінде JS содан try to get experimental-webgl
      // Edge and webkit use experimental-webgl
      егер not (unbox webgl) содан
          getContext "experimental-webgl"
      басқа
          webgl

  болсын createShaderProgram (gl:GL) vertex fragment =
      болсын vertexShader = gl.createShader(gl.VERTEX_SHADER)
      gl.shaderSource(vertexShader, vertex)
      gl.compileShader(vertexShader)

      болсын fragShader = gl.createShader(gl.FRAGMENT_SHADER)
      gl.shaderSource(fragShader, fragment)
      gl.compileShader(fragShader)

      болсын program = gl.createProgram()
      gl.attachShader(program, vertexShader)
      gl.attachShader(program, fragShader)
      gl.linkProgram(program)

      program

  болсын createUniformLocation (gl:GL) program name =
      болсын uniformLocation = gl.getUniformLocation(program, name)
      uniformLocation

  болсын createAttributeLocation (gl : GL) program name =
      болсын attributeLocation = gl.getAttribLocation(program, name)
      gl.enableVertexAttribArray(attributeLocation)

      attributeLocation

  болсын createBuffer (items : float[]) (gl:GL) =
      болсын buffer = gl.createBuffer()

      gl.bindBuffer(gl.ARRAY_BUFFER, buffer)
      gl.bufferData(gl.ARRAY_BUFFER, (createNew Fable.Core.JS.Constructors.Float32Array items) |> unbox, gl.STATIC_DRAW)

      buffer

  болсын clear (gl:GL) (width, height) =
      gl.clearColor(1.0, 1.0, 1.0, 1.0)

      gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA)
      //gl.enable(gl.DEPTH_TEST)
      gl.enable(gl.BLEND)

      gl.viewport(0., 0., width, height)
      gl.clear(float (int gl.COLOR_BUFFER_BIT ||| int gl.DEPTH_BUFFER_BIT))

модуль Types =

  ашық Browser.Types

  түрі Msg =
      | MandelbrotClick
      | JuliaClick
      | JuliaMoveClick
      | JuliaChangeSeedClick
      | MouseDownMsg бастап MouseEvent
      | MouseUpMsg бастап MouseEvent
      | MouseMoveMsg бастап MouseEvent
      | MouseLeaveMsg бастап MouseEvent
    //   | WheelMsg бастап WheelEvent
    //   | TouchStartMsg бастап TouchEvent
    //   | TouchEndMsg бастап TouchEvent
    //   | TouchMoveMsg бастап TouchEvent
      | RenderMsg

  түрі JuliaSeed = { SeedX: float; SeedY: float }
  түрі JuliaScrolling = Move | ChangeSeed

  түрі FractalType =
      | Mandelbrot
      | Julia бастап JuliaSeed * JuliaScrolling

  түрі Transform =
      | Scrolling бастап float * float
      | Pinching бастап float
      | NoTransform

  түрі Model =
      {
          CanvasHeight: float
          Zoom: float
          FractalType: FractalType
          X: float
          Y: float
          Now: System.DateTime
          Render: (Model -> unit) option
          Transform: Transform
      }

модуль FractalRenderer =

  ашық System
  ашық Browser
  ашық Browser.Types
  ашық WebGLHelper
  ашық Types

  болсын myVertex = """
      precision highp float;
      precision highp int;

      attribute vec4 aVertexPosition;
      attribute vec2 aTextureCoord;
      varying vec2 vTextureCoord;
      void main() {
        gl_Position = aVertexPosition;
        vTextureCoord = aTextureCoord;
      }
  """

  болсын myFragment = """
      precision highp float;
      precision highp int;
      uniform float uWidthOverHeight;
      uniform float uZoom;
      uniform vec2 uOffset, uJuliaSeed;
      uniform bool uIsJulia;
      varying vec2 vTextureCoord;
      vec2 calculatePosition(vec2 inputCoords, float zoom, float widthOverHeight, vec2 offset)
      {
          return (inputCoords - 0.5) * vec2(widthOverHeight, 1.0) / zoom + offset;
      }
      vec4 applyColourMap(float x)
      {
          return vec4(sin(x * 4.0), sin (x * 5.0), sin (x * 6.0), 1.0);
      }
      vec2 cConj(vec2 z)
      {
          return vec2(z.x, -z.y);
      }
      vec2 cMul(vec2 a, vec2 b)
      {
          return vec2(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);
      }
      vec2 cSq(vec2 z)
      {
          return cMul(z, z);
      }
      vec2 cCube(vec2 z)
      {
          return cMul(z, cMul(z, z));
      }
      vec2 cPow4(vec2 z)
      {
          return cSq(cSq(z));
      }
      vec2 cDiv(vec2 a, vec2 b)
      {
          return cMul(a, cConj(b));
      }
      vec2 cRecip(vec2 z)
      {
          return cDiv(vec2(1.0, 0.0), z);
      }
      vec2 f(vec2 z, vec2 offset)
      {
          return cSq(z) + offset;
      }
      float pixelResult(vec2 z, vec2 offset)
      {
          float result = 0.0;
          vec2 zsq = z * z;
          int iterations = 0;
          үшін (int i = 0; i < 128; i++)
          {
              iterations = i;
              егер (zsq.x + zsq.y > 49.0)
              {
                  break;
              }
              z = f(z, offset);
              zsq = z * z;
          }
          егер (iterations == 127)
          {
              result = 0.0;
          }
          басқа
          {
              result = float(iterations) + (log(2.0 * log(7.0)) - log(log(zsq.x + zsq.y))) / log(2.0);
              result = log(result * 0.4) / log(128.0);
          }
          return result;
      }
      void main(void)
      {
          vec2 z = calculatePosition(vTextureCoord, uZoom, uWidthOverHeight, uOffset);
          float result = pixelResult(z, uIsJulia ? uJuliaSeed : z);
          gl_FragColor = applyColourMap(result);
      }
  """

  болсын initBuffers gl =
      болсын positions =
          createBuffer
              [|
                  -1.0; -1.0;
                    1.0; -1.0;
                  -1.0;  1.0;
                    1.0;  1.0
              |] gl
      болсын textureCoords =
          createBuffer
              [|
                  0.0; 0.0;
                  1.0; 0.0;
                  0.0; 1.0;
                  1.0; 1.0
              |] gl
      positions, textureCoords

  болсын create (holder : Element) =

      болсын canvas = document.createElement "canvas" :?> HTMLCanvasElement
      болсын width = 640
      болсын height = 480

      canvas.width <- float width
      canvas.height <- float height

      holder.appendChild(canvas) |> ignore

      болсын context = getWebGLContext canvas

      болсын program = createShaderProgram context myVertex myFragment

      болсын positionBuffer, colourBuffer = initBuffers context
      болсын vertexPositionAttribute = createAttributeLocation context program "aVertexPosition"
      болсын textureCoordAttribute = createAttributeLocation context program "aTextureCoord"
      болсын widthOverHeightUniform = createUniformLocation context program "uWidthOverHeight"
      болсын zoomUniform = createUniformLocation context program "uZoom"
      болсын offsetUniform = createUniformLocation context program "uOffset"
      болсын juliaSeedUniform = createUniformLocation context program "uJuliaSeed"
      болсын isJuliaUniform = createUniformLocation context program "uIsJulia"

      болсын draw widthOverHeight zoom x y jx jy isJulia =
          context.useProgram(program)

          context.bindBuffer(context.ARRAY_BUFFER, positionBuffer)
          context.vertexAttribPointer(vertexPositionAttribute, 2.0, context.FLOAT, false, 0.0, 0.0)
          context.bindBuffer(context.ARRAY_BUFFER, colourBuffer)
          context.vertexAttribPointer(textureCoordAttribute, 2.0, context.FLOAT, false, 0.0, 0.0)

          context.uniform1f(widthOverHeightUniform, widthOverHeight)
          context.uniform1f(zoomUniform, zoom)
          context.uniform2f(offsetUniform, x, y)
          context.uniform2f(juliaSeedUniform, jx, jy)
          context.uniform1i(isJuliaUniform, егер isJulia содан 1.0 басқа 0.0)

          context.drawArrays (context.TRIANGLE_STRIP, 0., 4.0)

      болсын clear = clear context

      // Try not to use "context" after this point, bind a функция above.

      болсын imageLoadCanvas = document.createElement "canvas" :?> HTMLCanvasElement
      болсын imageLoadCanvasContext = imageLoadCanvas.getContext_2d()

      болсын mutable last = DateTime.Now

      болсын render model =
          сәйкестік model с
          | model when model.Now <> last ->
              last <- model.Now

              болсын resolution = canvas.width, canvas.height
              болсын widthOverHeight = егер canvas.height = 0.0 содан 1.0 басқа canvas.width / canvas.height
              clear resolution

              сәйкестік model.FractalType с
              | Mandelbrot ->
                  draw widthOverHeight model.Zoom model.X model.Y 0.0 0.0 false
              | Julia ({ SeedX = seedX; SeedY = seedY }, _) ->
                  draw widthOverHeight model.Zoom model.X model.Y seedX seedY true

          | _ -> ignore()

      render, height

модуль State =

    ашық Browser
    ашық Browser.Types
    ашық Fable.Core.JsInterop
    ашық Types

    // түрі INormalizedWheel =
    //     abstract мүшесі pixelX: float
    //     abstract мүшесі pixelY: float
    //     abstract мүшесі spinX: float
    //     abstract мүшесі spinY: float

    // болсын normalizeWheel : WheelEvent -> INormalizedWheel = importDefault "normalize-wheel"

    болсын renderCommand =
        болсын sub dispatch =
            window.requestAnimationFrame(функ _ -> dispatch RenderMsg) |> ignore
        [sub]

    болсын initMandelbrot =
        {
            CanvasHeight = 1.0
            Zoom = 0.314
            FractalType = Mandelbrot
            X = -0.5
            Y = 0.0
            Now = System.DateTime.Now
            Render = None
            Transform = NoTransform
        }

    болсын initJulia =
        {
            CanvasHeight = 1.0
            Zoom = 0.314
            FractalType = Julia ({ SeedX = 0.0; SeedY = 0.0 }, ChangeSeed)
            X = 0.0
            Y = 0.0
            Now = System.DateTime.Now
            Render = None
            Transform = NoTransform
        }

    болсын init() =
        document.addEventListener("gesturestart", (функ e -> e.preventDefault()), true)
        document.addEventListener("gesturechange", (функ e -> e.preventDefault()), true)
        document.addEventListener("gestureend", (функ e -> e.preventDefault()), true)
        document.addEventListener("scroll", (функ e -> e.preventDefault()), true)
        initMandelbrot, renderCommand

    болсын updateForMove x y model =
        сәйкестік model.Transform с
        | Scrolling (lastScreenX, lastScreenY) ->
            { model с
                X = model.X - (x - lastScreenX) / (model.Zoom * model.CanvasHeight)
                Y = model.Y + (y - lastScreenY) / (model.Zoom * model.CanvasHeight)
                Transform = Scrolling (x, y)
            }, []
        | _ -> model, []

    болсын updateForSeedChange seed x y model =
        сәйкестік model.Transform с
        | Scrolling (lastScreenX, lastScreenY) ->
            { model с
                FractalType = Julia ( {
                                        SeedX = seed.SeedX - (x - lastScreenX) / (model.Zoom * model.CanvasHeight)
                                        SeedY = seed.SeedY - (y - lastScreenY) / (model.Zoom * model.CanvasHeight)}, ChangeSeed)
                Transform = Scrolling (x, y)
            }, []
        | _ -> model, []

    болсын update msg model =
        сәйкестік model.FractalType, msg с
        | Julia _, MandelbrotClick _ ->
            { model с
                Zoom = 0.314; FractalType = Mandelbrot; X = -0.5; Y = 0.0
            }, []

        | Mandelbrot, JuliaClick ->
            { model с
                Zoom = 0.314; FractalType = Julia ({ SeedX = 0.0; SeedY = 0.0 }, ChangeSeed); X = 0.0; Y = 0.0
            }, []

        | Julia (seed, _), JuliaMoveClick ->
            { model с FractalType = Julia (seed, Move) }, []

        | Julia (seed, _), JuliaChangeSeedClick ->
            { model с FractalType = Julia (seed, ChangeSeed) }, []

        | _, MouseDownMsg me when me.button = 0.0 ->
            { model с
                Transform = Scrolling (me.screenX, me.screenY)
            }, []

        | _, MouseUpMsg me when me.button = 0.0 -> { model с Transform = NoTransform }, []

        | _, MouseLeaveMsg _ -> { model с Transform = NoTransform }, []

        | Mandelbrot, MouseMoveMsg me
        | Julia (_, Move), MouseMoveMsg me ->
            updateForMove me.screenX me.screenY model

        | Julia (seed, ChangeSeed), MouseMoveMsg me ->
            updateForSeedChange seed me.screenX me.screenY model

        // | _, WheelMsg we ->
        //     болсын zoom = (normalizeWheel we).pixelY / 100.0
        //     { model с Zoom = model.Zoom * 0.99 ** zoom }, []

        // | _, TouchEndMsg _ -> { model с Transform = NoTransform }, []

        // | _, TouchStartMsg te when te.touches.Length = 1 ->
        //     { model с
        //         Transform = Scrolling (te.touches.[0].clientX, te.touches.[0].clientY)
        //     }, []

        // | _, TouchStartMsg te when te.touches.Length = 2 ->
        //     болсын dx = te.touches.[1].clientX - te.touches.[0].clientX
        //     болсын dy = te.touches.[1].clientY - te.touches.[0].clientY
        //     болсын distance = sqrt (dx * dx + dy * dy)
        //     { model с
        //         Transform = Pinching distance
        //     }, []

        // | Mandelbrot, TouchMoveMsg te
        // | Julia (_, Move), TouchMoveMsg te when te.touches.Length = 1 ->
        //     updateForMove te.touches.[0].screenX te.touches.[0].screenY model

        // | Julia (seed, ChangeSeed), TouchMoveMsg te when te.touches.Length = 1 ->
        //     updateForSeedChange seed te.touches.[0].screenX te.touches.[0].screenY model

        // | Mandelbrot, TouchMoveMsg te
        // | Julia _, TouchMoveMsg te when te.touches.Length = 2 ->
        //     сәйкестік model.Transform с
        //     | Pinching lastDistance ->
        //         болсын dx = te.touches.[1].clientX - te.touches.[0].clientX
        //         болсын dy = te.touches.[1].clientY - te.touches.[0].clientY
        //         болсын distance = sqrt (dx * dx + dy * dy)
        //         { model с
        //             Zoom = model.Zoom * 0.99 ** (lastDistance - distance)
        //             Transform = Pinching distance
        //         }, []
        //     | _ -> model, []

        | _, RenderMsg ->
            сәйкестік model.Render с
            | None ->
                болсын holder = document.getElementById("Fractal")
                сәйкестік holder с
                | null -> model, renderCommand
                | h ->
                    болсын renderer, height = FractalRenderer.create h
                    { model с Render = Some renderer; CanvasHeight = float height }, renderCommand
            | Some render ->
                render model
                { model с Now = System.DateTime.Now }, renderCommand

        | _ -> model, []

модуль View =

    ашық Elmish
    ашық Types
    ашық State

    болсын showParams model =
        сәйкестік model.FractalType с
        | Julia (seed, _) ->
            [
                h?p [] [ Text $"X = %.6f{model.X}" ]
                h?p [] [ Text $"Y = %.6f{model.Y}" ]
                h?p [] [ Text $"Zoom = %.6f{model.Zoom}" ]
                h?p [] [ Text $"Seed X = %.6f{seed.SeedX}" ]
                h?p [] [ Text $"Seed Y = %.6f{seed.SeedY}" ]
            ]
        | Mandelbrot ->
            [
                h?p [] [ Text $"X = %.6f{model.X}" ]
                h?p [] [ Text $"Y = %.6f{model.Y}" ]
                h?p [] [ Text $"Zoom = %.6f{model.Zoom}" ]
            ]

    болсын showButtons model dispatch =
        h?div [] [
            h?div [ "class" => "field has-addons" ] [
                h?button [
                    (сәйкестік model.FractalType с
                        | Mandelbrot -> "class" => "button is-primary is-selected"
                        | Julia _ -> "class" => "button")
                    "onclick" =!> (функ _ -> MandelbrotClick |> dispatch)
                ] [ Text "Mandelbrot" ]
                h?button [
                    (сәйкестік model.FractalType с
                        | Mandelbrot -> "class" => "button"
                        | Julia _ -> "class" => "button is-primary is-selected")
                    "onclick" =!> (функ _ -> JuliaClick |> dispatch)
                ] [ Text "Julia" ]
            ]
            h?div [] [
                сәйкестік model.FractalType с
                | Julia (_, scrollType) ->
                    yield h?button [
                        (сәйкестік scrollType с
                            | Move -> "class" => "button is-primary is-selected"
                            | ChangeSeed -> "class" => "button")
                        "onclick" =!> (функ _ -> JuliaMoveClick |> dispatch)
                    ] [ Text "Move" ]
                    yield h?button [
                        (сәйкестік scrollType с
                            | Move -> "class" => "button"
                            | ChangeSeed -> "class" => "button is-primary is-selected")
                        "onclick" =!> (функ _ -> JuliaChangeSeedClick |> dispatch)
                    ] [ Text "ChangeSeed" ]
                | _ -> ()
            ]
        ]

    болсын hud model dispatch =
        h?div [ "class" => "columns" ] [
            h?div [ "class" => "column" ] (showParams model)
            h?div [ "class" => "column" ] [ showButtons model dispatch ]
        ]

    болсын fractalCanvas dispatch =
        болсын dispatch (msg: 'Event -> Msg) (e: Browser.Types.Event) =
            e.preventDefault()
            msg (e :?> 'Event) |> dispatch

        h?div [
            "id" => "Fractal"
            "onmousedown" =!> dispatch MouseDownMsg
            "onmouseup" =!> dispatch MouseUpMsg
            "onmousemove" =!> dispatch MouseMoveMsg
            "onmouseleave" =!> dispatch MouseLeaveMsg
            // "onwheel" =!> dispatch WheelMsg
            // "ontouchstart" =!> dispatch TouchStartMsg
            // "ontouchmove" =!> dispatch TouchMoveMsg
            // "ontouchend" =!> dispatch TouchEndMsg
            // "ontouchcancel" =!> dispatch TouchEndMsg
        ] []

    болсын root model dispatch =
        h?div [] [
            hud model dispatch
            fractalCanvas dispatch
        ]

    app "FableFractal" init update root
