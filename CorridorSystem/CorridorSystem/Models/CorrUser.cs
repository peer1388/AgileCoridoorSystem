﻿using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
using CorridorSystem.Models.DAL;

namespace CorridorSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual CorrUser CorrUser { get; set; }
        public virtual RemovedUsers RemovedUsers { get; set; }
    }

    public class CorrUser
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        [Required]
        public int UserType { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string signature { get; set; }
        public string status { get; set; }
        public scheduleModel schedule { get; set; }
        public void updateStatus(ModelContext db)
        {
            
            bool hasBeenChanged = false;
            DateTime now = DateTime.Now;
            foreach(eventModel eve in db.schedule.Include("events").FirstOrDefault(s => s.Id == schedule.Id).events)
            {
                if(eve.DTStart < now && eve.DTEnd > now)
                {
                    status = eve.status;
                    hasBeenChanged = true;
                }
            }
            if (!hasBeenChanged)
            {
                status = "Busy";
            }
            db.SaveChanges();
        }

    }
}