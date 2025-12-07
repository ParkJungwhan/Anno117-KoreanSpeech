namespace ToolMain.Lib;

public class Calc
{
    public static string ToHex(int value)
    {
        if (value == 0) return "0";

        const string hexChars = "0123456789ABCDEF";
        string result = "";

        while (value > 0)
        {
            int remainder = value % 16;
            result = hexChars[remainder] + result;
            value /= 16;
        }

        return result;
    }

    public static string ToHexbylong(long value, bool lowercase = false, int minWidth = 0)
    {
        string format = lowercase ? "x" : "X";

        if (minWidth > 0)
            format = (lowercase ? "x" : "X") + minWidth;

        return value.ToString(format);
    }
}