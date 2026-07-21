модуль Tour.Classes

// From https://docs.microsoft.com/en-us/dotnet/fsharp/tour
// Visit the link above үшін more information on each topic
// You can also find more learning resources at https://fsharp.org/

/// Classes are a way бастап defining жаңа object types ішінде F#, and support standard Object-oriented constructs.
/// They can have a variety бастап members (methods, properties, events, etc.)
///
/// To learn more about Classes, see: https://docs.microsoft.com/dotnet/fsharp/language-reference/classes
///
/// To learn more about Members, see: https://docs.microsoft.com/dotnet/fsharp/language-reference/members

/// A simple two-dimensional Vector class.
///
/// The class's constructor is on the first line,
/// and takes two arguments: dx and dy, both бастап түрі 'double'.
түрі Vector2D(dx : double, dy : double) =

    /// This internal field stores the length бастап the vector, computed when the
    /// object is constructed
    болсын length = sqrt (dx*dx + dy*dy)

    // 'this' specifies a name үшін the object's self-identifier.
    // In instance methods, it must appear before the мүшесі name.
    мүшесі this.DX = dx

    мүшесі this.DY = dy

    мүшесі this.Length = length

    /// This мүшесі is a method.  The previous members were properties.
    мүшесі this.Scale(k) = Vector2D(k * this.DX, k * this.DY)

/// This is how you instantiate the Vector2D class.
болсын vector1 = Vector2D(3.0, 4.0)

/// Get a жаңа scaled vector object, without modifying the original object.
болсын vector2 = vector1.Scale(10.0)

printfn "Length бастап vector1: %f\nLength бастап vector2: %f" vector1.Length vector2.Length


/// Generic classes allow types to be defined с respect to a set бастап түрі parameters.
/// In the following, 'T is the түрі parameter үшін the class.
///
/// To learn more, see: https://docs.microsoft.com/dotnet/fsharp/language-reference/generics/

түрі StateTracker<'T>(initialElement: 'T) =

    /// This internal field store the states ішінде a list.
    болсын mutable states = [ initialElement ]

    /// Add a жаңа element to the list бастап states.
    мүшесі this.UpdateState newState =
        states <- newState :: states  // use the '<-' operator to mutate the value.

    /// Get the entire list бастап historical states.
    мүшесі this.History = states

    /// Get the latest state.
    мүшесі this.Current = states.Head

/// An 'int' instance бастап the state tracker class. Note that the түрі parameter is inferred.
болсын tracker = StateTracker 10

// Add a state
tracker.UpdateState 17


/// Interfaces are object types с only 'abstract' members.
/// Object types and object expressions can implement interfaces.
///
/// To learn more, see: https://docs.microsoft.com/dotnet/fsharp/language-reference/interfaces

/// This is a түрі that implements IDisposable.
түрі ReadFile(path: string) =
    мүшесі this.ReadLine() = printfn "Reading %s..." path

    // This is the implementation бастап IDisposable members.
    interface System.IDisposable с
        мүшесі this.Dispose() = printfn "Closing %s..." path


/// This is an object that implements IDisposable via an Object Expression
/// Unlike other languages such as C# or Java, a жаңа түрі definition is not needed
/// to implement an interface.
болсын interfaceImplementation =
    { жаңа System.IDisposable с
        мүшесі this.Dispose() = printfn "disposed" }
