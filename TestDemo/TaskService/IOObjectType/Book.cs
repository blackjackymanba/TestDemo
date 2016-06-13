using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.IOObjectType
{
   public class Book
    {
        public int ID;
        public string Name;
        public string Description;
        public BookComment bookComment;
        public BookPic bookPic;
    }
}
