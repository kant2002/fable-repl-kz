модуль Mario

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Browser.Types
ашық Browser

модуль Keyboard =

    болсын mutable keysPressed = Set.empty

    /// Returns 1 егер key с given code is pressed
    болсын code x =
        егер keysPressed.Contains(x) содан 1 басқа 0

    /// Update the state бастап the set үшін given key event
    болсын update (e : KeyboardEvent, pressed) =
        болсын key = e.key
        болсын op =  егер pressed содан Set.add басқа Set.remove
        keysPressed <- op key keysPressed

    /// Returns pair с -1 үшін left or down and +1
    /// үшін right or up (0 егер no or both keys are pressed)
    болсын arrows () =
        (code "ArrowRight" - code "ArrowLeft", code "ArrowUp" - code "ArrowDown")

    болсын initKeyboard () =
        document.addEventListener("keydown", функ e -> update(e :?> _, true))
        document.addEventListener("keyup", функ e -> update(e :?> _, false))

модуль Physics =

    түрі MarioModel =
        { x:float; y:float;
          vx:float; vy:float;
          dir:string }


    // If the Up key is pressed (y > 0) and Mario is on the ground,
    // содан create Mario с the y velocity 'vy' set to 5.0
    болсын jump (_,y) m =
        егер y > 0 && m.y = 0. содан { m с vy = 5. } басқа m

    // If Mario is ішінде the air, содан his "up" velocity is decreasing
    болсын gravity m =
        егер m.y > 0. содан { m с vy = m.vy - 0.1 } басқа m

    // Apply physics - move Mario according to the current velocities
    болсын physics m =
        { m с x = m.x + m.vx; y = max 0. (m.y + m.vy) }

    // When Left/Right keys are pressed, change 'vx' and direction
    болсын walk (x,_) m =
        болсын dir = егер x < 0 содан "left" басегер x > 0 содан "right" басқа m.dir
        { m с vx = float x; dir = dir }


    болсын marioStep dir mario =
        mario
        |> physics
        |> walk dir
        |> gravity
        |> jump dir

модуль Canvas =

    // Get the canvas context үшін drawing
    болсын canvas = document.getElementsByTagName("canvas").[0] :?> HTMLCanvasElement
    болсын context = canvas.getContext_2d()

    // Format RGB color as "rgb(r,g,b)"
    болсын ($) s n = s + n.ToString()
    болсын rgb r g b = "rgb(" $ r $ "," $ g $ "," $ b $ ")"

    /// Fill rectangle с given color
    болсын filled (color: string) rect =
        болсын ctx = context
        ctx.fillStyle <- !^ color
        ctx.fillRect rect

    /// Move element to a specified X Y position
    болсын position (x,y) (img : HTMLImageElement) =
        img?style?left <- x.ToString() + "px"
        img?style?top <- (canvas.offsetTop + y).ToString() + "px"

    болсын getWindowDimensions () =
        canvas.width, canvas.height

    /// Get the first <img /> element and set `src` (жасау
    /// nothing егер it is the right one to keep animation)
    болсын image (src:string) =
        болсын image = document.getElementsByTagName("img").[0] :?> HTMLImageElement
        егер image.src.IndexOf(src) = -1 содан image.src <- src
        image

ашық Canvas
ашық Physics

болсын origin =
    // Sample is running ішінде an iframe, so get the location бастап parent
    болсын topLocation = window.top.location
    topLocation.origin + topLocation.pathname

болсын render (w,h) (mario: MarioModel) =
    (0., 0., w, h) |> filled (rgb 174 238 238)
    (0., h-50., w, 50.) |> filled (rgb 74 163 41)
    // Select and position Mario
    // (walking is represented as an animated gif)
    болсын verb =
        егер mario.y > 0. содан "jump"
        басегер mario.vx <> 0. содан "walk"
        басқа "stand"
    origin + "img/mario/mario" + verb + mario.dir + ".gif"
    |> image
    |> position (w/2.-16.+mario.x,  h-50.-31.-mario.y)

Keyboard.initKeyboard()

болсын w, h = getWindowDimensions()

болсын rec update mario () =
    болсын mario = mario |> Physics.marioStep (Keyboard.arrows())
    render (w,h) mario
    window.setTimeout(update mario, 1000 / 60) |> ignore

болсын mario = { x=0.; y=0.; vx=0.; vy=0.; dir="right" }
update mario ()
