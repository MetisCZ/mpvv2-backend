using System;
using System.Collections.Generic;
using System.Linq;
using mpvv2.DbModels;

namespace mpvv2.Models.DBModels
{
    public class VehicleManager
    {

        public static bool UpdateVehicle(string vehId, mpvContext context)
        {
            Vehicle veh = context.Vehicles.FirstOrDefault(v => v.Id == vehId);
            if (veh != null)
            {
                Depart dep = context.Depart
                    .Where(d => d.IdVeh == vehId || d.IdVeh2 == vehId || d.IdVeh3 == vehId)
                    .OrderByDescending(d => d.ActDate).FirstOrDefault();
                
                if (dep != null)
                    veh.LastSeen = dep.ActDate;
                else
                    veh.LastSeen = null;

                int departs = 0;
                var query = context.Depart.Where(d => d.IdVeh == vehId).GroupBy(d => d.IdVeh).Select(g => new {name = g.Key, count = g.Count()}).FirstOrDefault();
                if (query != null)
                    departs = query.count;
                
                var query2 = context.Depart.Where(d => d.IdVeh2 == vehId).GroupBy(d => d.IdVeh2).Select(g => new {name = g.Key, count = g.Count()}).FirstOrDefault();
                if (query2 != null)
                    departs += query2.count;
                
                var query3 = context.Depart.Where(d => d.IdVeh3 == vehId).GroupBy(d => d.IdVeh3).Select(g => new {name = g.Key, count = g.Count()}).FirstOrDefault();
                if (query3 != null)
                    departs += query3.count;
                
                veh.Departs = departs;

                return true;
            }

            return false;
        }

        public static bool UpdateDepartsSet(string vehId, string vehId2, string vehId3, DateTime from, DateTime to, mpvContext context)
        {
            var departs = context.Depart.Where(d => d.IdVeh == vehId && d.Date.Date.CompareTo(from.Date) >= 0 && d.Date.Date.CompareTo(to.Date) <= 0).ToList();

            List<string> vehsToUpdate = new List<string>();
            foreach (var depart in departs)
            {
                if (depart.IdVeh2 != null && depart.IdVeh2 != vehId2 && !vehsToUpdate.Contains(depart.IdVeh2))
                    vehsToUpdate.Add(depart.IdVeh2);
                if (depart.IdVeh3 != null && depart.IdVeh3 != vehId3 && !vehsToUpdate.Contains(depart.IdVeh3))
                    vehsToUpdate.Add(depart.IdVeh3);
                depart.IdVeh2 = vehId2;
                depart.IdVeh3 = vehId3;
            }
            if(vehId2 != null && !vehsToUpdate.Contains(vehId2))
                vehsToUpdate.Add(vehId2);
            if(vehId3 != null && !vehsToUpdate.Contains(vehId3))
                vehsToUpdate.Add(vehId3);

            context.SaveChanges();

            foreach (var vId in vehsToUpdate)
            {
                UpdateVehicle(vId, context);
            }
            
            return true;
        }

        public static bool UpdateLastSeen(string vehId, mpvContext context)
        {
            Vehicle veh = context.Vehicles.FirstOrDefault(v => v.Id == vehId);
            if (veh != null)
            {
                Depart dep = context.Depart.Where(d => d.IdVeh == vehId || d.IdVeh2 == vehId || d.IdVeh3 == vehId).OrderByDescending(d=>d.ActDate).FirstOrDefault();
                if (dep != null)
                {
                    veh.LastSeen = dep.ActDate;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;

            return true;
        }
    }
}