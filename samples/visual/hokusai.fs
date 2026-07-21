модуль Hokusai

(*
Hokusai and Julia: rendering fractals using HTML5 canvas
This demo is based on Tomas Petricek's F# Advent Calendar post that explores
Japanese art and renders The Great Wave by Hokusai using the Julia fractal.
*)

ашық Fable.Core
ашық Browser.Types
ашық Browser

түрі Complex =
  | Complex бастап float * float
  /// Calculate the absolute value бастап a complex number
  статикалық мүшесі Abs(Complex(r, i)) =
    болсын num1, num2 = abs r, abs i
    егер (num1 > num2) содан
      болсын num3 = num2 / num1
      num1 * sqrt(1.0 + num3 * num3)
    басегер num2 = 0.0 содан
      num1
    басқа
      болсын num4 = num1 / num2
      num2 * sqrt(1.0 + num4 * num4)
  /// Add real and imaginary components pointwise
  статикалық мүшесі (+) (Complex(r1, i1), Complex(r2, i2)) =
    Complex(r1+r2, i1+i2)

модуль ComplexModule =
  /// Calculates nth power бастап a complex number
  болсын Pow(Complex(r, i), power) =
    болсын num = Complex.Abs(Complex(r, i))
    болсын num2 = atan2 i r
    болсын num3 = power * num2
    болсын num4 = num ** power
    Complex(num4 * cos(num3), num4 * sin(num3))

/// Constant that generates nice fractal
болсын c = Complex(-0.70176, -0.3842)

/// Generates sequence үшін given coordinates
болсын iterate x y =
  болсын rec loop current = seq {
    yield current
    yield! loop (ComplexModule.Pow(current, 2.0) + c) }
  loop (Complex(x, y))

болсын countIterations max x y =
  iterate x y
  |> Seq.take (max - 1)
  |> Seq.takeWhile (функ v -> Complex.Abs(v) < 2.0)
  |> Seq.length

// Transition between colors ішінде 'count' steps
болсын (--) clr count = clr, count
болсын (-->) ((r1, g1, b1), count) (r2, g2, b2) = [
  үшін c ішінде 0 .. count - 1 ->
    болсын k = c / count |> byte
    болсын mid v1 v2 =
      (v1 + (v2 - v1) * k)
    (mid r1 r2, mid g1 g2, mid b1 b2) ]

// Palette с colors used by Hokusai
болсын palette =
  [| // 3x sky color & transition to light blue
     yield! (245uy, 219uy, 184uy) --3--> (245uy, 219uy, 184uy)
     yield! (245uy, 219uy, 184uy) --4--> (138uy, 173uy, 179uy)
     // to dark blue and содан medium dark blue
     yield! (138uy, 173uy, 179uy) --4--> (2uy, 12uy, 74uy)
     yield! (2uy, 12uy, 74uy)     --4--> (61uy, 102uy, 130uy)
     // to wave coloruy,  содан light blue & back to wave
     yield! (61uy, 102uy, 130uy)  -- 8--> (249uy, 243uy, 221uy)
     yield! (249uy, 243uy, 221uy) --32--> (138uy, 173uy, 179uy)
     yield! (138uy, 173uy, 179uy) --32--> (61uy, 102uy, 130uy)
  |]

// Specifies what range бастап the set to draw
болсын w = -0.4, 0.4
болсын h = -0.95, -0.35

// Create bitmap that matches the size бастап the canvas
болсын width = 400.0
болсын height = 300.0


/// Set pixel value ішінде ImageData to a given color
болсын setPixel (img:ImageData) x y width (r, g, b) =
  болсын index = (x + y * int width) * 4
  img.data.[index+0] <- r
  img.data.[index+1] <- g
  img.data.[index+2] <- b
  img.data.[index+3] <- 255uy

/// Dynamic operator that returns HTML element by ID
болсын (?) (doc:Document) name :'R =
  doc.getElementById(name) :?> 'R

/// Render fractal asynchronously с sleep after every line
болсын render () = async {
  // Get <canvas> element & create image үшін drawing
  болсын canv : HTMLCanvasElement = document?canvas
  болсын ctx = canv.getContext_2d()
  болсын img = ctx.createImageData(float width, float height)

  // For each pixel, transform to the specified range
  // and get color using countInterations and palette
  үшін x ішінде 0 .. int width - 1 жасау
    үшін y ішінде 0 .. int height - 1 жасау
      болсын x' = (float x / width * (snd w - fst w)) + fst w
      болсын y' = (float y / height * (snd h - fst h)) + fst h
      болсын it = countIterations palette.Length x' y'
      setPixel img x y width (palette.[it])

    // Insert non-blocking waiting & update the fractal
    do! Async.Sleep(1)
    ctx.putImageData(img, 0.0, 0.0) }

/// Setup button event handler to start the rendering
болсын go : HTMLButtonElement = document?go
go.addEventListener("click", функ _ ->
  render() |> Async.StartImmediate)
