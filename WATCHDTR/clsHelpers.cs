using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WATCHDTR
{
    public class clsHelpers
    {
        IniFile MyIni = new IniFile("Settings.ini");
        public void SetIni(string Key, string Section, string Value)
        {
            try
            {
                MyIni.Write(Key, Value, Section);
            }
            catch (Exception)
            {
            }
        }
        public string GetIniValue(string Key, string Section, string DefaultValue)
        {
            string result = DefaultValue;
            result = MyIni.Read(Key, Section);
            if (string.IsNullOrEmpty(result))
            {
                result = DefaultValue;
            }
            return result;
        }
        public string GetRegex(string Pattern, string value)
        {
            string Result = string.Empty;
            try
            {
                Result = Regex.Match(value, Pattern, RegexOptions.IgnoreCase).Value;
            }
            catch (Exception ex)
            {
            }
            return Result;
        }
        public string GetEmployeeNumber(string value)
        {
            string Result = "";
            try
            {
                Result = value.Replace("(", "").Replace(")", "");
            }
            catch (Exception)
            {
            }
            return Result;
        }
        public int GetInteger(string Value)
        {
            int Result = -1;
            try
            {
                Result = int.Parse(Value.Replace("(", "").Replace(")", ""));
            }
            catch (Exception)
            {
            }
            return Result;
        }
        public DateTime getDate(string Value)
        {
            DateTime dateTime = new DateTime(1901, 01, 01, 0, 0, 0);
            try
            {
                string format = "M/d/yyyy";
                if (!string.IsNullOrEmpty(Value))
                    dateTime = DateTime.ParseExact(Value, format, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {

            }
            return dateTime;
        }
        public DateTime getDate(int SYear,int SMonth,int Sday)
        {
            DateTime SdateTime = new DateTime(SYear, SMonth, Sday, 0, 0, 0);
            return SdateTime;
            
            try
            {
                string sMonth = "0";
                string sDay = "0";

                if (SMonth <= 9)
                {
                    sMonth = "0" + SMonth.ToString();
                }
                else
                {
                    sMonth = SMonth.ToString();
                }

                if (Sday <= 9)
                {
                    sDay = "0" + Sday.ToString();
                }
                else
                {
                    sDay = Sday.ToString();
                }
                DateTime parsedDate1 = DateTime.ParseExact("2025/06/01", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                string inputDate = SYear + "/" + sMonth + "/" + sDay; // MM-dd-yyyy
                //inputDate = "2025/06/01";
                DateTime parsedDate = DateTime.ParseExact(inputDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                string formattedDate = parsedDate.ToString("yyyy-MM-dd");

                string format = "M/d/yyyy";
                return DateTime.ParseExact(SdateTime.ToString("MM-dd-yyyy"), format, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {

            }
            return new DateTime(SYear,SMonth, Sday, 0, 0, 0); ;
        }
        public DateTime GetTimeConvert(string Value)
        {
            DateTime dateTime = new DateTime(1901, 01, 01, 0, 0, 0);
            try
            {
                if (!string.IsNullOrEmpty(Value))
                    dateTime = DateTime.Parse(DateTime.Parse(Value).ToLongTimeString());
            }
            catch (Exception ex)
            {

            }
            return dateTime;
        }
    }
}
