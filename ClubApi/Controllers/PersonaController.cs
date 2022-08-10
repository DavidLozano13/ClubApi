using ClubApi.Models;
using ClubApi.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Web.Http;

namespace ClubApi.Controllers
{
    public class PersonaController : ApiController
    {
        [HttpGet]
        public IEnumerable<PERSONA> Get(Guid idClub, string buscar, int pagina = 1, int registrosTotales = 10)
        {
            var clubService = new ClubService();
            return clubService.GetPerson(idClub, buscar, pagina, registrosTotales);
        }

        /// <summary>
        /// Dar de alta un jugador/entrenador
        /// </summary>
        /// <param name="persona"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AltaPersona([FromBody] PERSONA persona)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var clubService = new ClubService();
                    persona = clubService.AddPerson(persona);
                    return Ok(persona);
                }
                catch (ObjectNotFoundException)
                {
                    return NotFound();
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Modificar un jugador/entrenador
        /// </summary>
        /// <param name="persona"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult ModificarPersona(Guid id, [FromBody] PERSONA persona)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var clubService = new ClubService();
                    persona = clubService.ModifyPerson(persona);
                    return Ok(persona);
                }
                catch (ObjectNotFoundException)
                {
                    return NotFound();
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Dar de baja entrenador/jugador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult BajaPersona(Guid id)
        {
            try
            {
                var clubService = new ClubService();
                clubService.DeletePersona(id);
                return Ok();
            }
            catch (ObjectNotFoundException)
            {
                return NotFound();
            }
        }
    }
}