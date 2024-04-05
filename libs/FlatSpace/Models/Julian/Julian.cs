namespace FlatSpace.Models;

public class Julian
{
    private const double EPOCH_JAN0_12H_1900 = 2415020.0; // Dec 31.5 1899 = Dec 31 1899 12h UTC
    private const double EPOCH_JAN1_00H_1900 = 2415020.5; // Jan  1.0 1900 = Jan  1 1900 00h UTC
    private const double EPOCH_JAN1_12H_1900 = 2415021.0; // Jan  1.5 1900 = Jan  1 1900 12h UTC
    private const double EPOCH_JAN1_12H_2000 = 2451545.0; // Jan  1.5 2000 = Jan  1 2000 12h UTC
    private const double EPOCH_J2000 = 2451544.99926; // Jan  1 2000 11h 58m 55.816s UTC

    internal Julian(DateTime utc)
    {
        double day =
            utc.DayOfYear
            + (utc.Hour + (utc.Minute + (utc.Second + utc.Millisecond / 1000.0) / 60.0) / 60.0)
                / 24.0;

        int year = utc.Year;

        // Arbitrary years used for error checking
        if (year < 1900 || year > 2100)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        // The last day of a leap year is day 366
        if (day < 1.0 || day >= 367.0)
        {
            throw new ArgumentOutOfRangeException(nameof(day));
        }

        year--;

        int A = year / 100;
        int B = 2 - A + A / 4;

        double NewYears = (int)(365.25 * year) + (int)(30.6001 * 14) + 1720994.5 + B;

        Date = NewYears + day;
    }

    // Calculate Greenwich Mean Sidereal Time for the Julian date
    //public double ToGmst()
    //{
    //    double UT = (Date + 0.5) % 1.0;
    //    //double TU = (FromJan1_12h_2000() - UT) / 36525.0;
    //    double TU = (FromJan1_12h_2000() - UT) / 36525.0;


    //    double GMST = 24110.54841 + TU * (8640184.812866 + TU * (0.093104 - TU * 6.2e-06));

    //    GMST = (GMST + Globals.SecPerDay * Globals.OmegaE * UT) % Globals.SecPerDay;

    //    if (GMST < 0.0)
    //    {
    //        GMST += Globals.SecPerDay;  // "wrap" negative modulo value
    //    }

    //    return (2.0 * Math.PI * (GMST / Globals.SecPerDay));
    //}

    public double ToGmst()
    {
        // greenwich mean sidereal time
        return Meeus.Gast2(Date, 0.0, 0) * 360.0 / 24.0;
    }

    public double ToGast()
    {
        // greenwich apparent sidereal time
        return Meeus.Gast2(Date, 0.0, 1) * 360.0 / 24.0;
    }

    public double Date { get; private set; } // Julian date

    public double FromJan0_12h_1900()
    {
        return Date - EPOCH_JAN0_12H_1900;
    }

    public double FromJan1_00h_1900()
    {
        return Date - EPOCH_JAN1_00H_1900;
    }

    public double FromJan1_12h_1900()
    {
        return Date - EPOCH_JAN1_12H_1900;
    }

    public double FromJan1_12h_2000()
    {
        return Date - EPOCH_JAN1_12H_2000;
    }

    public double FromJ2000()
    {
        return Date - EPOCH_J2000;
    }
}
