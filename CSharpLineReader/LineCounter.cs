using System;
using System.Collections.Generic;
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
                if (manifestation == InputManifestation.Unknown)
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

        public enum InputManifestation
        {
            EndOfFile,
            MultiLineStart,
            MultiLineEnd,
            SingleLine,
            LineBreak,
            Unknown
        }

        private class InputWalker
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
                        _context.HasNonWhiteSpaceCharacter = true;
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
                        _context.HasNonWhiteSpaceCharacter = false;
                        return new [] {InputManifestation.Unknown, manifestationImOn};
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

            private (char Current, char Next) Value => (_text[_index], _text[_index + 1]);

            private class ParserContext
            {
                public bool HasNonWhiteSpaceCharacter { get; set; }
                public bool IsWithinSingleLineComment { get; set; }
                public bool IsWithinMultiLineComment { get; set; }
                public bool IsWithinComment => IsWithinMultiLineComment || IsWithinSingleLineComment;
            }
        }
    }
}