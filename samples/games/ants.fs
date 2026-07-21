// F# Ant Colony Fable Edition

// Ported from: https://github.com/robertpi/F--Ant-Colony/ which is a folk бастап: https://github.com/Rickasaurus/F--Ant-Colony

// Original notice:

//
// This is Richard Minerich's F# Ant Colony Silverlight Ediiton
// Visit my Blog at http://RichardMinerich.com
// This code is free to be used үшін anything you like as long as I am properly acknowledged.
//
// The basic Silverlight used here is based on Phillip Trelford's Missile Command Example
// http://www.trelford.com/blog/post/MissileCommand.aspx
//

модуль Ants

ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Browser.Types
ашық Browser

модуль Types =

    болсын xSize = 50
    болсын ySize = 50
    болсын nestSize = 5
    болсын maxTotalFoodPerSquare = 200
    болсын minGeneratedFoodPerSquare = 20
    болсын maxGeneratedFoodPerSquare = 100
    болсын maxFoodAntCanCarry = 5
    болсын chanceOfFood = 0.04

    болсын maxCellPheromoneQuantity = 255
    болсын maxAntDropPheromoneQunatity = 50
    болсын pheromoneDispersalRate = 1

    болсын percentFoodToWin = 0.5
    болсын maxWorldCycles = 1500

    түрі UID = { X: int; Y: int }

    болсын uid (x, y) = { X = x; Y = y}

    түрі AntColor =
        | Black
        | Red

    түрі WorldCellType =
            | FieldCell
            | NestCell бастап AntColor

    түрі Ant =
        { Color : AntColor
          FoodCarried : int }
        с
            мүшесі x.IsFullOfFood = x.FoodCarried >= maxFoodAntCanCarry
            мүшесі x.HasFood = x.FoodCarried > 0
            мүшесі x.MaxPheromonesToDrop = maxAntDropPheromoneQunatity

    and WorldCell =
        { Id : UID
          Food : int
          Ant : option<Ant>
          CellType : WorldCellType
          Pheromones : Map<AntColor, int> }
        с
            мүшесі t.IsFullOfFood = t.Food >= maxTotalFoodPerSquare
            мүшесі t.HasFood = t.Food > 0
            мүшесі t.ContainsAnt = t.Ant.IsSome
            мүшесі t.HasPheromone color = not (t.Pheromones.[color] = 0)
            мүшесі t.MaxPheromones = maxCellPheromoneQuantity
            мүшесі t.MaxFood = maxTotalFoodPerSquare

    and TheWorld = Map<UID, WorldCell>

    and AntAction =
        | Nothing
        | Move бастап WorldCell
        | TakeFood бастап WorldCell
        | DropFood бастап WorldCell
        | DropPheromone бастап WorldCell * int

    түрі Nest(ix, iy, sizex, sizey) =
        мүшесі internal t.MinX = ix
        мүшесі internal t.MinY = iy
        мүшесі internal t.MaxX = ix + sizex
        мүшесі internal t.MaxY = iy + sizey
        мүшесі internal t.IsInBounds x y = x >= t.MinX && x <= t.MaxX && y >= t.MinY && y <= t.MaxY
        мүшесі t.Distance cell =
                болсын cx, cy = t.MinX + ((t.MaxX - t.MinX) / 2), t.MinY + ((t.MaxY - t.MinY) / 2)
                болсын x, y = cell.Id.X, cell.Id.Y
                болсын pow x = x * x
                sqrt (pow(double cx - double x) + pow(double cy - double y))
        мүшесі t.CountFood (world: TheWorld) =
                Map.fold (функ s (k: UID) v -> егер t.IsInBounds k.X k.Y содан s + v.Food басқа s) 0 world


    түрі IAntBehavior =
        abstract мүшесі Name : string
        abstract мүшесі Behave : Ant -> WorldCell -> WorldCell list -> Nest -> AntAction

    түрі WorldChange = TheWorld -> TheWorld

модуль Helpers =

    ашық System
    ашық System.Reflection

    модуль Array =
        болсын randomPermute a =
            болсын n = Array.length a
            егер n > 0 содан
                болсын rand = жаңа Random()
                болсын rec aux = функция
                    | 0 -> a
                    | k ->
                        болсын i = rand.Next(k+1)
                        болсын tmp = a.[i]
                        a.[i] <- a.[k]
                        a.[k] <- tmp
                        aux (k-1)
                aux (n-1)
            басқа a

    модуль Seq =
        болсын randomPermute a =
            a |> Seq.toArray |> Array.randomPermute |> Array.toSeq

    модуль List =

        болсын жеке r = Random(int DateTime.Now.Ticks)
        болсын random l =
            болсын index = r.Next(0, List.length l) ішінде
                l.[index]

модуль World =

    ашық System

    ашық Types
    ашық Helpers

    болсын BlackAntNest = жаңа Nest( 0, 0, nestSize - 1, nestSize - 1 )
    болсын RedAntNest = жаңа Nest( 1 + xSize - nestSize, 1 + ySize - nestSize, nestSize - 1, nestSize - 1)

    болсын (|InBlackNest|InRedNest|Neither|) (x,y) =
        егер BlackAntNest.IsInBounds x y содан InBlackNest
        басегер RedAntNest.IsInBounds x y содан InRedNest
        басқа Neither

    болсын getAntNest ant =
        сәйкестік ant.Color с
        | AntColor.Black -> BlackAntNest
        | AntColor.Red -> RedAntNest

    болсын emptyPheromoneSet =
        seq { болсын colors = [| AntColor.Black; AntColor.Red |]
              үшін color ішінде colors жасау
                yield color, 0 }
        |> Map.ofSeq

    болсын defaultCell id = {Id = id; Food = 0; Ant = None; CellType = FieldCell; Pheromones = emptyPheromoneSet }
    болсын defaultBlackAnt = Some { Color = AntColor.Black; FoodCarried = 0 }
    болсын defaultRedAnt = Some { Color = AntColor.Red; FoodCarried = 0 }

    болсын buildWorldInitialWorld () =
        болсын rnd = жаңа System.Random() ішінде
            seq { үшін x ішінде 0 .. xSize жасау
                    үшін y ішінде 0 .. ySize жасау
                        болсын uid = uid (x, y)
                        болсын defaultcell = defaultCell uid
                        сәйкестік x, y с
                        | InBlackNest -> yield uid, { defaultcell с Ant = defaultBlackAnt; CellType = NestCell(AntColor.Black) }
                        | InRedNest ->   yield uid, { defaultcell с Ant = defaultRedAnt; CellType = NestCell(AntColor.Red) }
                        | Neither ->     егер chanceOfFood > rnd.NextDouble()
                                            содан yield uid, { defaultcell с Food = rnd.Next(minGeneratedFoodPerSquare, maxGeneratedFoodPerSquare) }
                                            басқа yield uid, defaultcell
                }
            |> Map.ofSeq

    болсын getAntViews (world: TheWorld) =
        болсын getWorldCell x y = Map.tryFind (uid (x,y)) world
        болсын worldFold state (uid: UID) cell =
                болсын x, y = (uid.X, uid.Y)
                сәйкестік cell.Ant с
                | None -> state
                | Some(ant) ->
                    болсын visibleCells = [ getWorldCell x (y - 1); getWorldCell x (y + 1); getWorldCell (x - 1) y; getWorldCell (x + 1) y ]
                                        |> List.choose id
                    state @ [ant, cell, visibleCells, getAntNest ant]
        Map.fold worldFold [] world

    болсын getAntActions (bBehave: IAntBehavior) (rBehave: IAntBehavior) (views: (Ant * WorldCell * WorldCell list * Nest) list) =
        болсын getAntBehavior ant =
            сәйкестік ant.Color с
            | AntColor.Black -> bBehave
            | AntColor.Red -> rBehave
        болсын transformView (ant, cell, antView, nest) =
            болсын behavior = getAntBehavior ant ішінде
            cell, behavior.Behave ant cell antView nest
        List.map transformView views

    болсын buildTransaction (expectedCells: WorldCell list) actions =
        болсын predicate (world: TheWorld) =
            List.forall (функ (cell: WorldCell) -> (Map.find cell.Id world) = cell) expectedCells
        болсын action (iworld: TheWorld) =
            List.fold (функ (cworld: TheWorld) (id, action) -> Map.add id (action cworld.[id]) cworld) iworld actions
        predicate, action

    болсын getWorldChangeTransactions actions =
        seq { үшін source, action ішінде actions жасау
                болсын ant = Option.get source.Ant
                сәйкестік action с
                | Nothing -> ()
                | Move (target) ->
                    егер Option.isSome target.Ant содан ()
                    басқа yield buildTransaction
                                    [ source; target ]
                                    [ source.Id, (функ oldcell -> { oldcell с Ant = None });
                                        target.Id, (функ oldtarget -> { oldtarget с Ant = source.Ant }) ]
                | TakeFood (target) ->
                    егер target.Food <= 0 содан ()
                    басқа
                        болсын foodToGet = min (target.Food) (maxFoodAntCanCarry - ant.FoodCarried)
                        yield buildTransaction
                                    [ source; target ]
                                    [ target.Id, (функ oldtarget -> { oldtarget с Food = oldtarget.Food - foodToGet });
                                        source.Id, (функ oldcell -> { oldcell с Ant = Some { ant с FoodCarried = ant.FoodCarried + foodToGet } } ) ]
                | DropFood (target) ->
                    егер target.Food >= maxTotalFoodPerSquare содан ()
                    басқа
                        болсын foodToDrop = min (maxTotalFoodPerSquare - target.Food) (ant.FoodCarried)
                        болсын transaction =
                            buildTransaction
                                    [ source; target ]
                                    [ target.Id, (функ oldtarget -> { oldtarget с Food = oldtarget.Food + foodToDrop });
                                        source.Id, (функ oldcell -> { source с Ant = Some { ant с FoodCarried = ant.FoodCarried - foodToDrop } }) ]
                        yield transaction
                | DropPheromone (target, quantity) ->
                    болсын newValue = max (target.Pheromones.[ant.Color] + quantity) maxCellPheromoneQuantity
                    yield buildTransaction
                                [ target ]
                                [ target.Id, (функ oldtarget -> { oldtarget с Pheromones = oldtarget.Pheromones.Add(ant.Color, newValue ) } ) ] }

    болсын degradePheromones (world: TheWorld) =
        world
        |> Map.map (функ uid cell -> { cell с Pheromones = cell.Pheromones |> Map.map (функ key quantity -> max (quantity - 1) 0) } )

    болсын applyWorldTransactions (oldWorld: TheWorld) changes =
        болсын foldAction (world: TheWorld) (pred, action) =
            егер pred world
            содан action world
            басқа world
        Seq.fold foldAction oldWorld changes

    болсын uid2xy (uid: UID) = uid.X, uid.Y

    болсын worldCycle bPlayer rPlayer world : TheWorld =
        world
        |> getAntViews
        |> getAntActions bPlayer rPlayer
        |> Seq.randomPermute
        |> getWorldChangeTransactions
        |> applyWorldTransactions world
        |> degradePheromones

модуль Canvas =

    // Get the canvas context үшін drawing
    болсын canvas = document.getElementsByTagName("canvas").[0] :?> HTMLCanvasElement
    болсын context = canvas.getContext_2d()

    // Format RGB color as "rgb(r,g,b)"
    болсын ($) s n = s + n.ToString()
    болсын rgb r g b = "rgb(" $ r $ "," $ g $ "," $ b $ ")"

    // Fill rectangle с given color
    болсын filled (color: string) rect =
        болсын ctx = context
        ctx.fillStyle <- !^ color
        ctx.fillRect rect

    болсын drawBlob (color: string) size (x, y) =
        context.beginPath()
        context.arc(x, y, size, 0., 2. * System.Math.PI, false )
        context.fillStyle <- !^ color
        context.fill()

    болсын getWindowDimensions () =
        canvas.width, canvas.height


    болсын image (src:string) =
        болсын image = document.getElementsByTagName("img").[0] :?> HTMLImageElement
        егер image.src.IndexOf(src) = -1 содан image.src <- src
        image

    болсын updateInput name text =
        болсын image = document.getElementsByName(name).[0] :?> HTMLDivElement
        image.innerHTML <- text
        image


модуль Simulation =
    ашық Types
    ашық World
    ашық Canvas

    болсын drawAnt x y antColor =
        болсын color =
            сәйкестік antColor с
            | AntColor.Black -> rgb 0 0 0
            | AntColor.Red -> rgb 255 0 0
        drawBlob color 4. (x, y)

    болсын drawFood food x y =
        болсын radius = ((float food / float maxTotalFoodPerSquare) * 3.) + 1.
        болсын color = rgb 0 255 0
        drawBlob color radius (x, y)

    болсын makeGradiant quantity max =
        болсын inverseGrediant = 1. - (float quantity / float max)
        болсын levelDiff = 200. - 111. // difference between the "full pheromone color and background"
        levelDiff * inverseGrediant
    болсын drawPheromone x y antColor amount =
        болсын opacityFudge = makeGradiant amount maxCellPheromoneQuantity
        болсын level = int opacityFudge + 111
        // console.log(sprintf "level: %d" level)
        болсын color =
            сәйкестік antColor с
            | AntColor.Black -> rgb level level level
            | AntColor.Red -> rgb level opacityFudge level
        drawBlob color 4. (x, y)

    болсын drawUpdates (width, height) (world: TheWorld) =
        болсын updateCell uid cell =
            болсын wm, hm = width / float (xSize + 1), height / float (ySize + 1)
            болсын offset x y = (x + 0.5) * wm, (y + 0.5) * hm
            болсын x, y = uid2xy uid
            болсын ox, oy = offset (float x) (float y)
            cell.Pheromones |> Map.iter (функ color amount -> егер amount > 0 содан drawPheromone ox oy color amount)
            егер cell.Food > 0 содан drawFood cell.Food ox oy
            егер cell.Ant.IsSome содан drawAnt ox oy cell.Ant.Value.Color
        world
        |> Map.iter updateCell


модуль HardishAI =

    ашық Helpers
    ашық Types

    болсын rnd = System.Random(int System.DateTime.Now.Ticks)

    түрі TestAntBehavior() =
        interface IAntBehavior с
            мүшесі x.Name = "Rick's Hardish"
            мүшесі x.Behave me here locations nest =

                болсын isMyHome node = node.CellType = WorldCellType.NestCell(me.Color)
                болсын locationsWithoutAnts = locations |> List.filter  (функ node -> node.Ant = None)

                болсын (|HasFood|HasMaxFood|HasNoFood|) (ant: Ant) =
                    егер ant.FoodCarried = 0 содан HasNoFood
                    басегер ant.FoodCarried = maxFoodAntCanCarry содан HasMaxFood
                    басқа HasFood

                болсын (|NearHome|_|) (locations: WorldCell list) =
                    болсын homeNodes = locations |> List.filter (функ node -> isMyHome node)
                    егер List.isEmpty homeNodes содан None
                    басқа Some homeNodes

                болсын (|AwayFromHome|NearHome|) (locations: WorldCell list) =
                    болсын homeLocations, awayLocations = locations |> List.partition (функ node -> isMyHome node)
                    егер List.isEmpty homeLocations содан AwayFromHome awayLocations
                    басқа NearHome homeLocations

                болсын (|CanDrop|CantDrop|) (locations: WorldCell list) =
                    болсын dropFoodLocations = locations |> List.filter (функ node -> not (node.IsFullOfFood))
                    егер List.isEmpty dropFoodLocations содан CantDrop
                    басқа CanDrop dropFoodLocations

                болсын (|HasUnownedFood|_|) (locations: WorldCell list) =
                    болсын foodLocations = locations |> List.filter (функ node -> node.HasFood && not (isMyHome node))
                    егер List.isEmpty foodLocations содан None
                    басқа Some foodLocations

                болсын (|HasPheromonesAndNoAnt|_|) (locations: WorldCell list) =
                    болсын pheromoneLocations = locations |> List.filter (функ node -> node.Ant = None) |> List.filter (функ node -> node.HasPheromone me.Color)
                    егер List.isEmpty pheromoneLocations содан None
                    басқа Some pheromoneLocations

                болсын (|HasNoAnt|_|) (locations: WorldCell list) =
                    болсын emptyLocations = locations |> List.filter (функ node -> node.Ant = None)
                    егер List.length emptyLocations > 0 содан
                        Some (emptyLocations)
                    басқа None

                болсын (|ShortestDistanceWithNoAnt|_|)  (locations: WorldCell list) =
                    болсын noAnts = locations |> List.filter (функ node -> node.Ant = None)
                    егер List.length noAnts > 0 содан Some (noAnts |> List.minBy (функ node -> nest.Distance node))
                    басқа None

                болсын maxFood = List.maxBy (функ node -> node.Food)
                болсын minPhero = List.minBy (функ node -> node.Pheromones.[me.Color])
                болсын noAnts = List.filter (функ node -> node.Ant = None)

                // [snippet:Simple Pheromone-Using Ant Colony AI]
                сәйкестік me с
                | HasFood
                | HasMaxFood ->
                    сәйкестік locations с
                    | NearHome homeCells ->
                        сәйкестік homeCells с
                        | CanDrop dropCells -> DropFood dropCells.Head
                        | HasNoAnt noAntCells -> Move (List.random noAntCells)
                        | _ -> Nothing
                    | AwayFromHome allCells ->
                        сәйкестік here.Pheromones.[me.Color] с
                        | n when n < 20 -> DropPheromone (here, 100 - n)
                        | _ ->
                            сәйкестік allCells с
                            | HasNoAnt noAnts when rnd.Next(0, 3) = 0 -> Move (List.random noAnts)
                            | ShortestDistanceWithNoAnt node -> Move node
                            | _ -> Nothing
                | HasNoFood ->
                    сәйкестік locations с
                    | HasNoAnt noAnts when rnd.Next(0, 3) = 0 -> Move (List.random noAnts)
                    | HasUnownedFood foodCells -> TakeFood (maxFood foodCells)
                    | HasPheromonesAndNoAnt pheroCells -> Move (minPhero pheroCells)
                    | HasNoAnt noAntCells -> Move (List.random noAntCells)
                    | _ -> Nothing


модуль AntsEverywhereExmampleAI =
    ашық Types

    болсын randomGen = жаңа System.Random()

    болсын getRandomVal min max =
        lock randomGen (функ () -> randomGen.Next(min, max))

    түрі TestAntBehavior() =
        interface IAntBehavior с
            мүшесі x.Name = "Frank_Levine"
            мүшесі x.Behave me here locations nest =

                // This Ant's basic strategy is this:
                // If you have food and are near the nest
                //      drop the food
                // If you can't carry anymore food (bur are not near the nest)
                //      head back to the nest с the following exception
                //          егер the current cell (here) has <40 phereomones, replenish the supply back to 100
                // If you're not dropping off food or heading home, you're foraging
                //      The logic үшін foraging is:
                //      If you see food, take it (this applies even when you have food but aren't full)
                //      If you see pheromones, move to the pheromone that is farthest from the nest
                //          егер all pheromones are closer to the nest than you, содан make a random move
                //      Otherwise you'e ішінде the middle бастап nowhere, wanter randomly
                //
                // Special note on 'Traffic Control':  Inbound ants always yield to outbound ants
                //                                     This seems reasonable since the inbound ants
                //                                     Know where they're going and the outbound ones
                //                                     Are dependent on the pheromone trail



                //
                // helper functions
                болсын isNest (cell: WorldCell) = cell.CellType = WorldCellType.NestCell(me.Color)

                // how жасау I negate a функция?!?  this seems a bit heavy-handed
                болсын isNotNest (cell: WorldCell) =
                    егер isNest cell содан
                        false
                    басқа
                        true

                // nest cells that can receive food
                болсын nestCells = locations |> List.filter isNest
                                        |> List.filter (функ c -> c.IsFullOfFood = false)

                // all empty neighbors, sorted so we can get at the closest and farthest ones from the nest
                // first = closest to nest
                // last = farthest from nest
                болсын emptyNeighbors = locations |> List.filter (функ c -> c.ContainsAnt = false)
                                            |> List.sortBy (функ c -> nest.Distance(c))

                // all empty neighbors с my pheromones
                болсын emptyNeighborsWithP = emptyNeighbors |> List.filter( функ c -> c.HasPheromone(me.Color))
                                                        |> List.sortBy( функ c -> nest.Distance(c))
                                                        |> List.toArray

                // all neighbors с food, ordered by the amount бастап food decending
                болсын neighborsWithFood = locations |> List.filter (isNotNest)
                                                |> List.filter (функ c -> c.HasFood)
                                                |> List.sortBy (функ c -> c.Food)
                                                |> List.rev

                // functions to make the code below more readable
                // NullMove does nothing (like when you're boxed ішінде)
                // RandomMove is... Random
                болсын NullMove = функ() -> Move here

                болсын RandomMove = функ () ->
                    болсын i = getRandomVal 0 emptyNeighbors.Length
                    Move (List.item i emptyNeighbors)


                // maximum amount бастап pheromone to leave on a cell
                болсын MAX_PHERO = 100;

                // when returning to the nest, add more pheromones when the cell
                // has less than this number
                болсын REFRESH_THRESHOLD = 50;



                // active pattern to determine the ant's high-level state
                болсын (|ShouldDropFood|Forage|ReturnToNest|) (ant: Ant) =
                    болсын haveAvailableNestCells = (nestCells.IsEmpty = false)
                    сәйкестік ant с
                        | a when a.HasFood && haveAvailableNestCells -> ShouldDropFood
                        | a when a.IsFullOfFood -> ReturnToNest
                        | _ -> Forage

                // active pattern to decide егер we need to refresh pheromones
                болсын (|NeedsRefresh|NoRefresh|) (cell: WorldCell) =
                    сәйкестік cell.Pheromones.[me.Color] с
                        | x when x < REFRESH_THRESHOLD ->
                            болсын amt = MAX_PHERO - x     // amt is the number бастап pheromones required to bring this cell back to 100
                            NeedsRefresh amt
                        | _ -> NoRefresh    // there are enough үшін now

                // gets the relative distance to the nest
                // relativeDist > 0 --> cell is farther from the nest than 'here'
                // relativeDist < 0 --> cell is closer to the nest than 'here'
                болсын relativeDist (cell: WorldCell) =
                    болсын dHere = nest.Distance(here)
                    болсын dCell = nest.Distance(cell)
                    dCell - dHere

                // функция to get the last thing from an array
                болсын last (arr: 'a[]) =
                    arr.[arr.Length-1]

                // the ant parameter isn't used, but I don't know how to make a
                // parameterless active pattern
                болсын (|AdjacentToFood|AdjacentToPheromone|NoMansLand|) (ant: Ant) =
                    егер neighborsWithFood.Length > 0 содан
                        AdjacentToFood
                    басегер emptyNeighborsWithP.Length > 0 && relativeDist (last emptyNeighborsWithP) > 0. содан
                        // remember emptyNeighborsWithP is sorted
                        AdjacentToPheromone (last emptyNeighborsWithP)
                    басқа
                        NoMansLand

                // The Actual logic...

                егер emptyNeighbors.IsEmpty содан
                    NullMove()
                басқа
                    сәйкестік me с
                    | ShouldDropFood -> DropFood nestCells.Head
                    | ReturnToNest ->
                        сәйкестік here с
                        | NeedsRefresh amt -> DropPheromone (here, amt)
                        | NoRefresh -> Move emptyNeighbors.Head
                    | Forage ->
                        сәйкестік me с
                        | AdjacentToFood -> TakeFood neighborsWithFood.Head
                        | AdjacentToPheromone pheroCell -> Move pheroCell
                        | NoMansLand -> RandomMove()

ашық Canvas
ашық Types
ашық World
ашық Simulation

болсын origin =
    // Sample is running ішінде an iframe, so get the location бастап parent
    болсын topLocation = window.top.location
    topLocation.origin + topLocation.pathname

болсын formatScoreCard bName bFood rName rFood =
    sprintf "Black (%s): %05d vs Red (%s): %05d" bName bFood rName rFood

болсын formatRemaining remaining =
    sprintf "Remaining Cycles: %05d" remaining


болсын maxCycles = 1000
болсын world = ref (buildWorldInitialWorld())
болсын foodToWin = int <| double (Map.fold (функ s k v -> s + v.Food) 0 world.Value) * percentFoodToWin
болсын cycles = ref 0

болсын blackAI = жаңа HardishAI.TestAntBehavior() :> IAntBehavior
болсын redAI = жаңа AntsEverywhereExmampleAI.TestAntBehavior() :> IAntBehavior

болсын render (w,h) =
    cycles.Value <- cycles.Value + 1

    болсын bScore = BlackAntNest.CountFood world.Value
    болсын rScore = RedAntNest.CountFood world.Value

    болсын remainig = maxCycles - cycles.Value

    болсын scoreString = formatScoreCard blackAI.Name bScore redAI.Name rScore
    updateInput "score" scoreString |> ignore

    болсын remainingString = formatRemaining remainig
    updateInput "secondline" remainingString |> ignore


    (0., 0., w, h) |> filled (rgb 200 200 200)
    drawUpdates (w,h) world.Value
    world.Value <- worldCycle blackAI redAI world.Value

    егер bScore > foodToWin || rScore > foodToWin || cycles.Value > maxCycles содан
        егер bScore > rScore содан Some blackAI.Name
        басегер rScore > bScore содан Some redAI.Name
        басқа None
    басқа None

болсын w, h = getWindowDimensions()

болсын rec update () =
    болсын result = render (w,h)
    сәйкестік result с
    | None ->
        window.setTimeout(update, 1000 / 30) |> ignore
    | Some winner ->
        updateInput "secondline" (sprintf "The winner is: %s" winner) |> ignore

update ()
