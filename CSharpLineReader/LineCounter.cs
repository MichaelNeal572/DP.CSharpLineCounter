using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpLineReader
{
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
}