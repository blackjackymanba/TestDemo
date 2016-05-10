using GPMTaskService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskService
{
    public class Program
    {
         public static void Main(string[] args)
        {
            TaskTest();
        }
        public static void TaskTest()
        {
            var td = new TestDataAccess();
            td.Test();
        }
    }
}
