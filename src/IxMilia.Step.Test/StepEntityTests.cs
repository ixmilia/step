// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using IxMilia.Step.Entities;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepEntityTests
    {
        private StepFile ReadFile(string data)
        {
            var text = $@"
ISO-10303-21;
HEADER;
ENDSEC;
DATA;
{data.Trim()}
ENDSEC;
END-ISO-10303-21;
";
            var file = StepFile.Parse(text.Trim());
            return file;
        }

        private StepEntity ReadEntity(string data)
        {
            var file = ReadFile(data);
            return file.Entities.Single();
        }

        [Fact]
        public void ReadCartesianPointTest1()
        {
            var point = (StepCartesianPoint)ReadEntity("#1=CARTESIAN_POINT('name',(1.0,2.0,3.0));");
            Assert.Equal("name", point.Label);
            Assert.Equal(1.0, point.X);
            Assert.Equal(2.0, point.Y);
            Assert.Equal(3.0, point.Z);
        }

        [Fact]
        public void ReadCartesianPointTest2()
        {
            var point = (StepCartesianPoint)ReadEntity("#1=CARTESIAN_POINT('name',(1.0));");
            Assert.Equal("name", point.Label);
            Assert.Equal(1.0, point.X);
            Assert.Equal(0.0, point.Y);
            Assert.Equal(0.0, point.Z);
        }

        [Fact]
        public void ReadDirectionTest()
        {
            var direction = (StepDirection)ReadEntity("#1=DIRECTION('name',(1.0,2.0,3.0));");
            Assert.Equal("name", direction.Label);
            Assert.Equal(1.0, direction.X);
            Assert.Equal(2.0, direction.Y);
            Assert.Equal(3.0, direction.Z);
        }

        [Fact]
        public void ReadSubReferencedEntityTest()
        {
            var vector = (StepVector)ReadEntity("#1=VECTOR('name',DIRECTION('',(0.0,0.0,1.0)),15.0);");
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), vector.Direction);
            Assert.Equal(15.0, vector.Length);
        }

        [Fact]
        public void ReadPreviouslyReferencedEntityTest()
        {
            var file = ReadFile(@"
#1=DIRECTION('',(0.0,0.0,1.0));
#2=VECTOR('',#1,15.0);
");
            Assert.Equal(2, file.Entities.Count);
            Assert.IsType<StepDirection>(file.Entities.First());
            Assert.IsType<StepVector>(file.Entities.Last());
            var vector = (StepVector)file.Entities.Last();
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), vector.Direction);
            Assert.Equal(15.0, vector.Length);
        }

        [Fact]
        public void ReadPostReferencedEntityTest()
        {
            var file = ReadFile(@"
#1=VECTOR('',#2,15.0);
#2=DIRECTION('',(0.0,0.0,1.0));
");
            Assert.Equal(2, file.Entities.Count);
            Assert.IsType<StepVector>(file.Entities.First());
            Assert.IsType<StepDirection>(file.Entities.Last());
            var vector = (StepVector)file.Entities.First();
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), vector.Direction);
            Assert.Equal(15.0, vector.Length);
        }

        [Fact]
        public void ReadLineTest()
        {
            var file = ReadFile(@"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=VECTOR('',#2,15.0);
#4=LINE('',#1,#3);
");
            Assert.Equal(4, file.Entities.Count);
            var line = file.Entities.OfType<StepLine>().Single();
            Assert.Equal(new StepCartesianPoint("", 1.0, 2.0, 3.0), line.Point);
            Assert.Equal(15.0, line.Vector.Length);
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), line.Vector.Direction);
        }
    }
}
