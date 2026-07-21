// Undertone - Programmable music
// Ported from desktop version: https://github.com/robertpi/undertone
// Inspired by Overtone / Sonic-PI
// By Robert Pickering

модуль Undertone

ашық System
ашық Fable.Core
ашық Fable.Core.JsInterop
ашық Browser.Types
ашық Browser

түрі Note =
    | Cflat     = -1
    | C         = 0
    | Csharp    = 1
    | Dflat     = 1
    | D         = 2
    | Dsharp    = 3
    | Eflat     = 3
    | E         = 4
    | Fflat     = 4
    | Esharp    = 5
    | F         = 5
    | Fsharp    = 6
    | Gflat     = 6
    | G         = 7
    | Gsharp    = 8
    | Aflat     = 8
    | A         = 9
    | Asharp    = 10
    | Bflat     = 10
    | B         = 11
    | Bsharp    = 12

модуль MiscConsts =

    /// standard sampling rate
    /// See: http://en.wikipedia.org/wiki/44,100_Hz
    болсын SampleRate = 44100

    /// "Standard Pitch" noted as A440. The a note that is above middle c
    /// See: http://en.wikipedia.org/wiki/A440_(pitch_standard)
    болсын A440 = 440.

модуль Waves =
    ашық System

    /// The ratio require to move from one semi-tone to the next
    /// See: http://en.wikipedia.org/wiki/Semitone
    болсын жеке semitone =
        Math.Pow(2., 1. / 12.)

    /// Since our Note enum is relative to c, we need to find middle c.
    /// We know A440 = 440 hz and that the next c is three semi tones
    /// above that, but this is c one ocative above middle c, so we
    /// half the result to get middle c.
    /// Middle c is around 261.626 Hz, and this approximately the value we get
    /// See: http://en.wikipedia.org/wiki/C_(musical_note)
    болсын жеке middleC =
        MiscConsts.A440 * Math.Pow(semitone, 3.) / 2.

    /// Converts from our note enum to the notes frequency
    болсын frequencyOfNote (note: Note) octave =
        middleC *
        // calculate the ratio need to move to the note's semitone
        Math.Pow(semitone, double (int note)) *
        // calculate the ratio need to move to the note's octave
        Math.Pow(2., double (octave - 4))

    /// calculates the distance you need to move ішінде each sample
    болсын phaseAngleIncrementOfFrequency frequency =
        frequency / double MiscConsts.SampleRate

/// functions an constants үшін manipulating musical time
модуль Time =
    /// this hard codes our модуль to the lower "4" ішінде 4/4 time
    болсын beatsPerSemibreve = 4.
    /// number bars
    болсын жеке beatsPerSecond bmp =  60. / bmp
    /// number бастап samples required to make a bar бастап m- usic
    болсын жеке samplesPerBar bmp = (float MiscConsts.SampleRate * beatsPerSecond bmp * beatsPerSemibreve)

    /// longa - either twice or three times as long as a breve (we choose twice)
    /// it is no longer used ішінде modern music notation
    болсын longa = 4.
    /// double whole note -  twice as long as semibreve
    болсын breve = 2.
    /// whole note -  its length is equal to four beats ішінде 4/4 time
    /// most other notes are fractions бастап the whole note
    болсын semibreve = 1.
    /// half note
    болсын minim = 1. / 2.
    /// quarter note
    болсын crotchet = 1. / 4.
    /// eighth note
    болсын quaver = 1. / 8.
    /// sixteenth note
    болсын semiquaver = 1. / 16.
    /// thirty-second note
    болсын demisemiquaver = 1. / 32.

    /// caculates a note's length ішінде samples
    болсын noteValue bmp note =
        samplesPerBar bmp * note |> int

/// Functions үшін creating waves
модуль Creation =

    /// make a period бастап silence
    болсын makeSilence length =
        Seq.init length (функ _ -> 0.)

    /// make a wave using the given функция, length and frequency
    болсын makeWave waveFunc length frequency =
        болсын phaseAngleIncrement = Waves.phaseAngleIncrementOfFrequency frequency
        Seq.init length (функ x ->
            болсын phaseAngle = phaseAngleIncrement * (float x)
            болсын x = Math.Floor(phaseAngle)
            waveFunc (phaseAngle - x))

    /// make a wave using the given функция, length note and octave
    болсын makeNote waveFunc length note octave =
        болсын frequency = Waves.frequencyOfNote note octave
        makeWave waveFunc length frequency

    /// функция үшін making a sine wave
    болсын sine phaseAngle =
        Math.Sin(2. * Math.PI * phaseAngle)

    /// функция үшін making a square wave
    болсын square phaseAngle =
        егер phaseAngle < 0.5 содан -1.0 басқа 1.0

    /// функция үшін making triangular waves
    болсын triangle phaseAngle =
        егер phaseAngle < 0.5 содан
            2. * phaseAngle
        басқа
            1. - (2. * phaseAngle)

    // функция үшін making making "saw tooth" wave
    болсын sawtooth phaseAngle =
        -1. + phaseAngle

    // функция үшін combining several waves into a cord combines
    болсын makeCord (waveDefs: seq<seq<float>>) =
        болсын wavesMatrix = waveDefs |> Seq.map (Seq.toArray) |> Seq.toArray
        болсын waveScaleFactor = 1. / float wavesMatrix.Length
        болсын maxLength = wavesMatrix |> Seq.maxBy (функ x -> x.Length)
        болсын getValues i =
            seq { үшін x ішінде 0 .. wavesMatrix.Length - 1 жасау
                    yield егер i > wavesMatrix.[x].Length содан 0. басқа wavesMatrix.[x].[i] }
        seq { үшін x ішінде 0 .. maxLength.Length - 1 жасау yield (getValues x |> Seq.sum) * waveScaleFactor }

    // same as makeCord but does use arrays so can handle long or even infinite sequences.
    болсын combine (waveDefs: seq<seq<float>>) =
        болсын enumerators = waveDefs |> Seq.map (функ x -> x.GetEnumerator()) |> Seq.cache
        болсын loop () =
            болсын values =
                enumerators
                |> Seq.choose
                    (функ x -> егер x.MoveNext() содан Some x.Current басқа None)
                |> Seq.toList
            сәйкестік values с
            | [] -> None
            | x -> Some ((x |> Seq.sum), ())
        Seq.unfold loop ()

/// functions үшін transforming waves
модуль Transformation =
    /// makes the waves amplitude large or small by scaling by the given multiplier
    болсын scaleHeight multiplier (waveDef: seq<float>) =
        waveDef |> Seq.map (функ x -> x * multiplier)

    болсын жеке rnd = жаңа Random()

    /// Adds some noise to the wave (not recommended)
    болсын addNoise multiplier (waveDef: seq<float>) =
        waveDef
        |> Seq.map (функ x ->
                        болсын rndValue = 0.5 - rnd.NextDouble()
                        x +  (rndValue * multiplier))

    /// flattens the wave at the given limit to give an overdrive effect
    болсын flatten limit (waveDef: seq<float>) =
        waveDef
        |> Seq.map (функ x -> max -limit (min x limit))

    /// provides a way to linearly tapper a wave, the startMultiplier is
    /// applied to the first value бастап the a wave, and endMultiplier is
    /// applied to the last value, the other values have value that is linearly
    /// interpolated between the two values
    болсын tapper startMultiplier endMultiplier (waveDef: seq<float>) =
        болсын waveVector = waveDef |> Seq.toArray
        болсын step = (endMultiplier - startMultiplier) / float waveVector.Length
        waveVector
        |> Seq.mapi (функ i x -> x * (startMultiplier + (step * float i)))

    /// gets a point on the gaussian distribution
    болсын жеке gaussian a b c x  = Math.Pow((a * Math.E), -(Math.Pow(x - b, 2.) / Math.Pow(c * 2., 2.)))

    /// applies a gaussian tapper to the front бастап a wave
    болсын gaussianTapper length (waveDef: seq<float>) =
        болсын waveVector = waveDef |> Seq.toArray
        болсын step = 1. / float waveVector.Length
        waveVector
        |> Seq.mapi (функ i x -> x * gaussian 1. 0. length (step * float i))

    /// applies a gaussian tapper to the back бастап a wave
    болсын revGaussianTapper length (waveDef: seq<float>) =
        болсын waveVector = waveDef |> Seq.toArray
        болсын len = float waveVector.Length
        болсын step = 1. / len
        waveVector
        |> Seq.mapi (функ i x -> x * gaussian 1. 0. length (step * (len - float i)))

    /// applies a gaussian tapper to the front and back бастап a wave
    болсын doubleGaussianTapper startLength endLength (waveDef: seq<float>) =
        болсын waveVector = waveDef |> Seq.toArray
        болсын len = float waveVector.Length
        болсын step = 1. / len
        waveVector
        |> Seq.mapi (функ i x -> x *
                                (gaussian 1. 0. startLength (step * (len - float i))) *
                                (gaussian 1. 0. endLength (step * float i)))

/// Functions to turn a list бастап chords into a playable sound wave
модуль NoteSequencer =
    түрі Chord = seq<Note*int>

    /// version бастап Seq.take that doesn't though exceptions егер you reach the end бастап the sequence
    болсын жеке safeTake wanted (source : seq<'T>) =
        (* Note: don't create or dispose any IEnumerable егер n = 0 *)
        егер wanted = 0 содан Seq.empty басқа
        seq { use e = source.GetEnumerator()
              болсын count = ref 0
              while e.MoveNext() && count.Value < wanted жасау
                count.Value <- count.Value + 1
                yield e.Current }

    // функция that does a функция the describes how a note should be played and list бастап chords
    // and generates a sound wave from them
    болсын sequence (noteTable: Note -> int -> seq<float>) (notes: seq<#Chord*int>) =
        seq { үшін cordNotes, length ішінде notes жасау
                болсын notes = cordNotes |> Seq.map (функ (note, octave) -> noteTable note octave)
                yield! Creation.combine notes |> safeTake length }

модуль WaveFormat =
    болсын sampleRate = 44100
    болсын channels = 1

    болсын bytesOfInt16 i =
        [ 0; 8; ]
        |> List.map (функ shift -> (i >>> shift) &&& 0x00ffs |> byte)

    болсын bytesOfInt i =
        [ 0; 8; 16; 24 ]
        |> List.map (функ shift -> (i >>> shift) &&& 0x000000ff |> byte)

    болсын wavOfBuffer (buffer: float[]) =
        болсын sixteenBitLength = 2 * buffer.Length

        [| yield! "RIFF" |> Seq.map byte
           yield! bytesOfInt (sixteenBitLength + 15)
           yield! "WAVE" |> Seq.map byte
           yield! "fmt " |> Seq.map byte
           yield 0x12uy // fmt chunksize: 18
           yield 0x00uy
           yield 0x00uy //
           yield 0x00uy
           yield 0x01uy // format tag : 1
           yield 0x00uy
           yield channels |> byte // channels
           yield 0x00uy
           yield! bytesOfInt (sampleRate)
           yield! bytesOfInt (2*channels*sampleRate)
           yield 0x04uy // block align
           yield 0x00uy
           yield 0x10uy // bit per sample
           yield 0x00uy
           yield 0x00uy // cb size
           yield 0x00uy
           yield! "data" |> Seq.map byte
           yield! bytesOfInt sixteenBitLength
           үшін i ішінде [ 0 .. buffer.Length - 1 ] жасау
                болсын tmp = buffer.[i]
                егер (tmp >= 1.) содан
                    yield 0xFFuy
                    yield 0xFFuy
                басегер (tmp <= -1.) содан
                    yield 0x00uy
                    yield 0x00uy
                басқа
                    yield! Math.Round(tmp * float (Int16.MaxValue)) |> int16 |> bytesOfInt16 |]

модуль Svg =
    болсын svg = document.getElementById("svg")

    болсын displayWave (points: float[]) =
        болсын margin = 10.
        болсын lineSpacing = 1.
        болсын lineWidth = 1.

        болсын length = (svg.clientWidth / lineSpacing) |> int
        болсын midPoint = svg.clientHeight / 2.
        болсын maxLine = midPoint - margin

        болсын rnd = жаңа Random()

        болсын chunkSize = points.Length / length

        болсын samples =
            points
            |> Seq.map (функ x -> Math.Abs(x))
            |> Seq.chunkBySize chunkSize
            |> Seq.map Array.average
            |> Seq.toArray

        болсын svgns = "http://www.w3.org/2000/svg";
        үшін i ішінде 1 .. length жасау
            болсын size = samples.[i] * maxLine
            болсын y1 = midPoint - size
            болсын y2 = midPoint + size
            болсын line = document.createElementNS(svgns, "line");
            болсын x = float i * lineSpacing

            line.setAttributeNS(null, "x1", string x);
            line.setAttributeNS(null, "y1", string y1);
            line.setAttributeNS(null, "x2", string x);
            line.setAttributeNS(null, "y2", string y2);
            line.setAttributeNS(null, "stroke-width", string lineWidth);
            line.setAttributeNS(null, "stroke", "#000000");

            document.getElementById("svg").appendChild(line) |> ignore

модуль Html =
    болсын audio = document.getElementsByTagName("audio").[0] :?> HTMLAudioElement

    болсын loadSound (soundSequence: seq<float>) =
        болсын getBaseWav64 sound =
            болсын wav = WaveFormat.wavOfBuffer (sound |> Seq.toArray)
            Convert.ToBase64String(wav)

        болсын soundBuffer = soundSequence |> Seq.toArray

        болсын wavBase64 = getBaseWav64 soundBuffer
        audio.src <- "data:audio/wav;base64," + wavBase64

        Svg.displayWave soundBuffer


болсын bpm = 90.
болсын crotchet = Time.noteValue bpm Time.crotchet
болсын quaver = Time.noteValue bpm Time.quaver

болсын makeNote time note =
    Creation.makeNote Creation.sine time note 4
    |> Transformation.gaussianTapper 0.1

болсын baaBaaBlackSheepChorus =
    seq {
          //C C G G A A AA G
          //Baa baa black sheep have you any wool?
          yield! makeNote crotchet Note.C
          yield! makeNote crotchet Note.C
          yield! makeNote crotchet Note.G
          yield! makeNote crotchet Note.G
          yield! makeNote crotchet Note.A
          yield! makeNote crotchet Note.A
          yield! makeNote quaver Note.A
          yield! makeNote quaver Note.A
          yield! makeNote crotchet Note.G
          //F F E E D D C
          //Yes sir yes sir three bags full.
          yield! makeNote crotchet Note.F
          yield! makeNote crotchet Note.F
          yield! makeNote crotchet Note.E
          yield! makeNote crotchet Note.E
          yield! makeNote crotchet Note.D
          yield! makeNote crotchet Note.D
          yield! makeNote crotchet Note.C }

Html.loadSound baaBaaBlackSheepChorus
