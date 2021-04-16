using System.IO;

namespace CSharpLineReader
{
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
}