using System;
using System.Linq;

namespace CSharpLineReader
{


    public class LineCounter
    {
        public int CountLines(string input)
        {
            var iterator = new InputIterator(input);
            var context = new ParserContext();


            while(iterator.MoveNext())
            {
                if (iterator.IsSingleLineComment())
                {
                    context.IsWithinSingleLineComment = true;
                }

                if (iterator.IsSourceCharacter() && !context.IsWithinSingleLineComment)
                {
                    context.HasNonWhiteSpaceCharacter = true;
                }

                if (iterator.IsNewLine())
                {
                    if (context.HasNonWhiteSpaceCharacter)
                    {
                        context.LineCount++;
                    }

                    context.HasNonWhiteSpaceCharacter = false;
                    context.IsWithinSingleLineComment = false;
                }
            }

            return context.LineCount;
        }

        private class ParserContext
        {

            public bool HasNonWhiteSpaceCharacter { get; set; }
            public bool IsWithinSingleLineComment { get; set; }
            public int LineCount { get; set; }

        }

        private class InputIterator
        {
            private string _text;
            private int _index;

            public InputIterator(string text)
            {
                _text = text ?? "";
                _text += "\n\n";
                _index = -1;
            }

            public bool IsSingleLineComment()
            {
                var value = Value;
                return  value.Current== '/' && value.Next == '/';
            }

            public bool IsSourceCharacter()
            {
                var current = Value.Current;
                return !Char.IsWhiteSpace(current);
            }

            public bool IsNewLine()
            {
                var current = Value.Current;
                return current == '\n';
            }

            public bool MoveNext()
            {
                if (_index < _text.Length - 2)
                {
                    _index++;
                    return true;
                }

                return false;
            }

            public (char Current, char Next) Value => (_text[_index], _text[_index + 1]);

        }
    }
}