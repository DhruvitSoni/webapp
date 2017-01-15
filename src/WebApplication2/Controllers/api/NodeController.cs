using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using Newtonsoft.Json;
using WebApplication2.Data;
using Microsoft.EntityFrameworkCore;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication2.Controllers.api
{
    [Route("api/[controller]")]
    public class NodeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public NodeController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult post([FromBody]NodeDTO value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Node nodeDb = new Node();
                    if(value.ParentId == -1)
                    {
                        nodeDb.name = value.name;
                        _db.nodes.Add(nodeDb);
                        _db.SaveChanges();
                    }else
                    {
                        Node parentNodeDb = _db.nodes.Include(x => x.subNodes).Where(x => x.id == value.ParentId).First();
                        nodeDb.name = value.name;
                        parentNodeDb.subNodes.Add(nodeDb);
                        _db.SaveChanges();
                    }
                    return Json(new { data = nodeDb });
                }catch(Exception ex)
                {
                    return Json(new { error = ex.Message });
                }
            }else
            {
                return StatusCode(500, new { error = ModelState.Values.SelectMany(x => x.Errors).ToList() });
            }
        }

        [HttpGet]
        public IActionResult get()
        {
            List<Node> temp = _db.nodes.Include(x => x.subNodes).Where(x => x.ParentNode == null).Select(f => new Node { id = f.id, name = f.name, ParentId = f.ParentId, subNodes = f.subNodes }).ToList();

            return Content(JsonConvert.SerializeObject(new { data = get_all(temp) },Formatting.Indented),"application/json");
        }
        
        public List<Node> get_all(List<Node> list)
        {
            int z = 0;
            List<Node> lists = new List<Node>();

            if(list.Count > 0)
            {
                lists.AddRange(list);
            }

            foreach(Node x in list)
            {
                Node dbNode = _db.nodes.Include(y => y.subNodes).Where(y => y.id == x.id).Select(y => new Node { id = y.id, name = y.name, ParentId = y.ParentId, subNodes = y.subNodes }).First();
                if(dbNode.subNodes == null)
                {
                    z++;
                    continue;
                }

                List<Node> sub = dbNode.subNodes.ToList();
                dbNode.subNodes = get_all(sub);
                lists[z] = dbNode;
                z++;
            }
            return lists;
        }
        
    }
}
