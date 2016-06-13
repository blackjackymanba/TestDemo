using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillFileConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            ConvertBillFile();
        }

        public static void ConvertBillFile()
        {
            var files = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "BillFilesFrom"));
            if (files.GetFiles().Length > 0)
            {
                foreach (var file in files.GetFiles())
                {
                    FileStream fsRead = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);

                    StreamReader sr = new StreamReader(fsRead);
                    String line;
                    var listt = new List<string>();
                    while ((line = sr.ReadLine()) != null)
                    {
                        listt.Add(line.ToString());
                    }

                    if (listt.Count > 0)
                    {
                        var newList = new List<string>();
                        string firstLines = listt[0];

                        string secondLine = "总笔数|总金额|商户号|清算日期(4位月日)|进账单编号";
                        newList.Add(secondLine.Replace("|", ","));
                        newList.Add("`" + firstLines.Replace("|", "`,"));
                        var a = "商户号|银联终端号|交易金额|银联(微信)交易流水|交易日期(YYYYMMDD)|交易时间(HHMMSS)|清算日期(4位月日)|核算单位|终端编号|电力交易流水号|用户交易编号（用户编号）|电卡编号|终端操作员|收款单位|缴费交易备注";
                        newList.Add(a.Replace("|", ","));

                        //var newfileName = file.Name;

                        var newfileName = file.Name.Substring(0, file.Name.Length - 3) + "csv";


                        foreach (var item in listt)
                        {
                            string[] details = item.Split('|');
                            if (details.Length >= 8)
                            {
                                newList.Add("`" + item.Replace("|", "`,"));
                            }
                        }

                        var content = string.Empty;
                        foreach (var item in newList)
                        {
                            content += item + "\r\n";
                        }

                        //byte[] data = Encoding.GetEncoding("GB2312").GetBytes(content);
                        byte[] data = Encoding.UTF8.GetBytes(content);

                        Stream stream = new MemoryStream(data);
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "BillFilesTo", newfileName);


                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        using (var fs = new StreamWriter(new FileStream(path, FileMode.Create), Encoding.GetEncoding(936)))
                        {
                            fs.AutoFlush = true;
                            fs.Write(content);
                        }
                    }
                }
            }

        }


    }
}
