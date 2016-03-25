// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using IxMilia.Step.Entities;

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

        public List<StepEntity> Entities { get; }

        public StepFile()
        {
            Timestamp = DateTime.Now;
            Schemas = new HashSet<StepSchemaTypes>();
            Entities = new List<StepEntity>();
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

        public string GetContentsAsString()
        {
            var writer = new StepWriter(this);
            return writer.GetContents();
        }

        public void Save(Stream stream)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(GetContentsAsString());
                streamWriter.Flush();
            }
        }
    }
}
