using ClubApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;

namespace ClubApi.Services
{
    public class ClubService : IClubService
    {
        private entities Context = new entities();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="club"></param>
        public CLUB AddClub(CLUB club)
        {
            club.id = Guid.NewGuid();
            Context.CLUB.Add(club);
            Context.SaveChanges();
            return club;
        }

        public CLUB ModifyClub(CLUB club)
        {
            // Comprobamos que exista el club a modificar
            var existeClub = Context.CLUB.Any(o => o.id == club.id);

            if (!existeClub)
            {
                throw new ObjectNotFoundException();
            }

            // Comprobamos que los salarios actuales no excedan el nuevo presupuesto
            int salariosClub = Context.PERSONA.Where(o => o.idClub == club.id).Sum(o => o.salario) ?? 0;
            if (salariosClub > club.presupuestoInicial)
            {
                throw new ApplicationException("El presupuesto del club debe ser superior a los salarios de los jugadores y entrenadores.");
            }

            Context.Entry(club).State = EntityState.Modified;
            Context.SaveChanges();
            return club;
        }

        public void DeleteClub(Guid id)
        {
            var club = Context.CLUB.Find(id);

            if (club == null)
            {
                throw new ObjectNotFoundException();
            }

            Context.PERSONA.RemoveRange(Context.PERSONA.Where(o => o.idClub == id));
            Context.CLUB.Remove(club);
            Context.SaveChanges();
        }

        public List<PERSONA> GetPerson(Guid idClub, string buscar, int pagina = 1, int registrosTotales = 10)
        {
            List<PERSONA> personas = Context.PERSONA.Where(o => o.idClub == idClub).ToList();

            // Filtramos el resultado
            if (!string.IsNullOrEmpty(buscar))
            {
                personas = personas.Where(x => x.nombre.Contains(buscar)).ToList();
            }

            // Paginamos el resultado
            personas = personas
                .Skip((pagina - 1) * registrosTotales)
                .Take(registrosTotales)
                .ToList();

            return personas;
        }

        public PERSONA AddPerson(PERSONA persona)
        {
            // Si se va a asociar la persona a un club, su salario es obligatorio
            if (persona.idClub != null && persona.salario == null)
            {
                throw new ApplicationException($"{nameof(persona.salario)} required");
            }

            // En caso de que la persona venga con club, comprobamos que exista
            if (persona.idClub != null && !Context.CLUB.Any(o => o.id == persona.idClub))
            {
                throw new ApplicationException($"{nameof(persona.idClub)} not found");
            }

            // Comprobamos que los salarios del club no excedan el nuevo presupuesto
            if (persona.idClub != null)
            {
                var presupuestoInicial = Context.CLUB.Where(o => o.id == persona.idClub).Select(o => o.presupuestoInicial).FirstOrDefault();
                int salariosClub = Context.PERSONA.Where(o => o.idClub == persona.idClub).Sum(o => o.salario) ?? 0;
                if (salariosClub + persona.salario > presupuestoInicial)
                {
                    throw new ApplicationException("El presupuesto del club debe ser superior a los salarios de los jugadores y entrenadores.");
                }
            }

            persona.id = Guid.NewGuid();
            Context.PERSONA.Add(persona);
            Context.SaveChanges();

            this.SendNotification(persona.mail);
            return persona;
        }

        public PERSONA ModifyPerson(PERSONA persona)
        {
            var existePersona = Context.PERSONA.Any(o => o.id == persona.id);

            if (!existePersona)
            {
                throw new ObjectNotFoundException();
            }

            // Comprobamos que los salarios del club no excedan el nuevo presupuesto
            if (persona.idClub != null)
            {
                var presupuestoInicial = Context.CLUB.Where(o => o.id == persona.idClub).Select(o => o.presupuestoInicial).FirstOrDefault();
                int salariosClub = Context.PERSONA.Where(o => o.idClub == persona.idClub && o.id != persona.id).Sum(o => o.salario) ?? 0;
                if (salariosClub + persona.salario > presupuestoInicial)
                {
                    throw new ApplicationException("El presupuesto del club debe ser superior a los salarios de los jugadores y entrenadores.");
                }
            }

            Context.Entry(persona).State = EntityState.Modified;
            Context.SaveChanges();
            return persona;
        }

        public void DeletePersona(Guid id)
        {
            var persona = Context.PERSONA.Find(id);

            if (persona == null)
            {
                throw new ObjectNotFoundException();
            }
            
            Context.PERSONA.Remove(persona);
            Context.SaveChanges();
        }

        /// <summary>
        /// Notificar al jugador/entrenador que ha sido dado de alta/baja de un club
        /// </summary>
        /// <param name="mail"></param>
        private void SendNotification(string mail)
        {
            // Enviamos notificación para todos los posibles modos de notificación del sistema
            var notificationServices = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(INotificationService).IsAssignableFrom(p) && p.IsClass);

            foreach (var notificationService in notificationServices)
            {
                ((INotificationService)notificationService).SendNotification(mail);
            }
        }
    }
}