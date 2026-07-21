модуль Tour.Functions

// From https://docs.microsoft.com/en-us/dotnet/fsharp/tour
// Visit the link above үшін more information on each topic
// You can also find more learning resources at https://fsharp.org/

модуль НегізгіФункциялар =

    /// You use 'болсын' to define a функция. This one accepts an integer argument and returns an integer.
    /// Parentheses are optional үшін функция arguments, except үшін when you use an explicit түрі annotation.
    болсын үлгіФункция1 x = x*x + 3

    /// Apply the функция, naming the функция return result using 'болсын'.
    /// The variable түрі is inferred from the функция return түрі.
    болсын нәтиже1 = үлгіФункция1 4573

    // This line uses '%d' to print the result as an integer. This is түрі-safe.
    // If 'нәтиже1' were not бастап түрі 'int', содан the line would fail to compile.
    printfn "4573 бүтін санын квадраттап, 3-ті қосқанда %d шығады" нәтиже1

    /// When needed, annotate the түрі бастап a parameter name using '(argument:түрі)'.  Parentheses are required.
    болсын үлгіФункция2 (x:int) = 2*x*x - x/5 + 3

    болсын нәтиже2 = үлгіФункция2 (7 + 4)
    printfn "(7 + 4) 2 таңдау функциясын қолдану нәтижесі %d болады" нәтиже2

    /// Conditionals use егер/содан/басегер/басқа.
    ///
    /// Note that F# uses white space indentation-aware syntax, similar to languages like Python.
    болсын үлгіФункци3 x =
        егер x < 100.0 содан
            2.0*x*x - x/5.0 + 3.0
        басқа
            2.0*x*x + x/5.0 - 37.0

    болсын нәтиже3 = үлгіФункци3 (6.5 + 4.5)

    // This line uses '%f' to print the result as a float.  As с '%d' above, this is түрі-safe.
    printfn "The result бастап applying the 3rd sample функция to (6.5 + 4.5) is %f" нәтиже3


модуль Immutability =

    /// Binding a value to a name via 'болсын' makes it immutable.
    ///
    /// The second line бастап code fails to compile because 'number' is immutable and bound.
    /// Re-defining 'number' to be a different value is not allowed ішінде F#.
    болсын number = 2
    // болсын number = 3

    /// A mutable binding.  This is required to be able to mutate the value бастап 'otherNumber'.
    болсын mutable otherNumber = 2

    printfn "'otherNumber' is %d" otherNumber

    // When mutating a value, use '<-' to assign a жаңа value.
    //
    // Note that '=' is not the same as '<-'.  '=' is used to test equality.
    otherNumber <- otherNumber + 1

    printfn "'otherNumber' changed to be %d" otherNumber


модуль PipelinesAndComposition =

    /// Squares a value.
    болсын square x = x * x

    /// Adds 1 to a value.
    болсын addOne x = x + 1

    /// Tests егер an integer value is odd via modulo.
    болсын isOdd x = x % 2 <> 0

    /// A list бастап 5 numbers.  More on lists later.
    болсын numbers = [ 1; 2; 3; 4; 5 ]

    /// Given a list бастап integers, it filters out the even numbers,
    /// squares the resulting odds, and adds 1 to the squared odds.
    болсын squareOddValuesAndAddOne values =
        болсын odds = List.filter isOdd values
        болсын squares = List.map square odds
        болсын result = List.map addOne squares
        result

    printfn "processing %A through 'squareOddValuesAndAddOne' produces: %A" numbers (squareOddValuesAndAddOne numbers)

    /// A shorter way to write 'squareOddValuesAndAddOne' is to nest each
    /// sub-result into the функция calls themselves.
    ///
    /// This makes the функция much shorter, but it's difficult to see the
    /// order ішінде which the data is processed.
    болсын squareOddValuesAndAddOneNested values =
        List.map addOne (List.map square (List.filter isOdd values))

    printfn "processing %A through 'squareOddValuesAndAddOneNested' produces: %A" numbers (squareOddValuesAndAddOneNested numbers)

    /// A preferred way to write 'squareOddValuesAndAddOne' is to use F# pipe operators.
    /// This allows you to avoid creating intermediate results, but is much more readable
    /// than nesting функция calls like 'squareOddValuesAndAddOneNested'
    болсын squareOddValuesAndAddOnePipeline values =
        values
        |> List.filter isOdd
        |> List.map square
        |> List.map addOne

    printfn "processing %A through 'squareOddValuesAndAddOnePipeline' produces: %A" numbers (squareOddValuesAndAddOnePipeline numbers)

    /// You can shorten 'squareOddValuesAndAddOnePipeline' by moving the second `List.map` call
    /// into the first, using a Lambda Function.
    ///
    /// Note that pipelines are also being used inside the lambda функция.  F# pipe operators
    /// can be used үшін single values as well.  This makes them very powerful үшін processing data.
    болсын squareOddValuesAndAddOneShorterPipeline values =
        values
        |> List.filter isOdd
        |> List.map(функ x -> x |> square |> addOne)

    printfn "processing %A through 'squareOddValuesAndAddOneShorterPipeline' produces: %A" numbers (squareOddValuesAndAddOneShorterPipeline numbers)


модуль RecursiveFunctions =

    /// This example shows a recursive функция that computes the factorial бастап an
    /// integer. It uses 'болсын rec' to define a recursive функция.
    болсын rec factorial n =
        егер n = 0 содан 1 басқа n * factorial (n-1)

    printfn "Factorial бастап 6 is: %d" (factorial 6)

    /// Computes the greatest common factor бастап two integers.
    ///
    /// Since all бастап the recursive calls are tail calls,
    /// the compiler will turn the функция into a loop,
    /// which improves performance and reduces memory consumption.
    болсын rec greatestCommonFactor a b =
        егер a = 0 содан b
        басегер a < b содан greatestCommonFactor a (b - a)
        басқа greatestCommonFactor (a - b) b

    printfn "The Greatest Common Factor бастап 300 and 620 is %d" (greatestCommonFactor 300 620)

    /// This example computes the sum бастап a list бастап integers using recursion.
    болсын rec sumList xs =
        сәйкестік xs с
        | []    -> 0
        | y::ys -> y + sumList ys

    /// This makes 'sumList' tail recursive, using a helper функция с a result accumulator.
    болсын rec жеке sumListTailRecHelper accumulator xs =
        сәйкестік xs с
        | []    -> accumulator
        | y::ys -> sumListTailRecHelper (accumulator+y) ys

    /// This invokes the tail recursive helper функция, providing '0' as a seed accumulator.
    /// An approach like this is common ішінде F#.
    болсын sumListTailRecursive xs = sumListTailRecHelper 0 xs

    болсын oneThroughTen = [1; 2; 3; 4; 5; 6; 7; 8; 9; 10]

    printfn "The sum 1-10 is %d" (sumListTailRecursive oneThroughTen)
