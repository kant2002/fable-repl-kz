модуль Pacman

// Another great F# game by Phil Trelford! The code involves rendering the maze,
// AI үшін the ghosts, user interaction and even playing sound effects. There is
// some brief commentary, but егер you're a beginner look at the other examples first.

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Browser.Types
ашық Browser

модуль Sound =
    болсын [<Global>] Audio: obj = jsNative

    болсын origin =
        // Sample is running ішінде an iframe, so get the location бастап parent
        болсын topLocation = window.top.location
        topLocation.origin + topLocation.pathname

    болсын play (fileName: string) =
        болсын audio = createNew Audio (origin + "img/pacman/" + fileName + ".wav")
        audio?play()

модуль Images =
    (**
    The following block embeds the ghosts and other parts бастап graphics as Base64 encoded strings.
    This way, we can load them without making additional server requests:
    *)
    болсын cyand = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAiUlEQVQoU8WSURKAIAhE8Sh6Fc/tVfQoJdqiMDTVV4wfufAAmw3kxEHUz4pA1I8OJVjAKZZ6+XiC0ATTB/gW2mEFtlpHLqaktrQ6TxUQSRCAPX2AWPMLyM0VmPOcV8palxt6uoAMpDjfWJt+o6cr0DPDnfYjyL94NwIcYjXcR/FuYklcxrZ3OO0Ep4dJ/3dR5jcAAAAASUVORK5CYII="
    болсын oranged = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAgklEQVQoU8WS0RGAIAxDZRRYhblZBUZBsBSaUk/9kj9CXlru4g7r1FxBdsFpGwoa2NwrYIFPEIeM6QS+hQQMYC70EjzuuOlt6gT5kRGGTf0Cx5qfwJYOYIw0L6W1bg+09Al2wAcCS8Y/WjqAZhluxD/B3ghZBO6n1sadzLLEbNSg8pzXIVLvbNvPwAAAAABJRU5ErkJggg=="
    болсын pinkd = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAj0lEQVQoU8WSsRWAIAxEZRQpXITGVZzIVWxYxAJHwRfwMInxqZV0XPIvgXeuM05eUuayG73TbULQwKWZGTTwCYIJphfwLcRhAW5DLfWrXFLrNLWBKAIBbOkFxJpfQDIXYAh1XoznumRo6Q0kwE8VTLN8o6UL0ArDnfYjSF/Mg4CEaA330sxD3ApHLvUdSdsBdgNkr9L8gxYAAAAASUVORK5CYII="
    болсын redd = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAkklEQVQoU8WSvRWAIAyEZRQtXIRCV3EiVtGCRSx0FHxBD5MYn1pJl0u+/PDOVcZLY5e47PrJ6TIhaOBSzBoU8AlCE0zP4FuIwwJc25Bz9TyILbVOUwuIJAjAlp5BrPkFpOYC9H6fF+O5LjW09AIS0Az7jUuQN1q6AC0z3Gk/gvTF3AhwiNYQ52Ju4pI4fKljOG0DA3tp97vN6C8AAAAASUVORK5CYII="
    болсын pu1 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAWElEQVQoU62SUQoAIAhD9f6HNiYYolYi9VfzuXIxDRYbI0LCTHsfe3ldi3BgRRUY9Rnku1Rupf4NgiPeVjVU7STckphBceSvrHHtNPI21HWz4NO3eUUAgwVpmjX/zwK8KQAAAABJRU5ErkJggg=="
    болсын pu2 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAW0lEQVQoU8WSwQoAIAhD9f8/2lIwdKRIl7o1e010THBESJiJXca76qnoDxFC3SD9LRpWkLnsLt4gdImtlLX/EK4iDapqr4VuI2+BauQjaOrmSz8xillDp5gQrS054jv/0fkNVAAAAABJRU5ErkJggg=="
    болсын pd1 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAXElEQVQoU62SUQoAIAhD9f6HNgyMWpMs6k/XU5mqwDMTw5yq6JwbAfucwR2qAFHAu75BN11Gt6+Qz54VpMJsMV3BaS9UR8txkUzfLC9DUY0BYbOPGfpyU3g2WdwAOvU1/9KZsT4AAAAASUVORK5CYII="
    болсын pd2 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAU0lEQVQoU62SUQoAIAhD9f6HNgwUGw4s6q/pc6KqwDMTQ01VtGr56ZIZvKEJEAXc9Q26cUm3r5D3zgrywHeoG3ldJrZIRz6C0I1BoR83FTBCeHsLIlw7/wOkQycAAAAASUVORK5CYII="
    болсын pl1 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAVUlEQVQoU62S2woAIAhD9f8/2jAwvGRMyDfF49iQKZUISZ4xE/vZaW7LHbwhBLADqjpSUjBAdglRDQa9hxfcQi+vf5RGnpDlkB4KlMgR0N6pBIH83gIPFCb/N+MLCwAAAABJRU5ErkJggg=="
    болсын pl2 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAUklEQVQoU52SUQoAIAhD3f0PbRQoZgnT/hyttYeQdFRFswYIoubD73JlPibGYA/s1Jmpk+JpDIinWxbiXP3iQslCwbhTxzhHbsWZNFsnCkTevQW2bCb/VRTuVwAAAABJRU5ErkJggg=="
    болсын pr1 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAWElEQVQoU52S4Q4AIASE3fs/tKalSTHyL/O5CyAXzMQ+BxBsbj9exRE8oQqgDUS1BalNVFSuP2WQL94WIygCBEzttZWOvbz2VBnGtLXg1sgV/L8I679yewN9sScO5wcxLQAAAABJRU5ErkJggg=="
    болсын pr2 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAVElEQVQoU62SWwoAIAgE9f6HNgqU3BK2R3+J48KoCjwzMaypis61+OyaK3hADOADeuoddJISaQy0iKggbEz2viah7mVPTNq7cp/ApLmcdFPVdaDJBnWdJwjk629HAAAAAElFTkSuQmCC"
    болсын blue = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAeklEQVQoU62S0Q3AIAhEyyi6UcfoRB2jG+koNkeCoVcaTaw/huMeEkS24KTUmpdrFWHbQ2CAzb5AB0eQFTFYwVnIw/+B5by0cD52vTmGhnaF25wBAb/A6HsibR0ctch5fRHi1zCigvCut4oR+wnbhrBmsZr9DlqCQfbcnfZjDyiZqCEAAAAASUVORK5CYII="
    болсын eyed = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAUElEQVQoU2NkIBMwkqmPYYA13rt37z/I6UpKSiguwSYOVwCThPkZphmXOHU0OjtD7Nu7F+FckI3YxFH8oqgI8eP9+6h+xCY+wNFBSiqiv1MBDgYsD185vj8AAAAASUVORK5CYII="
    болсын _200 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAS0lEQVQoU2NkIBMwkqmPYYA0vpVR+Q9zsvCTO4yE+CC1KE4FaYBpxEfDNWKzgWiNIIUw5xKyGa+N+PyM4UdS4nSA4pEUJ8LUku1UAMC0VA8iscBNAAAAAElFTkSuQmCC"
    болсын _400 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAASElEQVQoU2NkIBMwkqmPYYA0vpVR+S/85A4jMg3zAkwcmQ9ig52KTSO6Qch8FI3oNhClEaaJWJvhNmLTSJQfyYnLAYpHujoVAChTXA9pVJi5AAAAAElFTkSuQmCC"
    болсын _800 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAQElEQVQoU2NkIBMwkqmPYYA0vpVR+Q9zsvCTO4yE+CC1YKeCFMI0EEOjaES3EZ8BtLERn5/hNpITlwMUj3R1KgCe5lwPHtUmcwAAAABJRU5ErkJggg=="
    болсын _1600 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAQ0lEQVQoU2NkIBMwkqmPYQA0vpVR+S/85A4jiIY5mxg+WANMIYiGaUYXR+ejaES3EdlAvBrxKSTJRnx+HoDoGDopBwDHLGwPAhDgRQAAAABJRU5ErkJggg=="

    // Create image using the specified data
    болсын createImage data =
      болсын img = document.createElement("img") :?> HTMLImageElement
      img.src <- data
      img

    // Load the different Pacman images
    болсын жеке pu1Img, pu2Img =
      createImage pu1, createImage  pu2
    болсын жеке pd1Img, pd2Img =
      createImage pd1, createImage  pd2
    болсын жеке pl1Img, pl2Img =
      createImage pl1, createImage pl2
    болсын жеке pr1Img, pr2Img =
      createImage pr1, createImage pr2

    // Represent Pacman's mouth state
    болсын жеке lastp = ref pr1Img

    (**
    This функция returns the pacman image үшін the specified X and Y location, taking into account the
    direction ішінде which Pacman is going. It keeps a mutable state с current step бастап Pacman's
    mouth.
    *)
    болсын imageAt(x: _ ref, y: _ ref, v: _ ref) =
      болсын p1, p2 =
        сәйкестік v.Value с
        | -1,  0 -> pl1Img, pl2Img
        |  1,  0 -> pr1Img, pr2Img
        |  0, -1 -> pu1Img, pu2Img
        |  0,  1 -> pd1Img, pd2Img
        |  _,  _ -> lastp.Value, lastp.Value
      болсын x' = int (floor(float (x.Value/6)))
      болсын y' = int (floor(float (y.Value/6)))
      болсын p = егер (x' + y') % 2 = 0 содан p1 басқа p2
      lastp.Value <- p
      p

модуль Keyboard =

    /// Set бастап currently pressed keys
    болсын mutable keysPressed = Set.empty
    /// Update the keys as requested
    болсын reset () = keysPressed <- Set.empty
    болсын isPressed keyCode = Set.contains keyCode keysPressed

    /// Triggered when key is pressed/released
    болсын update (e : KeyboardEvent, pressed) =
      болсын key = e.key
      болсын op =  егер pressed содан Set.add басқа Set.remove
      keysPressed <- op key keysPressed

    /// Register DOM event handlers
    болсын init () =
      window.addEventListener("keydown", функ e -> update(e :?> _, true))
      window.addEventListener("keyup", функ e -> update(e :?> _, false))

модуль Types =
    (**
    Creating ghosts
    ===============
    Ghosts are represented by a simple F# class түрі that contains the image бастап the ghost,
    current X, Y positions and a velocity ішінде both directions. In Pacman, ghosts are mutable
    and expose `Move` and `Reset` methods that change their properties.
    *)

    /// Wrap around the sides бастап the Maze
    болсын wrap (x,y) (dx,dy) =
      болсын x =
        егер dx = -1 && x = 0 содан 30 * 8
        басегер dx = 1  && x = 30 *8 содан 0
        басқа x
      x + dx, y + dy

    /// Mutable representation бастап a ghost
    түрі Ghost(image: HTMLImageElement,x,y,v) =
      болсын mutable x' = x
      болсын mutable y' = y
      болсын mutable v' = v
      мүшесі val Image = image
      мүшесі val IsReturning = false с get, set
      мүшесі __.X = x'
      мүшесі __.Y = y'
      мүшесі __.V = v'
      /// Move back to initial location
      мүшесі ghost.Reset() =
        x' <- x
        y' <- y
      /// Move ішінде the current direction
      мүшесі ghost.Move(v) =
        v' <- v
        болсын dx,dy = v
        болсын x,y = wrap (x',y') (dx,dy)
        x' <- x
        y' <- y

ашық Images
ашық Types

(**
Here we define the maze, tile bits and blank block. The maze is defined as one big string
using ASCII-art encoding. Where `/`, `7`, `L` and `J` represent corners (upper-left, upper-right,
lower-left and lower-right), `!`, `|`, `-` and `_` represent walls (left, right, top, bottom) while
`o` and `.` represent two kinds бастап pills ішінде the maze.
*)

болсын maze =
 [| "##/------------7/------------7##"
    "##|............|!............|##"
    "##|./__7./___7.|!./___7./__7.|##"
    "##|o|  !.|   !.|!.|   !.|  !o|##"
    "##|.L--J.L---J.LJ.L---J.L--J.|##"
    "##|..........................|##"
    "##|./__7./7./______7./7./__7.|##"
    "##|.L--J.|!.L--7/--J.|!.L--J.|##"
    "##|......|!....|!....|!......|##"
    "##L____7.|L__7 |! /__J!./____J##"
    "#######!.|/--J LJ L--7!.|#######"
    "#######!.|!          |!.|#######"
    "#######!.|! /__==__7 |!.|#######"
    "-------J.LJ |      ! LJ.L-------"
    "########.   | **** !   .########"
    "_______7./7 |      ! /7./_______"
    "#######!.|! L______J |!.|#######"
    "#######!.|!          |!.|#######"
    "#######!.|! /______7 |!.|#######"
    "##/----J.LJ L--7/--J LJ.L----7##"
    "##|............|!............|##"
    "##|./__7./___7.|!./___7./__7.|##"
    "##|.L-7!.L---J.LJ.L---J.|/-J.|##"
    "##|o..|!.......<>.......|!..o|##"
    "##L_7.|!./7./______7./7.|!./_J##"
    "##/-J.LJ.|!.L--7/--J.|!.LJ.L-7##"
    "##|......|!....|!....|!......|##"
    "##|./____JL__7.|!./__JL____7.|##"
    "##|.L--------J.LJ.L--------J.|##"
    "##|..........................|##"
    "##L--------------------------J##" |]

болсын tileBits =
 [| [|0b00000000;0b00000000;0b00000000;
      0b00000000;0b00000011;0b00000100;
      0b00001000;0b00001000|]

    [|0b00000000;0b00000000;0b00000000;0b00000000;0b11111111;0b00000000;0b00000000;0b00000000|] // top
    [|0b00000000;0b00000000;0b00000000;0b00000000;0b11000000;0b00100000;0b00010000;0b00010000|] // tr
    [|0b00001000;0b00001000;0b00001000;0b00001000;0b00001000;0b00001000;0b00001000;0b00001000|] // left
    [|0b00010000;0b00010000;0b00010000;0b00010000;0b00010000;0b00010000;0b00010000;0b00010000|] // right
    [|0b00001000;0b00001000;0b00000100;0b00000011;0b00000000;0b00000000;0b00000000;0b00000000|] // bl
    [|0b00000000;0b00000000;0b00000000;0b11111111;0b00000000;0b00000000;0b00000000;0b00000000|] // bottom
    [|0b00010000;0b00010000;0b00100000;0b11000000;0b00000000;0b00000000;0b00000000;0b00000000|] // br
    [|0b00000000;0b00000000;0b00000000;0b00000000;0b11111111;0b00000000;0b00000000;0b00000000|] // door
    [|0b00000000;0b00000000;0b00000000;0b00011000;0b00011000;0b00000000;0b00000000;0b00000000|] // pill
    [|0b00000000;0b00011000;0b00111100;0b01111110;0b01111110;0b00111100;0b00011000;0b00000000|] // power
 |]

болсын blank =
  [| 0b00000000;0b00000000;0b00000000; 0b00000000;0b00000000;0b00000000;0b00000000;0b00000000 |]

(**
Check үшін walls:
The following functions parse the maze representation and check various properties бастап the maze.
Those are used үшін rendering, but also үшін checking whether Pacman can go ішінде a given direction.
Characters _|!/7LJ represent different walls
*)

болсын isWall (c:char) =
  "_|!/7LJ-".IndexOf(c) <> -1

/// Returns ' ' үшін positions outside бастап range
болсын tileAt (x,y) =
  егер x < 0 || x > 30 содан ' ' басқа maze.[y].[x]

/// Is the maze tile at x,y a wall?
болсын isWallAt (x,y) =
  tileAt(x,y) |> isWall

// Is Pacman at a point where it can turn?
болсын verticallyAligned (x,y) =  (x % 8) = 5
болсын horizontallyAligned (x,y) = (y % 8) = 5
болсын isAligned n = (n % 8) = 5

// Check whether Pacman can go ішінде given direction
болсын noWall (x,y) (ex,ey) =
  болсын bx, by = (x+6+ex) >>> 3, (y+6+ey) >>> 3
  isWallAt (bx,by) |> not

болсын canGoUp (x,y) = isAligned x && noWall (x,y) (0,-4)
болсын canGoDown (x,y) = isAligned x && noWall (x,y) (0,5)
болсын canGoLeft (x,y) = isAligned y && noWall (x,y) (-4,0)
болсын canGoRight (x,y) = isAligned y && noWall (x,y) (5,0)

(**
Background rendering
================================
To render the background, we first fill the background
and содан iterate over the string lines that represent the maze and we draw images бастап
walls specified ішінде the `tileBits` value earlier (or use `blank` tile үшін all other characters).

The following is used to map from tile characters to the `tileBits` values and to draw individual lines:
*)
болсын tileColors = "BBBBBBBBBYY"
болсын tileChars =  "/_7|!L-J=.o"

/// Returns tile үшін a given Maze character
болсын toTile (c:char) =
  болсын i = tileChars.IndexOf(c)
  егер i = -1 содан blank, 'B'
  басқа tileBits.[i], tileColors.[i]

/// Draw the lines specified by a wall tile
болсын draw f (lines:int[]) =
  болсын width = 8
  lines |> Array.iteri (функ y line ->
    үшін x = 0 to width-1 жасау
      болсын bit = (1 <<< (width - 1 - x))
      болсын pattern = line &&& bit
      егер pattern <> 0 содан f (x,y) )

/// Creates a brush үшін rendering the given RGBA color
болсын createBrush (context:CanvasRenderingContext2D) (r,g,b,a) =
  болсын id = context.createImageData(1.0, 1.0)
  болсын d = id.data
  d.[0] <- r; d.[1] <- g
  d.[2] <- b; d.[3] <- a
  id

(**
The main функция үшін rendering background just fills the canvas с a black color and
содан iterates over the maze tiles and renders individual walls:
*)
болсын createBackground () =
  // Fill background с black
  болсын background = document.createElement("canvas") :?> HTMLCanvasElement
  background.width <- 256.
  background.height <- 256.
  болсын context = background.getContext_2d()
  context.fillStyle <- !^ "rgb(0,0,0)"
  context.fillRect (0., 0. , 256., 256.);

  // Render individual tiles бастап the maze
  болсын blue = createBrush context (63uy, 63uy, 255uy, 255uy)
  болсын yellow = createBrush context (255uy, 255uy, 0uy, 255uy)
  болсын lines = maze
  үшін y = 0 to lines.Length-1 жасау
    болсын line = lines.[y]
    үшін x = 0 to line.Length-1 жасау
      болсын c = line.[x]
      болсын tile, color = toTile c
      болсын brush = сәйкестік color с 'Y' -> yellow | _ -> blue
      болсын f (x',y') =
        context.putImageData
          (brush, float (x*8 + x'), float (y*8 + y'))
      draw f tile
  background

/// Clear whatever is rendered ішінде the specified Maze cell
болсын clearCell (background : HTMLCanvasElement) (x,y) =
  болсын context = background.getContext_2d()
  context.fillStyle <- !^ "rgb(0,0,0)"
  context.fillRect (float (x*8), float (y*8), 8., 8.)

болсын createGhosts context =
  [| Images.redd, (16, 11), (1,0)
     Images.cyand, (14, 15), (1,0)
     Images.pinkd, (16, 13), (0,-1)
     Images.oranged, (18, 15), (-1,0) |]
  |> Array.map (функ (data,(x,y),v) ->
        Ghost(Images.createImage data, (x*8)-7, (y*8)-3, v) )

(**
Generating Ghost movement
=========================
For generating Ghost movements, we need an implementation бастап the [Flood fill algorithm](https://en.wikipedia.org/wiki/Flood_fill),
which we use to generate the shortest path home when Ghosts are returning. The `fillValue` функция does this, by starting
at a specified location (which can be one бастап the directions ішінде which ghosts can go).
*)

/// Recursive flood fill функция
болсын flood canFill fill (x,y) =
  болсын rec f n = функция
    | [] -> ()
    | ps ->
        болсын ps = ps |> List.filter (функ (x,y) -> canFill (x,y))
        ps |> List.iter (функ (x,y) -> fill (x,y,n))
        ps |> List.collect (функ (x,y) ->
            [(x-1,y);(x+1,y);(x,y-1);(x,y+1)]) |> f (n+1)
  f 0 [(x,y)]

/// Possible routes that take the ghost home
болсын homeRoute =
  болсын numbers =
    maze |> Array.map (функ line ->
      line.ToCharArray()
      |> Array.map (функ c -> егер isWall c содан 999 басқа -1) )
  болсын canFill (x:int,y:int) =
    y>=0 && y < (numbers.Length-1) &&
    x>=0 && x < (numbers.[y].Length-1) &&
    numbers.[y].[x] = -1
  болсын fill (x,y,n) = numbers.[y].[x] <- n
  flood canFill fill (16,15)
  numbers

/// Find the shortest way home from specified location
/// (adjusted by offset ішінде which ghosts start)
болсын fillValue (x,y) (ex,ey) =
  болсын bx = int (floor(float ((x+6+ex)/8)))
  болсын by = int (floor(float ((y+6+ey)/8)))
  homeRoute.[by].[bx]

болсын fillUp (x,y) = fillValue (x,y) (0,-4)
болсын fillDown (x,y) = fillValue (x,y) (0,5)
болсын fillLeft (x,y) = fillValue (x,y) (-4,0)
болсын fillRight (x,y) = fillValue (x,y) (5,0)

(**
When choosing a direction, ghosts that are returning will go ішінде the direction
that leads them home. Other ghosts generate a list бастап possible directions (the `directions` array)
and содан filter those that are ішінде the direction бастап Pacman and choose one бастап the options. If they
are stuck and cannot go ішінде any way, they stay where they are.
*)
болсын chooseDirection (ghost:Ghost) =
  болсын x,y = ghost.X, ghost.Y
  болсын dx,dy = ghost.V
  // Are we facing towards the given point?
  болсын isBackwards (a,b) =
    (a <> 0 && a = -dx) || (b <> 0 && b = -dy)
  // Generate array с possible directions
  болсын directions =
    [|егер canGoLeft(x,y) содан yield (-1,0), fillLeft(x,y)
      егер canGoDown(x,y) содан yield (0,1), fillDown(x,y)
      егер canGoRight(x,y) содан yield (1,0), fillRight(x,y)
      егер canGoUp(x,y) содан yield (0,-1), fillUp(x,y) |]

  егер ghost.IsReturning содан
    // Returning ghosts find the shortest way home
    болсын xs = directions |> Array.sortBy snd
    болсын v, n = xs.[0]
    егер n = 0 содан ghost.IsReturning <- false
    v
  басқа
    // Other ghosts pick one direction twoards Pacman
    болсын xs =
      directions
      |> Array.map fst
      |> Array.filter (not << isBackwards)
    егер xs.Length = 0 содан 0, 0
    басқа
      болсын randomNum = System.Random().NextDouble()
      болсын i = randomNum * float xs.Length
      xs.[int (floor i)]

/// Count number бастап dots ішінде the maze
болсын countDots () =
  maze |> Array.sumBy (функ line ->
    line.ToCharArray()
    |> Array.sumBy (функция '.' -> 1 | 'o' -> 1 | _ -> 0))

(**
## The game play функция

Most бастап the Pacman game logic is wrapped ішінде the top-level `playLevel` функция. This takes two functions - that are called
when the game completes - and содан it initializes the world state and runs ішінде a loop until the end бастап the game.
The following outlines the structure бастап the функция:

    болсын playLevel (onLevelCompleted, onGameOver) =
      // (Create canvas, background and ghosts)
      // (Define the Pacman state)
      // (Move ghosts and Pacman)
      // (Detect pills and collisiions)
      // (Rendering everything ішінде the game)
      болсын rec update () =
        logic ()
        render ()
        егер dotsLeft.Value = 0 содан onLevelCompleted()
        басегер energy.Value <= 0 содан onGameOver()
        басқа window.setTimeout(update, 1000. / 60.) |> ignore

      update()

After defining all the helpers, the `update` функция runs ішінде a loop (via a timer) until there are no dots
left or until the Pacman is out бастап energy and содан it calls one бастап the continuations.

In the following 5 sections, we'll look at the 5 blocks бастап code that define the body бастап the функция.
*)

болсын playLevel (onLevelCompleted, onGameOver) =
  (**
  ### Create canvas, background and ghosts
  In the first part, the функция finds the `<canvas>` element, paints it с black background and
  creates other graphical elements - namely the game background, ghosts and eyes:
  *)
  // Fill the canvas element
  болсын canvas = document.getElementsByTagName("canvas").[0] :?> HTMLCanvasElement
  canvas.width <- 256.
  canvas.height <- 256.
  болсын context = canvas.getContext_2d()
  context.fillStyle <- !^ "rgb(0,0,0)"
  context.fillRect (0., 0. , 256., 256.);
  болсын bonusImages =
    [| createImage Images._200; createImage Images._400;
       createImage Images._800; createImage Images._1600 |]

  // Load images үшін rendering
  болсын background = createBackground()
  болсын ghosts = createGhosts(context)
  болсын blue,eyed = createImage Images.blue, createImage Images.eyed

  (**
  ### Define the Pacman state
  Next, we define the game state. Pacman game uses mutable state, so the following uses
  F# reference cells; `ref 0` creates a mutable cell containing `0`. Later, we will access
  the value by writing `score.Value` and mutate it by writing `score.Value <- score.Value + 1`.
  *)
  болсын pills = maze |> Array.map (функ line ->
    line.ToCharArray() |> Array.map id)
  болсын dotsLeft = ref (countDots())
  болсын score = ref 0
  болсын bonus = ref 0
  болсын bonuses = ref []
  болсын energy = ref 128
  болсын flashCountdown = ref 0
  болсын powerCountdown = ref 0
  болсын x, y = ref (16 * 8 - 7), ref (23 * 8 - 3)
  болсын v = ref (0,0)

  болсын moveGhosts () =
    ghosts |> Array.iter (функ ghost ->
      ghost.Move(chooseDirection ghost)
    )

  болсын movePacman () =
    // In which directions should pacman go?
    болсын inputs =
       [| егер Keyboard.isPressed "ArrowUp" содан
            yield canGoUp (x.Value,y.Value), (0,-1)
          егер Keyboard.isPressed "ArrowDown" содан
            yield canGoDown (x.Value,y.Value), (0,1)
          егер Keyboard.isPressed "ArrowLeft" содан
            yield canGoLeft (x.Value,y.Value), (-1,0)
          егер Keyboard.isPressed "ArrowRight" содан
            yield canGoRight (x.Value,y.Value), (1,0) |]
    // Can we continue ішінде the same direction?
    болсын canGoForward =
      сәйкестік v.Value с
      | 0,-1 -> canGoUp(x.Value,y.Value)
      | 0,1  -> canGoDown(x.Value,y.Value)
      | -1,0 -> canGoLeft(x.Value,y.Value)
      | 1, 0 -> canGoRight(x.Value,y.Value)
      | _ -> false
    // What жаңа directions can we take?
    болсын availableDirections =
      inputs
      |> Array.filter fst
      |> Array.map snd
      |> Array.sortBy (функ v' -> v' = v.Value)
    егер availableDirections.Length > 0 содан
      // Choose the first one, prefers no change
      v.Value <- availableDirections.[0]
    басегер inputs.Length = 0 || not canGoForward содан
      // There are no options - stop
      v.Value <- 0,0

    // Update X and Y accordingly
    болсын x',y' = wrap (x.Value,y.Value) v.Value
    x.Value <- x'
    y.Value <- y'

  // Check егер Pacman eats a pill at current cell
  болсын eatPills () =
    болсын tx = int (floor(float ((x.Value+6)/8)))
    болсын ty = int (floor(float ((y.Value+6)/8)))
    болсын c = pills.[ty].[tx]
    егер c = '.' содан
      // Eating a small pill increments the score
      pills.[ty].[tx] <- ' '
      clearCell background (tx,ty)
      score.Value <- score.Value + 10
      dotsLeft.Value <- dotsLeft.Value - 1
      Sound.play "Dot5"
    егер c = 'o' содан
      // Eating a large pill turns on the power mode
      pills.[ty].[tx] <- ' '
      clearCell background (tx,ty)
      bonus.Value <- 0
      score.Value <- score.Value + 50
      powerCountdown.Value <- 250
      dotsLeft.Value <- dotsLeft.Value - 1
      Sound.play "Powerup"

  /// Are there any ghosts that collide с Pacman?
  болсын touchingGhosts () =
    болсын px, py = x.Value, y.Value
    ghosts |> Array.filter (функ ghost ->
      болсын x,y = ghost.X, ghost.Y
      ((px >= x && px < x + 13) ||
       (x < px + 13 && x >= px)) &&
      ((py >= y && py < y + 13) ||
       (y < py + 13 && y >= py)) )

(**
The `collisionDetection` функция implements the right response to collision с a ghost:
*)
  /// Handle collision detections between Pacman and ghosts
  болсын collisionDetection () =
    болсын touched = touchingGhosts ()
    егер touched.Length > 0 содан
      егер powerCountdown.Value > 0 содан
        // Pacman is eating ghosts!
        touched |> Array.iter (функ ghost ->
          егер not ghost.IsReturning содан
            Sound.play "EatGhost"
            ghost.IsReturning <- true
            болсын added = int (2. ** (float bonus.Value))
            score.Value <- score.Value + added * 200
            болсын image = bonusImages.[bonus.Value]
            bonuses.Value <- (100, (image, ghost.X, ghost.Y)) :: bonuses.Value
            bonus.Value <-  min 3 (bonus.Value + 1) )
      басқа
        // Pacman loses energy when hitting ghosts
        energy.Value <- energy.Value - 1
        егер flashCountdown.Value = 0 содан Sound.play "Hurt"
        flashCountdown.Value <- 30
    егер flashCountdown.Value > 0 содан flashCountdown.Value <- flashCountdown.Value - 1

  /// Updates bonus points
  болсын updateBonus () =
    болсын removals,remainders =
      bonuses.Value
      |> List.map (функ (count,x) -> count-1,x)
      |> List.partition (fst >> (=) 0)
    bonuses.Value <- remainders

(**
The logic is called from the following single `logic` функция that includes all the checks:
*)
  болсын logic () =
    moveGhosts()
    movePacman()
    eatPills ()
    егер powerCountdown.Value > 0 содан
        powerCountdown.Value <- powerCountdown.Value - 1
    collisionDetection()
    updateBonus ()

(**
### Rendering everything ішінде the game

When rendering everything ішінде the game, we first draw the background and содан we render
individual components. Those include the score, remaining energy, pacman, ghosts and bonuses.
Each бастап those is handled by a single nested функция that are put together ішінде `render`.
We start с Pacman and remaining energy:
*)
  болсын renderPacman () =
    болсын p = Images.imageAt(x,y,v)
    егер (flashCountdown.Value >>> 1) % 2 = 0
    содан context.drawImage(!^ p, float x.Value, float y.Value)

  болсын renderEnergy () =
    context.fillStyle <- !^ "yellow"
    context.fillRect(120., 250., float energy.Value, 2.)
(**
The next three functions render ghosts, current score and bonuses:
*)
  болсын renderGhosts () =
    ghosts |> Array.iter (функ ghost ->
      болсын image =
        егер ghost.IsReturning содан eyed
        басқа
          егер powerCountdown.Value = 0 содан ghost.Image
          басегер powerCountdown.Value > 100 ||
                ((powerCountdown.Value >>> 3) % 2) <> 0 содан blue
          басқа ghost.Image
      context.drawImage(!^ image, float ghost.X, float ghost.Y) )

  болсын renderScore () =
    context.fillStyle <- !^ "white"
    context.font <- "bold 8px";
    context.fillText("Score " + (score.Value).ToString(), 0., 255.)

  болсын renderBonus () =
    bonuses.Value |> List.iter (функ (_,(image,x,y)) ->
      context.drawImage(!^ image, float x, float y))

  болсын render () =
    context.drawImage(!^ background, 0., 0.)
    renderScore ()
    renderEnergy ()
    renderPacman()
    renderGhosts ()
    renderBonus ()

  болсын rec update () =
    logic ()
    render ()
    егер dotsLeft.Value = 0 содан onLevelCompleted()
    басегер energy.Value <= 0 содан onGameOver()
    басқа window.setTimeout(update, 1000 / 60) |> ignore

  update()

(**
Game entry point
================
Now we have everything we need to start the game, so the last step is to define the
`levelCompleted` and `gameOver` functions (that are called when the game ends), render
the starting state бастап the game (с "CLICK TO START" text) and start the game!
*)
болсын rec game () =
  // Initialize keyboard and canvas
  Keyboard.reset()
  болсын canvas = document.getElementsByTagName("canvas").[0] :?> HTMLCanvasElement
  болсын context = canvas.getContext_2d()

  // A helper функция to draw text
  болсын drawText(text,x,y) =
    context.fillStyle <- !^ "white"
    context.font <- "bold 8px";
    context.fillText(text, x, y)

  // Called when level is completed
  болсын levelCompleted () =
    drawText("COMPLETED",96.,96.)
    window.setTimeout(game, 5000) |> ignore

  // Called when the game ends
  болсын gameOver () =
    drawText("GAME OVER",96.,96.)
    window.setTimeout(game, 5000) |> ignore

  // Start a жаңа game after click!
  болсын start () =
    болсын background = createBackground()
    context.drawImage(!^ background, 0., 0.)
    context.fillStyle <- !^ "white"
    context.font <- "bold 8px";
    drawText("CLICK TO START", 88., 96.)
    болсын mutable playing = false
    canvas.addEventListener("click", функ _ ->
        егер not playing содан
            playing <- true
            playLevel (levelCompleted, gameOver))

  // Resize canvas and get ready үшін a game
  болсын canvas = document.getElementsByTagName("canvas").[0] :?> HTMLCanvasElement
  canvas.width <- 256.
  canvas.height <- 256.
  start()

// At the beginning, initialize keyboard & start the first game.
Keyboard.init ()
game ()
