open FParsec
open IxMilia.Step.SchemaParser

[<EntryPoint>]
let main argv =
    let schemaText = System.IO.File.ReadAllText(@"Schemas\ap201.exp")
    let parser = SchemaParser.parser
    match run parser schemaText with
    | Success(result, _, _) -> printfn "File parsed successfully"
    | Failure(errorMessage, parserState, _) -> printfn "Parse failed at [%d, %d]: %s" parserState.Position.Line parserState.Position.Column errorMessage
    0
