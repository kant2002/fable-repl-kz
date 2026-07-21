модуль Ozmo

// Phil Trelford's classic Ozmo game ported to Fable!
// Shows how to handle keyboard events and use HTML5 canvas.
// You can also get it (as a JavaScript app) from the Windows Store.

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Browser.Types
ашық Browser

модуль Keyboard =

    болсын mutable keysPressed = Set.empty

    болсын code x = егер keysPressed.Contains(x) содан 1 басқа 0

    болсын arrows () =
        (code "ArrowRight" - code "ArrowLeft", code "ArrowUp" - code "ArrowDown")

    болсын update (e : KeyboardEvent, pressed) =
        болсын key = e.key
        болсын op = егер pressed содан Set.add басқа Set.remove
        keysPressed <- op key keysPressed

    болсын init () =
        window.addEventListener("keydown", функ e -> update(e :?> _, true))
        window.addEventListener("keyup", функ e -> update(e :?> _, false))

// Main

/// Scale to make it fit ішінде a 1920*1080 screen
болсын scale = 0.8

/// The width бастап the canvas
болсын width = 900. * scale
/// The height бастап the canvas
болсын height = 668. * scale
/// Height бастап the floor - the bottom black part
болсын floorHeight = 100. * scale
/// Height бастап the atmosphere - the yellow gradient
болсын atmosHeight = 300. * scale

Keyboard.init()

болсын canvas = document.getElementsByTagName("canvas").[0] :?> HTMLCanvasElement
болсын ctx = canvas.getContext_2d()
canvas.width <- width
canvas.height <- height

/// Draw gradient between two Y offsets and two colours
болсын drawGrd (ctx:CanvasRenderingContext2D)
    (canvas:HTMLCanvasElement) (y0,y1) (c0,c1) =
    болсын grd = ctx.createLinearGradient(0.,y0,0.,y1)
    grd.addColorStop(0.,c0)
    grd.addColorStop(1.,c1)
    ctx.fillStyle <- !^ grd
    ctx.fillRect(0.,y0, canvas.width, y1- y0)


/// Draw background бастап the Ozmo game
болсын drawBg ctx canvas =
    drawGrd ctx canvas
        (0.,atmosHeight) ("yellow","orange")
    drawGrd ctx canvas
        (atmosHeight, canvas.height-floorHeight)
        ("grey","white")
    ctx.fillStyle <- !^ "black"
    ctx.fillRect
        ( 0.,canvas.height-floorHeight,
          canvas.width,floorHeight )

/// Draw the specified text (when game finishes)
болсын drawText(text,x,y) =
    ctx.fillStyle <- !^ "white"
    ctx.font <- "bold 40pt";
    ctx.fillText(text, x, y)


түрі Blob =
    { X:float; Y:float;
      vx:float; vy:float;
      Radius:float; color:string }

болсын drawBlob (ctx:CanvasRenderingContext2D)
    (canvas:HTMLCanvasElement) (blob:Blob) =
    ctx.beginPath()
    ctx.arc
        ( blob.X, canvas.height - (blob.Y + floorHeight + blob.Radius),
          blob.Radius, 0., 2. * System.Math.PI, false )
    ctx.fillStyle <- !^ blob.color
    ctx.fill()
    ctx.lineWidth <- 3.
    ctx.strokeStyle <- !^ blob.color
    ctx.stroke()


/// Apply key effects on Player's blob - changes X speed
болсын direct (dx,dy) (blob:Blob) =
    { blob с vx = blob.vx + (float dx)/4.0 }

/// Apply gravity on falling blobs - gets faster every step
болсын gravity (blob:Blob) =
    егер blob.Y > 0. содан { blob с vy = blob.vy - 0.1 }
    басқа blob

/// Bounde Player's blob off the wall егер it hits it
болсын bounce (blob:Blob) =
    болсын n = width
    егер blob.X < 0. содан
        { blob с X = -blob.X; vx = -blob.vx }
    басегер (blob.X > n) содан
        { blob с X = n - (blob.X - n); vx = -blob.vx }
    басқа blob


/// Move blob by one step - adds X and Y
/// velocities to the X and Y coordinates
болсын move (blob:Blob) =
    { blob с
        X = blob.X + blob.vx
        Y = max 0.0 (blob.Y + blob.vy) }

/// Apply step on Player's blob. Composes above functions.
болсын step dir blob =
    blob |> direct dir |> move |> bounce

/// Check whether two blobs collide
болсын collide (a:Blob) (b:Blob) =
    болсын dx = (a.X - b.X)*(a.X - b.X)
    болсын dy = (a.Y - b.Y)*(a.Y - b.Y)
    болсын dist = sqrt(dx + dy)
    dist < abs(a.Radius - b.Radius)

/// Remove all falling blobs that hit Player's blob
болсын absorb (blob:Blob) (drops:Blob list) =
    drops
    |> List.filter (функ drop ->
        collide blob drop |> not )


// Game helpers
// =============

болсын grow = "black"
болсын shrink = "white"

болсын newDrop color =
    { X = JS.Math.random()*width*0.8 + (width*0.1)
      Y=600.; Radius=10.; vx=0.; vy = 0.0
      color=color }

болсын newGrow () = newDrop grow
болсын newShrink () = newDrop shrink

/// Update drops and countdown ішінде each step
болсын updateDrops drops countdown =
    егер countdown > 0 содан
        drops, countdown - 1
    басегер floor(JS.Math.random()*8.) = 0. содан
        болсын drop =
            егер floor(JS.Math.random()*3.) = 0. содан newGrow()
            басқа newShrink()
        drop::drops, 8
    басқа drops, countdown


/// Count growing and shrinking drops ішінде the list
болсын countDrops drops =
    болсын count color =
        drops
        |> List.filter (функ drop -> drop.color = color)
        |> List.length
    count grow, count shrink

// Asynchronous game loop
// ========================

болсын rec game () = async {
    болсын blob =
        { X = 300.; Y=0.; Radius=50.;
          vx=0.; vy=0.; color="black" }
    return! update blob [newGrow ()] 0 }

and completed () = async {
    drawText ("COMPLETED",320.,300.)
    do! Async.Sleep 10000
    return! game () }

/// Keeps current state үшін Player's blob, falling
/// drops and the countdown since last drop was generated
and update blob drops countdown = async {
    // Update the drops & countdown
    болсын drops, countdown = updateDrops drops countdown

    // Count drops, apply physics and count them again
    болсын beforeGrow, beforeShrink = countDrops drops
    болсын drops =
        drops
        |> List.map (gravity >> move)
        |> absorb blob
    болсын afterGrow, afterShrink = countDrops drops
    болсын drops = drops |> List.filter (функ blob -> blob.Y > 0.)

    // Calculate жаңа player's size based on absorbed drops
    болсын radius = blob.Radius + float (beforeGrow - afterGrow) *4.
    болсын radius = radius - float (beforeShrink - afterShrink) * 4.
    болсын radius = max 5.0 radius

    // Update radius and apply keyboard events
    болсын blob = { blob с Radius = radius }
    болсын blob = blob |> step (Keyboard.arrows())

    // Render the жаңа game state
    drawBg ctx canvas
    үшін drop ішінде drops жасау drawBlob ctx canvas drop
    drawBlob ctx canvas blob

    // If the game completed, switch state
    // otherwise sleep and update recursively!
    егер blob.Radius > 150. содан
        return! completed()
    басқа
        do! Async.Sleep(int (1000. / 60.))
        return! update blob drops countdown }

game () |> Async.StartImmediate
