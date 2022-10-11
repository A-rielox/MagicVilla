using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Models;
//using MagicVilla_WebAPI.Logging;
using MagicVilla_WebAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// logging ya viene listo para usarse, no necesito declararle en Program.cs para poder inyectarlo

namespace MagicVilla_WebAPI.Controllers
{
    [Route("api/villaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        //private readonly ILogging _logger;

        public VillaAPIController(ApplicationDbContext db /*ILogging logger*/)
        {
            this._db = db;
            //this._logger = logger;
        }

        /////////////////////////////////////
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            //_logger.Log("Getting all villas.", "warning");
            var villas = _db.Villas.ToList();

            return Ok(villas);
        }

        /////////////////////////////////////
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                //_logger.Log("GetVilla error with Id: " + id, "error");
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        /////////////////////////////////////
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            // para revisar q el nombre sea unico
            //if (VillaStore.villaList.FirstOrDefault(v => v.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            if (_db.Villas.FirstOrDefault(v => v.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Already Exists.");

                return BadRequest(ModelState);
            }


            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            if (villaDTO.Id > 0)
            {
                // si quiero devolver algo q no esta dentro del ActionResult< ... > puedo devolver el StatusCode
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
            };

            _db.Villas.Add(model);
            _db.SaveChanges();

            // para devolver el link donde se encuentra el creado
            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);

        }

        /////////////////////////////////////
        // devuelvo IActionResult solito ( sin tipo ) cuando no voy a devolver un tipo
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            _db.SaveChanges();

            return NoContent();
        }

        /////////////////////////////////////
        ///
        [HttpPut("{id:int}", Name = "UpdateVilla")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }

            //var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
            };

            _db.Villas.Update(model);
            _db.SaveChanges();

            return NoContent();
        }

        /////////////////////////////////////
        ///
        // para patch instalo y añado a controllers en program.cs
        // <PackageReference Include = "Microsoft.AspNetCore.JsonPatch" Version="6.0.9" />
        // <PackageReference Include = "Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="6.0.9" />

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            // .AsNoTracking() p' q no trackee esta entity, ya q la q quiero salvar es el model q 
            // defino abajo ( y los 2 van a tener el mismo Id )
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return BadRequest();
                // o notfound()
            }

            VillaDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };

            // el 2do arg para q si hay error lo mande al model state
            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
            };

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _db.Villas.Update(model);
            _db.SaveChanges();

            return NoContent();

            // el body q voy a necesitar para hacer update al nombre  ( info https://jsonpatch.com/ )
            //[
            //  {
            //    "path": "name",
            //    "op": "replace",
            //    "value": "cacuna"
            //  }
            //]
        }
    }
}
