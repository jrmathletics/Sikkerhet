﻿using Oblig1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Oblig1.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            if (Session["LoggedIn"] == null)
            {
                Session["LoggedIn"] = false;
                ViewBag.LoggedIn = false;
            }
            else
            {
                ViewBag.LoggedIn = (bool)Session["LoggedIn"];
            }
            
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(Oblig1.Models.User user)
        {
            if (Bruker_i_DB(user))
            {
                Session["LoggedIn"] = true;
                ViewBag.LoggedIn = true;
                return View();
            }
            else
            {
                Session["LoggedIn"] = false;
                ViewBag.LoggedIn = false;
                return View();
            }
            
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Oblig1.Models.User user)
        {
            Oblig1.Models.PastaContext db = new PastaContext();
            if (ModelState.IsValid)
            {
                try
                {
                    var newUser = new dbUser();
                    byte[] passwordDb = createHash(user.Password);
                    newUser.Password = passwordDb;
                    user.Password = "11111111111";
                    newUser.Email = user.Email;

                    string innPostCode = user.city.Postcode;
                    
                    var foundCity = db.Cities.FirstOrDefault(p => p.Postcode == innPostCode);
                    
                    if (foundCity == null)
                    {
                        Debug.WriteLine("1");
                        db.dbUsers.Add(newUser);
                        Debug.WriteLine("2");
                        db.Users.Add(user);
                        db.SaveChanges();
                        Debug.WriteLine("3");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        user.city = foundCity;
                        db.Users.Add(user);
                        db.dbUsers.Add(newUser);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception fail)
                {

                }


            }

            return View(user);
        }
        private static byte[] createHash(string inPassword)
        {
            byte[] inData, outData;
            var algorithm = System.Security.Cryptography.SHA256.Create();
            inData = System.Text.Encoding.ASCII.GetBytes(inPassword);
            outData = algorithm.ComputeHash(inData);
            return outData;
        }

        private static bool Bruker_i_DB(Oblig1.Models.User user)
        {
            using (var db = new PastaContext())
            {
                byte[] passordDb = createHash(user.Password);
                dbUser foundUser = db.dbUsers.FirstOrDefault
                    (b => b.Password == passordDb && b.Email == user.Email);
                if (foundUser == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public ActionResult LoggedInSite()
        {
            if (Session["LoggedIn"] != null)
            {
                bool loggedIn = (bool)Session["LoggedIn"];
                if(loggedIn)
                {
                    return View();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Minside()
        {
            return View();
        }


    }
}
