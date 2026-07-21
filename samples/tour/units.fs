модуль Tour.UnitsOfMeasure

// From https://docs.microsoft.com/en-us/dotnet/fsharp/tour
// Visit the link above үшін more information on each topic
// You can also find more learning resources at https://fsharp.org/

// Units бастап measure are a way to annotate primitive numeric types ішінде a түрі-safe way.
// You can содан perform түрі-safe arithmetic on these values.
//
// To learn more, see: https://docs.microsoft.com/dotnet/fsharp/language-reference/units-бастап-measure

// First, ашық a collection бастап common unit names
ашық Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

/// Define a unitized constant
болсын sampleValue1 = 1600.0<meter>

/// Next, define a жаңа unit түрі
[<Measure>]
түрі mile =
    /// Conversion factor mile to meter.
    статикалық мүшесі asMeter = 1609.34<meter/mile>

/// Define a unitized constant
болсын sampleValue2 = 500.0<mile>

/// Compute  metric-system constant
болсын sampleValue3 = sampleValue2 * mile.asMeter

// Values using Units бастап Measure can be used just like the primitive numeric түрі үшін things like printing.
printfn "After a %f race I would walk %f miles which would be %f meters" sampleValue1 sampleValue2 sampleValue3
