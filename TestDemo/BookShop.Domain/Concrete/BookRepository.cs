using BookShop.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Domain.Entities;

namespace BookShop.Domain.Concrete
{
    public class BookRepository : IBookRepository
    {
        public IQueryable<Book> Books
        {
            get
            {
                return GetBooks().AsQueryable();
            }
        }
        private static List<Book> GetBooks()
        {
            //为了演示，这里手工造一些数据，后面会介绍使用EF从数据库中读取。
            List<Book> books = new List<Book>{
            new Book { ID = 1, Title = "ASP.NET MVC 4 编程", Price = 52},
            new Book { ID = 2, Title = "CLR Via C#", Price = 46},
            new Book { ID = 3, Title = "平凡的世界", Price = 37}
        };
            return books;
        }
    }
}
