// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using IxMilia.Step.Syntax;
using IxMilia.Step.Tokens;

namespace IxMilia.Step
{
    internal class StepLexer
    {
        private List<StepToken> _tokens;
        private int _offset = 0;

        public StepLexer(IEnumerable<StepToken> tokens)
        {
            _tokens = new List<StepToken>(tokens);
        }

        private bool TokensRemain()
        {
            return _offset < _tokens.Count;
        }

        private void MoveNext()
        {
            _offset++;
        }

        private StepToken Current => _tokens[_offset];

        public StepFileSyntax LexFileSyntax()
        {
            _offset = 0;
            SwallowKeywordAndSemicolon(StepFile.MagicHeader);

            var header = LexHeaderSection();
            var data = LexDataSection();

            var file = new StepFileSyntax(header, data);

            SwallowKeywordAndSemicolon(StepFile.MagicFooter);

            return file;
        }

        private StepHeaderSectionSyntax LexHeaderSection()
        {
            AssertTokensRemain();
            var headerLine = Current.Line;
            var headerColumn = Current.Column;
            SwallowKeywordAndSemicolon(StepFile.HeaderText);
            var macros = new List<StepHeaderMacroSyntax>();
            while (TokensRemain() && Current.Kind == StepTokenKind.Keyword && !IsCurrentEndSec())
            {
                var macro = LexHeaderMacro();
                macros.Add(macro);
            }

            SwallowKeywordAndSemicolon(StepFile.EndSectionText);
            return new StepHeaderSectionSyntax(headerLine, headerColumn, macros);
        }

        private StepHeaderMacroSyntax LexHeaderMacro()
        {
            AssertNextTokenKind(StepTokenKind.Keyword);
            var name = ((StepKeywordToken)Current).Value;
            MoveNext();
            var syntaxList = LexSyntaxList();
            SwallowSemicolon();
            return new StepHeaderMacroSyntax(name, syntaxList);
        }

        private StepSyntax LexIndividualValue()
        {
            StepSyntax result;
            AssertTokensRemain();
            switch (Current.Kind)
            {
                case StepTokenKind.Integer:
                    result = new StepIntegerSyntax((StepIntegerToken)Current);
                    MoveNext();
                    break;
                case StepTokenKind.Real:
                    result = new StepRealSyntax((StepRealToken)Current);
                    MoveNext();
                    break;
                case StepTokenKind.String:
                    result = new StepStringSyntax((StepStringToken)Current);
                    MoveNext();
                    break;
                case StepTokenKind.Asterisk:
                    result = new StepAutoSyntax((StepAsteriskToken)Current);
                    MoveNext();
                    break;
                case StepTokenKind.Omitted:
                    result = new StepOmittedSyntax((StepOmittedToken)Current);
                    MoveNext();
                    break;
                case StepTokenKind.Enumeration:
                    result = new StepEnumerationValueSyntax((StepEnumerationToken)Current);
                    MoveNext();
                    break;
                case StepTokenKind.LeftParen:
                    result = LexSyntaxList();
                    break;
                case StepTokenKind.Keyword:
                    result = LexSimpleEntity();
                    break;
                case StepTokenKind.EntityInstance:
                    result = new StepEntityInstanceReferenceSyntax((StepEntityInstanceToken)Current);
                    MoveNext();
                    break;
                default:
                    ReportError($"Unexpected syntax token '{Current.Kind}'");
                    result = null; // unreachable
                    break;
            }

            return result;
        }

        private StepSyntaxList LexSyntaxList()
        {
            AssertTokensRemain();
            var listLine = Current.Line;
            var listColumn = Current.Column;
            SwallowLeftParen();
            var values = new List<StepSyntax>();
            bool keepReading = true;
            bool expectingValue = true;
            while (keepReading)
            {
                AssertTokensRemain();
                if (expectingValue || values.Count == 0)
                {
                    // expect a value or a close paren
                    switch (Current.Kind)
                    {
                        case StepTokenKind.RightParen:
                            keepReading = false;
                            MoveNext();
                            break;
                        default:
                            values.Add(LexIndividualValue());
                            break;
                    }
                }
                else
                {
                    // expect a comma or close paren
                    switch (Current.Kind)
                    {
                        case StepTokenKind.RightParen:
                            keepReading = false;
                            MoveNext();
                            break;
                        case StepTokenKind.Comma:
                            MoveNext();
                            break;
                        default:
                            ReportError($"Expected right paren or comma but found '{Current.Kind}'");
                            break;
                    }
                }

                expectingValue = !expectingValue;
            }

            return new StepSyntaxList(listLine, listColumn, values);
        }

        private StepDataSectionSyntax LexDataSection()
        {
            AssertTokensRemain();
            var dataLine = Current.Line;
            var dataColumn = Current.Column;
            SwallowKeywordAndSemicolon(StepFile.DataText);
            var entityInstsances = new List<StepEntityInstanceSyntax>();
            while (TokensRemain() && Current.Kind == StepTokenKind.EntityInstance)
            {
                var entityInstance = LexEntityInstance();
                entityInstsances.Add(entityInstance);
            }

            SwallowKeywordAndSemicolon(StepFile.EndSectionText);
            return new StepDataSectionSyntax(dataLine, dataColumn, entityInstsances);
        }

        private StepEntityInstanceSyntax LexEntityInstance()
        {
            var line = Current.Line;
            var column = Current.Column;

            AssertNextTokenKind(StepTokenKind.EntityInstance);
            var reference = (StepEntityInstanceToken)Current;
            MoveNext();

            SwallowEquals();

            AssertTokensRemain();
            StepEntitySyntax entity = null;
            switch (Current.Kind)
            {
                case StepTokenKind.Keyword:
                    entity = LexSimpleEntity();
                    break;
                case StepTokenKind.LeftParen:
                    entity = LexComplexEntity();
                    break;
                default:
                    ReportError($"Expected left paren but found {Current.Kind}");
                    break; // unreachable
            }

            SwallowSemicolon();

            return new StepEntityInstanceSyntax(reference, entity);
        }

        private StepSimpleEntitySyntax LexSimpleEntity()
        {
            AssertNextTokenKind(StepTokenKind.Keyword);
            var keyword = (StepKeywordToken)Current;
            MoveNext();

            var parameters = LexSyntaxList();
            return new StepSimpleEntitySyntax(keyword, parameters);
        }

        private StepComplexEntitySyntax LexComplexEntity()
        {
            var entities = new List<StepSimpleEntitySyntax>();
            var entityLine = Current.Line;
            var entityColumn = Current.Column;
            SwallowLeftParen();
            entities.Add(LexSimpleEntity()); // there's always at least one

            bool keepReading = true;
            while (keepReading)
            {
                AssertTokensRemain();
                switch (Current.Kind)
                {
                    case StepTokenKind.RightParen:
                        SwallowRightParen();
                        keepReading = false;
                        break;
                    case StepTokenKind.Keyword:
                        entities.Add(LexSimpleEntity());
                        break;
                    default:
                        ReportError($"Expected right paren or keyword but found {Current.Kind}");
                        break; // unreachable
                }
            }

            return new StepComplexEntitySyntax(entityLine, entityColumn, entities);
        }

        private bool IsCurrentEndSec()
        {
            return Current.Kind == StepTokenKind.Keyword && ((StepKeywordToken)Current).Value == StepFile.EndSectionText;
        }

        private void SwallowKeyword(string keyword)
        {
            AssertNextTokenKind(StepTokenKind.Keyword);
            if (((StepKeywordToken)Current).Value != keyword)
            {
                ReportError($"Expected keyword '{keyword}' but found '{((StepKeywordToken)Current).Value}'");
            }

            MoveNext();
        }

        private void SwallowKeywordAndSemicolon(string keyword)
        {
            SwallowKeyword(keyword);
            SwallowSemicolon();
        }

        private void SwallowSemicolon()
        {
            SwallowToken(StepTokenKind.Semicolon);
        }

        private void SwallowLeftParen()
        {
            SwallowToken(StepTokenKind.LeftParen);
        }

        private void SwallowRightParen()
        {
            SwallowToken(StepTokenKind.RightParen);
        }

        private void SwallowEquals()
        {
            SwallowToken(StepTokenKind.Equals);
        }

        private void SwallowToken(StepTokenKind kind)
        {
            AssertNextTokenKind(kind);
            MoveNext();
        }

        private void AssertNextTokenKind(StepTokenKind kind)
        {
            AssertTokensRemain();
            if (Current.Kind != kind)
            {
                ReportError($"Expected '{kind}' token but found '{Current.Kind}'");
            }
        }

        private void AssertTokensRemain()
        {
            if (!TokensRemain())
            {
                ReportError("Unexpected end of token stream", 0, 0);
            }
        }

        private void ReportError(string message, int? line = null, int? column = null)
        {
            throw new StepReadException(message, line ?? Current.Line, column ?? Current.Column);
        }
    }
}
