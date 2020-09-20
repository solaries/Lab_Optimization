using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Security.Cryptography; 
using System.Text;
using Lo.Data;
using Lo.Data.Models;
using Lo.Models;  
using Lo.BusinessLogic;  
using System.Net;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace Lo.Controllers
{ 
    [Authorize]
    public class SUPERADMINController : Controller
    {  


          private void getStatus(bool clearStatus =true)
        { 
            if (Session["status"] != null)
            {
                if (Session["status"].ToString().Trim().Length > 0)
                {
                    ViewBag.status = Session["status"].ToString();
                    if (clearStatus)
                    {
                         Session["status"] = "";
                    }
                }
            }
            if (Session["down"] != null)
            {
                if (Session["down"].ToString().Trim().Length > 0)
                {
                    ViewBag.down = Session["down"].ToString();
                    Session["down"] = ""; 
                }
            }
        }
        private bool validateAccessToken(string token)
        {
             string urlPath = Request.Url.ToString().Split(new string[] { Request.Path.ToString() }, StringSplitOptions.RemoveEmptyEntries)[0];
             if (Request.Path.ToString().Trim().Length == 1)
             {
                 urlPath = Request.Url.ToString();
             }
             var newHttpRequest = (HttpWebRequest)WebRequest.Create(urlPath + "/api/GoodToken");
             var data = Encoding.ASCII.GetBytes("");
             newHttpRequest.Method = "GET";
             newHttpRequest.Headers.Add("Authorization", "Bearer " + token); 
             try
             {
                 var newHttpResponse = (HttpWebResponse)newHttpRequest.GetResponse();
                 var responseString = new StreamReader(newHttpResponse.GetResponseStream()).ReadToEnd();
             }
             catch (Exception ex)
             {
                 return false;
             } 
             return true;
        } 
        private string doAuthenticate(string userName, string password, string clientID)
        {
             string result = ""; 
             string dataToSend = "&username=" + HttpUtility.UrlEncode(userName) + "&password=" + HttpUtility.UrlEncode(password) + "&clientid=" + HttpUtility.UrlEncode(clientID) + "&grant_type=password";
             string urlPath = Request.Url.ToString().Split(new string[] { Request.Path.ToString() }, StringSplitOptions.RemoveEmptyEntries)[0];
             if (Request.Path.ToString().Trim().Length == 1)
             {
                 urlPath = Request.Url.ToString();
             }
             var newHttpRequest = (HttpWebRequest)WebRequest.Create(urlPath + "/token");
             var data = Encoding.ASCII.GetBytes(dataToSend);
             newHttpRequest.Method = "POST"; 
             newHttpRequest.ContentType = "application/x-www-form-urlencoded"; 
             newHttpRequest.ContentLength = data.Length;
             using (var streamProcess = newHttpRequest.GetRequestStream())
             {
                 streamProcess.Write(data, 0, data.Length);
             }
             try
             {
                 var newHttpResponse = (HttpWebResponse)newHttpRequest.GetResponse();
                 var responseString = new StreamReader(newHttpResponse.GetResponseStream()).ReadToEnd();
                 dynamic passString = JsonConvert.DeserializeObject<dynamic>(responseString);
                 result = (string)passString.access_token; 
             }
             catch (Exception d)
             { 
             }
             return result;
        }

        [AllowAnonymous]
        public ActionResult login()
        {
            Audit.protocol();
            getStatus();
            Session.Clear(); 
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult login(string First_name,string Last_name,string Email,string Password,string Password2,   string forgot)
        {  
            Audit.protocol();
            string token = doAuthenticate(userName: Email, password: Password, clientID: "SuperAdmin");
            bool result = validateAccessToken(token);
            List<Lo_authenticate_SuperAdmin> response =null;
            if (String.IsNullOrEmpty(forgot))
            {
                ActionResult xx = authenticate( password:  Password, email: Email ); 
                response = ( List<Lo_authenticate_SuperAdmin>)Session["response"];
                if(response !=null ){
                    if(response.Count > 0){
                         if (Encoding.ASCII.GetString(response[0].Password) == Encoding.ASCII.GetString(response[0].Password2))
                         { 
                            Session["userID"] = response[0].Id ;
                            Session["userType"] = "SuperAdmin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["status"] = "Please change your password";
                            return RedirectToAction("Change_Password", "SuperAdmin");
                        }
                        else  
                        {  
                            Session["userID"] = response[0].Id ;
                            Session["status"] = "";
                            Session["userType"] = "SuperAdmin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            return RedirectToAction("Change_Password", "SuperAdmin"); 
                        }
                    }
                    else
                    {
                        Session["status"] = "Authentication Not successful";
                        return RedirectToAction("Login", "SuperAdmin");
                    } 
                }
                else
                {
                    Session["status"] = "Authentication Not successful";
                    return RedirectToAction("Login", "SuperAdmin");
                } 
            }
            else { 
                ActionResult xx = forgotauthenticate_SuperAdmin(   Email: Email ); 
                response = ( List<Lo_authenticate_SuperAdmin>)Session["response"]; 
                return RedirectToAction("Login", "SuperAdmin");
            }
        } 

        [AllowAnonymous]
        public ActionResult authenticate(string email, string password )
        { 
            Audit.protocol(); 
            List<Lo_authenticate_SuperAdmin> response = null;  
            password =  Audit.GetEncodedHash(password, "doing it well") ;
            response =  centralCalls.get_authenticate_SuperAdmin(" where replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            Session["response"]  = response;
            return Content(JsonConvert.SerializeObject((List<Lo_authenticate_SuperAdmin>)response));
        }

        [AllowAnonymous]
        public ActionResult forgotauthenticate_SuperAdmin(string Email )
        {   
            Audit.protocol();
            List<Lo_authenticate_SuperAdmin> response = null; 
            response =  centralCalls.get_authenticate_SuperAdmin(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = Audit.GenerateRandom();
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        centralCalls.update_authenticate_SuperAdmin(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)   , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString(arr)) ; 
                        string mailSubject = "Profile password reset on (code joh) Lab Optimum";
                        Session["status"] = "Password reset successful, please check your email for your new password.";
                        string mailBody = "Hi <br><br>Your password has been successfully reset on the (code joh) Lab Optimum platform. Please log in with following credentials: <br><br> Email:" + response[0].Email + "<br><br>password :" + strRND + "<br><br><br>Regards<br><br>";
                        Audit.SendMail(Email, mailSubject, mailBody, " "); 
                } 
            } 
            Session["response"]  = response; 
             return Content(JsonConvert.SerializeObject((List<Lo_authenticate_SuperAdmin>)response)); ;
        }   


        [AllowAnonymous]
        public ActionResult Change_Password()
        {
                Audit.protocol();
            getStatus(false);
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            ViewBag.first_name=Session["first_name"];
            ViewBag.last_name=Session["last_name"];
            ViewBag.email=Session["email"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Change_Password(string email,string password,string npassword )
        {  
                Audit.protocol();
                ActionResult xx = updatePassword( password:  password,npassword:  npassword , email: email ); 
                Session["status"] = (string)Session["response"];
                if(Session["response"].ToString().IndexOf("update successful") > -1)
                {
                    Session["status"] = "Password Changed Successfully";
                    return RedirectToAction("Login", "SuperAdmin") ;
                }
                else
                {
                    return RedirectToAction("Change_Password", "SuperAdmin") ;
                }
        } 


        [AllowAnonymous]
        public ActionResult updatePassword(string email,string password,string npassword )
        {   
            Audit.protocol();
            List<Lo_authenticate_SuperAdmin> response = null; 
            string result = "Authentication failed"; 
            string strRND11 = password;
            byte[] arr11 = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND11, "doing it well")); 
            response =  centralCalls.get_authenticate_SuperAdmin(" where replace(password, '@','#')  = '" + Encoding.ASCII.GetString(arr11).Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = npassword;
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        result =  centralCalls.update_authenticate_SuperAdmin(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)   , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString( response[0].Password2)) ; 
                } 
            } 
            Session["response"]  = result; 
            return Content((string)result);
        }   


        [AllowAnonymous]
        public ActionResult new_Labs()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Labs(string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
                ActionResult xx =  add_Labs(Lab: Lab,token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Labs", "SuperAdmin");
        } 

        [AllowAnonymous]
        public ActionResult add_Labs(string Lab,string token)
        { 
                Audit.protocol();
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_Lab(Lab: Lab);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Labs()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            List<Lo_Lab> response = null; 
           ActionResult d =  view_it_Labs(Session["token"].ToString()  ); 
            return View((List<Lo_Lab>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Labs(string token)
        {
                Audit.protocol();
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_Lab>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Lab("");
            return Content(JsonConvert.SerializeObject( ((List<Lo_Lab>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Labs(string id,string Lab  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Lab = Lab;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Labs(string id,string oLab,string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            string response =null;
                ActionResult xx =  update_Labs(id:id,oLab:  oLab,Lab: Lab, token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Labs", "SuperAdmin");
                }
                else{
                    return RedirectToAction("view_Labs", "SuperAdmin");
                     return View();
                }
                return RedirectToAction("new_Labs", "SuperAdmin");
        } 

        [AllowAnonymous]
        public ActionResult update_Labs(string id, string oLab,string Lab,string token)
        { 
                Audit.protocol();
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_Lab(id:id,oLab:  oLab,Lab: Lab,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Administrators()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            ViewBag.Data0 =  centralCalls.get_role_Admin("");
            ViewBag.Data1 =  centralCalls.get_Lab("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Administrators(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
                
            if(Password == null)
            {
                 Password = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes( Audit.GetEncodedHash(Audit.GenerateRandom(), "doing it well")));;
            }
            if(Password2 == null)
            {
                 Password2 = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes( Audit.GetEncodedHash(Audit.GenerateRandom(), "doing it well")));;
            }
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
                ActionResult xx =  add_Administrators(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Lab: Lab,token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Administrators", "SuperAdmin");
        } 

        [AllowAnonymous]
        public ActionResult add_Administrators(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab,string token)
        { 
                Audit.protocol();
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_authenticate_Admin(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Lab: Lab);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Administrators()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            List<Lo_authenticate_Admin> response = null; 
           ActionResult d =  view_it_Administrators(Session["token"].ToString()  ); 
            return View((List<Lo_authenticate_Admin_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Administrators(string token)
        {
                Audit.protocol();
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_authenticate_Admin>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_authenticate_Admin("");
            return Content(JsonConvert.SerializeObject( ((List<Lo_authenticate_Admin_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Administrators(string id,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
            ViewBag.Data0 =  centralCalls.get_role_Admin("");
            ViewBag.Data1 =  centralCalls.get_Lab("");
            getStatus();
            ViewBag.id=id;
             ViewBag.First_name = First_name;
 ViewBag.Last_name = Last_name;
 ViewBag.Email = Email;
 ViewBag.Role = Role;
 ViewBag.Password = Password;
 ViewBag.Password2 = Password2;
 ViewBag.Lab = Lab;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Administrators(string id,string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oLab,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "SuperAdmin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "SuperAdmin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "SuperAdmin");
                }
            string response =null;
                ActionResult xx =  update_Administrators(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,oLab:  oLab,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Lab: Lab, token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Administrators", "SuperAdmin");
                }
                else{
                    return RedirectToAction("view_Administrators", "SuperAdmin");
                     return View();
                }
                return RedirectToAction("new_Administrators", "SuperAdmin");
        } 

        [AllowAnonymous]
        public ActionResult update_Administrators(string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oLab,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab,string token)
        { 
                Audit.protocol();
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_authenticate_Admin(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,oLab:  oLab,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Lab: Lab,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }


   
    }
} 
