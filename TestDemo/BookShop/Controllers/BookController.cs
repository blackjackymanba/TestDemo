using BookShop.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookShop.Controllers
{
    public class BookController : Controller
    {
        private IBookRepository repository;
        public BookController(IBookRepository bookRepository)
        {
            repository = bookRepository;
        }
        // GET: Book
        public ViewResult List()
        {
            return View(repository.Books);
        }
    }
}