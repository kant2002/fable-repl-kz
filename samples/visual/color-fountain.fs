модуль Color.Fountain

// Color Fountain by Erik Novales: https://github.com/enovales

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Browser.Types
ашық Browser

болсын canvas = document.getElementsByTagName("canvas").[0] :?> HTMLCanvasElement
canvas.width <- 1000.
canvas.height <- 800.
болсын ctx = canvas.getContext_2d()

болсын rng (): float = JS.Math.random()

болсын particleLimit = 200

түрі Particle = {
    x: double
    y: double
    xvel: double
    yvel: double
    c: (int * int * int)
    rot: double
    rotVel: double
}
с
    override this.ToString() =
        болсын (r,g,b) = this.c
        sprintf "Particle(x = %O, y = %O, xvel = %O, yvel = %O, c = (%O, %O, %O))"
            this.x this.y this.xvel this.yvel r g b


болсын updateParticle(dt: double)(p: Particle) =
    {
        p с
            x = p.x + p.xvel * dt
            y = p.y + p.yvel * dt
            yvel = p.yvel + 1. * dt
            rot = (p.rot + p.rotVel * dt) % (2. * 3.14159)
    }

болсын refillParticles(p: Particle array, dt: double) =
    болсын stillValid =
        p |> Array.filter(функ pt -> (pt.y < 1000.))
    //System.Console.WriteLine("stillValid.Length = " + stillValid.Length.ToString())
    болсын updatedPos =
        stillValid
        |> Array.map(updateParticle(dt))

    //System.Console.WriteLine("updatedPos = " + updatedPos |> Array.map(функ p -> p.ToString()).ToString())
    болсын toCreate = particleLimit - stillValid.Length
    //System.Console.WriteLine("going to create " + toCreate.ToString() + " particles")
    болсын newParticles =
        seq {
            үшін i ішінде 0..toCreate жасау
                yield {
                    Particle.x = 200.
                    y = 300.
                    xvel = (rng() - 0.5) * (rng() * 30.)
                    yvel = -(rng() * 25.)
                    c = (int (rng() * 255.), int (rng() * 255.), int (rng() * 255.))
                    rot = (rng() * 2. * 3.14159)
                    rotVel = (rng() * 1.5)
                }
        }
        |> Seq.toArray

    updatedPos |> Array.append(newParticles)

болсын mutable particles = [||]
болсын timestep = 0.8

болсын rec loop last t =
    // Comment out this line to make sure the animation runs
    // с same speed on different frame rates
    // болсын timestep = (t - last) / 20.
    particles <- refillParticles(particles, timestep)

    ctx.clearRect(0., 0., 10000., 10000.)
    болсын drawParticle(p: Particle) =
        болсын (r,g,b) = p.c
        болсын fs = "rgb(" + r.ToString() + ", " + g.ToString() + ", " + b.ToString() + ")"
        ctx.fillStyle <- !^fs

        болсын x1 = (p.x - 5.)
        болсын x2 = (p.x + 5.)
        болсын y1 = (p.y - 5.)
        болсын y2 = (p.y + 5.)

        // болсын x1 = (p.x - (10. * System.Math.Cos(p.rot)))
        // болсын x2 = (p.x + (10. * System.Math.Cos(p.rot)))
        // болсын y1 = (p.y - (10. * System.Math.Sin(p.rot)))
        // болсын y2 = (p.y + (10. * System.Math.Sin(p.rot)))

        // ctx.fillRect(x1, y1, 10., 10.)
        ctx.beginPath()
        ctx.moveTo(x1, y1)
        ctx.lineTo(x2, y1)
        ctx.lineTo(x2, y2)
        ctx.lineTo(x1, y2)
        ctx.lineTo(x1, y1)
        ctx.closePath()
        ctx.fill()

    particles
    |> Array.iter drawParticle

    window.requestAnimationFrame(loop t) |> ignore

// start the loop
loop 0. 0.
