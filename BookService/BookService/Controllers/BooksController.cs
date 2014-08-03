using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BookService.Models;
using System.Web.Http.Description;
using System.Threading.Tasks;


namespace BookService.Controllers
{
    public class BooksController : ApiController
    {
        private BookServiceContext db = new BookServiceContext();

        // GET api/Books
        public IQueryable <BookDTO> GetBooks()
        {
            var books = from b in db.Books
                        select new BookDTO()
                        {
                            Id = b.Id,
                            Title = b.Title,
                            AuthorName = b.Author.Name
                        };

            return books;
        }

        // GET api/Books/5
        [ResponseType(typeof(BookDetailDTO))]  
        public async  Task<IHttpActionResult> GetBook(int id)
        {
            var book = await db.Books.Include(b => b.Author).Select(b =>
                new BookDetailDTO()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    Genre = b.Genre,
                    AuthorName = b.Author.Name,
                    Year = b.Year
                }).SingleOrDefaultAsync(b => b.Id == id); 
                
            if (book == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return Ok(book);
        }

        // PUT api/Books/5
        public HttpResponseMessage PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != book.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Books
        [ResponseType(typeof(Book) )]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

                db.Books.Add(book);
               await  db.SaveChangesAsync();

                //new code to load author name
                db.Entry(book).Reference(x=>x.Author).Load();
                var dto= new BookDTO()
                {
                        Id=book.Id,
                        Title =book.Title ,
                        AuthorName =book.Author.Name 
                };
                
                return CreatedAtRoute("DefaultApi", new {id=book.Id},dto);
        }

        // DELETE api/Books/5
        public HttpResponseMessage DeleteBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Books.Remove(book);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, book);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}