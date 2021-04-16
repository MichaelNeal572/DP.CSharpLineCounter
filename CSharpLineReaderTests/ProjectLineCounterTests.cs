using System;
using System.Collections.Generic;
using System.Text;
using CSharpLineReader;
using NUnit.Framework;

namespace CSharpLineReaderTests
{
  [TestFixture]
  public class ProjectLineCounterTests
  {
    [Test]
    public void ShouldLoadFilesFromDirectory()
    {
      // Arrange
      var sut = CreateSut();
      // Act
      var totalLines = sut.CountLines("C:\\Systems\\HOMiiIT");
      // Assert
      Assert.AreEqual(totalLines, 432423423);
    }

    private static ProjectLineCounter CreateSut()
    {
      return new ProjectLineCounter(new GetSourceFilePathsInDirectory(), new GetSourceFileContentsFromFilePath());
    }
  }
}
