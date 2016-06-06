// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IxMilia.Step.Items;
using IxMilia.Step.Syntax;

namespace IxMilia.Step
{
    internal class StepWriter
    {
        private StepFile _file;
        private StringBuilder _builder;
        private bool _inlineReferences;
        private Dictionary<StepRepresentationItem, int> _itemMap;
        private int _nextId;
        private const string Semicolon = ";";

        public StepWriter(StepFile stepFile, bool inlineReferences)
        {
            _file = stepFile;
            _builder = new StringBuilder();
            _itemMap = new Dictionary<StepRepresentationItem, int>();
            _inlineReferences = inlineReferences;
        }

        internal string GetContents()
        {
            AppendLine(StepFile.MagicHeader);

            // output header
            AppendLine(StepFile.HeaderText);
            AppendLine($"{StepFile.FileDescriptionText}{ToString(SplitStringIntoParts(_file.Description), _file.ImplementationLevel)}");
            AppendLine($"{StepFile.FileNameText}{ToString(_file.Name, _file.Timestamp, SplitStringIntoParts(_file.Author), SplitStringIntoParts(_file.Organization), _file.PreprocessorVersion, _file.OriginatingSystem, _file.Authorization)}");
            var schemas = _file.Schemas.Select(s => s.ToSchemaName())
                .Concat(_file.UnsupportedSchemas)
                .Cast<object>()
                .ToArray();
            AppendLine($"{StepFile.FileSchemaText}({ToString(schemas)})");
            AppendLine(StepFile.EndSectionText);

            // data section
            AppendLine(StepFile.DataText);
            foreach (var item in _file.Items)
            {
                WriteItem(item);
            }

            AppendLine(StepFile.EndSectionText);

            AppendLine(StepFile.MagicFooter);

            return _builder.ToString();
        }

        private int WriteItem(StepRepresentationItem item)
        {
            if (!_inlineReferences)
            {
                // not inlining references, need to write out entities as we see them
                foreach (var referencedItem in item.GetReferencedItems())
                {
                    if (!_itemMap.ContainsKey(referencedItem))
                    {
                        var refid = WriteItem(referencedItem);
                    }
                }
            }

            var id = ++_nextId;
            var syntax = GetItemSyntax(item, id);
            AppendLine($"#{id}={syntax.ToString(this)}");
            return id;
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

        private void AppendLine(string contents)
        {
            _builder.Append(contents);
            _builder.AppendLine(Semicolon);
        }

        public string ToString(object obj)
        {
            if (obj == null)
            {
                // assume empty string
                return "''";
            }
            if (obj is double)
            {
                return ((double)obj).ToString("0.0");
            }
            else if (obj is string)
            {
                // TODO: escaping
                return "'" + (string)obj + "'";
            }
            else if (obj is DateTime)
            {
                var dt = (DateTime)obj;
                return "'" + dt.ToString(StepReader.DateTimeFormat) + "'";
            }
            else if (obj is object[])
            {
                var array = (object[])obj;
                return "(" + string.Join(",", array.Select(ToString)) + ")";
            }
            else
            {
                throw new Exception("Unsupported object type: " + obj.GetType().Name);
            }
        }

        private string ToString(params object[] items)
        {
            return ToString((object)items);
        }

        private object[] SplitStringIntoParts(string str, int maxLength = 256)
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

            return parts.Cast<object>().ToArray();
        }
    }
}
