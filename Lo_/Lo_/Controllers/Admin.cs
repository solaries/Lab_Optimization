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
    public class ADMINController : Controller
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
        public ActionResult login(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab,   string forgot)
        {  
            Audit.protocol();
            string token = doAuthenticate(userName: Email, password: Password, clientID: "Admin");
            bool result = validateAccessToken(token);
            List<Lo_authenticate_Admin_data> response =null;
            if (String.IsNullOrEmpty(forgot))
            {
                ActionResult xx = authenticate( password:  Password, email: Email ); 
                response = ( List<Lo_authenticate_Admin_data>)Session["response"];
                if(response !=null ){
                    if(response.Count > 0){
                         if (Encoding.ASCII.GetString(response[0].Password) == Encoding.ASCII.GetString(response[0].Password2))
                         { 
                            Session["userID"] = response[0].Id ;
                            Session["userType"] = "Admin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Lab"] =  response[0].Lab;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            List<Lo_right_Admin> rightList = centralCalls.get_right_Admin(" where id in (select `right` from Lo_role_right_Admin where role =" + response[0].Role.ToString() + " )   order by rightname");
                            string strRightList = "";
                            foreach (Lo_right_Admin right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList; 
                            Session["role"] = response[0].Role;
                            Session["status"] = "Please change your password";
                            return RedirectToAction("Change_Password", "Admin");
                        }
                        else  
                        {  
                            Session["userID"] = response[0].Id ;
                            Session["status"] = "";
                            Session["userType"] = "Admin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Lab"] =  response[0].Lab;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["role"] = response[0].Role;
                            List<Lo_right_Admin> rightList = centralCalls.get_right_Admin(" where id in (select `right` from Lo_role_right_Admin where role =" + response[0].Role.ToString() + " )   order by rightname");
                            string strRightList = "";
                            foreach (Lo_right_Admin right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList;
                            return RedirectToAction("view_Staff", "Admin"); 
                        }
                    }
                    else
                    {
                        Session["status"] = "Authentication Not successful";
                        return RedirectToAction("Login", "Admin");
                    } 
                }
                else
                {
                    Session["status"] = "Authentication Not successful";
                    return RedirectToAction("Login", "Admin");
                } 
            }
            else { 
                ActionResult xx = forgotauthenticate_Admin(   Email: Email ); 
                response = ( List<Lo_authenticate_Admin_data>)Session["response"]; 
                return RedirectToAction("Login", "Admin");
            }
        } 

        [AllowAnonymous]
        public ActionResult authenticate(string email, string password )
        { 
            Audit.protocol(); 
            List<Lo_authenticate_Admin_data> response = null;  
            password =  Audit.GetEncodedHash(password, "doing it well") ;
            response =  centralCalls.get_authenticate_Admin(" where replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            Session["response"]  = response;
            return Content(JsonConvert.SerializeObject((List<Lo_authenticate_Admin_data>)response));
        }

        [AllowAnonymous]
        public ActionResult forgotauthenticate_Admin(string Email )
        {   
            Audit.protocol();
            List<Lo_authenticate_Admin_data> response = null; 
            response =  centralCalls.get_authenticate_Admin(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = Audit.GenerateRandom();
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        centralCalls.update_authenticate_Admin(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oLab: response[0].Lab.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString(arr),Lab: response[0].Lab.ToString() ) ; 
                        string mailSubject = "Profile password reset on (code joh) Lab Optimum";
                        Session["status"] = "Password reset successful, please check your email for your new password.";
                        string mailBody = "Hi <br><br>Your password has been successfully reset on the (code joh) Lab Optimum platform. Please log in with following credentials: <br><br> Email:" + response[0].Email + "<br><br>password :" + strRND + "<br><br><br>Regards<br><br>";
                        Audit.SendMail(Email, mailSubject, mailBody, " "); 
                } 
            } 
            Session["response"]  = response; 
             return Content(JsonConvert.SerializeObject((List<Lo_authenticate_Admin_data>)response)); ;
        }   


        [AllowAnonymous]
        public ActionResult Change_Password()
        {
                Audit.protocol();
            getStatus(false);
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
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
                    return RedirectToAction("Login", "Admin") ;
                }
                else
                {
                    return RedirectToAction("Change_Password", "Admin") ;
                }
        } 


        [AllowAnonymous]
        public ActionResult updatePassword(string email,string password,string npassword )
        {   
            Audit.protocol();
            List<Lo_authenticate_Admin_data> response = null; 
            string result = "Authentication failed"; 
            string strRND11 = password;
            byte[] arr11 = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND11, "doing it well")); 
            response =  centralCalls.get_authenticate_Admin(" where replace(password, '@','#')  = '" + Encoding.ASCII.GetString(arr11).Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = npassword;
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        result =  centralCalls.update_authenticate_Admin(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oLab: response[0].Lab.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString( response[0].Password2),Lab: response[0].Lab.ToString() ) ; 
                } 
            } 
            Session["response"]  = result; 
            return Content((string)result);
        }   


        [AllowAnonymous]
        public ActionResult new_Roles()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'new Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 = centralCalls.get_right_Admin("   order by rightname");
            ViewBag.Data1 =  centralCalls.get_Lab("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Roles(string Rolename,string Lab, string selectedRights )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'new Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Roles(Rolename: Rolename,Lab: Lab,token: Session["token"].ToString() ,role: Session["role"].ToString()  , selectedRights: selectedRights); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Roles", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Roles(string Rolename,string Lab,string token, string role, string selectedRights)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'new Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_role_Admin(Rolename: Rolename,Lab: Lab, selectedRights: selectedRights);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Roles()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'view Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Lo_role_Admin> response = null; 
           ActionResult d =  view_it_Roles( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Lo_role_Admin_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Roles(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'view Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Lo_role_Admin>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_role_Admin>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_role_Admin(" where a.lab = " + Session["Lab"]);
            return Content(JsonConvert.SerializeObject( ((List<Lo_role_Admin_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Roles(string id  ,string Rolename,string Lab  )
        {  
                Audit.protocol();
            ViewBag.Data0 = centralCalls.get_right_Admin("   order by rightname");
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data1 =  centralCalls.get_Lab("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Rolename = Rolename;
 ViewBag.Lab = Lab;

            List<Lo_role_right_Admin_data> roleRightAdminList = centralCalls.get_role_right_Admin(" where role = " + id);
            string rightSet = "";
            foreach(Lo_role_right_Admin_data roleRightAdmin in roleRightAdminList)
            {
                rightSet += "sphinxcol" + roleRightAdmin.Right + "sphinxcol";
            }
            ViewBag.rightSet = rightSet;
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Roles(string id,string oRolename,string oLab,string Rolename,string Lab , string selectedRights )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Roles(id:id,oRolename:  oRolename,oLab:  oLab,Rolename: Rolename,Lab: Lab, token: Session["token"].ToString() ,role: Session["role"].ToString()  , selectedRights: selectedRights ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Roles", "Admin");
                }
                else{
                    return RedirectToAction("view_Roles", "Admin");
                    ViewBag.id=id;
                     ViewBag.Rolename = Rolename;
 ViewBag.Lab = Lab;

                    List<Lo_role_right_Admin_data> roleRightAdminList = centralCalls.get_role_right_Admin(" where role = " + id);
                    string rightSet = "";
                    foreach(Lo_role_right_Admin_data roleRightAdmin in roleRightAdminList)
                    {
                        rightSet += "sphinxcol" + roleRightAdmin.Right + "sphinxcol";
                    }
                    ViewBag.rightSet = rightSet;
                     return View();
                }
                return RedirectToAction("new_Roles", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Roles(string id, string oRolename,string oLab,string Rolename,string Lab,string token, string role, string selectedRights)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_role_Admin(id:id,oRolename:  oRolename,oLab:  oLab,Rolename: Rolename,Lab: Lab,andPassword: false, selectedRights: selectedRights);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Staff()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'new Staff' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_role_Staff("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Staff(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'new Staff' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
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
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Staff(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Lab: Lab,token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Staff", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Staff(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'new Staff' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_authenticate_Staff(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Lab: Lab);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Staff()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'view Staff' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Lo_authenticate_Staff> response = null; 
           ActionResult d =  view_it_Staff(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Lo_authenticate_Staff_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Staff(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'view Staff' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Lo_authenticate_Staff>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_authenticate_Staff>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_authenticate_Staff(" where a.lab = " + Session["Lab"]);
            return Content(JsonConvert.SerializeObject( ((List<Lo_authenticate_Staff_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Staff(string id,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'edit Staff' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_role_Staff("");
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
        public ActionResult edit_Staff(string id,string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oLab,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'edit Staff' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Staff(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,oLab:  oLab,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Lab: Lab, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Staff", "Admin");
                }
                else{
                    return RedirectToAction("view_Staff", "Admin");
                     return View();
                }
                return RedirectToAction("new_Staff", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Staff(string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oLab,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Admin where  replace(rightName,'_',' ') = 'edit Staff' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_authenticate_Staff(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,oLab:  oLab,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,Lab: Lab,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }


   
    }
} 
