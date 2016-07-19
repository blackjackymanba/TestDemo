using GPMTaskService.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TaskService
{
    public class Program
    {
        internal static string _aa = "";
        internal static readonly Dictionary<string, string> _dicPlatForm = new Dictionary<string, string>();
        internal static readonly textcc tc = new textcc();
        internal static readonly List<textcc> tclist = new List<textcc>();
        static Program()
        {
            FluentConsole.White.Background.Black.Line("Black");
            FluentConsole
                .Cyan.Line("Cyan")
                .DarkBlue.Line("DarkBlue")
                .DarkCyan.Line("DarkCyan")
                .DarkGray.Line("DarkGray")
                .DarkGreen.Line("DarkGreen")
                .DarkMagenta.Line("DarkMagenta")
                .DarkRed.Line("DarkRed")
                .DarkYellow.Line("DarkYellow")
                .Gray.Line("Gray")
                .Green.Background.Red.Line("Green")
                .Magenta.Line("Magenta")
                .Red.Line("Red")
                .White.Line("White")
                .Yellow.Line("Yellow");
            Console.ReadKey();


            tc.qqq = "1";
            tc.www = "2";

            tclist.Add(new textcc() { www = "1111", qqq = "4444" });
            tclist.Add(new textcc() { www = "11121", qqq = "44244" });

            _dicPlatForm.Add("11", "22");
            _dicPlatForm.Add("12", "23");
            var _aa = ConfigurationManager.ConnectionStrings["STATEGRID"].ConnectionString;
        }

        public static void Main(string[] args)
        {
           
            TaskTest();
        }
        public static void TaskTest()
        {
            var td = new TestDataAccess();
            td.Test();
        }
        public static string bb()
        {
            return _aa;
        }
    }
    public class textcc
    {
        public string qqq;
        public string www;

    }
}
