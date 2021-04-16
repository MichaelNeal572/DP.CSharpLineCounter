namespace CSharpLineReader
{
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