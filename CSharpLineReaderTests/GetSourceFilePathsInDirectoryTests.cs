using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CSharpLineReader;
using FluentAssertions;
using NUnit.Framework;

namespace CSharpLineReaderTests
{
  [TestFixture]
  public class GetSourceFilePathsInDirectoryTests
  {
    [TestCase(@"Resources")]
    public void ShouldOnlyReturnCSharpSourceFiles(string relativePath)
    {
      // Arrange
      var inputFolder = GetFullPathFromRelativePath(relativePath);
      var sut = CreateSut();
      // Act
      var filePaths = sut.GetFiles(inputFolder);
      //Assert
      filePaths.Select(s => Path.GetExtension(s).ToLowerInvariant()).Distinct().Should().BeEquivalentTo(".cs");
    }
    
    [TestCase(@"Resources\Should Allow Mixed Case Extensions")]
    public void ShouldAllowMixedCaseExtensions(string relativePath)
    {
      // Arrange
      var inputFolder = GetFullPathFromRelativePath(relativePath);
      var sut = CreateSut();
      // Act
      var filePaths = sut.GetFiles(inputFolder);
      //Assert
      filePaths.Select(Path.GetExtension).Distinct().Should().BeSubsetOf(new [] {".cS", ".CS"});
    }

    [TestCase(@"Resources\No Files In Root")]
    public void ShouldReturnNoDuplicateSourceFilePaths(string relativePath)
    {
      // Arrange
      var inputFolder = GetFullPathFromRelativePath(relativePath);
      var sut = CreateSut();
      // Act
      var filePaths = sut.GetFiles(inputFolder);
      //Assert
      filePaths.Should().OnlyHaveUniqueItems();
    }

    [TestCase(@"Resources\Does Recurse Deeply")]
    public void ShouldRecursivelyFetchFilesFromNestedDirectories(string relativePath)
    {
      // Arrange
      var inputFolder = GetFullPathFromRelativePath(relativePath);
      var sut = CreateSut();
      // Act
      var filePaths = sut.GetFiles(inputFolder);
      //Assert
      filePaths.Should().HaveCount(2);
    }

    [TestCase(@"Resources\No Files")]
    public void GivenNoSourceFilesInPath_ShouldReturnAnEmptyList(string relativePath)
    {
      // Arrange
      var inputFolder = GetFullPathFromRelativePath(relativePath);
      var sut = CreateSut();
      // Act
      var filePaths = sut.GetFiles(inputFolder);
      //Assert
      filePaths.Should().BeEmpty();
    }

    [TestCase(@"Resources\Files In Root")]
    public void GivenSourceFilesExistInRootFolder_ShouldReturnSourceFilePaths(string relativePath)
    {
      // Arrange
      var inputFolder = GetFullPathFromRelativePath(relativePath);
      var sut = CreateSut();
      // Act
      var filePaths = sut.GetFiles(inputFolder);
      //Assert
      filePaths.Select(GetRelativePathFromFullPath).Should()
        .Contain(@"Resources\Files In Root\MoreSexySource.cs");
    }
    
    [TestCase(@"Resources\No Files In Root")]
    public void GivenSourceFilesExistInNestedFolder_ShouldReturnSourceFilePaths(string relativePath)
    {
      // Arrange
      var inputFolder = GetFullPathFromRelativePath(relativePath);
      var sut = CreateSut();
      // Act
      var filePaths = sut.GetFiles(inputFolder);
      //Assert
      filePaths.Select(GetRelativePathFromFullPath).Should()
        .Contain(@"Resources\No Files In Root\Nested Folder\HeNeverImagined.cs");
    }



    private static string GetRelativePathFromFullPath(string fullPath)
    {
      return Path.GetRelativePath(TestContext.CurrentContext.TestDirectory, fullPath);
    }

    private static string GetFullPathFromRelativePath(string resourcesFilesInRoot)
    {
      return Path.Combine(TestContext.CurrentContext.TestDirectory, resourcesFilesInRoot);
    }

    private static GetSourceFilePathsInDirectory CreateSut()
    {
      return new GetSourceFilePathsInDirectory();
    }
  }
}