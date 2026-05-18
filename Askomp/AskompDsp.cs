namespace Askomp;

public class AskompDsp : mydsp
{
    public double Attack
    {
        get => fHslider1;
        set => fHslider1 = value;
    }

    public double NegativeThreshold
    {
        get => fVslider0;
        set => fVslider0 = value;
    }

    public double Release
    {
        get => fHslider0;
        set => fHslider0 = value;
    }

    public double Ratio
    {
        get => fHslider2;
        set => fHslider2 = value;
    }

    public double PositiveThreshold
    {
        get => fVslider1;
        set => fVslider1 = value;
    }

    public double Trim
    {
        get => fHslider3;
        set => fHslider3 = value;
    }
}