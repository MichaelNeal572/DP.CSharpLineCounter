using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

  public class GetSourceFilePathsInDirectory : IGetSourceFilePathsInDirectory
  {
    public IEnumerable<string> GetFiles(string directoryPath)
    {
      return Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);
    }
  }

  public interface IGetSourceFilePathsInDirectory
  {
    IEnumerable<string> GetFiles(string directoryPath);
  }

  public class GetSourceFileContentsFromFilePath : IGetSourceFileContentsFromFilePath
  {
    public Stream GetContents(string filePath)
    {
      return File.Open(filePath, FileMode.Open, FileAccess.Read);
    }
  }

  public interface IGetSourceFileContentsFromFilePath
  {
    Stream GetContents(string filePath);
  }

  public class LineCounter
  {
    public int CountLines(string input)
    {
      var encounteredUnknown = false;
      var lineCount = 0;
      var inputWalker = new InputWalker(input);

      foreach (var manifestation in inputWalker.Walk())
      {
        if (manifestation == TokenType.Unknown)
        {
          encounteredUnknown = true;
        }

        if (manifestation == InputManifestation.LineBreak && encounteredUnknown)
        {
          lineCount++;
          encounteredUnknown = false;
        }
      }

      return lineCount;
    }
  }

  public class LetterCounter
  {
    public int CountLetters(string input)
    {
      var letterCount = 0;
      var inputWalker = new InputWalker(input);

      foreach (var manifestation in inputWalker.Walk())
        if (manifestation == TokenType.Unknown)
          letterCount += manifestation.Length;

      return letterCount;
    }
  }
}