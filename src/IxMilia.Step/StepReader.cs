// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using IxMilia.Step.Tokens;

namespace IxMilia.Step
{
    internal class StepReader
    {
        private StepTokenizer _tokenizer;
        private IEnumerator<StepToken> _tokenEnumerator;
        private bool _tokensRemain;
        private StepFile _file;

        private bool TokensRemain => _tokensRemain;
        private StepToken Current => _tokenEnumerator.Current;
        private int CurrentLine => _tokenizer.CurrentLine;
        private int CurrentColumn => _tokenizer.CurrentColumn;

        public StepReader(Stream stream)
        {
            _file = new StepFile();
            _tokenizer = new StepTokenizer(stream);
            _tokenEnumerator = _tokenizer.GetTokens().GetEnumerator();
            MoveNext();
        }

        public StepFile ReadFile()
        {
            // header
            SwallowKeywordAndSemicolon(StepFile.MagicHeader);
            SwallowKeywordAndSemicolon(StepFile.HeaderText);

            // read header data
            while (!IsNextTokenKeyword(StepFile.EndSectionText))
            {
                ApplyMacro(LexMacro());
            }

            SwallowKeywordAndSemicolon(StepFile.EndSectionText);

            // data
            SwallowKeywordAndSemicolon(StepFile.DataText);

            // TODO: read data, but skip it in the mean time
            while (!IsNextTokenKeyword(StepFile.EndSectionText))
            {
                MoveNext();
            }

            SwallowKeywordAndSemicolon(StepFile.EndSectionText);
            SwallowKeywordAndSemicolon(StepFile.MagicFooter);

            return _file;
        }

        private void MoveNext()
        {
            _tokensRemain = _tokenEnumerator.MoveNext();
            if (Current == null)
            {
                ReportError("Unexpected null token");
            }
        }

        private void ReportError(string message, int? line = null, int? column = null)
        {
            throw new StepReadException(message, line ?? CurrentLine, column ?? CurrentColumn);
        }

        private void ApplyMacro(StepMacro macro)
        {
            switch (macro.Keyword.Value)
            {
                case StepFile.FileDescriptionText:
                    ApplyFileDescription(macro.Values);
                    break;
                default:
                    // TODO:
                    break;
            }
        }

        private void ApplyFileDescription(StepValueList valueList)
        {
            if (valueList.Values.Count != 2)
            {
                ReportError($"Expected 2 values but got {valueList.Values.Count}", valueList.Line, valueList.Column);
            }

            _file.Description = string.Join(string.Empty, GetListValues(valueList.Values[0]).Select(v => GetStringValue(v)));
            _file.ImplementationLevel = GetStringValue(valueList.Values[1]);
        }

        public List<StepValue> GetListValues(StepValue value)
        {
            if (value is StepValueList)
            {
                return ((StepValueList)value).Values;
            }
            else
            {
                ReportError("Expected list of values");
                return null; // unreachable
            }
        }

        public string GetStringValue(StepValue value)
        {
            if (value is StepIndividualValue && ((StepIndividualValue)value).Value.Kind == StepTokenKind.String)
            {
                return ((StepStringToken)(((StepIndividualValue)value).Value)).Value;
            }
            else
            {
                ReportError("Expected string token", value.Line, value.Column);
                return null; // unreachable
            }
        }

        private bool IsNextTokenKeyword(string keyword)
        {
            return Current.Kind == StepTokenKind.Keyword && ((StepKeywordToken)Current).Value == keyword;
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

        private void SwallowToken(StepTokenKind kind)
        {
            AssertNextTokenKind(kind);
            MoveNext();
        }

        private void SwallowSemicolon()
        {
            SwallowToken(StepTokenKind.Semicolon);
        }

        private void SwallowLeftParen()
        {
            SwallowToken(StepTokenKind.LeftParen);
        }

        private void SwallowKeywordAndSemicolon(string keyword)
        {
            SwallowKeyword(keyword);
            SwallowSemicolon();
        }

        private void AssertNextTokenKind(StepTokenKind kind)
        {
            AssertTokensRemain();
            if (Current.Kind != kind)
            {
                ReportError($"Expected keyword token but found '{Current.Kind}'");
            }
        }

        private void AssertTokensRemain()
        {
            if (!TokensRemain)
            {
                ReportError("Unexpected end of token stream");
            }
        }

        private void AssertNextKindIs(StepTokenKind kind)
        {
            AssertTokensRemain();
            if (Current.Kind != kind)
            {
                ReportError($"Expected token of kind '{kind}' but found '{Current.Kind}'");
            }
        }

        private StepMacro LexMacro()
        {
            AssertNextKindIs(StepTokenKind.Keyword);
            var keyword = (StepKeywordToken)Current;
            MoveNext();

            var values = LexValueList();
            SwallowSemicolon();

            return new StepMacro(keyword, values);
        }

        private StepValueList LexValueList()
        {
            var listLine = CurrentLine;
            var listColumn = CurrentColumn;
            SwallowLeftParen();
            var values = new List<StepValue>();
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
                        case StepTokenKind.ConstantInstance:
                        case StepTokenKind.ConstantValue:
                        case StepTokenKind.EntityInstance:
                        case StepTokenKind.Enumeration:
                        case StepTokenKind.InstanceValue:
                        case StepTokenKind.Integer:
                        case StepTokenKind.Keyword:
                        case StepTokenKind.Omitted:
                        case StepTokenKind.Real:
                        case StepTokenKind.String:
                            values.Add(new StepIndividualValue(Current));
                            MoveNext();
                            break;
                        case StepTokenKind.LeftParen:
                            values.Add(LexValueList());
                            break;
                        default:
                            ReportError($"Unexepcted token kind '{Current.Kind}'");
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

            return new StepValueList(values, listLine, listColumn);
        }
    }
}
