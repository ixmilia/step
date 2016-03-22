// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

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
        public List<string> Schemas { get; }

        public StepFile()
        {
            Timestamp = DateTime.Now;
            Schemas = new List<string>();
        }

        public static StepFile Load(Stream stream)
        {
            return new StepReader(stream).ReadFile();
        }
    }
}
