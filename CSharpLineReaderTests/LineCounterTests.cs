using System.Runtime.InteropServices.ComTypes;
using CSharpLineReader;
using FluentAssertions;
using NUnit.Framework;

namespace CSharpLineReaderTests
{
    [TestFixture]
    public class LineCounterTests
    {
        [TestFixture]
        public class CountLines
        {
            [Test]
            public void GivenEmptyString_ShouldReturn0()
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines("");

                //Assert
                actual.Should().Be(0);
            }
            
            [Test]
            public void GivenNullString_ShouldReturn0()
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(null);

                //Assert
                actual.Should().Be(0);
            }

            [Test]
            public void GivenLineStartsWithForwardSlash_ShouldCountAsLine()
            {
                // Arrange
                var sut = CreateSut();
                // Act
                var actual = sut.CountLines(@"var bob = 5\n
                                            / 3; ");

                //Assert
                actual.Should().Be(2);
            }

            [Test]
            public void GivenLineEndsInSingleLineComment_ShouldCountAsLine()
            {
                // Arrange
                var sut = CreateSut();
                // Act
                var actual = sut.CountLines(@"var bob = 5/ 3; //comment");

                //Assert
                actual.Should().Be(1);
            }
            
            [TestCase("var a = 0;")]
            [TestCase("int c = 5;")]
            [TestCase("b != 7")]
            public void Given1Line_ShouldReturn1(string input)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(1);
            }

            [TestCase("var a = 0;\nvar a = 0")]
            [TestCase("double a = 0;\nvar a = 0")]
            [TestCase("return\nvar b = 4;")]
            public void Given2Lines_ShouldReturn2(string input)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(2);
            }
            
            [TestCase("var a = 0;\nreturn\nreturn")]
            [TestCase("return;\nreturn\nreturn")]
            [TestCase("return;\nreturn aaaaaaaaaaaaaaaaaaa\nreturn")]
            public void Given3Lines_ShouldReturn3(string input)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(3);
            }
            
            [TestCase("return;\n", 1)]
            [TestCase("int a = true;\nreturn\n", 2)]
            [TestCase("return\nreturn\nreturn\n", 3)]
            public void GivenLastLineIsEmpty_ShouldNotCountTheLine(string input, int expected)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(expected);
            }
            
            [TestCase("\nreturn;", 1)]
            [TestCase("\nint a = true;\nreturn", 2)]
            [TestCase("\nreturn\nreturn\nreturn", 3)]
            public void GivenFirstLineIsEmpty_ShouldNotCountTheLine(string input, int expected)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(expected);
            }
            
            [TestCase("\nreturn;", 1)]
            [TestCase("int a = true;\n\nreturn", 2)]
            [TestCase("return\nreturn\n\nreturn", 3)]
            public void GivenAnyLineIsEmpty_ShouldNotCountTheLine(string input, int expected)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(expected);
            }
            
            [TestCase("\n\nreturn;", 1)]
            [TestCase("int a = true;\n\nreturn\n", 2)]
            [TestCase("return\n\nreturn\n\n\nreturn", 3)]
            public void GivenManyLinesAreEmpty_ShouldNotCountTheEmptyLines(string input, int expected)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(expected);
            }
            
            [TestCase("\n//comment\nreturn;", 1)]
            [TestCase("int a = true;\n//Comment\nreturn\n", 2)]
            [TestCase("return\n\nreturn\n//Comment\n//Comment\nreturn", 3)]
            [TestCase("return\n\nreturn\n\n\nreturn\n//Comment", 3)]
            public void GivenManyLinesAreComments_ShouldNotCountTheCommentLines(string input, int expected)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(expected);
            }

            [TestCase(" //Comment", 0)]
            [TestCase("\t//Comment", 0)]
            [TestCase(" \t return;", 1)]
            [TestCase(" \t return;\n return;\n //coment", 2)]
            public void ShouldIgnoreWhiteSpaceBeforeLines(string input, int expected)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(expected);
            }

            [TestCase(" /*Comment*/", 0)]
            [TestCase("\t/*//Comment\n*/", 0)]
            [TestCase("/*foreach(var i in things)\n{\n return\n}*/", 0)]
            [TestCase("var a = 0;/*foreach(var i in things)\n{\n return\n}*/", 1)]
            [TestCase("/*foreach(var i in things)\n{\n return\n}*/var a = 0;", 1)]
            [TestCase("///*asdfasdfasdfasdfasdf\nasdfasdf*/", 1)]
            [TestCase("asdfasdf///*asdfasdfasdfasdfasdf\nasdfasdf*/", 2)]
            [TestCase("asdfasdf///*\n*/", 2)]
            [TestCase("/**/asdfasdf", 1)]
            [TestCase("/**/asdfasdf", 1)]
            public void ShouldIgnoreAllLinesOfMultilineComments(string input, int expected)
            {
              // Arrange
              var sut = CreateSut();

              // Act
              var actual = sut.CountLines(input);

              //Assert
              actual.Should().Be(expected);
            }


            [TestCase("Bob(\"/*Comment*/\")", 1)]
            [TestCase("Bob(@\"/*Comment*/\")", 1)]
            [TestCase(@"Bob(@""/*Comment*/"")", 1)]
            [TestCase(@"Bob(@""/*Com
1
ment*/"")", 2)]
            [TestCase(@"Bob(@""aasddsadasd
//Comment*/"")", 2)]
            [TestCase(@"Bob(@""aasddsadasd
1
2
//Comment*/"")", 2)]
            public void ShouldNotTreatCommentSyntaxWithinStringLiteralsAsComments(string input, int expected)
            {
                // Arrange
                var sut = CreateSut();

                // Act
                var actual = sut.CountLines(input);

                //Assert
                actual.Should().Be(expected);
            }
        }

        private static LineCounter CreateSut()
        {
            var sut = new LineCounter();
            return sut;
        }
    }
}