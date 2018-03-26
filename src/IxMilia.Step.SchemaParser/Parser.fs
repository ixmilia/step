// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.SchemaParser

open System
open FParsec

module SchemaParser =
    let parser : Parser<Schema, unit> =
        let str = pstringCI
        let pcomment = str "--" >>. many1Satisfy ((<>) '\n')
        let pspaces = spaces >>. many (spaces >>. pcomment >>. spaces)
        let pmlcomment = pstring "(*" >>. skipCharsTillString "*)" true System.Int32.MaxValue
        let ws = pspaces >>. many (pspaces >>. pmlcomment >>. pspaces) |>> ignore
        let str_ws s = str s .>> ws

        // using grammar from https://github.com/dustintownsend/EXPRESS-Modeling-Language-References/blob/master/express.bnf
        let ABS = str_ws "abs"
        let ABSTRACT = str_ws "abstract"
        let ACOS = str_ws "acos"
        let AGGREGATE = str_ws "aggregate"
        let ALIAS = str_ws "alias"
        let AND = str_ws "and"
        let ANDOR = str_ws "andor"
        let ARRAY = str_ws "array"
        let AS = str_ws "as"
        let ASIN = str_ws "asin"
        let ATAN = str_ws "atan"
        let BAG = str_ws "bag"
        let BEGIN = str_ws "begin"
        let BINARY = str_ws "binary"
        let BLENGTH = str_ws "blength"
        let BOOLEAN = str_ws "boolean"
        let BY = str_ws "by"
        let CASE = str_ws "case"
        let CONSTANT = str_ws "constant"
        let CONST_E = str_ws "const_e"
        let CONTEXT = str_ws "context"
        let COS = str_ws "cos"
        let DERIVE = str_ws "derive"
        let DIV = str_ws "div"
        let ELSE = str_ws "else"
        let END = str_ws "end"
        let END_ALIAS = str_ws "end_alias"
        let END_CASE = str_ws "end_case"
        let END_CONSTANT = str_ws "end_constant"
        let END_CONTEXT = str_ws "end_context"
        let END_ENTITY = str_ws "end_entity"
        let END_FUNCTION = str_ws "end_function"
        let END_IF = str_ws "end_if"
        let END_LOCAL = str_ws "end_local"
        let END_MODEL = str_ws "end_model"
        let END_PROCEDURE = str_ws "end_procedure"
        let END_REPEAT = str_ws "end_repeat"
        let END_RULE = str_ws "end_rule"
        let END_SCHEMA = str_ws "end_schema"
        let END_TYPE = str_ws "end_type"
        let ENTITY = str_ws "entity"
        let ENUMERATION = str_ws "enumeration"
        let ESCAPE = str_ws "escape"
        let EXISTS = str_ws "exists"
        let EXP = str_ws "exp"
        let FALSE = str_ws "false"
        let FIXED = str_ws "fixed"
        let FOR = str_ws "for"
        let FORMAT = str_ws "format"
        let FROM = str_ws "from"
        let FUNCTION = str_ws "function"
        let GENERIC = str_ws "generic"
        let HIBOUND = str_ws "hibound"
        let HIINDEX = str_ws "hiindex"
        let IF = str_ws "if"
        let IN = str_ws "in"
        let INSERT = str_ws "insert"
        let INTEGER = str_ws "integer"
        let INVERSE = str_ws "inverse"
        let LENGTH = str_ws "length"
        let LIKE = str_ws "like"
        let LIST = str_ws "list"
        let LOBOUND = str_ws "lobound"
        let LOCAL = str_ws "local"
        let LOG = str_ws "log"
        let LOG10 = str_ws "log10"
        let LOG2 = str_ws "log2"
        let LOGICAL = str_ws "logical"
        let LOINDEX = str_ws "loindex"
        let MOD = str_ws "mod"
        let MODEL = str_ws "model"
        let NOT = str_ws "not"
        let NUMBER = str_ws "number"
        let NVL = str_ws "nvl"
        let ODD = str_ws "odd"
        let OF = str_ws "of"
        let ONEOF = str_ws "oneof"
        let OPTIONAL = str_ws "optional"
        let OR = str_ws "or"
        let OTHERWISE = str_ws "otherwise"
        let PI = str_ws "pi"
        let PROCEDURE = str_ws "procedure"
        let QUERY = str_ws "query"
        let REAL = str_ws "real"
        let REFERENCE = str_ws "reference"
        let REMOVE = str_ws "remove"
        let REPEAT = str_ws "repeat"
        let RETURN = str_ws "return"
        let ROLESOF = str_ws "rolesof"
        let RULE = str_ws "rule"
        let SCHEMA = str_ws "schema"
        let SELECT = str_ws "select"
        let SELF = str_ws "self"
        let SET = str_ws "set"
        let SIN = str_ws "sin"
        let SIZEOF = str_ws "sizeof"
        let SKIP = str_ws "skip"
        let SQRT = str_ws "sqrt"
        let STRING = str_ws "string"
        let SUBTYPE = str_ws "subtype"
        let SUPERTYPE = str_ws "supertype"
        let TAN = str_ws "tan"
        let THEN = str_ws "then"
        let TO = str_ws "to"
        let TRUE = str_ws "true"
        let TYPE = str_ws "type"
        let TYPEOF = str_ws "typeof"
        let UNIQUE = str_ws "unique"
        let UNKNOWN = str_ws "unknown"
        let UNTIL = str_ws "until"
        let USE = str_ws "use"
        let USEDIN = str_ws "usedin"
        let VALUE = str_ws "value"
        let VALUE_IN = str_ws "value_in"
        let VALUE_UNIQUE = str_ws "value_unique"
        let VAR = str_ws "var"
        let WHERE = str_ws "where"
        let WHILE = str_ws "while"
        let XOR = str_ws "xor"

        let PERCENT = str_ws "%"
        let PLUS = str_ws "+"
        let MINUS = str_ws "-"
        let SEMI = str_ws ";"
        let COLON = str_ws ":"
        let COLON_EQUALS = str_ws ":="
        let COMMA = str_ws ","
        let DOUBLE_QUOTE = pchar '"'
        let EQUALS = str_ws "="
        let LEFT_PAREN = str_ws "("
        let RIGHT_PAREN = str_ws ")"

        let hex_digit = hex
        let octet = hex_digit .>>. hex_digit |>> (fun (a, b) -> ((int a) <<< 4) + int b) |>> byte
        let encoded_character = tuple4 octet octet octet octet |>> (fun (a, b, c, d) -> (int a <<< 24) + (int b <<< 16) + (int c <<< 8) + int d) |>> char

        let simple_id = many1Satisfy2 isLetter (fun c -> isLetter c || isDigit c || c = '_') .>> ws
        let not_paren_star_quote_special = anyOf ['!'; '#'; '$'; '%'; '&'; '+'; ','; '-'; '.'; '/'; ':'; ';'; '<'; '='; '>'; '?'; '@'; '\\'; '^'; '_'; '\''; '{'; '|'; '}'; '~'; '['; ']'; ' ']
        let not_quote : Parser<char, unit> = not_paren_star_quote_special <|> letter <|> digit <|> anyOf ['('; ')'; '*']
        let char_list_to_string (chars: char list) = String.Join(String.Empty, chars)
        let encoded_string_literal = DOUBLE_QUOTE >>. many encoded_character .>> DOUBLE_QUOTE .>> ws |>> char_list_to_string
        let simple_string_literal = // \q { ( \q \q ); not_quote; \s; \o } \q .
            DOUBLE_QUOTE >>. many (stringReturn "\"\"" '"' <|> not_quote) .>> DOUBLE_QUOTE
            |>> char_list_to_string
        let string_literal = simple_string_literal <|> encoded_string_literal .>> ws |>> StringLiteral
        let schema_version_id = string_literal |>> function StringLiteral s -> s | _ -> failwith "unreachable"

        let attribute_id = simple_id
        let schema_id = simple_id
        let schema_ref = schema_id
        let entity_id = simple_id
        let function_id = simple_id
        let procedure_id = simple_id
        let type_id = simple_id
        let rename_id = entity_id <|> function_id <|> procedure_id <|> type_id
        let constant_id = simple_id
        let constant_ref = constant_id
        let entity_ref = entity_id
        let function_ref = function_id
        let procedure_ref = procedure_id
        let type_ref = type_id
        let resource_ref = constant_ref <|> entity_ref <|> function_ref <|> procedure_ref <|> type_ref
        let resource_or_rename = resource_ref .>>. opt (AS >>. rename_id) |>> Resource
        let reference_clause = REFERENCE >>. FROM >>. schema_ref .>>. opt ( LEFT_PAREN >>. sepBy1 resource_or_rename COMMA .>> RIGHT_PAREN) .>> SEMI |>> ReferenceClause
        let interface_specification = reference_clause //<|> use_clause

        // expressions
        let bit = str_ws "0" <|> str_ws "1"
        let binary_literal =
            PERCENT >>. many bit
            |>> List.fold (fun acc bit -> (acc * 2L) + (if bit = "1" then 1L else 0L)) 0L
            |>> IntegerLiteral
        let sign = PLUS <|> MINUS
        let integer_literal =
            opt sign .>>. pint64 .>> ws
            |>> (function
                 | Some "+", value -> value
                 | Some "-", value -> -value
                 | _, value -> value)
            |>> IntegerLiteral
        let logical_literal = (FALSE |>> (fun _ -> LogicalLiteral(Some false))) <|> (TRUE |>> (fun _ -> LogicalLiteral(Some true))) <|> (UNKNOWN |>> (fun _ -> LogicalLiteral(None)))
        let real_literal =
            opt sign .>>. pfloat .>> ws
            |>> (function
                 | Some "+", value -> value
                 | Some "-", value -> -value
                 | _, value -> value)
            |>> RealLiteral
        let literal = binary_literal <|> integer_literal <|> logical_literal <|> real_literal <|> string_literal
        let opp = new OperatorPrecedenceParser<Expression, unit, unit>()
        let expr = opp.ExpressionParser
        opp.TermParser <- (literal |>> LiteralValue) <|> (simple_id |>> AttributeName) <|> between LEFT_PAREN RIGHT_PAREN expr
        opp.AddOperator(InfixOperator("+", ws, 1, Associativity.Left, (fun a b -> Add(a, b))))
        opp.AddOperator(InfixOperator("-", ws, 1, Associativity.Left, (fun a b -> Subtract(a, b))))
        opp.AddOperator(InfixOperator("or", ws, 1, Associativity.Left, (fun a b -> Or(a, b))))
        opp.AddOperator(InfixOperator("xor", ws, 1, Associativity.Left, (fun a b -> Xor(a, b))))
        opp.AddOperator(InfixOperator("*", ws, 2, Associativity.Left, (fun a b -> Multiply(a, b))))
        opp.AddOperator(InfixOperator("/", ws, 2, Associativity.Left, (fun a b -> Divide(a, b))))
        opp.AddOperator(InfixOperator("%", ws, 2, Associativity.Left, (fun a b -> Modulus(a, b))))
        opp.AddOperator(InfixOperator("and", ws, 2, Associativity.Left, (fun a b -> And(a, b))))
        opp.AddOperator(InfixOperator("**", ws, 3, Associativity.Right, (fun a b -> Exponent(a, b))))
        opp.AddOperator(PrefixOperator("-", ws, 4, true, Negate))
        //let add_like_op = PLUS <|> MINUS <|> OR <|> XOR
        //let multiplication_like_op = ASTERISK <|> SLASH <|> DIV <|> MOD <|> AND <|> DOUBLE_PIPE
        //let primary = literal // <|> qualifialble_factor { qualifier }
        //let simple_factor = primary // aggregate_initializer <|> entity_constructor <|> enumeration_reference <|> interval <|> query_expression <|>( [ NOT ] '(' expression ')' ) | ( [ unary_op ] primary ) .
        //let factor = simple_factor // [ '**' simple_factor ]
        //let term = factor // { multiplication_like_op factor }
        //let simple_expression = term // { add_like_op term }
        //let expression = simple_expression // [ rel_op_extended simple_expression ]
        let expression = expr

        let binary_type =
            BINARY >>. opt (between LEFT_PAREN RIGHT_PAREN expression) .>>. (opt FIXED |>> Option.isSome)
            |>> BinaryType
        let real_type = REAL >>. opt (between LEFT_PAREN RIGHT_PAREN expression) |>> RealType
        let simple_types = binary_type <|> real_type |>> SimpleType // <|> binary ...
        let named_types = entity_ref <|> type_ref |>> NamedType
        let base_type = (* aggregation_types <|>*) simple_types <|> named_types
        let attribute_decl = attribute_id // <|> qualified_attribute
        let derive_attr =
            pipe3
                (attribute_decl .>> COLON)
                (base_type .>> COLON_EQUALS |>> (fun baseType -> AttributeType(baseType, false)))
                (expression .>> SEMI)
                (fun name typ expression -> DerivedAttribute(name, typ, expression))
        let derive_clause = DERIVE >>. many1 (attempt derive_attr)
        let explicit_attr =
            pipe4
                (attribute_decl)
                (COLON)
                (pipe2
                    (opt OPTIONAL |>> Option.isSome)
                    (base_type)
                    (fun isOptional baseType -> AttributeType(baseType, isOptional)))
                (SEMI)
                (fun name _ typ _ ->
                    ExplicitAttribute(name, typ))
        let entity_body =
            many (attempt explicit_attr) .>>. opt derive_clause // derive_clause inverse_clause unique_clause where_clause
        let entity_head =
            ENTITY >>. entity_id .>> SEMI
        let entity_decl =
            pipe3
                (entity_head)
                (entity_body)
                (END_ENTITY .>> SEMI)
                (fun name body _ ->
                    let attributes, derivedAttributes = body
                    let derivedAttributes = Option.defaultValue [] derivedAttributes
                    Entity(name, attributes, derivedAttributes))
            |>> EntityDeclaration

        let underlying_type = (*constructed_types <|> aggregation_types <|>*) simple_types <|> (type_ref |>> NamedType)
        let label = simple_id
        let domain_rule =
            pipe2
                (opt label .>> COLON)
                (expression)
                (fun label expression -> DomainRule(Option.defaultValue null label, expression))
        let where_clause = WHERE >>. many1 (domain_rule .>> SEMI)
        let type_decl =
            pipe3
                (TYPE >>. type_id .>> EQUALS)
                (underlying_type .>> SEMI)
                (opt (attempt where_clause) .>> END_TYPE .>> SEMI)
                (fun typeName baseType whereClause -> SchemaType(typeName, baseType, Option.defaultValue [] whereClause))
            |>> TypeDeclaration

        let declaration = entity_decl (* <|> function_decl <|> procedure_decl *) <|> type_decl

        let string_opt_to_nullable = function
            | Some(value) -> value
            | None -> null
        let schema_body =
            // { interface_specification } [ constant_decl ] { declaration | rule_decl } .
            pipe2
                (many interface_specification)
                (many declaration)
                (fun interfaces declarations ->
                    let entities =
                        declarations
                        |> List.choose (fun d -> match d with | EntityDeclaration e -> Some e | _ -> None)
                    let types =
                        declarations
                        |> List.choose (fun d -> match d with | TypeDeclaration t -> Some t | _ -> None)
                    SchemaBody(interfaces, entities, types))
        let schema_decl =
            // schema_decl = SCHEMA schema_id [ schema_version_id ] ';' schema_body END_SCHEMA ';' .
            pipe3
                (SCHEMA >>. schema_id)
                (opt schema_version_id |>> string_opt_to_nullable)
                (SEMI >>. schema_body .>> END_SCHEMA .>> SEMI)
                (fun a b c -> Schema(a, b, c))

        ws >>. schema_decl .>> eof
