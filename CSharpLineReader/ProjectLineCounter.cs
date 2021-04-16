using System.IO;

namespace CSharpLineReader
{
  public class ProjectLineCounter : IProjectLineCounter
  {
    private readonly IGetSourceFilePathsInDirectory _getSourceFilePathsInDirectory;
    private readonly IGetSourceFileContentsFromFilePath _getSourceFileContentsFromFilePath;

    public ProjectLineCounter(IGetSourceFilePathsInDirectory getSourceFilePathsInDirectory,
      IGetSourceFileContentsFromFilePath getSourceFileContentsFromFilePath)
    {
      _getSourceFilePathsInDirectory = getSourceFilePathsInDirectory;
      _getSourceFileContentsFromFilePath = getSourceFileContentsFromFilePath;
    }

    public int CountLines(string directoryPath)
    {
      var allFiles = _getSourceFilePathsInDirectory.GetFiles(directoryPath);
      var totalLines = 0;

      foreach (var filePath in allFiles)
      {
        using var fileStream = _getSourceFileContentsFromFilePath.GetContents(filePath);
        using var streamReader = new StreamReader(fileStream);
        var source = streamReader.ReadToEnd();
        var lineCounter = new LineCounter();
        totalLines += lineCounter.CountLines(source);
      }

      return totalLines;
    }
  }

  public interface IProjectLineCounter
  {
    int CountLines(string directoryPath);
  }
}