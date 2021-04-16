using System.Collections.Generic;
using System.IO;

namespace CSharpLineReader
{
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
}