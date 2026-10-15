namespace MarketAnalyzer.Captcha
{
  public readonly struct Padding
  {
    public Padding(int padding) : this(padding, padding)
    {

    }

    public Padding(int updown, int rightleft) : this(updown, rightleft, updown, rightleft)
    {

    }

    public Padding(int top, int right, int bottom, int left)
    {
      Top = top;
      Right = right;
      Bottom = bottom;
      Left = left;
    }

    public int Top { get; }

    public int Right { get; }

    public int Bottom { get; }

    public int Left { get; }

    public override bool Equals(object obj)
    {
      return obj is Padding padding && padding.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode()
    {
      return Top ^ Right ^ Bottom ^ Left;
    }

    public static bool operator ==(Padding left, Padding right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(Padding left, Padding right)
    {
      return !(left == right);
    }
  }
}
