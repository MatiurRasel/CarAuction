using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace BiddingService.RequestHelpers
{
    public static class ExceptionExtension
    {
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }
        public static string GetAllMessages(this Exception exception)
        {
            var messages = exception
                .FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message).ToList();
            return string.Join(Environment.NewLine, messages);
        }

        public static IEnumerable<ApplicationError> GetAll(this Exception exception)
        {
            var exceptions = exception.FromHierarchy(ex => ex.InnerException);

            var list = from ex in exceptions
                       let st = new StackTrace(ex, true)
                       let frame = st.GetFrame(st.FrameCount - 1)
                       select new ApplicationError
                       {
                           FileName = Path.GetFileName(frame.GetFileName()),
                           MethodName = frame.GetMethod().Name,
                           LineNumber = frame.GetFileLineNumber(),
                           Message = ex.Message,
                           StackTrace = ex.StackTrace
                       };

            return list;
        }
        public static void ToTextFileLog(this Exception ex)
        {
            var startupPath =
                new DirectoryInfo(
                        Path.GetDirectoryName(
                            Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path))).Parent
                    .FullName;

            ToTextFileLog(ex, startupPath);
        }

        public static void ToTextFileLog(this Exception ex, string startupPath, string fileName = "ErrorLog.txt")
        {

            File.AppendAllText(startupPath + "\\" + fileName, ApplicationError.GetAllMessage(ex.GetAll()));
        }
    }

    public class ApplicationError
    {
        public ApplicationError()
        {
            Id = Guid.NewGuid().ToString().ToLower();
            ErrorType = ErrorType.ERROR;
            ErrorTime = DateTime.Now.ToLong();
            BranchId = 0;
            UserId = 0;
        }
        
        public string Id { get; set; }
        
        public string FileName { get; set; }
       
        public string MethodName { get; set; }
        public int LineNumber { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public long ErrorTime { get; set; }
        public ErrorType ErrorType { get; set; }
        public int BranchId { get; set; }
        public long UserId { get; set; }

        public static string GetAllMessage(IEnumerable<ApplicationError> exErrors)
        {
            return exErrors.Aggregate(string.Empty,
                (current, ex) => current + (DateTimeExtensions.LongToDateTime(ex.ErrorTime).ToString("dd/MMM/yyyy HH:mm") + ":: " + ex.FileName + ":: " +
                                            ex.MethodName + "::" + ex.LineNumber + " :: " + ex.Message +
                                            Environment.NewLine));
        }
        public string Get()
        {
            return (ErrorTime.LongToDateTime().ToString("dd/MMM/yyyy HH:mm") + ":: " + FileName + ":: " +
                    MethodName + "::" + LineNumber + " :: " + Message +
                    Environment.NewLine);
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek)
        {
            var diff = dateTime.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dateTime.AddDays(-1 * diff).Date;
        }

        public static DateTime FirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }
        public static int TimeDifference(this DateTime startTime)
        {
            TimeSpan span = DateTime.Now - startTime;
            return (int)span.TotalMilliseconds;
        }
        public static DateTime LastDayOfMonth(this DateTime dateTime)
        {
            var firstDayOfTheMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }
        public static string ToFriendlyDateTime(this long value)
        {

            var dateTime = DateTime.ParseExact(value.ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var localDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
            var span = DateTime.UtcNow - localDateTime;

            if (span > TimeSpan.FromHours(24))
            {
                return dateTime.ToString("MMM dd");
            }

            if (span > TimeSpan.FromMinutes(60))
            {
                return string.Format("{0}h", span.Hours);
            }

            return span > TimeSpan.FromSeconds(60) ? string.Format("{0}m ago", span.Minutes) : "Just now";
        }
        public static DateTime ToDateTime(this string value)
        {
            var time = new DateTime();
            var matchingCulture =
                CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .FirstOrDefault(ci => DateTime.TryParse(value, ci, DateTimeStyles.None, out time));
            return time;
        }
        //kuhefkf
        public static DateTime ToDate(this string value)
        {
            string[] datepart = value.Trim().Split('-');
            return Convert.ToDateTime(datepart[1] + "-" + datepart[0] + "-" + datepart[2]);
        }
        
        public static DateTime LongToDateTime(this long value)
        {
            var span = DateTime.ParseExact(value.ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            return span;
        }

        public static string LongDateTimeToString(this long value)
        {
            var span =
                DateTime.ParseExact(value.ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var returnValue = span.ToString("dd-MM-yyyy");
            return returnValue;
        }
        public static long ToLong(this DateTime dateTime)
        {
            var date = dateTime.ToString("yyyyMMddHHmmss");
            var span = Convert.ToInt64(date);
            return span;
        }
        //---------------------------Matiur------------------------------ 
        public static TimeSpan TimeToTimeSpan(this string time)
        {
            DateTime dt = new DateTime();
            if (time.Contains(':'))
            {
                string newTime = "";
                string[] splitTime = time.Split(':');
                if (splitTime[0].Length == 1)
                {
                    newTime = "0" + splitTime[0] + ":" + splitTime[1];
                }
                else
                {
                    newTime = time;
                }
                dt = DateTime.ParseExact(newTime, "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            }
            var timeSpan = dt.TimeOfDay;
            return timeSpan;
        }
        public static DateTime ToDateLastTime(this string value)
        {
            string[] datepart = value.Trim().Split('-');
            string newTime = " 23:59:59 PM";
            return Convert.ToDateTime(datepart[1] + "-" + datepart[0] + "-" + datepart[2] + newTime);
        }
    }

    public enum ErrorType
    {
        DEBUG = 0,
        INFO = 1,
        WARN = 2,
        ERROR = 3,
        FATAL = 4
    }
}