using System;

namespace CSharpLineReader
{
  public readonly struct InputManifestation : IEquatable<InputManifestation>
  {
    public static readonly InputManifestation EndOfFile = new InputManifestation(0, TokenType.EndOfFile);
    public static readonly InputManifestation MultiLineStart = new InputManifestation(2, TokenType.MultiLineStart);
    public static readonly InputManifestation MultiLineEnd = new InputManifestation(2, TokenType.MultiLineEnd);
    public static readonly InputManifestation SingleLine = new InputManifestation(2, TokenType.SingleLine);
    public static readonly InputManifestation LineBreak = new InputManifestation(1, TokenType.LineBreak);
    public static InputManifestation Unknown(int length) => new InputManifestation(length, TokenType.Unknown);
    
    public readonly int Length;
    public readonly TokenType Type;

    public InputManifestation(int length, TokenType type)
    {
      Length = length;
      Type = type;
    }

    public bool Equals(InputManifestation other)
    {
      return Type == other.Type && Length == other.Length;
    }

    public override bool Equals(object obj)
    {
      return obj is InputManifestation other && Equals(other);
    }

    public override int GetHashCode()
    {
      return ((int)Type << 16) | Length;
    }

    public static bool operator ==(InputManifestation left, InputManifestation right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(InputManifestation left, InputManifestation right)
    {
      return !left.Equals(right);
    }

    public static bool operator ==(InputManifestation left, TokenType right)
    {
      return left.Type == right;
    }

    public static bool operator !=(InputManifestation left, TokenType right)
    {
      return left.Type != right;
    }

    public static bool operator ==(TokenType right, InputManifestation left)
    {
      return right == left.Type;
    }

    public static bool operator !=(TokenType right, InputManifestation left)
    {
      return right != left.Type;
    }

  }

  public enum TokenType
  {
    EndOfFile,
    MultiLineStart,
    MultiLineEnd,
    SingleLine,
    LineBreak,
    Unknown
  }
}