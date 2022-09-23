using System;
using System.Collections.Generic;
using System.Linq;

namespace mpvv2.Models.DBModels
{
    public class ModelsManager
    {

        public static bool isValidPosition(int value)
        {
            if (value < 0 || value >= 100)
                return false;
            return true;
        }

        public static bool isValidCustomDate(string value)
        {
            if (value.Equals(":frompro:") || value.Equals(":tonow:"))
                return true;
            string[] data = value.Split('-');
            int year = 0, month = 0, day = 0;
            bool isMonth = false, isDay = false;
            year = Helper.getInt(data[0]);
            if (year < 1 || year > 10000)
                return false;
            if (data.Length == 2)
            {
                month = Helper.getInt(data[0]);
                year = Helper.getInt(data[1]);
                isMonth = true;
            }
            else if (data.Length == 3)
            {
                month = Helper.getInt(data[1]);
                day = Helper.getInt(data[0]);
                year = Helper.getInt(data[2]);
                isMonth = true;
                isDay = true;
            }

            if ((day < 1 || day > 31) && !isDay)
                return false;
            if ((month < 1 || month > 12) && !isMonth)
                return false;
            if (year < 1000 || year > 10000)
                return false;
            return true;
        }

        public static List<Dictionary<dynamic,dynamic>> OrderByCustomDates(List<Dictionary<dynamic,dynamic>> list, string primaryName)
        {
            foreach (var item in list)
                item["ordered_date"] = customDateToDate(item[primaryName]);
            list.Sort((a, b) => a["ordered_date"]+b["ordered_date"]);
            for (int i = 0; i < list.Count(); i++)
                list[i]["ordered_date"] = i;
            return list;
        }

        public static DateTime customDateToDate(string customDate)
        {
            if(customDate == ":frompro:")
                return DateTime.MinValue;
            if(customDate == ":tonow:")
                return DateTime.Now;
            string[] data = customDate.Split('-');
            string year = data[0];
            string day = "1", month = "1";
            if (data.Length == 2)
            {
                day = "1";
                year = data[1];
                month = data[0];
            }
            else if (data.Length == 3)
            {
                day = data[0];
                month = data[1];
                year = data[2];
            }
            if (day == "?")
                day = "1";
            if (month == "?")
                month = "1";
            if (year == "?")
                year = "1";
            return DateTime.Parse(year + "-" + month + "-" + day);
        }

        public static bool isValidDate(string date)
        {
            try {
                DateTime.Parse(date);
                return true;
            } catch (Exception) { return false; }
        }

        public static bool isValidCarrierVehState(int state)
        {
            if (state < 0 || state > 1)
                return false;
            return true;
        }

        public static string getCarrierVehStateString(int id)
        {
            switch (id)
            {
                case 1: return "zapůjčen";
                default: return "poté";
            }
        }

        public static bool isValidBool(string value)
        {
            if (value == "1" || value == "true" || value == "0" || value == "false")
                return true;
            return false;
        }
        
        public static bool getBoolFromString(string value)
        {
            if (value == "1" || value == "true")
                return true;
            return false;
        }

        public static string getBoolName(bool value)
        {
            if (value)
                return "ano";
            return "ne";
        }

        public static string getFormattedCustomDate(string value)
        {
            if (value.IsNullOrWhiteSpace())
                return "";
            if (value == ":frompro:")
                return "od výroby";
            if (value == ":tonow:")
                return "dodnes";
            string[] date = value.Split('-');
            if (date.Length == 1)
                return date[0];
            if (date.Length == 2)
                return date[0] + "." + date[1];
            return date[0] + "." + date[1] + "." + date[2];
        }

        public static string getFormattedDateTime(DateTime date)
        {
            return date.ToString("dd.MM.yyyy H:mm:ss");
        }
        
        public static string getFormattedDateTimeNoSecs(DateTime date)
        {
            return date.ToString("dd.MM.yyyy H:mm");
        }

        public static string getDateDifferenceString(DateTime date1, DateTime date2)
        {
            if (date1 == null || date2 == null)
                return "bez dat";
            double diff = (date2-date1).TotalSeconds;
            if (diff < 7 * 60)
                return "nyní";

            double years = Math.Floor(diff / (365 * 60 * 60 * 24));
            double months = Math.Floor((diff - years * 365 * 60 * 60 * 24) / (30 * 60 * 60 * 24));
            double days = Math.Floor((diff - years * 365 * 60 * 60 * 24 - months * 30 * 60 * 60 * 24) / (60 * 60 * 24));
            double hours = Math.Floor((diff - years * 365 * 60 * 60 * 24 - months * 30 * 60 * 60 * 24 - days * 60 * 60 * 24) / (60 * 60));
            double minutes = Math.Floor((diff - years * 365 * 60 * 60 * 24 - months * 30 * 60 * 60 * 24 - days * 60 * 60 * 24 - hours * 60 * 60) / 60);
            //double seconds = Math.Floor((diff - years * 365 * 60 * 60 * 24 - months * 30 * 60 * 60 * 24 - days * 60 * 60 * 24 - hours * 60 * 60 - minutes * 60));

            if (years == 0 && months == 0 && days == 0 && hours == 0) // Pod hodinu
                return "před " + minutes + " " + inclineMins(minutes, 2);
            else if (years == 0 && months == 0 && days == 0) // Pod den
                return "před " + hours + " " + inclineHours(hours, 2);
            else if (years == 0 && months == 0) // Pod mesicem
            {
                if (days <= 7)
                    return "před " + days + " " + inclineDays(days, 2);
                else
                    return "před " + days + " " + inclineDays(days, 2);
            }
            else if (years == 0) // Pod rokem
            {
                if (months <= 3)
                    return "před " + months + " " + inclineMonths(months, 2);
                else
                    return "před " + months + " " + inclineMonths(months, 2);
            }
            else
                return "před " + years + " " + inclineYears(years, 2);
        }
        
        public static int getDateDifferenceType(DateTime date1, DateTime date2)
        {
            if (date1 == null || date2 == null)
                return 0;
            double diff = (date2-date1).TotalSeconds;
            if (diff < 7 * 60)
                return 1;

            double years = Math.Floor(diff / (365 * 60 * 60 * 24));
            double months = Math.Floor((diff - years * 365 * 60 * 60 * 24) / (30 * 60 * 60 * 24));
            double days = Math.Floor((diff - years * 365 * 60 * 60 * 24 - months * 30 * 60 * 60 * 24) / (60 * 60 * 24));
            double hours = Math.Floor((diff - years * 365 * 60 * 60 * 24 - months * 30 * 60 * 60 * 24 - days * 60 * 60 * 24) / (60 * 60));
            double minutes = Math.Floor((diff - years * 365 * 60 * 60 * 24 - months * 30 * 60 * 60 * 24 - days * 60 * 60 * 24 - hours * 60 * 60) / 60);
            //double seconds = Math.Floor((diff - years * 365 * 60 * 60 * 24 - months * 30 * 60 * 60 * 24 - days * 60 * 60 * 24 - hours * 60 * 60 - minutes * 60));

            if (years == 0 && months == 0 && days == 0 && hours == 0) // Pod hodinu
                return 2;
            else if (years == 0 && months == 0 && days == 0) // Pod den
                return 3;
            else if (years == 0 && months == 0) // Pod mesicem
            {
                if (days <= 7)
                    return 4;
                else
                    return 5;
            }
            else if (years == 0) // Pod rokem
            {
                if (months <= 3)
                    return 6;
                else
                    return 7;
            }
            else
                return 8;
        }

        protected static string inclineDays(double amount, int type=1)
        {
            if (amount == 1)
                return (type == 1) ? "den" : "dnem";
            else if (amount < 5)
                return (type == 1) ? "dny" : "dny";
            else
                return (type == 1) ? "dnů" : "dny";
        }
        protected static string inclineHours(double amount, int type=1)
        {
            if (amount == 1)
                return (type == 1) ? "hodina":"hodinou";
            else if (amount < 5)
                return (type == 1) ? "hodiny":"hodinami";
            else
                return (type == 1) ? "hodin":"hodinami";
        }
        protected static string inclineMins(double amount, int type=1)
        {
            if (amount == 1)
                return (type == 1) ? "minuta":"minutou";
            else if (amount < 5)
                return (type == 1) ? "minuty":"minutami";
            else
                return (type == 1) ? "minut":"minutami";
        }
        protected static string inclineMonths(double amount, int type=1)
        {
            if (amount == 1)
                return (type == 1) ? "měsíc":"měsícem";
            else if (amount < 5)
                return (type == 1) ? "měsíce":"měsíci";
            else
                return (type == 1) ? "měsíců":"měsíci";
        }
        protected static string inclineYears(double amount, int type=1)
        {
            if (amount == 1)
                return (type == 1) ? "rok":"rokem";
            else if (amount < 5)
                return (type == 1) ? "roky":"lety";
            else
                return (type == 1) ? "let":"lety";
        }

        public static string generateUuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static string getAirConditionAsStringStatic(int ac)
        {
            switch (ac)
            {
                case 0: return "žádná";
                case 1: return "celovozová";
                case 2: return "pouze pro řidiče";
                case 3: return "pro cestující";
                case 4: return "částečná pro cestující";
            }
            return null;
        }

        public static bool isValidAirCondition(int ac)
        {
            if (ac < 0 || ac > 4)
                return false;
            return true;
        }

        public static bool isValidCondition(int value)
        {
            if (value < 0 || value > 7)
                return false;
            return true;
        }

        public static string getConditionAsString(int condition)
        {
            switch (condition)
            {
                case 0: return "sešrotován";
                case 1: return "provozní";
                case 2: return "dosud nezařazen";
                case 3: return "služební";
                case 4: return "jiný dopravce";
                case 5: return "historický";
                case 6: return "definitivně odstaven";
                case 7: return "dočasně odstaven";
                case 8: return "neznámý";
                case 9: return "dočasně nevyužívaný";
            }
            return null;
        }

        public static bool isValidLowFloor(int value)
        {
            if (value < 0 || value > 2)
                return false;
            return true;
        }

        public static string getLowFloorAsString(int lowFloor)
        {
            switch (lowFloor)
            {
                case 0: return "není";
                case 1: return "částečně";
                case 2: return "plně";
            }

            return null;
        }

        public static bool isValidStringForDatabase(string str)
        {
            if (str.Contains('"'))
                return false;
            return true;
        }
    }
}