using DAL.Employees;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Employees
{
    public class EmployeesManager
    {
        private EmployeesHandler employeesHandler;
        public EmployeesManager()
        {
            employeesHandler = new EmployeesHandler();
        }

        public int GetAmountOfEmployees() => employeesHandler.GetAmountOfEmployees();
        public string[] EmployeeNames() => employeesHandler.GetEmployeeNames();
        public List<string[]> GetNamesAndUserIDs() => employeesHandler.GetNamesAndUserIDs();
        public string[] EmployeeInfo(string naam) => employeesHandler.EmployeeInfo(naam);
    }
}
