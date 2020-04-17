using System.Linq;
using IxMilia.Step.Items;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepItemTests : StepTestBase
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
            return file.GetTopLevelItems().Single();
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
        public void ReadCartesianPointWithOmittedNameTest()
        {
            var point = (StepCartesianPoint)ReadTopLevelItem("#1=CARTESIAN_POINT($,(0.0,0.0,0.0));");
            Assert.Equal(string.Empty, point.Name);
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
            AssertFileContains(file, @"
#1=LINE('',CARTESIAN_POINT('',(1.0,2.0,3.0)),VECTOR('',DIRECTION('',(1.0,0.0,0.0
)),4.0));
", inlineReferences: true);
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
            var ellipse = (StepEllipse)file.GetTopLevelItems().Single();
        }

        [Fact]
        public void ReadTopLevelInlinedItemsTest()
        {
            var file = ReadFile("#1=ELLIPSE('',AXIS2_PLACEMENT_2D('',CARTESIAN_POINT('',(1.0,2.0,3.0)),DIRECTION('',(0.0,0.0,1.0))),3.0,4.0);");

            Assert.Single(file.Items);

            // only ELLIPSE() isn't referenced by another item
            var ellipse = (StepEllipse)file.GetTopLevelItems().Single();
        }

        [Fact]
        public void ReadBSplineWithKnotsItemsTest()
        {
            var spline = (StepBSplineCurveWithKnots)ReadTopLevelItem(@"
#1=CARTESIAN_POINT('Ctrl Pts',(-2.09228759117738,32.4775276519752,7.66388871568773));
#2=CARTESIAN_POINT('Ctrl Pts',(-2.09228759389655,30.5382976972817,7.66388872564781));
#3=CARTESIAN_POINT('Ctrl Pts',(-2.09228913953809,28.5997370344523,7.66389438721404));
#4=CARTESIAN_POINT('Ctrl Pts',(-2.09228986816456,26.5173645163537,7.66389705611683));
#5=CARTESIAN_POINT('Ctrl Pts',(-2.09228981902238,26.4462306775892,7.6638968761128));
#6=CARTESIAN_POINT('Ctrl Pts',(-2.0922902432834,25.9015378442672,7.66389843014835));
#7=CARTESIAN_POINT('Ctrl Pts',(-2.11494520023805,24.945781133428,7.74688179710576));
#8=CARTESIAN_POINT('Ctrl Pts',(-2.21905874762543,23.5389919187115,8.1282417231202));
#9=CARTESIAN_POINT('Ctrl Pts',(-2.39215391928761,22.2144888644552,8.76227603964325));
#10=CARTESIAN_POINT('Ctrl Pts',(-2.62666223390231,21.041382550633,9.62126198100665));
#11=CARTESIAN_POINT('Ctrl Pts',(-2.81820100260438,20.3836711077483,10.3228537766386));
#12=CARTESIAN_POINT('Ctrl Pts',(-2.91923318155533,20.0960030522361,10.6929268867829));
#13=B_SPLINE_CURVE_WITH_KNOTS('',3,(#1,#2,#3,#4,#5,#6,#7,#8,#9,#10,#11,#12)
,.UNSPECIFIED.,.F.,.F.,(4,2,2,1,1,1,1,4),(0.00162506910839039,0.4223270995939,
0.437186866643407,0.53596295034332,0.634739034043234,0.733515117743147,
0.832291201443061,0.927367642384199),.UNSPECIFIED.);
");
            Assert.Equal(12, spline.ControlPointsList.Count);
            Assert.Equal(8, spline.Knots.Count);
            Assert.Equal(8, spline.KnotMultiplicities.Count);
            Assert.Equal(3, spline.Degree);
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
#2=DIRECTION('',(0.58,0.58,0.58));
#3=VECTOR('',#2,5.2);
#4=LINE('',#1,#3);
#5=EDGE_CURVE('',*,*,#4,.T.);
#6=ORIENTED_EDGE('',*,*,#5,.T.);
#7=CARTESIAN_POINT('',(7.0,8.0,9.0));
#8=VECTOR('',#2,5.2);
#9=LINE('',#7,#8);
#10=EDGE_CURVE('',*,*,#9,.F.);
#11=EDGE_LOOP('',(#6,#12));
/* ensure that forward references are resolved when binding */
#12=ORIENTED_EDGE('',*,*,#10,.F.);
");
            Assert.Equal(2, edgeLoop.EdgeList.Count);
            Assert.NotNull(edgeLoop.EdgeList[0]);
            Assert.NotNull(edgeLoop.EdgeList[1]);
        }

        [Fact]
        public void ReadAdvancedFaceTest()
        {
            var file = ReadFile(@"
#1=FACE_OUTER_BOUND('',#2,.T.);
#2=EDGE_LOOP('',(#12,#13,#14,#15));
#3=LINE('',#31,#4);
#4=VECTOR('',#24,2.5);
#5=CIRCLE('',#18,2.5);
#6=CIRCLE('',#19,2.5);
#7=VERTEX_POINT('',#28);
#8=VERTEX_POINT('',#30);
#9=EDGE_CURVE('',#7,#7,#5,.T.);
#10=EDGE_CURVE('',#7,#8,#3,.T.);
#11=EDGE_CURVE('',#8,#8,#6,.T.);
#12=ORIENTED_EDGE('',*,*,#9,.F.);
#13=ORIENTED_EDGE('',*,*,#10,.T.);
#14=ORIENTED_EDGE('',*,*,#11,.F.);
#15=ORIENTED_EDGE('',*,*,#10,.F.);
#16=CYLINDRICAL_SURFACE('',#17,2.5);
#17=AXIS2_PLACEMENT_3D('',#27,#20,#21);
#18=AXIS2_PLACEMENT_3D('',#29,#22,#23);
#19=AXIS2_PLACEMENT_3D('',#32,#25,#26);
#20=DIRECTION('center_axis',(0.,0.,-1.));
#21=DIRECTION('ref_axis',(-1.,0.,0.));
#22=DIRECTION('center_axis',(0.,0.,-1.));
#23=DIRECTION('ref_axis',(-1.,0.,0.));
#24=DIRECTION('',(0.,0.,-1.));
#25=DIRECTION('center_axis',(0.,0.,1.));
#26=DIRECTION('ref_axis',(-1.,0.,0.));
#27=CARTESIAN_POINT('Origin',(0.,0.,5.));
#28=CARTESIAN_POINT('',(2.5,3.06161699786838E-16,5.));
#29=CARTESIAN_POINT('Origin',(0.,0.,5.));
#30=CARTESIAN_POINT('',(2.5,3.06161699786838E-16,0.));
#31=CARTESIAN_POINT('',(2.5,-3.06161699786838E-16,5.));
#32=CARTESIAN_POINT('Origin',(0.,0.,0.));
#33=ADVANCED_FACE('',(#1),#16,.F.);
");
            var face = file.GetTopLevelItems().OfType<StepAdvancedFace>().FirstOrDefault();
            Assert.NotNull(face);
            Assert.NotNull(face.FaceGeometry);
            Assert.Single(face.Bounds);
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
#2=DIRECTION('',(0.58,0.58,0.58));
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

        [Fact]
        public void ReadFaceBoundTest()
        {
            var faceBound = (StepFaceBound)ReadTopLevelItem(@"
#1=CARTESIAN_POINT('',(0.0,0.0,0.0));
#2=DIRECTION('',(1.0,0.0,0.0));
#3=VECTOR('',#2,1.0);
#4=LINE('',#1,#3);
#5=EDGE_CURVE('',*,*,#4,.T.);
#6=ORIENTED_EDGE('',*,*,#5,.T.);
#7=EDGE_LOOP('',(#6));
#8=FACE_BOUND('',#7,.T.);
");
            Assert.True(faceBound.Orientation);
        }

        [Fact]
        public void WriteFaceBoundTest()
        {
            var faceBound = new StepFaceBound(
                "",
                new StepEdgeLoop(
                    "",
                    new StepOrientedEdge(
                        "",
                        null,
                        null,
                        new StepEdgeCurve(
                            "",
                            null,
                            null,
                            StepLine.FromPoints(0.0, 0.0, 0.0, 1.0, 0.0, 0.0),
                            true),
                        true)),
                true);
            AssertFileContains(faceBound, @"
#1=CARTESIAN_POINT('',(0.0,0.0,0.0));
#2=DIRECTION('',(1.0,0.0,0.0));
#3=VECTOR('',#2,1.0);
#4=LINE('',#1,#3);
#5=EDGE_CURVE('',*,*,#4,.T.);
#6=ORIENTED_EDGE('',*,*,#5,.T.);
#7=EDGE_LOOP('',(#6));
#8=FACE_BOUND('',#7,.T.);
");
        }

        [Fact]
        public void ReadFaceOuterBoundTest()
        {
            var faceOuterBound = (StepFaceOuterBound)ReadTopLevelItem(@"
#1=CARTESIAN_POINT('',(0.0,0.0,0.0));
#2=DIRECTION('',(1.0,0.0,0.0));
#3=VECTOR('',#2,1.0);
#4=LINE('',#1,#3);
#5=EDGE_CURVE('',*,*,#4,.T.);
#6=ORIENTED_EDGE('',*,*,#5,.T.);
#7=EDGE_LOOP('',(#6));
#8=FACE_OUTER_BOUND('',#7,.T.);
");
            Assert.True(faceOuterBound.Orientation);
        }

        [Fact]
        public void WriteFaceOuterBoundTest()
        {
            var faceOuterBound = new StepFaceOuterBound(
                "",
                new StepEdgeLoop(
                    "",
                    new StepOrientedEdge(
                        "",
                        null,
                        null,
                        new StepEdgeCurve(
                            "",
                            null,
                            null,
                            StepLine.FromPoints(0.0, 0.0, 0.0, 1.0, 0.0, 0.0),
                            true),
                        true)),
                true);
            AssertFileContains(faceOuterBound, @"
#1=CARTESIAN_POINT('',(0.0,0.0,0.0));
#2=DIRECTION('',(1.0,0.0,0.0));
#3=VECTOR('',#2,1.0);
#4=LINE('',#1,#3);
#5=EDGE_CURVE('',*,*,#4,.T.);
#6=ORIENTED_EDGE('',*,*,#5,.T.);
#7=EDGE_LOOP('',(#6));
#8=FACE_OUTER_BOUND('',#7,.T.);
");
        }

        [Fact]
        public void ReadCylindricalSurfaceTest()
        {
            var surface = (StepCylindricalSurface)ReadTopLevelItem(@"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=DIRECTION('',(1.0,0.0,0.0));
#4=AXIS2_PLACEMENT_3D('',#1,#2,#3);
#5=CYLINDRICAL_SURFACE('',#4,12.0);
");
            Assert.Equal(12.0, surface.Radius);
        }

        [Fact]
        public void WriteCylindricalSurfaceTest()
        {
            var surface = new StepCylindricalSurface(
                "",
                new StepAxis2Placement3D(
                    "",
                    new StepCartesianPoint("", 1.0, 2.0, 3.0),
                    new StepDirection("", 0.0, 0.0, 1.0),
                    new StepDirection("", 1.0, 0.0, 0.0)),
                12.0);
            AssertFileContains(surface, @"
#1=CARTESIAN_POINT('',(1.0,2.0,3.0));
#2=DIRECTION('',(0.0,0.0,1.0));
#3=DIRECTION('',(1.0,0.0,0.0));
#4=AXIS2_PLACEMENT_3D('',#1,#2,#3);
#5=CYLINDRICAL_SURFACE('',#4,12.0);
");
        }

        [Fact]
        public void WriteBSplineWithKnotsTest()
        {
            var spline = new StepBSplineCurveWithKnots(
                "",
                new StepCartesianPoint("", 0.0, 0.0, 0.0),
                new StepCartesianPoint("", 1.0, 0.0, 0.0),
                new StepCartesianPoint("", 1.0, 2.0, 0.0));
            spline.KnotMultiplicities.Add(1);
            spline.Knots.Add(2.0);

            AssertFileContains(spline, @"
#1=CARTESIAN_POINT('',(0.0,0.0,0.0));
#2=CARTESIAN_POINT('',(1.0,0.0,0.0));
#3=CARTESIAN_POINT('',(1.0,2.0,0.0));
#4=B_SPLINE_CURVE_WITH_KNOTS('',0,(#1,#2,#3),.UNSPECIFIED.,.F.,.F.,(1),(2.0),
.UNSPECIFIED.);
");
        }
    }
}
