модуль BasicCanvas

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Browser.Types
ашық Browser

болсын init() =
    болсын canvas = document.querySelector(".view") :?> HTMLCanvasElement

    болсын ctx = canvas.getContext_2d()
    // The (!^) operator checks and casts a value to an Erased Union түрі
    // See http://fable.io/docs/interacting.html#Erase-attribute
    ctx.fillStyle <- !^"rgb(200,0,0)"
    ctx.fillRect (10., 10., 55., 50.)
    ctx.fillStyle <- !^"rgba(0, 0, 200, 0.5)"
    ctx.fillRect (30., 30., 55., 50.)

init()
