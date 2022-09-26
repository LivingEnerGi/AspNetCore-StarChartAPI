using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext Context)
        {
            _context = Context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (celestialObject == null) return NotFound();

            var SatelliteObjects = _context.CelestialObjects.Where(co => co.OrbitedObjectId == id).ToList();
            if (SatelliteObjects.Count > 0)
            {
                celestialObject.Satellites = SatelliteObjects;
            }

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(co => co.Name == name).ToList();

            if (celestialObjects.Count == 0 ) return NotFound();

            SetSatelliteObjects(celestialObjects);

            return Ok(celestialObjects);
        }

        void SetSatelliteObjects(List<CelestialObject> CelestialObjects)
        {
            foreach (var celestialObj in CelestialObjects)
            {
                var SatelliteObjects = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObj.Id).ToList();
                if (SatelliteObjects.Count > 0)
                {
                    celestialObj.Satellites = SatelliteObjects;
                }
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            SetSatelliteObjects(celestialObjects);

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create(CelestialObject CelestialObject)
        {
            _context.CelestialObjects.Add(CelestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = CelestialObject.Id }, CelestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celObjIn)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (celestialObject == null) return NotFound();

            celestialObject.Name = celObjIn.Name;
            celestialObject.OrbitalPeriod = celObjIn.OrbitalPeriod;
            celestialObject.OrbitedObjectId = celObjIn.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (celestialObject == null) return NotFound();

            celestialObject.Name = name;

            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(co => (co.Id == id || co.OrbitedObjectId == id)).ToList();

            if (celestialObjects.Count == 0) return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
