
//	CTime.cs
//	Author: Lu Zexi
//	2014-12-07



//time util
public class CTime
{
	//convert from unix time to string date time
    public static string UNIXTimeToDateTimeString(long time)
    {
        DateTime date = UNIXTimeToDateTime(time);
        return date.ToString("yyyy/MM/dd hh:mm:ss");
    }

    //convert from unix time to string date
    public static string UNIXTimeToDateString(long time)
    {
        DateTime date = UNIXTimeToDateTime(time);
        return date.ToString("yyyy-MM-dd");
    }

    //convert form unix time to c# time
    public static DateTime UNIXTimeToDateTime(long time)
    {
        long timeL = time * 10000000 + (new DateTime(1970, 1, 1, 8, 0, 0).Ticks);

        DateTime date = new DateTime(timeL);
        return date;
    }

    //c# date time convert to unix time
    public static long DateTimeToUNIXTime( DateTime dt )
    {
    	long timeL = (dt.Ticks - (new DateTime(1970, 1, 1, 8, 0, 0).Ticks)) / 10000000L;
    	return timeL
    }
}