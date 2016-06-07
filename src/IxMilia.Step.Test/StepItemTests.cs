// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using IxMilia.Step.Items;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepItemTests
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

        private StepRepresentationItem ReadTopLevelItem(string data)
        {
            var file = ReadFile(data);
            return file.GetTopLevelitems().Single();
        }

        private void AssertFileContains(StepFile file, string expected, bool inlineReferences = false)
        {
            var actual = file.GetContentsAsString(inlineReferences);
            Assert.Contains(expected, actual.Trim());
        }

        private void AssertFileContains(StepRepresentationItem item, string expected)
        {
            var file = new StepFile();
            file.Items.Add(item);
            AssertFileContains(file, expected);
        }

        [Fact]
        public void ReadCartesianPointTest1()
        {
            var point = (StepCartesianPoint)ReadTopLevelItem("#1=CARTESIAN_POINT('name',(1.0,2.0,3.0));");
            Assert.Equal("name", point.Name);
            Assert.Equal(1.0, point.X);
            Assert.Equal(2.0, point.Y);
            Assert.Equal(3.0, point.Z);
        }

        [Fact]
        public void ReadCartesianPointTest2()
        {
            var point = (StepCartesianPoint)ReadTopLevelItem("#1=CARTESIAN_POINT('name',(1.0));");
            Assert.Equal("name", point.Name);
            Assert.Equal(1.0, point.X);
            Assert.Equal(0.0, point.Y);
            Assert.Equal(0.0, point.Z);
        }

        [Fact]
        public void ReadDirectionTest()
        {
            var direction = (StepDirection)ReadTopLevelItem("#1=DIRECTION('name',(1.0,2.0,3.0));");
            Assert.Equal("name", direction.Name);
            Assert.Equal(1.0, direction.X);
            Assert.Equal(2.0, direction.Y);
            Assert.Equal(3.0, direction.Z);
        }

        [Fact]
        public void ReadSubReferencedItemTest()
        {
            var vector = (StepVector)ReadTopLevelItem("#1=VECTOR('name',DIRECTION('',(0.0,0.0,1.0)),15.0);");
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), vector.Direction);
            Assert.Equal(15.0, vector.Length);
        }

        [Fact]
        public void ReadPreviouslyReferencedItemsTest()
        {
            var vector = (StepVector)ReadTopLevelItem(@"
#1=DIRECTION('',(0.0,0.0,1.0));
#2=VECTOR('',#1,15.0);
");
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), vector.Direction);
            Assert.Equal(15.0, vector.Length);
        }

        [Fact]
        public void ReadPostReferencedItemTest()
        {
            var vector = (StepVector)ReadTopLevelItem(@"
#1=VECTOR('',#2,15.0);
#2=DIRECTION('',(0.0,0.0,1.0));
");
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), vector.Direction);
            Assert.Equal(15.0, vector.Length);
        }

        [Fact]
        public void ReadLineTest()
        {
            var line = (StepLine)ReadTopLevelItem(@"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=VECTOR('',#2,15.0);
#4=LINE('',#1,#3);
");
            Assert.Equal(new StepCartesianPoint("", 1.0, 2.0, 3.0), line.Point);
            Assert.Equal(15.0, line.Vector.Length);
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), line.Vector.Direction);
        }

        [Fact]
        public void WriteLineTest()
        {
            var file = new StepFile();
            file.Items.Add(new StepLine("", new StepCartesianPoint("", 1.0, 2.0, 3.0), new StepVector("", new StepDirection("", 1.0, 0.0, 0.0), 4.0)));
            AssertFileContains(file, @"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(1.0,0.0,0.0));
#3=VECTOR('',#2,4.0);
#4=LINE('',#1,#3);
");
        }

        [Fact]
        public void WriteLineWithInlineReferencesTest()
        {
            var file = new StepFile();
            file.Items.Add(new StepLine("", new StepCartesianPoint("", 1.0, 2.0, 3.0), new StepVector("", new StepDirection("", 1.0, 0.0, 0.0), 4.0)));
            AssertFileContains(file, "#1=LINE('',CARTESIAN_POINT('',(1.0,2.0,3.0)),VECTOR('',DIRECTION('',(1.0,0.0,0.0)),4.0));", inlineReferences: true);
        }

        [Fact]
        public void ReadCircleTest()
        {
            var circle = (StepCircle)ReadTopLevelItem(@"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=AXIS2_PLACEMENT_2D('',#1,#2);
#4=CIRCLE('',#3,5.0);
");
            Assert.Equal(new StepCartesianPoint("", 1.0, 2.0, 3.0), ((StepAxis2Placement2D)circle.Position).Location);
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), ((StepAxis2Placement2D)circle.Position).RefDirection);
            Assert.Equal(5.0, circle.Radius);
        }

        [Fact]
        public void WriteCircleTest()
        {
            var circle = new StepCircle("", new StepAxis2Placement2D("", new StepCartesianPoint("", 1.0, 2.0, 3.0), new StepDirection("", 0.0, 0.0, 1.0)), 5.0);
            AssertFileContains(circle, @"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=AXIS2_PLACEMENT_2D('',#1,#2);
#4=CIRCLE('',#3,5.0);
");
        }

        [Fact]
        public void ReadEllipseTest()
        {
            var ellipse = (StepEllipse)ReadTopLevelItem(@"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=AXIS2_PLACEMENT_2D('',#1,#2);
#4=ELLIPSE('',#3,3.0,4.0);
");
            Assert.Equal(new StepCartesianPoint("", 1.0, 2.0, 3.0), ellipse.Position.Location);
            Assert.Equal(new StepDirection("", 0.0, 0.0, 1.0), ellipse.Position.RefDirection);
            Assert.Equal(3.0, ellipse.SemiAxis1);
            Assert.Equal(4.0, ellipse.SemiAxis2);
        }

        [Fact]
        public void ReadTopLevelReferencedItemsTest()
        {
            var file = ReadFile(@"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=AXIS2_PLACEMENT_2D('',#1,#2);
#4=ELLIPSE('',#3,3.0,4.0);
");

            Assert.Equal(4, file.Items.Count);

            // only ELLIPSE() isn't referenced by another item
            var ellipse = (StepEllipse)file.GetTopLevelitems().Single();
        }

        [Fact]
        public void ReadTopLevelInlinedItemsTest()
        {
            var file = ReadFile("#1=ELLIPSE('',AXIS2_PLACEMENT_2D('',CARTESIAN_POINT('',(1.0,2.0,3.0)),DIRECTION('',(0.0,0.0,1.0))),3.0,4.0);");

            Assert.Equal(1, file.Items.Count);

            // only ELLIPSE() isn't referenced by another item
            var ellipse = (StepEllipse)file.GetTopLevelitems().Single();
        }

        [Fact]
        public void WriteEllipseTest()
        {
            var ellipse = new StepEllipse("", new StepAxis2Placement2D("", new StepCartesianPoint("", 1.0, 2.0, 3.0), new StepDirection("", 0.0, 0.0, 1.0)), 3.0, 4.0);
            AssertFileContains(ellipse, @"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=AXIS2_PLACEMENT_2D('',#1,#2);
#4=ELLIPSE('',#3,3.0,4.0);
");
        }

        [Fact]
        public void ReadEdgeCurveTest()
        {
            var edgeCurve = (StepEdgeCurve)ReadTopLevelItem(@"
#1=CIRCLE('',AXIS2_PLACEMENT_2D('',CARTESIAN_POINT('',(0.0,0.0,0.0)),DIRECTION('',(0.0,0.0,1.0))),5.0);
#2=EDGE_CURVE('',VERTEX_POINT('',CARTESIAN_POINT('',(1.0,2.0,3.0))),VERTEX_POINT('',CARTESIAN_POINT('',(4.0,5.0,6.0))),#1,.T.);
");
            Assert.IsType<StepCircle>(edgeCurve.EdgeGeometry);
            Assert.True(edgeCurve.IsSameSense);
        }

        [Fact]
        public void WriteEdgeCurveTest()
        {
            var edgeCurve = new StepEdgeCurve(
                "",
                new StepVertexPoint("", new StepCartesianPoint("", 1.0, 2.0, 3.0)),
                new StepVertexPoint("", new StepCartesianPoint("", 4.0, 5.0, 6.0)),
                new StepCircle("",
                    new StepAxis2Placement2D("", new StepCartesianPoint("", 7.0, 8.0, 9.0), new StepDirection("", 0.0, 0.0, 1.0)),
                    5.0),
                true);
            AssertFileContains(edgeCurve, @"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=VERTEX_POINT('',#1);
#3=CARTESIAN_POINT('',(4.0,5.0,6.0));
#4=VERTEX_POINT('',#3);
#5=CARTESIAN_POINT('',(7.0,8.0,9.0));
#6=DIRECTION('',(0.0,0.0,1.0));
#7=AXIS2_PLACEMENT_2D('',#5,#6);
#8=CIRCLE('',#7,5.0);
#9=EDGE_CURVE('',#2,#4,#8,.T.);
");
        }

        [Fact]
        public void ReadPlaneTest()
        {
            var plane = (StepPlane)ReadTopLevelItem(@"
#1=CARTESIAN_POINT('',(0.0,0.0,0.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=DIRECTION('',(1.0,0.0,0.0));
#4=AXIS2_PLACEMENT_3D('',#1,#2,#3);
#5=PLANE('',#4);
");
        }

        [Fact]
        public void WritePlaneTest()
        {
            var plane = new StepPlane(
                "",
                new StepAxis2Placement3D("", new StepCartesianPoint("", 1.0, 2.0, 3.0), new StepDirection("", 0.0, 0.0, 1.0), new StepDirection("", 1.0, 0.0, 0.0)));
            AssertFileContains(plane, @"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=DIRECTION('',(1.0,0.0,0.0));
#4=AXIS2_PLACEMENT_3D('',#1,#2,#3);
#5=PLANE('',#4);
");
        }

        [Fact]
        public void ReadOrientedEdgeTest()
        {
            var orientedEdge = (StepOrientedEdge)ReadTopLevelItem(@"
#1=CIRCLE('',AXIS2_PLACEMENT_2D('',CARTESIAN_POINT('',(0.0,0.0,0.0)),DIRECTION('',(0.0,0.0,1.0))),5.0);
#2=EDGE_CURVE('',VERTEX_POINT('',CARTESIAN_POINT('',(1.0,2.0,3.0))),VERTEX_POINT('',CARTESIAN_POINT('',(4.0,5.0,6.0))),#1,.T.);
#3=ORIENTED_EDGE('',*,*,#2,.T.);
");
            Assert.True(orientedEdge.Orientation);
        }

        [Fact]
        public void WriteOrientedEdgeTest()
        {
            var orientedEdge = new StepOrientedEdge(
                "",
                null,
                null,
                new StepEdgeCurve(
                    "",
                    new StepVertexPoint("", new StepCartesianPoint("", 1.0, 2.0, 3.0)),
                    new StepVertexPoint("", new StepCartesianPoint("", 4.0, 5.0, 6.0)),
                    new StepCircle("",
                        new StepAxis2Placement2D("", new StepCartesianPoint("", 7.0, 8.0, 9.0), new StepDirection("", 0.0, 0.0, 1.0)),
                        5.0),
                    true),
                true);
            AssertFileContains(orientedEdge, @"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=VERTEX_POINT('',#1);
#3=CARTESIAN_POINT('',(4.0,5.0,6.0));
#4=VERTEX_POINT('',#3);
#5=CARTESIAN_POINT('',(7.0,8.0,9.0));
#6=DIRECTION('',(0.0,0.0,1.0));
#7=AXIS2_PLACEMENT_2D('',#5,#6);
#8=CIRCLE('',#7,5.0);
#9=EDGE_CURVE('',#2,#4,#8,.T.);
#10=ORIENTED_EDGE('',*,*,#9,.T.);
");
        }

        [Fact]
        public void ReadEdgeLoopTest()
        {
            var edgeLoop = (StepEdgeLoop)ReadTopLevelItem(@"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.6,0.6,0.6));
#3=VECTOR('',#2,5.2);
#4=LINE('',#1,#3);
#5=EDGE_CURVE('',*,*,#4,.T.);
#6=ORIENTED_EDGE('',*,*,#5,.T.);
#7=CARTESIAN_POINT('',(7.0,8.0,9.0));
#8=VECTOR('',#2,5.2);
#9=LINE('',#7,#8);
#10=EDGE_CURVE('',*,*,#9,.F.);
#11=ORIENTED_EDGE('',*,*,#10,.F.);
#12=EDGE_LOOP('',(#6,#11));
");
            Assert.Equal(2, edgeLoop.EdgeList.Count);
        }

        [Fact]
        public void WriteEdgeLoopTest()
        {
            var edgeLoop = new StepEdgeLoop(
                "",
                new StepOrientedEdge("", null, null, new StepEdgeCurve("", null, null, StepLine.FromPoints(1.0, 2.0, 3.0, 4.0, 5.0, 6.0), true), true),
                new StepOrientedEdge("", null, null, new StepEdgeCurve("", null, null, StepLine.FromPoints(7.0, 8.0, 9.0, 10.0, 11.0, 12.0), false), false));
            AssertFileContains(edgeLoop, @"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.6,0.6,0.6));
#3=VECTOR('',#2,5.2);
#4=LINE('',#1,#3);
#5=EDGE_CURVE('',*,*,#4,.T.);
#6=ORIENTED_EDGE('',*,*,#5,.T.);
#7=CARTESIAN_POINT('',(7.0,8.0,9.0));
#8=VECTOR('',#2,5.2);
#9=LINE('',#7,#8);
#10=EDGE_CURVE('',*,*,#9,.F.);
#11=ORIENTED_EDGE('',*,*,#10,.F.);
#12=EDGE_LOOP('',(#6,#11));
");
        }
    }
}
