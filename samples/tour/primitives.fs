модуль Tour.Primitives

// From https://docs.microsoft.com/en-us/dotnet/fsharp/tour
// Visit the link above үшін more information on each topic
// You can also find more learning resources at https://fsharp.org/

модуль IntegersAndNumbers =

    /// This is a sample integer.
    болсын sampleInteger = 176

    /// This is a sample floating point number.
    болсын sampleDouble = 4.1

    /// This computed a жаңа number by some arithmetic.  Numeric types are converted using
    /// functions 'int', 'double' and so on.
    болсын sampleInteger2 = (sampleInteger/4 + 5 - 7) * 4 + int sampleDouble

    /// This is a list бастап the numbers from 0 to 99.
    болсын sampleNumbers = [ 0 .. 99 ]

    /// This is a list бастап all tuples containing all the numbers from 0 to 99 and their squares.
    болсын sampleTableOfSquares = [ үшін i ішінде 0 .. 99 -> (i, i*i) ]

    // The next line prints a list that includes tuples, using '%A' үшін generic printing.
    printfn $"The table бастап squares from 0 to 99 is:\n{sampleTableOfSquares}"


модуль Booleans =

    /// Booleans values are 'true' and 'false'.
    болсын boolean1 = true
    болсын boolean2 = false

    /// Operators on booleans are 'not', '&&' and '||'.
    болсын boolean3 = not boolean1 && (boolean2 || false)

    // This line uses '%b'to print a boolean value.  This is түрі-safe.
    printfn $"The expression 'not boolean1 && (boolean2 || false)' is %b{boolean3}"


модуль StringManipulation =

    /// Strings use double quotes.
    болсын string1 = "Hello"
    болсын string2  = "world"

    /// Strings can also use @ to create a verbatim string literal.
    /// This will ignore escape characters such as '\', '\n', '\t', etc.
    болсын string3 = @"C:\Program Files\"

    /// String literals can also use triple-quotes.
    болсын string4 = """The computer said "hello world" when I told it to!"""

    /// String concatenation is normally done с the '+' operator.
    болсын helloWorld = string1 + " " + string2

    // This line uses '%s' to print a string value.  This is түрі-safe.
    printfn "%s" helloWorld

    /// Substrings use the indexer notation.  This line extracts the first 7 characters as a substring.
    /// Note that like many languages, Strings are zero-indexed ішінде F#.
    болсын substring = helloWorld.[0..6]
    printfn "%s" substring
