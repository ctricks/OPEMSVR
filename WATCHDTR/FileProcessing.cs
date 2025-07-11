using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WATCHDTR
{
    public class FileProcessing
    {

        private static readonly HttpClient client = new HttpClient();

        clsHelpers helper = new clsHelpers();
        public void Test()
        {
            Console.WriteLine("Test");
        }
        public string CheckFolderCSV(string path,string BackupPath)
        {
            string DateProcess = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + " : ";
            string StrMessage = "Done Checking...";
            try
            {
                Console.WriteLine(DateProcess + "Checking DTR Directory: " + path);

                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (Directory.GetFiles(path, "*.csv").Count() > 0)
                {
                    string FilenameToProcess = Directory.GetFiles(path, "*.csv").First().ToString();

                    if (!string.IsNullOrEmpty(FilenameToProcess))
                    {
                        Console.WriteLine(DateProcess + "File Found: " + FilenameToProcess);
                        Console.WriteLine(DateProcess + "Processing...");
                        FileProcess(FilenameToProcess, DateProcess,BackupPath);
                    }
                    else
                    {
                        Console.WriteLine(DateProcess + "No File Found...");
                    }
                }else
                {
                    Console.WriteLine(DateProcess + "No File Found...");
                }

            }
            catch (Exception ex)
            {
                StrMessage = "Error: " + ex.Message.ToString();
            }
            return DateProcess + StrMessage;
        }
        private void FileProcess(string filepath,string DateProcess,string BackupPath)
        {
            string FilenameFound = Path.GetFileName(filepath);

            string BackupFolder = Path.Combine(BackupPath,DateTime.Now.ToString("yyyyddMM"));
            string BackupFile = Path.Combine(BackupFolder, FilenameFound);

            if (!Directory.Exists(BackupFolder))
                Directory.CreateDirectory(BackupFolder);
            
            List<ExcelInfo> Elist = new List<ExcelInfo>();
            Elist = ReadCSV(filepath);

            if (Elist != null)
            {
                Console.WriteLine(DateProcess + "Entries Found: " + Elist.Count().ToString());
                Console.WriteLine(DateProcess + "Start Ingesting... ");
                IngestingAPI(Elist,DateProcess);
                Console.WriteLine(DateProcess + "Done Ingesting... ");
                Console.WriteLine(DateProcess + "Move to Backup... ");
                Console.WriteLine(DateProcess + "Backup: " + BackupFolder);
                File.Copy(filepath, BackupFile);
                Console.WriteLine(DateProcess + "File Copied: OK");
                if (File.Exists(BackupFile))
                {
                    File.Delete(filepath);
                    Console.WriteLine(DateProcess + "File Deleted: OK");
                }
              
            }
            else
            {
                Console.WriteLine(DateProcess + "No Content Found...");
            }
        }

        private void IngestingAPI(List<ExcelInfo> ExcelList,string DateProcess)
        {
            if(ExcelList != null) {
                string username = "bok@mail.com";
                string password = "admin123";
                string LoginToken = LoginAPI(username,password);
                Console.WriteLine(DateProcess + "Login Token: " + LoginToken);
                if (!string.IsNullOrEmpty(LoginToken) && !LoginToken.Contains("error"))
                {
                    foreach (ExcelInfo Excel in ExcelList)
                    {
                        string TimeIn = "00:00";
                        string TimeOut = TimeIn;

                        if(Excel.TimeIn_1 != null)
                        {
                            TimeIn = Excel.TimeIn_1.ToString("HH:mm");
                        }
                        else if(Excel.TimeIn_3 != null)
                        {
                            TimeIn = Excel.TimeIn_3.ToString("HH:mm");
                        }
                        else if (Excel.TimeIn_5 != null)
                        {
                            TimeIn = Excel.TimeIn_5.ToString("HH:mm");
                        }
                        if (Excel.TimeIn_2 != null)
                        {
                            TimeOut = Excel.TimeIn_2.ToString("HH:mm");
                        }
                        else if (Excel.TimeIn_4 != null)
                        {
                            TimeOut = Excel.TimeIn_4.ToString("HH:mm");
                        }
                        else if (Excel.TimeIn_6 != null)
                        {
                            TimeOut = Excel.TimeIn_6.ToString("HH:mm");
                        }

                        
                        DateTime parsedDate = DateTime.ParseExact(Excel.AttendanceDate.ToString("MM-dd-yyyy"), "MM-dd-yyyy", CultureInfo.InvariantCulture);
                        string formattedDate = parsedDate.ToString("yyyy-MM-dd");

                        var parameters = new
                        {
                            param1 = Excel.EmployeeName,
                            param2 = Excel.EmployeeNumber,
                            param3 = formattedDate,
                            param4 = TimeIn,
                            param5 = TimeOut,
                            param6 = "WATCHDTR",
                            param7 = Excel.AttendanceDate.ToString("MMMM")
                        };

                        var postData = "employee_id=" + Uri.EscapeDataString(Excel.EmployeeNumber);
                        postData += "&name=" + Excel.EmployeeName;
                        postData += "&select_date=" + formattedDate;
                        postData += "&month=" + Uri.EscapeDataString(Excel.AttendanceDate.ToString("MMMM"));
                        postData += "&check_in=" + TimeIn;
                        postData += "&check_out=" + TimeOut;
                        postData += "&DataSource=" + "WATCHDTR(" + DateTime.Now.ToString("MMddyyyy") + ")";

                        string RespCode = string.Empty;
                        string IDTracker = Excel.EmployeeName + "(" + formattedDate + ")";
                        CallPostApiAsync(LoginToken, postData,IDTracker );

                        Thread.Sleep(1000);
                    }
                }

            }
        }

        

        public async Task CallPostApiAsync(string token, string postData,string IDTracker)
        {
            string DateProcess = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss") + " : ";
            try
            {
                var url = "http://garuda45-001-site14.anytempurl.com/api/insertDTR2";

                var data = Encoding.ASCII.GetBytes(postData);

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                //request.Headers.Add("Authorization", "Bearer " + token);
                request.Accept = "application/json";
                request.ContentLength = data.Length;


                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                var obj = JsonConvert.DeserializeObject<dynamic>(responseString);

                Console.WriteLine(DateProcess + ": " + IDTracker + ":" +  responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateProcess + ": " + IDTracker + ": Error: " + ex.Message);
            }
            
            
            
        }

        public async Task CallPostLogoutApiAsync(string token, string postData)
        {
            var url = "http://garuda45-001-site14.anytempurl.com/api/logoutAPI";


            //// Set up the request headers
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //// Create the request body

            //var json = JsonConvert.SerializeObject(parameters);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            //// Send the POST request
            //var response = await client.PostAsync(url,content);

            //// Handle the response
            //var responseString = await response.Content.ReadAsStringAsync();

            var data = Encoding.ASCII.GetBytes(postData);

            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Accept = "application/json";
            request.ContentLength = data.Length;
            

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var obj = JsonConvert.DeserializeObject<dynamic>(responseString);

            //result = obj["token"].ToString();

            Console.WriteLine(responseString);
        }

        private string LoginAPI(string Email,string Password)
        {
            string result = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://garuda45-001-site14.anytempurl.com/api/loginAPI");

                var postData = "email=" + Uri.EscapeDataString(Email);
                postData += "&password=" + Uri.EscapeDataString(Password);
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                var obj = JsonConvert.DeserializeObject<dynamic>(responseString);

                result = obj["token"].ToString();  
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
            }
            return result;
        }

        private List<ExcelInfo> ReadCSV(string CSVFilePath)
        {
            List<ExcelInfo> Ei = new List<ExcelInfo>();
            try
            {
                FileInfo fi = new FileInfo(CSVFilePath);
                ExcelInfo xi = new ExcelInfo();

                string[] FileContent = File.ReadAllLines(fi.FullName);

                if (FileContent[FileContent.Count() - 1].ToString() != ",,,,,,")
                {
                    //Append to last line
                    File.AppendAllText(fi.FullName, ",,,,,,");
                }
                FileContent = File.ReadAllLines(fi.FullName);

                List<EmployeeDetails> employeeDetail = new List<EmployeeDetails>();



                bool isEmpFound = false;
                int isIntegerFound = -1;

                if (FileContent.Count() > 0)
                {
                    ExcelInfo ei = new ExcelInfo();
                    string EmployeeNumber = string.Empty;
                    string EmployeeName = string.Empty;
                    foreach (string fileline in FileContent)
                    {
                        string LineContent = fileline;

                        if (fileline.Substring(0, 1) == "\"" && fileline != ",,,,,,")
                        {
                            isEmpFound = true;
                            EmployeeDetails empData = new EmployeeDetails();
                            string EmpPattern = "\\((\\d+)\\)";
                            EmployeeNumber = helper.GetEmployeeNumber(helper.GetRegex(EmpPattern, LineContent));
                            string EmpNamePattern = "\\\"\\S.+\\,.+\\(";
                            EmployeeName = helper.GetRegex(EmpNamePattern, LineContent).Replace("\"", "").Replace("(", "");
                            employeeDetail.Add(empData);
                            continue;
                        }
                        if (isEmpFound)
                        {
                            isIntegerFound = helper.GetInteger(LineContent.Substring(0, 1));
                            if (isIntegerFound > 0)
                            {
                                string[] timeDetails = LineContent.Split(',');
                                string DateFound = timeDetails[0].Split(' ')[0].ToString();
                                ei.DateFound = DateFound;
                                string[] GivenDate = DateFound.Split('/');
                                string DateAtt = int.Parse(GivenDate[2]) + "-" + int.Parse(GivenDate[0]).ToString("00") + "-" + int.Parse(GivenDate[1]).ToString("00");
                                ei.AttendanceDate = helper.getDate(int.Parse(GivenDate[2]), int.Parse(GivenDate[0]), int.Parse(GivenDate[1]));
                                ei.EmployeeNumber = EmployeeNumber;
                                ei.EmployeeName = EmployeeName;
                                ei.TimeIn_1_Found = timeDetails[1].ToString();
                                ei.TimeIn_1 = helper.GetTimeConvert(ei.TimeIn_1_Found);
                                ei.TimeIn_2_Found = timeDetails[2].ToString();
                                ei.TimeIn_2 = helper.GetTimeConvert(ei.TimeIn_2_Found);
                                ei.TimeIn_3_Found = timeDetails[3].ToString();
                                ei.TimeIn_3 = helper.GetTimeConvert(ei.TimeIn_3_Found);
                                ei.TimeIn_4_Found = timeDetails[4].ToString();
                                ei.TimeIn_4 = helper.GetTimeConvert(ei.TimeIn_4_Found);
                                ei.TimeIn_5_Found = timeDetails[5].ToString();
                                ei.TimeIn_5 = helper.GetTimeConvert(ei.TimeIn_5_Found);
                                ei.TimeIn_6_Found = timeDetails[6].ToString();
                                ei.TimeIn_6 = helper.GetTimeConvert(ei.TimeIn_6_Found);
                                Ei.Add(ei);
                                ei = new ExcelInfo();
                            }
                        }
                        if (LineContent.Substring(0, 1) == ",")
                        {
                            isEmpFound = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message.ToString());
            }
            return Ei;
        }
    }
}
