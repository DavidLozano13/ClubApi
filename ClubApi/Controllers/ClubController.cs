using ClubApi.Models;
using ClubApi.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Web.Http;

namespace ClubApi.Controllers
{
    public class ClubController : ApiController
    {
        [HttpGet]
        public IEnumerable<CLUB> Get()
        {
            using (entities db = new entities())
            {
                return db.CLUB.ToList();
            }
        }

        /// <summary>
        /// Dar de alta un club
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AltaClub([FromBody] CLUB club)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var clubService = new ClubService();
                    club = clubService.AddClub(club);
                    return Ok(club);
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
        /// Modificar un club
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult ModificarClub(Guid id, [FromBody] CLUB club)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var clubService = new ClubService();
                    club = clubService.ModifyClub(club);
                    return Ok(club);
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
        /// Dar de baja
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult BajaClub(Guid id)
        {
            try
            {
                var clubService = new ClubService();
                clubService.DeleteClub(id);
                return Ok();
            }
            catch (ObjectNotFoundException)
            {
                return NotFound();
            }
        }
    }
}