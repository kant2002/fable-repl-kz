модуль Tour.Unions

// From https://docs.microsoft.com/en-us/dotnet/fsharp/tour
// Visit the link above үшін more information on each topic
// You can also find more learning resources at https://fsharp.org/

модуль DiscriminatedUnions =

    /// The following represents the suit бастап a playing card.
    түрі Suit =
        | Hearts
        | Clubs
        | Diamonds
        | Spades

    /// A Discriminated Union can also be used to represent the rank бастап a playing card.
    түрі Rank =
        /// Represents the rank бастап cards 2 .. 10
        | Value бастап int
        | Ace
        | King
        | Queen
        | Jack

        /// Discriminated Unions can also implement object-oriented members.
        статикалық мүшесі GetAllRanks() =
            [ yield Ace
              үшін i ішінде 2 .. 10 жасау yield Value i
              yield Jack
              yield Queen
              yield King ]

    /// This is a record түрі that combines a Suit and a Rank.
    /// It's common to use both Records and Discriminated Unions when representing data.
    түрі Card = { Suit: Suit; Rank: Rank }

    /// This computes a list representing all the cards ішінде the deck.
    болсын fullDeck =
        [ үшін suit ішінде [ Hearts; Diamonds; Clubs; Spades] жасау
              үшін rank ішінде Rank.GetAllRanks() жасау
                  yield { Suit=suit; Rank=rank } ]

    /// This example converts a 'Card' object to a string.
    болсын showPlayingCard (c: Card) =
        болсын rankString =
            сәйкестік c.Rank с
            | Ace -> "Ace"
            | King -> "King"
            | Queen -> "Queen"
            | Jack -> "Jack"
            | Value n -> string n
        болсын suitString =
            сәйкестік c.Suit с
            | Clubs -> "clubs"
            | Diamonds -> "diamonds"
            | Spades -> "spades"
            | Hearts -> "hearts"
        rankString  + " бастап " + suitString

    /// This example prints all the cards ішінде a playing deck.
    болсын printAllCards() =
        үшін card ішінде fullDeck жасау
            printfn "%s" (showPlayingCard card)


    // Single-case DUs are often used үшін domain modeling.  This can buy you extra түрі safety
    // over primitive types such as strings and ints.
    //
    // Single-case DUs cannot be implicitly converted to or from the түрі they wrap.
    // For example, a функция which takes ішінде an Address cannot accept a string as that input,
    // or vice versa.
    түрі Address = Address бастап string
    түрі Name = Name бастап string
    түрі SSN = SSN бастап int

    // You can easily instantiate a single-case DU as follows.
    болсын address = Address "111 Alf Way"
    болсын name = Name "Alf"
    болсын ssn = SSN 1234567890

    /// When you need the value, you can unwrap the underlying value с a simple функция.
    болсын unwrapAddress (Address a) = a
    болсын unwrapName (Name n) = n
    болсын unwrapSSN (SSN s) = s

    // Printing single-case DUs is simple с unwrapping functions.
    printfn "Address: %s, Name: %s, and SSN: %d" (address |> unwrapAddress) (name |> unwrapName) (ssn |> unwrapSSN)


    /// Discriminated Unions also support recursive definitions.
    ///
    /// This represents a Binary Search Tree, с one case being the Empty tree,
    /// and the other being a Node с a value and two subtrees.
    түрі BST<'T> =
        | Empty
        | Node бастап value:'T * left: BST<'T> * right: BST<'T>

    /// Check егер an item exists ішінде the binary search tree.
    /// Searches recursively using Pattern Matching.  Returns true егер it exists; otherwise, false.
    болсын rec exists item bst =
        сәйкестік bst с
        | Empty -> false
        | Node (x, left, right) ->
            егер item = x содан true
            басегер item < x содан (exists item left) // Check the left subtree.
            басқа (exists item right) // Check the right subtree.

    /// Inserts an item ішінде the Binary Search Tree.
    /// Finds the place to insert recursively using Pattern Matching, содан inserts a жаңа node.
    /// If the item is already present, it does not insert anything.
    болсын rec insert item bst =
        сәйкестік bst с
        | Empty -> Node(item, Empty, Empty)
        | Node(x, left, right) as node ->
            егер item = x содан node // No need to insert, it already exists; return the node.
            басегер item < x содан Node(x, insert item left, right) // Call into left subtree.
            басқа Node(x, left, insert item right) // Call into right subtree.


модуль PatternMatching =
    ашық System

    /// A record үшін a person's first and last name
    түрі Person = {
        First : string
        Last  : string
    }

    /// A Discriminated Union бастап 3 different kinds бастап employees
    түрі Employee =
        | Engineer бастап engineer: Person
        | Manager бастап manager: Person * reports: List<Employee>
        | Executive бастап executive: Person * reports: List<Employee> * assistant: Employee

    /// Count everyone underneath the employee ішінде the management hierarchy,
    /// including the employee. The matches bind names to the properties
    /// бастап the cases so that those names can be used inside the сәйкестік branches.
    /// Note that the names used үшін binding жасау not need to be the same as the
    /// names given ішінде the DU definition above.
    болсын rec countReports(emp : Employee) =
        1 + сәйкестік emp с
            | Engineer(person) ->
                0
            | Manager(person, reports) ->
                reports |> List.sumBy countReports
            | Executive(person, reports, assistant) ->
                (reports |> List.sumBy countReports) + countReports assistant


    /// Find all managers/executives named "Dave" who жасау not have any reports.
    /// This uses the 'функция' shorthand to as a lambda expression.
    болсын rec findDaveWithOpenPosition(emps : List<Employee>) =
        emps
        |> List.filter(функция
                       | Manager({First = "Dave"}, []) -> true // [] matches an empty list.
                       | Executive({First = "Dave"}, [], _) -> true
                       | _ -> false) // '_' is a wildcard pattern that matches anything.
                                     // This handles the "or басқа" case.


    /// You can also use the shorthand функция construct үшін pattern matching,
    /// which is useful when you're writing functions which make use бастап Partial Application.
    болсын жеке parseHelper f = f >> функция
        | (true, item) -> Some item
        | (false, _) -> None

    болсын parseDateTimeOffset: string -> _ = parseHelper DateTimeOffset.TryParse

    болсын result = parseDateTimeOffset "1970-01-01"
    сәйкестік result с
    | Some dto -> printfn "It parsed!"
    | None -> printfn "It didn't parse!"

    // Define some more functions which parse с the helper функция.
    болсын parseInt: string -> _  = parseHelper Int32.TryParse
    болсын parseDouble: string -> _  = parseHelper Double.TryParse
    болсын parseTimeSpan: string -> _  = parseHelper TimeSpan.TryParse


    // Active Patterns are another powerful construct to use с pattern matching.
    // They allow you to partition input data into custom forms, decomposing them at the pattern сәйкестік call site.
    //
    // To learn more, see: https://docs.microsoft.com/dotnet/fsharp/language-reference/active-patterns
    болсын (|Int|_|) = parseInt
    болсын (|Double|_|) = parseDouble
    болсын (|Date|_|) = parseDateTimeOffset
    болсын (|TimeSpan|_|) = parseTimeSpan

    /// Pattern Matching via 'функция' keyword and Active Patterns often looks like this.
    болсын printParseResult = функция
        | Int x -> printfn "%d" x
        | Double x -> printfn "%f" x
        | Date d -> printfn "%s" (d.ToString())
        | TimeSpan t -> printfn "%s" (t.ToString())
        | _ -> printfn "Nothing was parse-able!"

    // Call the printer с some different values to parse.
    printParseResult "12"
    printParseResult "12.045"
    printParseResult "12/28/2016"
    printParseResult "9:01PM"
    printParseResult "banana!"


модуль OptionValues =
    /// Option values are any kind бастап value tagged с either 'Some' or 'None'.
    /// They are used extensively ішінде F# code to represent the cases where many other
    /// languages would use null references.
    ///
    /// To learn more, see: https://docs.microsoft.com/dotnet/fsharp/language-reference/options

    /// First, define a zip code defined via Single-case Discriminated Union.
    түрі ZipCode = ZipCode бастап string

    /// Next, define a түрі where the ZipCode is optional.
    түрі Customer = { ZipCode: ZipCode option }

    /// Next, define an interface түрі the represents an object to compute the shipping zone үшін the customer's zip code,
    /// given implementations үшін the 'getState' and 'getShippingZone' abstract methods.
    түрі IShippingCalculator =
        abstract GetState : ZipCode -> string option
        abstract GetShippingZone : string -> int

    /// Next, calculate a shipping zone үшін a customer using a calculator instance.
    /// This uses combinators ішінде the Option модуль to allow a functional pipeline үшін
    /// transforming data с Optionals.
    болсын CustomerShippingZone (calculator: IShippingCalculator, customer: Customer) =
        customer.ZipCode
        |> Option.bind calculator.GetState
        |> Option.map calculator.GetShippingZone
