// Source: http://www.tryfsharp.org/create/cpoulain/shared/raytracer.fsx
// slightly modified to avoid some allocations

модуль RayTracer

[<Struct>]
түрі Vector =
    { X: float; Y: float; Z: float }
    статикалық мүшесі (*) (k, v: Vector) = { X = k * v.X; Y = k * v.Y; Z = k * v.Z }
    статикалық мүшесі (-) (v1: Vector, v2: Vector) = { X = v1.X - v2.X; Y = v1.Y - v2.Y; Z = v1.Z - v2.Z }
    статикалық мүшесі (+) (v1: Vector, v2: Vector) = { X = v1.X + v2.X; Y = v1.Y + v2.Y; Z = v1.Z + v2.Z }
    статикалық мүшесі Dot (v1: Vector, v2: Vector) = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z
    статикалық мүшесі Mag (v: Vector) = sqrt (v.X * v.X + v.Y * v.Y + v.Z * v.Z)
    статикалық мүшесі Norm (v: Vector) =
        болсын mag = Vector.Mag v
        болсын div = егер mag = 0.0 содан infinity басқа 1.0/mag
        div * v
    статикалық мүшесі Cross (v1: Vector, v2: Vector) =
        { X = v1.Y * v2.Z - v1.Z * v2.Y
        ; Y = v1.Z * v2.X - v1.X * v2.Z
        ; Z = v1.X * v2.Y - v1.Y * v2.X }

[<Struct>]
түрі Color =
    { R: float; G: float; B: float }
    статикалық мүшесі Scale (k, v: Color) = { R = k * v.R; G = k * v.G; B = k * v.B }
    статикалық мүшесі (+) (v1: Color, v2: Color) = { R = v1.R + v2.R; G = v1.G + v2.G; B = v1.B + v2.B }
    статикалық мүшесі (*) (v1: Color, v2: Color) = { R = v1.R * v2.R; G = v1.G * v2.G; B = v1.B * v2.B }
    статикалық мүшесі White = { R = 1.0; G = 1.0; B = 1.0 }
    статикалық мүшесі Grey = { R = 0.5; G = 0.5; B = 0.5 }
    статикалық мүшесі Black = { R = 0.0; G = 0.0; B = 0.0 }
    статикалық мүшесі Background = Color.Black
    статикалық мүшесі DefaultColor = Color.Black

түрі Camera (pos: Vector, lookAt: Vector) =
    болсын forward = Vector.Norm (lookAt - pos)
    болсын down = { X = 0.0; Y = -1.0; Z = 0.0 }
    болсын right = 1.5 * Vector.Norm (Vector.Cross (forward, down))
    болсын up = 1.5 * Vector.Norm (Vector.Cross (forward, right))
    мүшесі c.Pos     = pos
    мүшесі c.Forward = forward
    мүшесі c.Up      = up
    мүшесі c.Right   = right

[<Struct>]
түрі Ray =
    { Start: Vector;
      Dir: Vector }

түрі Surface =
    abstract Diffuse: Vector -> Color
    abstract Specular: Vector -> Color
    abstract Reflect: Vector -> float
    abstract Roughness : float

[<Struct>]
түрі Intersection =
    { Thing: SceneObject;
      Ray: Ray;
      Dist: float }

and SceneObject =
    abstract Surface: Surface
    abstract Intersect: Ray -> float
    abstract Normal: Vector -> Vector

түрі Light =
    { Pos : Vector;
      Color : Color }

түрі Scene =
    { Things : SceneObject[];
      Lights : Light[];
      Camera : Camera }

модуль RayTracer =

    болсын maxDepth = 5

    болсын NearestIntersection ray scene =
        болсын mutable acc = None
        үшін x ішінде scene.Things жасау
            болсын dist = x.Intersect ray
            егер acc.IsNone || dist < acc.Value.Dist содан
                acc <- Some { Thing = x; Ray = ray; Dist = dist }
        acc

    болсын TestRay ray scene =
        сәйкестік NearestIntersection ray scene с
        | None -> None
        | Some isect ->
            егер isect.Dist = infinity
            содан None
            басқа Some isect.Dist

    болсын rec TraceRay ray scene (depth: int) =
        сәйкестік NearestIntersection ray scene с
        | None -> Color.Background
        | Some isect ->
            егер isect.Dist = infinity
            содан Color.Background
            басқа Shade isect scene depth

    and Shade isect scene depth =
        болсын d = isect.Ray.Dir
        болсын pos = isect.Dist * d + isect.Ray.Start
        болсын normal = isect.Thing.Normal (pos)
        болсын reflectDir = d - 2.0 * Vector.Dot (normal, d) * normal
        болсын naturalcolor = Color.DefaultColor + (GetNaturalColor isect.Thing pos normal reflectDir scene)
        болсын reflectedColor =
            егер depth >= maxDepth содан Color.Grey
            басқа GetReflectionColor (isect.Thing, pos + (0.001*reflectDir), normal, reflectDir, scene, depth)
        naturalcolor + reflectedColor

    and GetReflectionColor (thing: SceneObject, pos, normal: Vector, rd: Vector, scene: Scene, depth: int) =
        Color.Scale (thing.Surface.Reflect (pos), TraceRay { Start = pos; Dir = rd } scene (depth + 1))

    and GetNaturalColor thing pos normal rd scene =
        болсын mutable color = Color.DefaultColor
        үшін light ішінде scene.Lights жасау
            color <- AddLight thing pos normal rd scene color light
        color

    and AddLight (thing: SceneObject) pos normal rd scene color light =
        болсын ldis = light.Pos - pos
        болсын livec = Vector.Norm (ldis)
        болсын neatIsect = TestRay { Start = pos; Dir = livec } scene
        болсын isInShadow =
            сәйкестік neatIsect с
            | None -> false
            | Some d -> not (d > Vector.Mag (ldis))
        егер isInShadow содан color
        басқа
            болсын illum = Vector.Dot (livec, normal)
            болсын lcolor =
                егер illum > 0.0
                содан Color.Scale (illum, light.Color)
                басқа Color.DefaultColor
            болсын specular = Vector.Dot (livec, Vector.Norm (rd))
            болсын scolor =
                егер specular > 0.0
                содан Color.Scale (specular ** thing.Surface.Roughness, light.Color)
                басқа Color.DefaultColor
            color + thing.Surface.Diffuse (pos) * lcolor +
                    thing.Surface.Specular (pos) * scolor

    болсын GetPoint x y width height (camera: Camera) =
        болсын RecenterX x =  (float x - (float width / 2.0))  / (2.0 * float width)
        болсын RecenterY y = -(float y - (float height / 2.0)) / (2.0 * float height)
        Vector.Norm (camera.Forward + RecenterX (x) * camera.Right + RecenterY (y) * camera.Up)

    болсын Render scene (data: byte[]) (x, y, width, height) =
        болсын clamp v = min (max (v * 255.0) 0.0) 255.0 |> byte
        үшін y = y to height-1 жасау
            болсын stride = y * width
            үшін x = x to width-1 жасау
                болсын index = (x + stride) * 4
                болсын dir = GetPoint x y width height scene.Camera
                болсын ray = { Start = scene.Camera.Pos; Dir = dir }
                болсын color = TraceRay ray scene 0
                data.[index+0] <- clamp color.R
                data.[index+1] <- clamp color.G
                data.[index+2] <- clamp color.B
                data.[index+3] <- 255uy

модуль SceneObjects =

    түрі Sphere (center, radius, surface) =
        interface SceneObject с
            мүшесі this.Surface = surface
            мүшесі this.Normal pos = Vector.Norm (pos - center)
            мүшесі this.Intersect ray =
                болсын eo = center - ray.Start
                болсын v = Vector.Dot (eo, ray.Dir)
                болсын dist =
                    егер (v < 0.0) содан infinity
                    басқа
                        болсын disc = radius * radius - (Vector.Dot (eo,eo) - (v*v))
                        егер disc < 0.0
                        содан infinity
                        басқа v - (sqrt (disc))
                dist

    түрі Plane (normal, offset, surface) =
        interface SceneObject с
            мүшесі this.Surface = surface
            мүшесі this.Normal pos = normal
            мүшесі this.Intersect ray =
                болсын denom = Vector.Dot (normal, ray.Dir)
                болсын dist =
                    егер denom > 0.0
                    содан infinity
                    басқа (Vector.Dot (normal, ray.Start) + offset) / (-denom)
                dist

модуль Surfaces =

    түрі Shiny() =
        interface Surface с
            мүшесі s.Diffuse pos = Color.White
            мүшесі s.Specular pos = Color.Grey
            мүшесі s.Reflect pos = 0.7
            мүшесі s.Roughness = 250.0

    түрі Checkerboard() =
        interface Surface с
            мүшесі s.Diffuse pos =
                егер (int (floor (pos.Z) + floor (pos.X))) % 2 <> 0
                содан Color.White
                басқа Color.Black
            мүшесі s.Specular pos = Color.White
            мүшесі s.Reflect pos =
                егер (int (floor (pos.Z) + floor (pos.X))) % 2 <> 0
                содан 0.1
                басқа 0.7
            мүшесі s.Roughness = 150.0

модуль Scenes =

    болсын TwoSpheresOnACheckerboard = {
        Things = [|
            SceneObjects.Plane ({ X = 0.0; Y = 1.0; Z = 0.0 }, 0.0, Surfaces.Checkerboard())
            SceneObjects.Sphere ({ X = 0.0; Y = 1.0; Z = -0.25 }, 1.0, Surfaces.Shiny())
            SceneObjects.Sphere ({ X = -1.0; Y = 0.5; Z = 1.5 }, 0.5, Surfaces.Shiny())
        |];
        Lights = [|
            { Pos = { X = -2.0; Y = 2.5; Z = 0.0 }; Color = { R = 0.49; G = 0.07; B = 0.07 } }
            { Pos = { X = 1.5; Y = 2.5; Z = 1.5 }; Color = { R = 0.07; G = 0.07; B = 0.49 } }
            { Pos = { X = 1.5; Y = 2.5; Z = -1.5 }; Color = { R = 0.07; G = 0.49; B = 0.071 } }
            { Pos = { X = 0.0; Y = 3.5; Z = 0.0 }; Color = { R = 0.21; G = 0.21; B = 0.35 } }
        |];
        Camera =
            Camera ({ X = 3.0; Y = 2.0; Z = 4.0 }, { X = -1.0; Y = 0.5; Z = 0.0 })
    }

ашық Fable.Core.JsInterop
ашық Browser.Types
ашық Browser

болсын renderScene scene (x, y, width, height) =
    болсын canvas = document.getElementsByTagName("canvas").[0] :?> HTMLCanvasElement
    болсын ctx = canvas.getContext_2d()
    болсын img = ctx.createImageData(float width, float height)
    RayTracer.Render scene img.data (x, y, width, height)
    ctx.putImageData(img, float -x, float -y)

болсын measure f x y =
    болсын dtStart = window?performance?now()
    болсын res = f x y
    болсын elapsed = window?performance?now() - dtStart
    res, elapsed

болсын x, y, w, h = (0, 0, 512, 512)
болсын _, elapsed = measure renderScene Scenes.TwoSpheresOnACheckerboard (x, y, w, h)
printfn "Ray tracing:\n - rendered image size: (%dx%d)\n - elapsed: %f ms" w h elapsed
