// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IxMilia.Step.Items;

namespace IxMilia.Step
{
    public class StepFile
    {
        internal const string MagicHeader = "ISO-10303-21";
        internal const string MagicFooter = "END-" + MagicHeader;
        internal const string HeaderText = "HEADER";
        internal const string EndSectionText = "ENDSEC";
        internal const string DataText = "DATA";

        internal const string FileDescriptionText = "FILE_DESCRIPTION";
        internal const string FileNameText = "FILE_NAME";
        internal const string FileSchemaText = "FILE_SCHEMA";

        // FILE_DESCRIPTION values
        public string Description { get; set; }
        public string ImplementationLevel { get; set; }

        // FILE_NAME values
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public string Author { get; set; }
        public string Organization { get; set; }
        public string PreprocessorVersion { get; set; }
        public string OriginatingSystem { get; set; }
        public string Authorization { get; set; }

        // FILE_SCHEMA values
        public HashSet<StepSchemaTypes> Schemas { get; }
        public List<string> UnsupportedSchemas { get; }

        public List<StepRepresentationItem> Items { get; }

        public StepFile()
        {
            Timestamp = DateTime.Now;
            Schemas = new HashSet<StepSchemaTypes>();
            UnsupportedSchemas = new List<string>();
            Items = new List<StepRepresentationItem>();
        }

        public static StepFile Load(Stream stream)
        {
            return new StepReader(stream).ReadFile();
        }

        public static StepFile Parse(string data)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(data);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return Load(stream);
            }
        }

        public string GetContentsAsString(bool inlineReferences = false)
        {
            var writer = new StepWriter(this, inlineReferences);
            return writer.GetContents();
        }

        public void Save(Stream stream, bool inlineReferences = false)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(GetContentsAsString(inlineReferences));
                streamWriter.Flush();
            }
        }

        /// <summary>
        /// Gets all top-level items (i.e., not referenced by any other item) in the file.
        /// </summary>
        public IEnumerable<StepRepresentationItem> GetTopLevelitems()
        {
            var visitedItems = new HashSet<StepRepresentationItem>();
            var referencedItems = new HashSet<StepRepresentationItem>();
            foreach (var item in Items)
            {
                MarkReferencedItems(item, visitedItems, referencedItems);
            }

            return Items.Where(item => !referencedItems.Contains(item));
        }

        private static void MarkReferencedItems(StepRepresentationItem item, HashSet<StepRepresentationItem> visitedItems, HashSet<StepRepresentationItem> referencedItems)
        {
            if (visitedItems.Add(item))
            {
                foreach (var referenced in item.GetReferencedItems())
                {
                    visitedItems.Add(referenced);
                    referencedItems.Add(referenced);
                    MarkReferencedItems(referenced, visitedItems, referencedItems);
                }
            }
        }
    }
}
