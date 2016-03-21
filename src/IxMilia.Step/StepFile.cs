// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IxMilia.Step
{
    public class StepFile
    {
        internal const string MagicHeader = "ISO-10303-21";
        internal const string MagicFooter = "END-" + MagicHeader;
        internal const string HeaderText = "HEADER";
        internal const string EndSectionText = "ENDSEC";
        internal const string DataText = "DATA";

        public static StepFile Load(Stream stream)
        {
            return new StepReader(stream).ReadFile();
        }
    }
}
