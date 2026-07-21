модуль Mandelbrot

// You can draw a rectangle to zoom ішінде an area (feature added by Avi Avni)

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Browser.Types
ашық Browser

түрі Complex = { r : double; i : double }
түрі Color = { r : uint8; g : uint8; b : uint8; a : uint8 }

болсын maxIter = 255

болсын height = 1024
болсын width = 1024

болсын mutable minX = -2.0
болсын mutable maxX = 2.0
болсын mutable minY = -1.5
болсын mutable maxY = 3.5
болсын mutable rectX = 0.0
болсын mutable rectY = 0.0
болсын mutable rectW = 0.0
болсын mutable rectH = 0.0

болсын iteratePoint (s : Complex) (p : Complex) : Complex =
    { r = s.r + p.r*p.r - p.i*p.i; i = s.i + 2.0 * p.i * p.r }

болсын getIterationCount (p : Complex) =
    болсын mutable z = p
    болсын mutable i = 0
    while i < maxIter && (z.r*z.r + z.i*z.i < 4.0) жасау
      z <- iteratePoint p z
      i <- i + 1
    i

болсын getCoord (x : int, y : int) : Complex =
    болсын p = { r = float x * (maxX - minX) / float width + minX
            ; i = float y * (maxY - minY) / float height + minY }
    p

болсын getCoordColor (x : int, y : int) : Color =
    болсын p = getCoord (x, y)
    болсын i = getIterationCount p
    { r = uint8 (255/(i%5)); g = uint8 (255/(i%3)); b = uint8 (255/(i%7)); a = 255uy }

болсын showSet() =
    болсын canvas = document.getElementsByTagName("canvas").[0] :?> HTMLCanvasElement
    болсын ctx = canvas.getContext_2d()

    болсын img = ctx.createImageData(float width, float height)
    үшін y = 0 to height-1 жасау
        үшін x = 0 to width-1 жасау
            болсын index = (x + y * width) * 4
            болсын color = getCoordColor (x, y)
            img.data.[index+0] <- color.r
            img.data.[index+1] <- color.g
            img.data.[index+2] <- color.b
            img.data.[index+3] <- color.a
    ctx.putImageData(img, 0., 0.)

    ctx.fillStyle <- !^"rgba(200,0,0,0.5)"
    ctx.fillRect (rectX, rectY, rectW, rectH)


document.addEventListener("mousedown", функ de ->
    болсын de = de :?> MouseEvent
    rectX <- de.clientX
    rectY <- de.clientY
    rectW <- 0.0
    rectH <- 0.0
    showSet())

document.addEventListener("mousemove", функ de ->
    болсын de = de :?> MouseEvent
    егер de.buttons = 1.0 содан
        rectW <- de.clientX - rectX
        rectH <- de.clientY - rectY
        showSet())

document.addEventListener("mouseup", функ de ->
    болсын de = de :?> MouseEvent
    болсын p1 = getCoord (int rectX, int rectY)
    болсын p2 = getCoord (int (rectX + rectW), int (rectY + rectH))
    minX <- min p1.r p2.r
    maxX <- max p1.r p2.r
    minY <- min p1.i p2.i
    maxY <- max p1.i p2.i
    rectX <- 0.0
    rectY <- 0.0
    rectW <- 0.0
    rectH <- 0.0
    showSet())

showSet()
