using System;
using System.Collections.Generic;

namespace CSharpLineReader
{
  public class InputWalker
  {
    private string _text;
    private int _index;
    private ParserContext _context = new ParserContext();

    public InputWalker(string text)
    {
      _text = text ?? "";
      _text += "\n\n";
      _index = -1;
    }

    public override string ToString()
    {
      return $"{_index} - {_text} - {(_index<0?"NA":_text[_index].ToString())}";
    }

    private bool IsSingleLineComment()
    {
      var value = Value;
      return  value.Current== '/' && value.Next == '/';
    }

    private bool IsStartOfMultiLineComment()
    {
      var value = Value;
      return  value.Current== '/' && value.Next == '*';
    }

    private bool IsEndOfMultiLineComment()
    {
      var value = Value;
      return  value.Current== '*' && value.Next == '/';
    }

    private bool IsStartOfVerbatimStringLiteral()
    {
      var value = Value;
      return  value.Current== '@' && value.Next == '"';
    }

    private bool IsStartOfStringLiteral()
    {
      var value = Value;
      return  value.Current == '"';
    }

    private bool IsEndOfStringLiteral()
    {
      var value = Value;
      if (value.Current != '"') return false;
      if (_context.IsWithinVerbatimStringLiteral && value.Next == '"') return false;
      if (!_context.IsWithinVerbatimStringLiteral && value.Previous == '\\') return false;
      return true;
    }

    private bool IsSourceCharacter()
    {
      var current = Value.Current;
      return !Char.IsWhiteSpace(current);
    }

    private bool IsNewLine()
    {
      var current = Value.Current;
      return current == '\n';
    }

    public IEnumerable<InputManifestation> Walk()
    {
      foreach (var steps in WalkInPotentialLargeSteps())
      {
        foreach (var step in steps)
        {
          yield return step;
        }
      }
    }
    private IEnumerable<InputManifestation[]> WalkInPotentialLargeSteps()
    {
      while (AdvanceInputCharacter())
      {
        if (!_context.IsWithinStringLiteral)
        {
          
        }
        else if (IsEndOfStringLiteral())
        {

        }


        if (!_context.IsWithinComment)
        {
          if (IsSingleLineComment())
          {
            _context.IsWithinSingleLineComment = true;
            AdvanceInputCharacter();
            yield return GetAccumulatedManifestations(InputManifestation.SingleLine);
            continue;
          }

          if (IsStartOfMultiLineComment())
          {
            _context.IsWithinMultiLineComment = true;
            AdvanceInputCharacter();
            yield return GetAccumulatedManifestations(InputManifestation.MultiLineStart);
            continue;
          }
        }
        else if (_context.IsWithinMultiLineComment && IsEndOfMultiLineComment())
        {
          _context.IsWithinMultiLineComment = false;
          AdvanceInputCharacter();
          yield return GetAccumulatedManifestations(InputManifestation.MultiLineEnd);
          continue;
        }

        if (IsSourceCharacter() && !_context.IsWithinComment)
        {
          _context.EncounteredNonWhiteSpaceCharacter();
        }

        if (IsNewLine())
        {
          _context.IsWithinSingleLineComment = false;

          yield return GetAccumulatedManifestations(InputManifestation.LineBreak);

          _context.HasNonWhiteSpaceCharacter = false;
        }
      }
      yield return GetAccumulatedManifestations(InputManifestation.EndOfFile);
                
      InputManifestation[] GetAccumulatedManifestations(InputManifestation manifestationImOn){
        if (_context.HasNonWhiteSpaceCharacter)
        {
          var length = _context.NumberOfNonWhiteSpaceCharacters;
          _context.HasNonWhiteSpaceCharacter = false;
          return new [] {InputManifestation.Unknown(length), manifestationImOn};
        }
        return new [] { manifestationImOn };
      }
    }

    private bool AdvanceInputCharacter()
    {
      if (_index < _text.Length - 2)
      {
        _index++;
        return true;
      }

      return false;
    }

    private (char Previous, char Current, char Next) Value => (_index < 1 ? '\0' : _text[_index - 1], _text[_index], _text[_index + 1]);

    private class ParserContext
    {
      private bool _hasNonWhiteSpaceCharacter;
      private int _numberOfNonWhiteSpaceCharacters;

      public bool HasNonWhiteSpaceCharacter
      {
        get => _hasNonWhiteSpaceCharacter;
        set
        {
          if (value) EncounteredNonWhiteSpaceCharacter();
          else
          {
            _hasNonWhiteSpaceCharacter = false;
            _numberOfNonWhiteSpaceCharacters = 0;
          }
        }
      }

      public int NumberOfNonWhiteSpaceCharacters => _numberOfNonWhiteSpaceCharacters;

      public bool IsWithinSingleLineComment { get; set; }
      public bool IsWithinMultiLineComment { get; set; }
      public bool IsWithinRegularStringLiteral { get; set; }
      public bool IsWithinVerbatimStringLiteral { get; set; }
      public bool IsWithinComment => IsWithinMultiLineComment || IsWithinSingleLineComment;
      public bool IsWithinStringLiteral => IsWithinRegularStringLiteral || IsWithinVerbatimStringLiteral;

      public void EncounteredNonWhiteSpaceCharacter()
      {
        _hasNonWhiteSpaceCharacter = true;
        _numberOfNonWhiteSpaceCharacters++;
      }
    }
  }
}