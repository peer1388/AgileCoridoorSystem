﻿using CorridorSystem.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using CorridorSystem.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CorridorSystem.Controllers
{
    public class UserController : ApiController
    {
        // Get a specific user by userId
        // GET: api/User/userId
        [Authorize]
        [Route("api/User/{uId}")]
        public IHttpActionResult Get(int uId)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            using (var db = new ModelContext())
            {
                CorrUser matchingUser = db.MyUsers.Where(x => x.Id == uId).FirstOrDefault<CorrUser>();
                if (matchingUser.Id == uId)
                {
                    matchingUser.updateStatus(db);
                    return Ok(matchingUser);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        //returns all users of that type
        // GET: api/Users/{type}
        [Route("api/Users/{uType}")]
        public IHttpActionResult Get1(int uType)
        {
            using (var db = new ModelContext())
            {
                List<CorrUser> allUsers = db.MyUsers.Include("schedule").Where(x => x.UserType == uType).ToList();
                foreach (CorrUser user in allUsers)
                    user.updateStatus(db);
                return Ok(allUsers);
            }
        }
        [Route("api/Users")]
        public IHttpActionResult Get2()
        {
            using (var db = new ModelContext())
            {
                List<int> alltypes = new List<int>();
                List<CorrUser> allUsers = db.MyUsers.ToList();
                foreach (CorrUser user in allUsers)
                {
                    if (!alltypes.Contains(user.UserType))
                    {
                        alltypes.Add(user.UserType);
                    }
                }
                return Ok(alltypes);
            }
        }

        [Authorize]
        [Route("api/User")]
        public IHttpActionResult Get()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            using (var db = new ModelContext())
            {
                var myUser = db.MyUsers.Include("schedule").Where(x => x.UserName == myUsername).FirstOrDefault<CorrUser>();
                myUser.updateStatus(db);
                return Ok(myUser);
            }
        }


        // POST: api/User
        public void Post([FromBody]string value)
        {
        }

        //Update info for one user
        // PUT: api/User/{uId}
        [Authorize]
        [Route("api/Users/{uId}")]
        public IHttpActionResult Put([FromBody]CorrUser newUserValues, int uId)
        {
            using (var db = new ModelContext())
            {
                var userToUpdate = db.MyUsers.Where(x => x.Id == uId).FirstOrDefault<CorrUser>();
                Dictionary<string, string> u = new Dictionary<string, string>();
                u.Add("UserName", newUserValues.UserName);
                u.Add("FirstName", newUserValues.FirstName);
                u.Add("LastName", newUserValues.LastName);
                u.Add("Email", newUserValues.Email);
                u.Add("Title", newUserValues.Title);
                foreach (var value in u)
                {
                    if (value.Value != null)
                    {
                        var newValue = value.Value;
                        var oldValue = userToUpdate.GetType().GetProperty(value.Key).GetValue(userToUpdate, null);
                        if (value.Key != "id")
                        {
                            userToUpdate.GetType().GetProperty(value.Key).SetValue(userToUpdate, newValue.ToString());
                        }
                    }
                }

                db.MyUsers.AddOrUpdate(userToUpdate);
                db.SaveChanges();
                return Ok();
            }
        }

        // Deactivate a user
        // DELETE: api/Users/{uId}
        [Authorize]
        [Route("api/Users/{uId}")]
        public IHttpActionResult Delete(int uId)
        {
            using (var db = new ModelContext())
            {
                var userToDeactivate = db.MyUsers.Where(x => x.Id == uId).FirstOrDefault<CorrUser>();
                RemovedUsers rmUser = new RemovedUsers();

                foreach (var value in userToDeactivate.GetType().GetProperties())
                {

                    var newValue = value.GetValue(userToDeactivate, null);

                    rmUser.GetType().GetProperty(value.Name).SetValue(rmUser, newValue.ToString());
                }

                db.MyUsers.Remove(userToDeactivate);
                db.RmUsers.AddOrUpdate();
                db.MyUsers.AddOrUpdate();
                db.SaveChanges();
                return Ok();

            }

        }

        //POST api/Users/{uId}/Mail
        [Route("api/Users/{uId}/Mail")]
        public IHttpActionResult PostSendMail([FromBody]PostSendMailModel request, int uId)
        {
            try
            {
                using (ModelContext db = new ModelContext())
                {
                    CorrUser user = db.MyUsers.FirstOrDefault(u => u.Id == uId);

                    MailMessage message = new MailMessage("corridor@rudbeck.nu", user.Email);
                    message.Subject = request.Subject;
                    message.Body = request.StudentName + " " + request.Content;

                    var client = new SmtpClient("mail.rudbeck.nu", 25);
                    client.Credentials = new NetworkCredential()
                    {
                        UserName = "corridor@rudbeck.nu",
                        Password = "pelle123"
                    };

                    client.Send(message);
                    message.Dispose();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
