using System.IO;

namespace CSharpLineReader
{
  public class ProjectLineCounter
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
      var totalLetters = 0;

      foreach (var filePath in allFiles)
      {
        using var fileStream = _getSourceFileContentsFromFilePath.GetContents(filePath);
        using var streamReader = new StreamReader(fileStream);
        var source = streamReader.ReadToEnd();
        var lineCounter = new LineCounter();
        totalLines += lineCounter.CountLines(source);
        var letterCounter = new LetterCounter();
        totalLetters += letterCounter.CountLetters(source);
      }

      return totalLines;
    }
  }
}