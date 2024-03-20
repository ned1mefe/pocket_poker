public enum Kind{
    Spade,
    Diamond,
    Club,
    Heart
}
public class Card
{
    public Card(short number, Kind kind)
    {
        Number = number;
        Kind = kind;
    }
    
    public readonly short Number;
    public readonly Kind Kind;

    public override string ToString()
    {
        const string faces = "JQKA";
        return Kind + " " + (Number > 10 ? faces[Number-11] : Number);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Card other = (Card)obj;

        return (other.Kind == Kind) && (other.Number == Number);
    }

    public static bool operator >(Card l, Card f)
    {
        return l.Number > f.Number;
    }

    public static bool operator <(Card l, Card f)
    {
        return l.Number < f.Number;
    }

    public static bool operator >=(Card l, Card f)
    {
        return l.Number >= f.Number;
    }

    public static bool operator <=(Card l, Card f)
    {
        return l.Number <= f.Number;
    }
    
    public override int GetHashCode()
    {
        return (Kind.GetHashCode()) ^ (Number.GetHashCode());
    }
}
