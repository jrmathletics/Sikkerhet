using Oblig1.Models;
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
            if (Session["LoggetInn"] == null)
            {
                Session["LoggetInn"] = false;
                ViewBag.Innlogget = false;
            }
            else
            {
                ViewBag.Innlogget = (bool)Session["LoggetInn"];
            }
            
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(Oblig1.Models.User user)
        {
            if (Bruker_i_DB(user))
            {
                Session["LoggetInn"] = true;
                ViewBag.Innlogget = true;
                return View();
            }
            else
            {
                Session["LoggetInn"] = false;
                ViewBag.Innlogget = false;
                return View();
            }
            
        }

        public ActionResult Registrer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrer(Oblig1.Models.User user)
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

        public ActionResult InnLoggetSide()
        {
            if (Session["LoggetInn"] != null)
            {
                bool loggetInn = (bool)Session["LoggetInn"];
                if(loggetInn)
                {
                    return View();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
