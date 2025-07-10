using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WATCHDTR
{
    public class AttendanceInfo
    {
        public int BiometricID { get; set; }
        public string EmpNumber { get; set; }
        public string EmployeeName { get; set; }
        public string SourceFilename { get; set; }
        public List<ExcelInfo> ExcelInfo { get; set; }
        public DateTime TotalHoursWork { get; set; }
        public int DaysPresent { get; set; }
    }
    public class ExcelInfo
    {
        public int BiometricID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string DateFound { get; set; }
        public string TimeIn_1_Found { get; set; }
        public string TimeIn_2_Found { get; set; }
        public string TimeIn_3_Found { get; set; }
        public string TimeIn_4_Found { get; set; }
        public string TimeIn_5_Found { get; set; }
        public string TimeIn_6_Found { get; set; }

        public DateTime AttendanceDate { get; set; }
        public DateTime TimeIn_1 { get; set; }
        public DateTime TimeIn_2 { get; set; }
        public DateTime TimeIn_3 { get; set; }
        public DateTime TimeIn_4 { get; set; }
        public DateTime TimeIn_5 { get; set; }
        public DateTime TimeIn_6 { get; set; }

        public DateTime TotalTimeRow { get; set; }

    }

    public class EmployeeDetails
    {
        public string EmployeeNumber { get; set; }
        public string EmpName { get; set; }
        public List<ExcelInfo> CSVTime { get; set; }
    }
}
