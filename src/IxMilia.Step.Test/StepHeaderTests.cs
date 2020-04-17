using System;
using System.Linq;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepHeaderTests : StepTestBase
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

        [Fact]
        public void ReadDifferentTimeStampsTest()
        {
            var file = ReadFileFromHeader(@"FILE_NAME('', '2016-06-26T13:59:52+02:00', (), (), '', '', '');");
            Assert.Equal(2016, file.Timestamp.Year);
            Assert.Equal(6, file.Timestamp.Month);
        }

        [Fact]
        public void ReadTimeStampMonthWithoutLeadingZerosTest()
        {
            var file = ReadFileFromHeader(@"FILE_NAME('', '2004-3-17T2:58:55 PM+8:00', (), (), '', '', '');");
            Assert.Equal(2004, file.Timestamp.Year);
            Assert.Equal(3, file.Timestamp.Month);
        }

        [Fact]
        public void WriteHeaderTest()
        {
            var file = new StepFile();
            file.Description = "some description";
            file.ImplementationLevel = "2;1";
            file.Name = "file-name";
            file.Timestamp = new DateTime(2010, 1, 1);
            file.Author = "author";
            file.Organization = "organization";
            file.PreprocessorVersion = "preprocessor";
            file.OriginatingSystem = "originator";
            file.Authorization = "authorization";
            file.Schemas.Add(StepSchemaTypes.ExplicitDraughting);
            AssertFileIs(file, @"
ISO-10303-21;
HEADER;
FILE_DESCRIPTION(('some description'),'2;1');
FILE_NAME('file-name','2010-01-01T00:00:00.0000000',('author'),('organization'),'preprocessor','originator','authorization');
FILE_SCHEMA(('EXPLICIT_DRAUGHTING'));
ENDSEC;
DATA;
ENDSEC;
END-ISO-10303-21;
".TrimStart());
        }

        [Fact]
        public void WriteHeaderWithLongDescriptionTest()
        {
            var file = new StepFile();
            file.Description = new string('a', 257);
            file.Timestamp = new DateTime(2010, 1, 1);
            file.Schemas.Add(StepSchemaTypes.ExplicitDraughting);
            AssertFileIs(file, $@"
ISO-10303-21;
HEADER;
FILE_DESCRIPTION(('{new string('a', 256)}','a'),'');
FILE_NAME('','2010-01-01T00:00:00.0000000',(''),(''),'','','');
FILE_SCHEMA(('EXPLICIT_DRAUGHTING'));
ENDSEC;
DATA;
ENDSEC;
END-ISO-10303-21;
".TrimStart());
        }

        [Fact]
        public void ReadHeaderWithUnsupportedSchemaTest()
        {
            var file = ReadFileFromHeader(@"FILE_SCHEMA(('EXPLICIT_DRAUGHTING','UNSUPPORTED_SCHEMA'));");
            Assert.Single(file.Schemas);
            Assert.Single(file.UnsupportedSchemas);
        }

        [Fact]
        public void WriteHeaderWithUnsupportedSchemaTest()
        {
            var file = new StepFile();
            file.UnsupportedSchemas.Add("UNSUPPORTED_SCHEMA");
            AssertFileContains(file, "FILE_SCHEMA(('UNSUPPORTED_SCHEMA'));");
        }
    }
}
