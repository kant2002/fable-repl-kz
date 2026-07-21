модуль Tour.TuplesAndRecords

// From https://docs.microsoft.com/en-us/dotnet/fsharp/tour
// Visit the link above үшін more information on each topic
// You can also find more learning resources at https://fsharp.org/

модуль Tuples =

    /// A simple tuple бастап integers.
    болсын tuple1 = (1, 2, 3)

    /// A функция that swaps the order бастап two values ішінде a tuple.
    ///
    /// F# Type Inference will automatically generalize the функция to have a generic түрі,
    /// meaning that it will work с any түрі.
    болсын swapElems (a, b) = (b, a)

    printfn "The result бастап swapping (1, 2) is %A" (swapElems (1,2))

    /// A tuple consisting бастап an integer, a string,
    /// and a double-precision floating point number.
    болсын tuple2 = (1, "fred", 3.1415)

    printfn "tuple1: %A\ttuple2: %A" tuple1 tuple2


модуль RecordTypes =

    /// This example shows how to define a жаңа record түрі.
    түрі ContactCard =
        { Name     : string
          Phone    : string
          Verified : bool }

    /// This example shows how to instantiate a record түрі.
    болсын contact1 =
        { Name = "Alf"
          Phone = "(206) 555-0157"
          Verified = false }

    /// You can also жасау this on the same line с ';' separators.
    болсын contactOnSameLine = { Name = "Alf"; Phone = "(206) 555-0157"; Verified = false }

    /// This example shows how to use "copy-and-update" on record values. It creates
    /// a жаңа record value that is a copy бастап contact1, but has different values үшін
    /// the 'Phone' and 'Verified' fields.
    ///
    /// To learn more, see: https://docs.microsoft.com/dotnet/fsharp/language-reference/copy-and-update-record-expressions
    болсын contact2 =
        { contact1 с
            Phone = "(206) 555-0112"
            Verified = true }

    /// This example shows how to write a функция that processes a record value.
    /// It converts a 'ContactCard' object to a string.
    болсын showContactCard (c: ContactCard) =
        c.Name + " Phone: " + c.Phone + (егер not c.Verified содан " (unverified)" басқа "")

    printfn "Alf's Contact Card: %s" (showContactCard contact1)

    /// This is an example бастап a Record с a мүшесі.
    түрі ContactCardAlternate =
        { Name     : string
          Phone    : string
          Address  : string
          Verified : bool }

        /// Members can implement object-oriented members.
        мүшесі this.PrintedContactCard =
            this.Name + " Phone: " + this.Phone + (егер not this.Verified содан " (unverified)" басқа "") + this.Address

    болсын contactAlternate =
        { Name = "Alf"
          Phone = "(206) 555-0157"
          Verified = false
          Address = "111 Alf Street" }

    // Members are accessed via the '.' operator on an instantiated түрі.
    printfn "Alf's alternate contact card is %s" contactAlternate.PrintedContactCard
