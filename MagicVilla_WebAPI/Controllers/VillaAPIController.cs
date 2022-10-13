using AutoMapper;
using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Models;
//using MagicVilla_WebAPI.Logging;
using MagicVilla_WebAPI.Models.Dto;
using MagicVilla_WebAPI.Repository.IRepository;
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
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;

        //private readonly ILogging _logger;

        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper /*ILogging logger*/)
        {
            this._dbVilla = dbVilla;
            this._mapper = mapper;
            //this._logger = logger;
        }

        //////////////////////////////////////////////
        /////////////////////////////////////////////////
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            //_logger.Log("Getting all villas.", "warning");
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
            var villasDTOList = _mapper.Map<List<VillaDTO>>(villaList);

            return Ok(villasDTOList);
        }

        //////////////////////////////////////////////
        /////////////////////////////////////////////////
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                //_logger.Log("GetVilla error with Id: " + id, "error");
                return BadRequest();
            }

            // var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            var villa = await _dbVilla.GetAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        //////////////////////////////////////////////
        /////////////////////////////////////////////////
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            // para revisar q el nombre sea unico
            //if (VillaStore.villaList.FirstOrDefault(v => v.Name.ToLower() == createDTO.Name.ToLower()) != null)
            if (await _dbVilla.GetAsync(v => v.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Already Exists.");

                return BadRequest(ModelState);
            }


            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            //if (createDTO.Id > 0)
            //{
            //    // si quiero devolver algo q no esta dentro del ActionResult< ... > puedo devolver el StatusCode
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            Villa model = _mapper.Map<Villa>(createDTO);

            await _dbVilla.CreateAsync(model);

            // ya que EF guarda los cambios automaticamente voy a tener el Id q asigno en la entity "model"

            // para devolver el link donde se encuentra el creado
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);

        }

        //////////////////////////////////////////////
        /////////////////////////////////////////////////
        // devuelvo IActionResult solito ( sin tipo ) cuando no voy a devolver un tipo
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            await _dbVilla.RemoveAsync(villa);

            return NoContent();
        }

        //////////////////////////////////////////////
        /////////////////////////////////////////////////
        [HttpPut("{id:int}", Name = "UpdateVilla")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }

            //var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            Villa model = _mapper.Map<Villa>(updateDTO);

            await _dbVilla.UpdateAsync(model);

            return NoContent();
        }

        //////////////////////////////////////////////
        /////////////////////////////////////////////////
        // para patch instalo y añado a controllers en program.cs
        // <PackageReference Include = "Microsoft.AspNetCore.JsonPatch" Version="6.0.9" />
        // <PackageReference Include = "Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="6.0.9" />

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            // .AsNoTracking() p' q no trackee esta entity, ya q la q quiero salvar es el model q 
            // defino abajo ( y los 2 van a tener el mismo Id )
            //var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            var villa = await _dbVilla.GetAsync(v => v.Id == id,tracked: false);

            if (villa == null)
            {
                return BadRequest();
                // o notfound()
            }

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            // el 2do arg para q si hay error lo mande al model state
            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _dbVilla.UpdateAsync(model);

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
