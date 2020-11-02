using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name ="GetById")]
        public IActionResult GetById(int id)
        {

            var celestialObj = _context.CelestialObjects.Find(id);          

            if (celestialObj == null)
            {
                return NotFound();
            }

            celestialObj.Satellites = _context.CelestialObjects.Where<CelestialObject>(c => c.OrbitedObjectId == id).ToList<CelestialObject>();          

            return Ok(celestialObj);
           

        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObj = _context.CelestialObjects.Where(c => c.Name == name);

            if (!celestialObj.Any())
            {
                return NotFound();
            }

            foreach (var obj in celestialObj)
            {
                obj.Satellites = _context.CelestialObjects.Where<CelestialObject>(c => c.OrbitedObjectId == obj.Id).ToList();
            }
           

            return Ok(celestialObj.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObj = _context.CelestialObjects.ToList();
            foreach (var obj in celestialObj)
            {
                obj.Satellites = _context.CelestialObjects.Where<CelestialObject>(c => c.OrbitedObjectId == obj.Id).ToList();
            }

            return Ok(celestialObj.ToList());

        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject obj)
        {
            _context.CelestialObjects.Add(obj);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = obj.Id }, obj);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject obj)
        {
            var celestialObj = _context.CelestialObjects.Find(id);
            if (celestialObj == null)
            {
                return NotFound();
            }

            celestialObj.Name = obj.Name;
            celestialObj.OrbitalPeriod = obj.OrbitalPeriod;
            celestialObj.OrbitedObjectId = obj.OrbitedObjectId;
            _context.CelestialObjects.Update(celestialObj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObj = _context.CelestialObjects.Find(id);

            if (celestialObj == null)
            {
                return NotFound();
            }

            celestialObj.Name = name;
            _context.CelestialObjects.Update(celestialObj);
            _context.SaveChanges();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObject = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id);

            if (! celestialObject.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialObject);
            _context.SaveChanges();

            return NoContent();

        }
    }
}
