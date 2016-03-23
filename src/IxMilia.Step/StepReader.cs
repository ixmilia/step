// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using IxMilia.Step.Entities;
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

            // read data
            while (IsNextTokenEntityInstance())
            {
                var tokenEntity = LexTokenEntity();
                var entity = StepEntity.FromMacro(tokenEntity.Macro);
                if (entity != null)
                {
                    _file.Entities.Add(entity);
                }
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
                case StepFile.FileNameText:
                    ApplyFileName(macro.Values);
                    break;
                case StepFile.FileSchemaText:
                    ApplyFileSchema(macro.Values);
                    break;
                default:
                    // TODO:
                    break;
            }
        }

        private void ApplyFileDescription(StepValueList valueList)
        {
            valueList.AssertValueListCount(2);
            _file.Description = GetConcatenatedStringValue(valueList.Values[0]);
            _file.ImplementationLevel = valueList.Values[1].GetStringValue(); // TODO: handle appropriate values
        }

        private void ApplyFileName(StepValueList valueList)
        {
            valueList.AssertValueListCount(7);
            _file.Name = valueList.Values[0].GetStringValue();
            _file.Timestamp = GetDateTimeValue(valueList.Values[1]);
            _file.Author = GetConcatenatedStringValue(valueList.Values[2]);
            _file.Organization = GetConcatenatedStringValue(valueList.Values[3]);
            _file.PreprocessorVersion = valueList.Values[4].GetStringValue();
            _file.OriginatingSystem = valueList.Values[5].GetStringValue();
            _file.Authorization = valueList.Values[6].GetStringValue();
        }

        private void ApplyFileSchema(StepValueList valueList)
        {
            valueList.AssertValueListCount(1);
            foreach (var schemaType in GetListValues(valueList.Values[0]).Select(v => StepSchemaTypeExtensions.SchemaTypeFromName(v.GetStringValue())))
            {
                _file.Schemas.Add(schemaType);
            }
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

        private string GetConcatenatedStringValue(StepValue value)
        {
            return string.Join(string.Empty, GetListValues(value).Select(v => v.GetStringValue()));
        }

        private DateTime GetDateTimeValue(StepValue value)
        {
            var str = value.GetStringValue();
            return DateTime.ParseExact(str, "yyyy-MM-ddT", CultureInfo.InvariantCulture);
        }

        private bool IsNextTokenKind(StepTokenKind kind)
        {
            return Current.Kind == kind;
        }

        private bool IsNextTokenEntityInstance()
        {
            return IsNextTokenKind(StepTokenKind.EntityInstance);
        }

        private bool IsNextTokenKeyword(string keyword)
        {
            return IsNextTokenKind(StepTokenKind.Keyword) && ((StepKeywordToken)Current).Value == keyword;
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

        private StepTokenEntity LexTokenEntity()
        {
            AssertNextKindIs(StepTokenKind.EntityInstance);
            var id = ((StepEntityInstanceToken)Current).Id;
            MoveNext();

            AssertNextKindIs(StepTokenKind.Equals);
            MoveNext();

            var macro = LexMacro();

            return new StepTokenEntity(id, macro);
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
                            // TODO: it could be a keyword followed by a parameter list, e.g., another macro (typed parameter)
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
