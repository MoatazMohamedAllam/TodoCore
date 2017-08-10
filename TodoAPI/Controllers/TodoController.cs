using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using TodoAPI.Context;
using TodoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [EnableCors("corsGlobalPolicy")]
    public class TodoController : Controller
    {
        private TodoContext db;

        public TodoController([FromServices] TodoContext db)
        {
            this.db = db;
        }


        //get Id from claims
        private string GetIdFromClaim()
        {
            var claim = User.Claims.Where(x => x.Type == "sub").SingleOrDefault();
            return claim.Value;
        }

        [HttpGet]
        [Route("Action")]
        public string GetInfo()
        {
            Console.WriteLine("############################################################################");
            var claim = User.Claims.Where(x => x.Type == "sub").SingleOrDefault();
            return claim.Value;
            //return new JsonResult(from c in User.Claims select new { c.Type, c.Value}); 
        }

        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<Todo>> Get()
        {
            return await db.Todo
                .Where(x => x.AddedBy.Equals(GetIdFromClaim()))
                .OrderBy(x => x.IsDone)
                .ThenByDescending(x => x.Date)
                .ToListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<Todo> Get(int id)
        {
            return await db.Todo.Where(x => x.Id == id && x.AddedBy.Equals(GetIdFromClaim())).SingleOrDefaultAsync();
        }

        // POST api/values
        [HttpPost]
        public async Task<Todo> Post([FromBody]Todo todo)
        {
            try
            {
                todo.AddedBy = GetIdFromClaim();
                todo.Date = DateTime.Now;
                db.Todo.Add(todo);
                await db.SaveChangesAsync();

                return todo;
            }
            catch (Exception error)
            {
                Console.WriteLine(" ************************************************************ ");
                Console.WriteLine(error.Message);
            }

            return null;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]Todo todo)
        {
            //if the update came from the id of the same user who added the todo item
            if (todo.AddedBy.Equals(GetIdFromClaim()))
            {
                db.Todo.Update(todo);
                await db.SaveChangesAsync();
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            try { 
            var todo = await db.Todo.FindAsync(id);

            //if the delete came from the id of the same user who added the todo item
            if (todo.AddedBy.Equals(GetIdFromClaim()))
            {
                db.Remove(todo);
                await db.SaveChangesAsync();
            }
            } catch(Exception error)
            {
                Console.WriteLine(" ************************************************************ ");
                Console.WriteLine(error.Message + " ---- ");
            }
        }
    }
}
