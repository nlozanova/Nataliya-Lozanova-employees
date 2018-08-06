using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TeamLongestPeriod
{
    class Program
    {
        public static Dictionary<int, List<Employee>> employeeByProjects = new Dictionary<int, List<Employee>>();
        public static Dictionary<int, List<Employee>> employeeByID = new Dictionary<int, List<Employee>>();

        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;

            StreamEntries();

            var result = employeeByProjects.Aggregate(new Dictionary<string, int>(), (d, kv) =>
            {

                for (int i = 0; i < kv.Value.Count - 1; i++)
                {
                    var employee = kv.Value[i];
                    for (int j = i + 1; j < kv.Value.Count; j++)
                    {
                        var employee2 = kv.Value[j];

                        if (employee2.EmployeeID == employee.EmployeeID)
                        {
                            continue;
                        }

                        var dateFrom = Max(employee.DateFrom, employee2.DateFrom);
                        var dateTo = Min(employee.DateTo, employee2.DateTo);

                        if (dateFrom < dateTo)
                        {
                            int days = (int)(dateTo - dateFrom).TotalDays;
                            var key = Math.Min(employee.EmployeeID, employee2.EmployeeID) + "-" + Math.Max(employee.EmployeeID, employee2.EmployeeID);

                            if (!d.ContainsKey(key))
                            {
                                d.Add(key, 0);
                            }
                            d[key] += days;
                        }
                    }
                }

                return d;

            }).OrderByDescending(kv => kv.Value).FirstOrDefault();

            Console.WriteLine("Result: {0} : {1} days", result.Key, result.Value);

            Console.ReadKey(true);
        }


        public static DateTime Min(DateTime t1, DateTime t2)
        {
            if (DateTime.Compare(t1, t2) > 0)
            {
                return t2;
            }

            return t1;
        }

        public static DateTime Max(DateTime t1, DateTime t2)
        {
            if (DateTime.Compare(t1, t2) < 0)
            {
                return t2;
            }

            return t1;
        }

        public static void StreamEntries()
        {
            DateTime now = DateTime.Now;

            using (var sr = new StreamReader(@".../.../data.csv"))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.Trim() == "")
                    {
                        continue;
                    }

                    var values = line.Split(',');

                    var employee = new Employee
                    {
                        EmployeeID = int.Parse(values[0]),
                        ProjectID = int.Parse(values[1]),
                        DateFrom = DateTime.Parse(values[2].Trim()),
                        DateTo = values[3].Trim().ToUpper() == "NULL" ? now : DateTime.Parse(values[3].Trim())

                    };

                    if (!employeeByProjects.ContainsKey(employee.ProjectID))
                    {
                        employeeByProjects.Add(employee.ProjectID, new List<Employee>());
                    }
                    employeeByProjects[employee.ProjectID].Add(employee);

                    if (!employeeByID.ContainsKey(employee.EmployeeID))
                    {
                        employeeByID.Add(employee.EmployeeID, new List<Employee>());
                    }
                    employeeByID[employee.EmployeeID].Add(employee);
                }

            }

        }
    }
}
