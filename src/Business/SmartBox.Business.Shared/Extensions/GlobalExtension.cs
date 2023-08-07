using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace SmartBox.Business.Shared.Extensions
{

    public static class GlobalExtension
    {
        #region HasText
        public static bool HasText(this string source)
        {
            return !string.IsNullOrWhiteSpace(source);
        }
        #endregion

        //#region SetMessage, SetMessageWithTextReplace - Setting Return Message
        //public static void SetMessage(this IBaseMessageModel source, int returnNumber, GlobalEnums.AppStatusType appStatusType)
        //{
        //    AppMessageService appMessageService = new AppMessageService();

        //    source.MessageReturnNumber = returnNumber;
        //    source.Message = returnNumber < 0 ? AppMessageService.GetAppErrorMessage(returnNumber) : AppMessageService.GetAppInfoMessage(returnNumber);

        //    switch (appStatusType)
        //    {
        //        case GlobalEnums.AppStatusType.Error:
        //            source.ReturnStatusType = GlobalEnums.AppStatusType.Error.GetAppStatusEnumName();
        //            break;
        //        case GlobalEnums.AppStatusType.Information:
        //            source.ReturnStatusType = GlobalEnums.AppStatusType.Information.GetAppStatusEnumName();
        //            break;
        //        case GlobalEnums.AppStatusType.Warning:
        //            source.ReturnStatusType = GlobalEnums.AppStatusType.Warning.GetAppStatusEnumName();
        //            break;
        //        default:
        //            source.ReturnStatusType = GlobalEnums.AppStatusType.Information.GetAppStatusEnumName();
        //            break;
        //    }
        //}

        //public static void SetFoundMessage(this IBaseMessageModel source)
        //{
        //    var ret = 505;
        //    source.MessageReturnNumber = ret;
        //    source.Message = AppMessageService.GetAppInfoMessage(ret);
        //    source.ReturnStatusType = GlobalEnums.AppStatusType.Information.GetAppStatusEnumName();
        //}

        //public static void SetNotificationSuccessMessage(this IBaseMessageModel source)
        //{
        //    var ret = 508;
        //    source.MessageReturnNumber = ret;
        //    source.Message = AppMessageService.GetAppInfoMessage(ret);
        //    source.ReturnStatusType = GlobalEnums.AppStatusType.Information.GetAppStatusEnumName();
        //}

        //public static void SetSuccessUpdateMessage(this IBaseMessageModel source)
        //{
        //    var ret = 502;
        //    source.MessageReturnNumber = ret;
        //    source.Message = AppMessageService.GetAppInfoMessage(ret);
        //    source.ReturnStatusType = GlobalEnums.AppStatusType.Information.GetAppStatusEnumName();
        //}

        //public static void SetFailUpdateMessage(this IBaseMessageModel source)
        //{
        //    var ret = -25;
        //    source.MessageReturnNumber = ret;
        //    source.Message = AppMessageService.GetAppInfoMessage(ret);
        //    source.ReturnStatusType = GlobalEnums.AppStatusType.Error.GetAppStatusEnumName();
        //}

        //public static void SetNotFoundMessage(this IBaseMessageModel source)
        //{
        //    var ret = 504;
        //    source.MessageReturnNumber = ret;
        //    source.Message = AppMessageService.GetAppInfoMessage(ret);
        //    source.ReturnStatusType = GlobalEnums.AppStatusType.Warning.GetAppStatusEnumName();
        //}

        //public static void SetMessageWithTextReplace(this IBaseMessageModel source, int returnNumber, GlobalEnums.AppStatusType appStatusType, string textReplace)
        //{
        //    string fullText = "";
        //    fullText = returnNumber < 0 ? AppMessageService.GetAppErrorMessage(returnNumber) : AppMessageService.GetAppInfoMessage(returnNumber);

        //    if (returnNumber == -4)
        //    {
        //        fullText = fullText.Replace("{counter}", textReplace);
        //        fullText = fullText.Replace("{attempts}", textReplace == "1" ? "attempt" : "attempts");
        //    }

        //    if (returnNumber == -19 || returnNumber == -21 || returnNumber == -22 || returnNumber == -23)
        //    {
        //        fullText = fullText.Replace("{employeeName}", textReplace);
        //    }

        //    if (returnNumber == 5)
        //    {
        //        fullText = fullText.Replace("{forReplacement}", textReplace);
        //    }

        //    if (returnNumber == -503)
        //    {
        //        fullText = fullText.Replace("{auto-name}", textReplace);
        //    }

        //    if (returnNumber == -28)
        //    {
        //        var str = textReplace.Split("|");
        //        fullText = fullText.Replace("{scheduleDate}", str[0]);
        //        fullText = fullText.Replace("{hours}", str[1]);

        //    }

        //    if (returnNumber == -29)
        //    {
        //        var str = textReplace.Split("|");
        //        fullText = fullText.Replace("{scheduleDate}", str[0]);
        //        fullText = fullText.Replace("{worktype}", str[1]);
        //    }

        //    //if (returnNumber == -30)
        //    //{
        //    //    fullText = fullText.Replace("{scheduleDate}", textReplace);
        //    //}

        //    if (returnNumber == -31)
        //    {
        //        var str = textReplace.Split("|");
        //        fullText = fullText.Replace("{checkIn}", str[0]);
        //        fullText = fullText.Replace("{scheduleDate}", str[1]);

        //    }

        //    if (returnNumber == -32)
        //    {
        //        fullText = fullText.Replace("{scheduleDate}", textReplace);
        //    }

        //    if (returnNumber == -33)
        //    {
        //        //You can no longer change your {worktype} for {scheduleDate} outside the {hours} allowable period.
        //        var str = textReplace.Split("|");
        //        fullText = fullText.Replace("{worktype}", str[0]);
        //        fullText = fullText.Replace("{scheduleDate}", str[1]);
        //        fullText = fullText.Replace("{hours}", str[2]);
        //    }

        //    if (returnNumber == -34)
        //    {
        //        fullText = fullText.Replace("{worktype}", textReplace);
        //    }

        //    if (returnNumber == -36)
        //    {
        //        fullText = fullText.Replace("{id}", textReplace);
        //    }

        //    if (returnNumber == -37)
        //    {
        //        fullText = fullText.Replace("{slot}", textReplace);
        //    }

        //    if (returnNumber == -38)
        //    {
        //        var str = textReplace.Split("|");
        //        fullText = fullText.Replace("{worktype}", str[0]);
        //        fullText = fullText.Replace("{scheduleDate}", str[1]);
        //    }

        //    source.MessageReturnNumber = returnNumber;
        //    source.Message = fullText;

        //    switch (appStatusType)
        //    {
        //        case GlobalEnums.AppStatusType.Error:
        //            source.ReturnStatusType = GlobalEnums.AppStatusType.Error.GetAppStatusEnumName();
        //            break;
        //        case GlobalEnums.AppStatusType.Information:
        //            source.ReturnStatusType = GlobalEnums.AppStatusType.Information.GetAppStatusEnumName();
        //            break;
        //        case GlobalEnums.AppStatusType.Warning:
        //            source.ReturnStatusType = GlobalEnums.AppStatusType.Warning.GetAppStatusEnumName();
        //            break;
        //        default:
        //            source.ReturnStatusType = GlobalEnums.AppStatusType.Information.GetAppStatusEnumName();
        //            break;
        //    }
        //}

        //public static void SetSystemErrorMessage(this IBaseMessageModel source, string message)
        //{
        //    source.MessageReturnNumber = -500;
        //    source.Message = message;
        //    source.ReturnStatusType = GlobalEnums.AppStatusType.Error.GetAppStatusEnumName();
        //}
        //#endregion

        //#region GetAppStatusEnumName - Get the Application Status string name
        //public static string GetAppStatusEnumName(this GlobalEnums.AppStatusType appStatusType)
        //{
        //    return Enum.GetName(typeof(GlobalEnums.AppStatusType), appStatusType);
        //}
        //#endregion

        #region GetAppStatusEnumName - Get the Application Status string name
        public static string GetAppStatusEnumName(this GlobalEnums.AppStatusType appStatusType)
        {
            return Enum.GetName(typeof(GlobalEnums.AppStatusType), appStatusType);
        }
        #endregion


        #region Set today date

        public static void SetTodayDate(this DateTime? source)
        {
            source = DateTime.Now;
        }

        #endregion

        #region Collections have data


        public static bool HasData<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                return false;
            else if (!source.Any())
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        #endregion

        #region Number Ordinal
        public static string ToOrdinalString(this sbyte num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }


        #endregion

        #region Month Name

        public static string GetMonthName(this DateTime month)
        {
            return month.ToString("MMMM");
        }

        public static string GetMonthName(this sbyte month)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }

        #endregion

        #region DateTime Extension

        public static bool DateTimeInRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }
        public static string ToDefaultFormat(this DateTime dateTime,string defaulFormat= "MM-dd-yyyy hh:mm")
        {
            return dateTime.ToString(defaulFormat);
        }
        #endregion

        #region Foreach Loop extension
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
        #endregion

        #region string extentions
        public static string GenerateNewGuidRefCode()
        {
            return Guid.NewGuid().ToString().Extract().ToUpper();
        }
        public static string Extract(this string str, int length = 10, string stringToRemove = "-")
        {
            if (str.Length <= length) return str;
            return str.Substring(0, length).Replace(stringToRemove, "");
        }
        #endregion
    }
}
