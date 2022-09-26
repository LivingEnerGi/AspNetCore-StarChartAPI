using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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

        void SetSatelliteObjects(List<Models.CelestialObject> CelestialObjects)
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
    }
}
