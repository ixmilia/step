// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IxMilia.Step.Tokens;

namespace IxMilia.Step
{
    internal class StepTokenizer
    {
        private StreamReader _reader;
        private string _currentLine;
        private int _offset;
        private int _currentLineNumber;
        private int _currentColumn;

        public StepTokenizer(Stream stream)
        {
            _reader = new StreamReader(stream);
            ReadNextLine();
        }

        private void ReadNextLine()
        {
            _currentLine = _reader.ReadLine();
            _offset = 0;
            _currentLineNumber++;
            _currentColumn = 1;
        }

        private char? PeekCharacter()
        {
            while (true)
            {
                if (_currentLine == null)
                {
                    return null;
                }
                else if (_offset >= _currentLine.Length)
                {
                    ReadNextLine();
                    if (_currentLine == null)
                    {
                        return null;
                    }
                }

                switch (_currentLine[_offset])
                {
                    // TODO: space ends a token, linebreaks do not
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':
                        // skip whitespace
                        _offset++;
                        continue;
                    default:
                        return _currentLine[_offset];
                }
            }
        }

        private void Advance()
        {
            _offset++;
            if (_offset > _currentLine.Length)
            {
                ReadNextLine();
            }
        }

        public IEnumerable<StepToken> GetTokens()
        {
            var cn = PeekCharacter();
            while (cn != null)
            {
                var c = cn.GetValueOrDefault();
                if (c == '$')
                {
                    Advance();
                    yield return new StepOmittedToken(_currentLineNumber, _currentColumn);
                }
                else if (c == ';')
                {
                    Advance();
                    yield return new StepSemiColonToken(_currentLineNumber, _currentColumn);
                }
                else if (IsNumberStart(c))
                {
                    yield return ParseNumber();
                }
                else if (IsApostrophe(c))
                {
                    yield return ParseString();
                }
                else if (IsHash(c))
                {
                    // constant instance: #INCH
                    // entity instance: #1234
                    yield return ParseHashValue();
                }
                else if (IsAt(c))
                {
                    // constant value: @PI
                    // instance value: @12
                    yield return ParseAtValue();
                }
                else if (IsDot(c))
                {
                    yield return ParseEnumeration();
                }
                else if (IsLeftParen(c))
                {
                    yield return new StepLeftParenToken(_currentLineNumber, _currentColumn);
                }
                else if (IsRightParen(c))
                {
                    yield return new StepRightParenToken(_currentLineNumber, _currentColumn);
                }
                else if (IsComma(c))
                {
                    yield return new StepCommaToken(_currentLineNumber, _currentColumn);
                }
                else if (IsUpper(c))
                {
                    
                }
                else
                {
                    throw new StepReadException($"Unexpected character '{c}'", _currentLineNumber, _currentColumn);
                }
            }

            yield break;
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsDot(char c)
        {
            return c == '.';
        }

        private bool IsE(char c)
        {
            return c == 'e' || c == 'E';
        }

        private bool IsPlus(char c)
        {
            return c == '+';
        }

        private bool IsMinus(char c)
        {
            return c == '-';
        }

        private bool IsNumberStart(char c)
        {
            return IsDigit(c)
                || c == '-'
                || c == '+';
        }

        private bool IsApostrophe(char c)
        {
            return c == '\'';
        }

        private bool IsBackslash(char c)
        {
            return c == '\\';
        }

        private bool IsHash(char c)
        {
            return c == '#';
        }

        private bool IsAt(char c)
        {
            return c == '@';
        }

        private bool IsUpper(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        private bool IsUpperOrDigit(char c)
        {
            return IsUpper(c) || IsDigit(c);
        }

        private bool IsLeftParen(char c)
        {
            return c == '(';
        }

        private bool IsRightParen(char c)
        {
            return c == ')';
        }

        private bool IsComma(char c)
        {
            return c == ',';
        }

        private bool IsKeywordCharacter(char c)
        {
            return IsUpperOrDigit(c)
                || c == '-';
        }

        private StepToken ParseNumber()
        {
            var tokenLine = _currentLineNumber;
            var tokenColumn = _currentColumn;
            var sb = new StringBuilder();
            sb.Append(PeekCharacter());
            Advance();

            bool seenDecimal = false;
            bool seenE = false;
            char? cn;
            while ((cn = PeekCharacter()) != null)
            {
                var c = cn.GetValueOrDefault();
                if (IsDigit(c))
                {
                    sb.Append(c);
                    Advance();
                }
                else if (IsDot(c) && !seenDecimal && !seenE)
                {
                    sb.Append(c);
                    seenDecimal = true;
                    Advance();
                }
                else if (IsE(c) && !seenE)
                {
                    sb.Append(c);
                    seenE = true;
                    Advance();
                }
                else if ((IsPlus(c) || IsMinus(c)) && seenE)
                {
                    // TODO: this will fail on "1.0E+-+-+-+-1"
                    sb.Append(c);
                    Advance();
                }
                else
                {
                    break;
                }
            }

            var str = sb.ToString();
            return seenDecimal || seenE
                ? (StepToken)new StepRealToken(double.Parse(str), tokenLine, tokenColumn)
                : new StepIntegerToken(int.Parse(str), tokenLine, tokenColumn);
        }

        private StepStringToken ParseString()
        {
            var tokenLine = _currentLineNumber;
            var tokenColumn = _currentColumn;
            var sb = new StringBuilder();
            Advance();

            char? cn;
            bool wasApostropheLast = false;
            bool wasBackslashLast = false;
            while ((cn = PeekCharacter()) != null)
            {
                var c = cn.GetValueOrDefault();
                if (IsApostrophe(c) && wasApostropheLast)
                {
                    // escaped
                    sb.Append(c);
                    Advance();
                }
                else if (IsApostrophe(c) && !wasApostropheLast)
                {
                    // maybe the end
                    wasApostropheLast = true;
                    Advance();
                }
                else if (!IsApostrophe(c) && wasApostropheLast)
                {
                    // end of string
                    break;
                }
                else if (IsBackslash(c) && !wasBackslashLast)
                {
                    // start escaping
                    wasBackslashLast = true;
                    Advance();
                }
                else if (wasBackslashLast)
                {
                    // TODO: handle real escaping
                    sb.Append(c);
                    Advance();
                }
                else
                {
                    // just a normal string
                    sb.Append(c);
                }
            }

            var str = sb.ToString();
            return new StepStringToken(str, tokenLine, tokenColumn);
        }

        private StepToken ParseHashValue()
        {
            var tokenLine = _currentLineNumber;
            var tokenColumn = _currentColumn;
            Advance(); // swallow '#'
            var next = PeekCharacter();
            if (next == null)
            {
                throw new StepReadException("Expected constant instance or entity instance", tokenLine, tokenColumn);
            }

            if (IsDigit(next.GetValueOrDefault()))
            {
                // entity instance: #1234
                return new StepEntityInstanceToken(int.Parse(TakeWhile(IsDigit)), tokenLine, tokenColumn);
            }
            else if (IsUpper(next.GetValueOrDefault()))
            {
                // constant instance: #INCH
                return new StepConstantInstanceToken(TakeWhile(IsUpperOrDigit), tokenLine, tokenColumn);
            }
            else
            {
                throw new StepReadException("Expected constant instance or entity instance", tokenLine, tokenColumn);
            }
        }

        private StepToken ParseAtValue()
        {
            var tokenLine = _currentLineNumber;
            var tokenColumn = _currentColumn;
            Advance(); // swallow '@'
            var next = PeekCharacter();
            if (next == null)
            {
                throw new StepReadException("Expected constant value or instance value", tokenLine, tokenColumn);
            }

            if (IsDigit(next.GetValueOrDefault()))
            {
                // constant value: @PI
                return new StepConstantValueToken(TakeWhile(IsDigit), tokenLine, tokenColumn);
            }
            else if (IsUpper(next.GetValueOrDefault()))
            {
                // instance value: @12
                return new StepInstanceValueToken(int.Parse(TakeWhile(IsUpperOrDigit)), tokenLine, tokenColumn);
            }
            else
            {
                throw new StepReadException("Expected constant value or instance value", tokenLine, tokenColumn);
            }
        }

        private StepEnumerationToken ParseEnumeration()
        {
            var tokenLine = _currentLineNumber;
            var tokenColumn = _currentColumn;
            var sb = new StringBuilder();
            Advance(); // swallow leading '.'
            var value = TakeWhile(IsUpperOrDigit);
            if (string.IsNullOrEmpty(value))
            {
                throw new StepReadException("Expected enumeration value", tokenLine, tokenColumn);
            }

            var next = PeekCharacter();
            if (next.HasValue && IsDot(next.GetValueOrDefault()))
            {
                Advance();
                return new StepEnumerationToken(value, tokenLine, tokenColumn);
            }
            else
            {
                throw new StepReadException("Expected enumeration ending dot", _currentLineNumber, _currentColumn);
            }
        }

        private StepKeywordToken ParseKeyword()
        {
            var tokenLine = _currentLineNumber;
            var tokenColumn = _currentColumn;
            var value = TakeWhile(IsKeywordCharacter);
            return new StepKeywordToken(value, tokenLine, tokenColumn);
        }

        private string TakeWhile(Func<char, bool> predicate)
        {
            var sb = new StringBuilder();
            char? c;
            while ((c = PeekCharacter()) != null && c.HasValue)
            {
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
