// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.IO;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepHeaderTests
    {
        private StepFile ReadFile(string data)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(data);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return StepFile.Load(stream);
            }
        }

        private StepFile ReadFileFromHeader(string header)
        {
            var file = $@"
{StepFile.MagicHeader};
{StepFile.HeaderText};
{header}
{StepFile.EndSectionText};
{StepFile.DataText};
{StepFile.EndSectionText};
{StepFile.MagicFooter};
";
            return ReadFile(file.Trim());
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
    }
}
