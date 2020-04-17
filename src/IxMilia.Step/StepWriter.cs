using System;
using System.Collections.Generic;
using System.Text;
using IxMilia.Step.Items;
using IxMilia.Step.Syntax;
using IxMilia.Step.Tokens;

namespace IxMilia.Step
{
    internal class StepWriter
    {
        private StepFile _file;
        private int _currentLineLength;
        private bool _honorLineLength = true;
        private bool _inlineReferences;
        private Dictionary<StepRepresentationItem, int> _itemMap;
        private int _nextId;

        private const int MaxLineLength = 80;

        public StepWriter(StepFile stepFile, bool inlineReferences)
        {
            _file = stepFile;
            _itemMap = new Dictionary<StepRepresentationItem, int>();
            _inlineReferences = inlineReferences;
        }

        public string GetContents()
        {
            var builder = new StringBuilder();

            _honorLineLength = false;
            WriteDelimitedLine(StepFile.MagicHeader, builder);

            // output header
            WriteDelimitedLine(StepFile.HeaderText, builder);
            var headerSyntax = _file.GetHeaderSyntax();
            foreach (var macro in headerSyntax.Macros)
            {
                WriteHeaderMacro(macro, builder);
            }

            WriteDelimitedLine(StepFile.EndSectionText, builder);

            _honorLineLength = true;

            // data section
            WriteDelimitedLine(StepFile.DataText, builder);
            foreach (var item in _file.Items)
            {
                WriteItem(item, builder);
            }

            WriteDelimitedLine(StepFile.EndSectionText, builder);
            WriteDelimitedLine(StepFile.MagicFooter, builder);

            return builder.ToString();
        }

        private void WriteHeaderMacro(StepHeaderMacroSyntax macro, StringBuilder builder)
        {
            WriteText(macro.Name, builder);
            WriteTokens(macro.Values.GetTokens(), builder);
            WriteToken(StepSemicolonToken.Instance, builder);
            WriteNewLine(builder);
        }

        private int WriteItem(StepRepresentationItem item, StringBuilder builder)
        {
            if (!_inlineReferences)
            {
                // not inlining references, need to write out entities as we see them
                foreach (var referencedItem in item.GetReferencedItems())
                {
                    if (!_itemMap.ContainsKey(referencedItem))
                    {
                        var refid = WriteItem(referencedItem, builder);
                    }
                }
            }

            var id = ++_nextId;
            var syntax = GetItemSyntax(item, id);
            WriteToken(new StepEntityInstanceToken(id, -1, -1), builder);
            WriteToken(StepEqualsToken.Instance, builder);
            WriteTokens(syntax.GetTokens(), builder);
            WriteToken(StepSemicolonToken.Instance, builder);
            WriteNewLine(builder);
            return id;
        }

        /// <summary>
        /// Internal for testing.
        /// </summary>
        internal void WriteTokens(IEnumerable<StepToken> tokens, StringBuilder builder)
        {
            foreach (var token in tokens)
            {
                WriteToken(token, builder);
            }
        }

        private void WriteToken(StepToken token, StringBuilder builder)
        {
            WriteText(token.ToString(this), builder);
        }

        private void WriteDelimitedLine(string text, StringBuilder builder)
        {
            WriteText(text, builder);
            WriteToken(StepSemicolonToken.Instance, builder);
            WriteNewLine(builder);
        }

        private void WriteText(string text, StringBuilder builder)
        {
            if (_honorLineLength && _currentLineLength + text.Length > MaxLineLength)
            {
                WriteNewLine(builder);
            }

            builder.Append(text);
            _currentLineLength += text.Length;
        }

        private void WriteNewLine(StringBuilder builder)
        {
            builder.Append("\r\n");
            _currentLineLength = 0;
        }

        private StepSyntax GetItemSyntax(StepRepresentationItem item, int expectedId)
        {
            if (!_itemMap.ContainsKey(item))
            {
                var parameters = new StepSyntaxList(-1, -1, item.GetParameters(this));
                var syntax = new StepSimpleItemSyntax(item.ItemType.GetItemTypeString(), parameters);
                _itemMap.Add(item, expectedId);
                return syntax;
            }
            else
            {
                return GetItemSyntax(item);
            }
        }

        public StepSyntax GetItemSyntax(StepRepresentationItem item)
        {
            if (_inlineReferences)
            {
                var parameters = new StepSyntaxList(-1, -1, item.GetParameters(this));
                return new StepSimpleItemSyntax(item.ItemType.GetItemTypeString(), parameters);
            }
            else
            {
                return new StepEntityInstanceReferenceSyntax(_itemMap[item]);
            }
        }

        public StepSyntax GetItemSyntaxOrAuto(StepRepresentationItem item)
        {
            return item == null
                ? new StepAutoSyntax()
                : GetItemSyntax(item);
        }

        public static StepEnumerationValueSyntax GetBooleanSyntax(bool value)
        {
            var text = value ? "T" : "F";
            return new StepEnumerationValueSyntax(text);
        }

        internal static IEnumerable<string> SplitStringIntoParts(string str, int maxLength = 256)
        {
            var parts = new List<string>();
            if (str != null)
            {
                int offset = 0;
                while (offset < str.Length)
                {
                    var length = Math.Min(maxLength, str.Length - offset);
                    parts.Add(str.Substring(offset, length));
                    offset += length;
                }
            }
            else
            {
                parts.Add(string.Empty);
            }

            return parts;
        }
    }
}
