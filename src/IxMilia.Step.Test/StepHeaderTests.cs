// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepHeaderTests
    {
        private StepFile ReadFileFromHeader(string header)
        {
            var file = $@"
{StepFile.MagicHeader};
{StepFile.HeaderText};
{header.Trim()}
{StepFile.EndSectionText};
{StepFile.DataText};
{StepFile.EndSectionText};
{StepFile.MagicFooter};
";
            return StepFile.Parse(file.Trim());
        }

        [Fact]
        public void FileDescriptionTest()
        {
            var file = ReadFileFromHeader("FILE_DESCRIPTION(('some description'), '2;1');");
            Assert.Equal("some description", file.Description);
            Assert.Equal("2;1", file.ImplementationLevel);
        }

        [Fact]
        public void FileDescriptionWithMultiplePartsTest()
        {
            var file = ReadFileFromHeader("FILE_DESCRIPTION(('some description', ', ', 'more description'), '2;1');");
            Assert.Equal("some description, more description", file.Description);
            Assert.Equal("2;1", file.ImplementationLevel);
        }

        [Fact]
        public void FullHeaderTest()
        {
            var file = ReadFileFromHeader(@"
FILE_DESCRIPTION(('description'), '2;1');
FILE_NAME('file-name', '2010-01-01T', ('author'), ('organization'), 'preprocessor', 'originator', 'authorization');
FILE_SCHEMA(('EXPLICIT_DRAUGHTING'));
");
            Assert.Equal("description", file.Description);
            Assert.Equal("2;1", file.ImplementationLevel);
            Assert.Equal("file-name", file.Name);
            Assert.Equal(new DateTime(2010, 1, 1), file.Timestamp);
            Assert.Equal("author", file.Author);
            Assert.Equal("organization", file.Organization);
            Assert.Equal("preprocessor", file.PreprocessorVersion);
            Assert.Equal("originator", file.OriginatingSystem);
            Assert.Equal("authorization", file.Authorization);
            Assert.Equal(StepSchemaTypes.ExplicitDraughting, file.Schemas.Single());
        }
    }
}
