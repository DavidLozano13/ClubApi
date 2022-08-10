using System;
using System.Collections.Generic;
using ClubApi.Models;

namespace ClubApi.Services
{
    public interface IClubService
    {
        /// <summary>
        /// Añadir club
        /// </summary>
        /// <param name="club"></param>
        CLUB AddClub(CLUB club);

        /// <summary>
        /// Modificar club
        /// </summary>
        /// <param name="club"></param>
        CLUB ModifyClub(CLUB club);

        /// <summary>
        /// Eliminar club
        /// </summary>
        /// <param name="id"></param>
        void DeleteClub(Guid id);

        /// <summary>
        /// Obtener entrenador/jugador con búsqueda y paginado
        /// </summary>
        /// <param name="club"></param>
        List<PERSONA> GetPerson(Guid idClub, string buscar, int pagina = 1, int registrosTotales = 10);

        /// <summary>
        /// Añadir entrenador/jugador
        /// </summary>
        /// <param name="club"></param>
        PERSONA AddPerson(PERSONA persona);

        /// <summary>
        /// Modificar entrenador/jugador
        /// </summary>
        /// <param name="club"></param>
        PERSONA ModifyPerson(PERSONA persona);

        /// <summary>
        /// Eliminar entrenador/jugador
        /// </summary>
        /// <param name="id"></param>
        void DeletePersona(Guid id);
    }
}