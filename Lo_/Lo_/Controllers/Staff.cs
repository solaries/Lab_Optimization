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
    public class STAFFController : Controller
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
            string token = doAuthenticate(userName: Email, password: Password, clientID: "Staff");
            bool result = validateAccessToken(token);
            List<Lo_authenticate_Staff_data> response =null;
            if (String.IsNullOrEmpty(forgot))
            {
                ActionResult xx = authenticate( password:  Password, email: Email ); 
                response = ( List<Lo_authenticate_Staff_data>)Session["response"];
                if(response !=null ){
                    if(response.Count > 0){
                         if (Encoding.ASCII.GetString(response[0].Password) == Encoding.ASCII.GetString(response[0].Password2))
                         { 
                            Session["userID"] = response[0].Id ;
                            Session["userType"] = "Staff" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Lab"] = response[0].Lab;
                            Session["Lab_data"] = response[0].Lab_data;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            List<Lo_right_Staff> rightList = centralCalls.get_right_Staff(" where id in (select `right` from Lo_role_right_Staff where role =" + response[0].Role.ToString() + " )   order by rightname");
                            string strRightList = "";
                            foreach (Lo_right_Staff right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList; 
                            Session["role"] = response[0].Role;
                            Session["status"] = "Please change your password";
                            return RedirectToAction("Change_Password", "Staff");
                        }
                        else  
                        {  
                            Session["userID"] = response[0].Id ;
                            Session["status"] = "";
                            Session["userType"] = "Staff" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Lab"] = response[0].Lab;
                            Session["Lab_data"] = response[0].Lab_data;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["role"] = response[0].Role;
                            List<Lo_right_Staff> rightList = centralCalls.get_right_Staff(" where id in (select `right` from Lo_role_right_Staff where role =" + response[0].Role.ToString() + " )   order by rightname");
                            string strRightList = "";
                            foreach (Lo_right_Staff right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList;
                            return RedirectToAction("view_Patient_Tests", "Staff"); 
                        }
                    }
                    else
                    {
                        Session["status"] = "Authentication Not successful";
                        return RedirectToAction("Login", "Staff");
                    } 
                }
                else
                {
                    Session["status"] = "Authentication Not successful";
                    return RedirectToAction("Login", "Staff");
                } 
            }
            else { 
                ActionResult xx = forgotauthenticate_Staff(   Email: Email ); 
                response = ( List<Lo_authenticate_Staff_data>)Session["response"]; 
                return RedirectToAction("Login", "Staff");
            }
        } 

        [AllowAnonymous]
        public ActionResult authenticate(string email, string password )
        { 
            Audit.protocol(); 
            List<Lo_authenticate_Staff_data> response = null;  
            password =  Audit.GetEncodedHash(password, "doing it well") ;
            response =  centralCalls.get_authenticate_Staff(" where replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            Session["response"]  = response;
            return Content(JsonConvert.SerializeObject((List<Lo_authenticate_Staff_data>)response));
        }

        [AllowAnonymous]
        public ActionResult forgotauthenticate_Staff(string Email )
        {   
            Audit.protocol();
            List<Lo_authenticate_Staff_data> response = null; 
            response =  centralCalls.get_authenticate_Staff(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = Audit.GenerateRandom();
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        centralCalls.update_authenticate_Staff(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oLab: response[0].Lab.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString(arr),Lab: response[0].Lab.ToString() ) ; 
                        string mailSubject = "Profile password reset on (code joh) Lab Optimum";
                        Session["status"] = "Password reset successful, please check your email for your new password.";
                        string mailBody = "Hi <br><br>Your password has been successfully reset on the (code joh) Lab Optimum platform. Please log in with following credentials: <br><br> Email:" + response[0].Email + "<br><br>password :" + strRND + "<br><br><br>Regards<br><br>";
                        Audit.SendMail(Email, mailSubject, mailBody, " "); 
                } 
            } 
            Session["response"]  = response; 
             return Content(JsonConvert.SerializeObject((List<Lo_authenticate_Staff_data>)response)); ;
        }   


        [AllowAnonymous]
        public ActionResult Change_Password()
        {
                Audit.protocol();
            getStatus(false);
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
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
                    return RedirectToAction("Login", "Staff") ;
                }
                else
                {
                    return RedirectToAction("Change_Password", "Staff") ;
                }
        } 


        [AllowAnonymous]
        public ActionResult updatePassword(string email,string password,string npassword )
        {   
            Audit.protocol();
            List<Lo_authenticate_Staff_data> response = null; 
            string result = "Authentication failed"; 
            string strRND11 = password;
            byte[] arr11 = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND11, "doing it well")); 
            response =  centralCalls.get_authenticate_Staff(" where replace(password, '@','#')  = '" + Encoding.ASCII.GetString(arr11).Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = npassword;
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        result =  centralCalls.update_authenticate_Staff(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oLab: response[0].Lab.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString( response[0].Password2),Lab: response[0].Lab.ToString() ) ; 
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
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            ViewBag.Data0 = centralCalls.get_right_Staff("   order by rightname");
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
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
                ActionResult xx =  add_Roles(Rolename: Rolename,Lab: Lab,token: Session["token"].ToString() ,role: Session["role"].ToString()  , selectedRights: selectedRights); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Roles", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult add_Roles(string Rolename,string Lab,string token, string role, string selectedRights)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Roles' )   ").Count ==0){ 
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
            response =  centralCalls.add_new_role_Staff(Rolename: Rolename,Lab: Lab, selectedRights: selectedRights);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Roles()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            List<Lo_role_Staff> response = null; 
           ActionResult d =  view_it_Roles( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
            return View((List<Lo_role_Staff_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Roles(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Lo_role_Staff>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_role_Staff>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_role_Staff(" where a.lab = " + Session["Lab"]);
            return Content(JsonConvert.SerializeObject( ((List<Lo_role_Staff_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Roles(string id  ,string Rolename,string Lab  )
        {  
                Audit.protocol();
            ViewBag.Data0 = centralCalls.get_right_Staff("   order by rightname");
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            ViewBag.Data1 =  centralCalls.get_Lab("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Rolename = Rolename;
 ViewBag.Lab = Lab;

            List<Lo_role_right_Staff_data> roleRightStaffList = centralCalls.get_role_right_Staff(" where role = " + id);
            string rightSet = "";
            foreach(Lo_role_right_Staff_data roleRightStaff in roleRightStaffList)
            {
                rightSet += "sphinxcol" + roleRightStaff.Right + "sphinxcol";
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
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            string response =null;
                ActionResult xx =  update_Roles(id:id,oRolename:  oRolename,oLab:  oLab,Rolename: Rolename,Lab: Lab, token: Session["token"].ToString() ,role: Session["role"].ToString()  , selectedRights: selectedRights ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Roles", "Staff");
                }
                else{
                    return RedirectToAction("view_Roles", "Staff");
                    ViewBag.id=id;
                     ViewBag.Rolename = Rolename;
 ViewBag.Lab = Lab;

                    List<Lo_role_right_Staff_data> roleRightStaffList = centralCalls.get_role_right_Staff(" where role = " + id);
                    string rightSet = "";
                    foreach(Lo_role_right_Staff_data roleRightStaff in roleRightStaffList)
                    {
                        rightSet += "sphinxcol" + roleRightStaff.Right + "sphinxcol";
                    }
                    ViewBag.rightSet = rightSet;
                     return View();
                }
                return RedirectToAction("new_Roles", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult update_Roles(string id, string oRolename,string oLab,string Rolename,string Lab,string token, string role, string selectedRights)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count ==0){ 
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
            response =  centralCalls.update_role_Staff(id:id,oRolename:  oRolename,oLab:  oLab,Rolename: Rolename,Lab: Lab,andPassword: false, selectedRights: selectedRights);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Test_Type()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Test Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Test_Type(string Test_name,string Price,string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Test Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
                ActionResult xx =  add_Test_Type(Test_name: Test_name,Price: Price,Lab: Lab,token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Test_Type", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult add_Test_Type(string Test_name,string Price,string Lab,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Test Type' )   ").Count ==0){ 
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
            response =  centralCalls.add_new_Test_Type(Test_name: Test_name,Price: Price,Lab: Lab);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Test_Type()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Test Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            List<Lo_Test_Type> response = null; 
           ActionResult d =  view_it_Test_Type(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
            return View((List<Lo_Test_Type_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Test_Type(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Test Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Lo_Test_Type>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_Test_Type>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Test_Type(" where a.lab = " + Session["Lab"]);
            return Content(JsonConvert.SerializeObject( ((List<Lo_Test_Type_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Test_Type(string id,string Test_name,string Price,string Lab  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Test Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Test_name = Test_name;
 ViewBag.Price = Price;
 ViewBag.Lab = Lab;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Test_Type(string id,string oTest_name,string oPrice,string oLab,string Test_name,string Price,string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Test Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            string response =null;
                ActionResult xx =  update_Test_Type(id:id,oTest_name:  oTest_name,oPrice:  oPrice,oLab:  oLab,Test_name: Test_name,Price: Price,Lab: Lab, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Test_Type", "Staff");
                }
                else{
                    return RedirectToAction("view_Test_Type", "Staff");
                     return View();
                }
                return RedirectToAction("new_Test_Type", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult update_Test_Type(string id, string oTest_name,string oPrice,string oLab,string Test_name,string Price,string Lab,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Test Type' )   ").Count ==0){ 
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
            response =  centralCalls.update_Test_Type(id:id,oTest_name:  oTest_name,oPrice:  oPrice,oLab:  oLab,Test_name: Test_name,Price: Price,Lab: Lab,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Patient()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Patient' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Patient(string First_name,string Surname,string Phone_number,string Email,string Date_of_birth,string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Patient' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
                ActionResult xx =  add_Patient(First_name: First_name,Surname: Surname,Phone_number: Phone_number,Email: Email,Date_of_birth: Date_of_birth,Lab: Lab,token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Patient", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult add_Patient(string First_name,string Surname,string Phone_number,string Email,string Date_of_birth,string Lab,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Patient' )   ").Count ==0){ 
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
            response =  centralCalls.add_new_Patient(First_name: First_name,Surname: Surname,Phone_number: Phone_number,Email: Email,Date_of_birth: Date_of_birth,Lab: Lab);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Patient()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Patient' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            List<Lo_Patient> response = null; 
           ActionResult d =  view_it_Patient(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
            return View((List<Lo_Patient_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Patient(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Patient' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Lo_Patient>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_Patient>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Patient(" where a.lab = " + Session["Lab"]);
            return Content(JsonConvert.SerializeObject( ((List<Lo_Patient_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Patient(string id,string First_name,string Surname,string Phone_number,string Email,string Date_of_birth,string Lab  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Patient' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.First_name = First_name;
 ViewBag.Surname = Surname;
 ViewBag.Phone_number = Phone_number;
 ViewBag.Email = Email;
 ViewBag.Date_of_birth = Date_of_birth;
 ViewBag.Lab = Lab;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Patient(string id,string oFirst_name,string oSurname,string oPhone_number,string oEmail,string oDate_of_birth,string oLab,string First_name,string Surname,string Phone_number,string Email,string Date_of_birth,string Lab )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Patient' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            string response =null;
                ActionResult xx =  update_Patient(id:id,oFirst_name:  oFirst_name,oSurname:  oSurname,oPhone_number:  oPhone_number,oEmail:  oEmail,oDate_of_birth:  oDate_of_birth,oLab:  oLab,First_name: First_name,Surname: Surname,Phone_number: Phone_number,Email: Email,Date_of_birth: Date_of_birth,Lab: Lab, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Patient", "Staff");
                }
                else{
                    return RedirectToAction("view_Patient", "Staff");
                     return View();
                }
                return RedirectToAction("new_Patient", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult update_Patient(string id, string oFirst_name,string oSurname,string oPhone_number,string oEmail,string oDate_of_birth,string oLab,string First_name,string Surname,string Phone_number,string Email,string Date_of_birth,string Lab,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Patient' )   ").Count ==0){ 
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
            response =  centralCalls.update_Patient(id:id,oFirst_name:  oFirst_name,oSurname:  oSurname,oPhone_number:  oPhone_number,oEmail:  oEmail,oDate_of_birth:  oDate_of_birth,oLab:  oLab,First_name: First_name,Surname: Surname,Phone_number: Phone_number,Email: Email,Date_of_birth: Date_of_birth,Lab: Lab,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Patient_Tests()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Patient Tests' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            ViewBag.Data0 = centralCalls.get_Patient(" where a.lab = " + Session["Lab"]); 
            ViewBag.Data1 = centralCalls.get_Test_Type(" where a.lab = " + Session["Lab"]);
            //ViewBag.Data1 = centralCalls.get_Test_Type("");

            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Patient_Tests(string Patient, string Test_date, string selectedTests)
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Patient Tests' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
                ActionResult xx = add_Patient_Tests(Patient: Patient, Test_date: Test_date, token: Session["token"].ToString(), role: Session["role"].ToString(), selectedTests: selectedTests); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Patient_Tests", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult add_Patient_Tests(string Patient, string Test_date, string token, string role, string selectedTests)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Patient Tests' )   ").Count ==0){ 
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
            response = centralCalls.add_new_Tests(Patient: Patient, Test_date: Test_date, selectedTests: selectedTests);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Patient_Tests()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Patient Tests' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            List<Lo_Tests> response = null; 
           ActionResult d =  view_it_Patient_Tests(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
            return View((List<Lo_Tests_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Patient_Tests(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Patient Tests' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Lo_Tests>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_Tests>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Tests(" where a1.lab = " + Session["Lab"]);
            return Content(JsonConvert.SerializeObject( ((List<Lo_Tests_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Patient_Tests(string id, string Patient, string Test_date, string price)
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Patient Tests' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            ViewBag.Data0 =  centralCalls.get_Patient("");
            getStatus();
            ViewBag.id=id;
            ViewBag.Patient = Patient;
            ViewBag.Test_date = Test_date;
            ViewBag.price = price;
            ViewBag.Data1 = centralCalls.get_Test_Type2(" where a.lab = " + Session["Lab"], id);

            List<Lo_Test_List_data> testList = centralCalls.get_Test_List(" where a.Test = " + id );
            string testSet = "";
            foreach (Lo_Test_List_data test in testList)
            {
                testSet += "sphinxcol" + test.Test_type + "sphinxcol";
               // testsWithPrice


            }
            ViewBag.testSet = testSet;


            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Patient_Tests(string id,string oPatient,string oTest_date,string Patient,string Test_date, string selectedTests )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Patient Tests' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            string response =null;
            ActionResult xx = update_Patient_Tests(id: id, oPatient: oPatient, oTest_date: oTest_date, Patient: Patient, Test_date: Test_date, token: Session["token"].ToString(), role: Session["role"].ToString(), selectedTests: selectedTests); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Patient_Tests", "Staff");
                }
                else{
                    return RedirectToAction("view_Patient_Tests", "Staff");
                     return View();
                }
                return RedirectToAction("new_Patient_Tests", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult update_Patient_Tests(string id, string oPatient, string oTest_date, string Patient, string Test_date, string token, string role, string selectedTests)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Patient Tests' )   ").Count ==0){ 
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
            response = centralCalls.update_Tests(id: id, oPatient: oPatient, oTest_date: oTest_date, Patient: Patient, Test_date: Test_date, andPassword: false, selectedTests: selectedTests);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Test_List()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Test List' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            ViewBag.Data0 =  centralCalls.get_Tests("");
            ViewBag.Data1 = centralCalls.get_Test_Type(" where a.lab = " + Session["Lab"]);
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Test_List(string Test,string Test_type )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Test List' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
                ActionResult xx =  add_Test_List(Test: Test,Test_type: Test_type,token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Test_List", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult add_Test_List(string Test,string Test_type,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Test List' )   ").Count ==0){ 
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
            response =  centralCalls.add_new_Test_List(Test: Test,Test_type: Test_type);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Test_List()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Test List' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            List<Lo_Test_List> response = null; 
           ActionResult d =  view_it_Test_List(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
            return View((List<Lo_Test_List_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Test_List(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Test List' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Lo_Test_List>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_Test_List>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Test_List("");
            return Content(JsonConvert.SerializeObject( ((List<Lo_Test_List_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Test_List(string id,string Test,string Test_type  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Test List' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            ViewBag.Data0 =  centralCalls.get_Tests("");
            ViewBag.Data1 =  centralCalls.get_Test_Type("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Test = Test;
 ViewBag.Test_type = Test_type;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Test_List(string id,string oTest,string oTest_type,string Test,string Test_type )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Test List' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            string response =null;
                ActionResult xx =  update_Test_List(id:id,oTest:  oTest,oTest_type:  oTest_type,Test: Test,Test_type: Test_type, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Test_List", "Staff");
                }
                else{
                    return RedirectToAction("view_Test_List", "Staff");
                     return View();
                }
                return RedirectToAction("new_Test_List", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult update_Test_List(string id, string oTest,string oTest_type,string Test,string Test_type,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Test List' )   ").Count ==0){ 
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
            response =  centralCalls.update_Test_List(id:id,oTest:  oTest,oTest_type:  oTest_type,Test: Test,Test_type: Test_type,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Inventory()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Inventory' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Inventory(string Item_name )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Inventory' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
                ActionResult xx =  add_Inventory(Item_name: Item_name,token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Inventory", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult add_Inventory(string Item_name,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Inventory' )   ").Count ==0){ 
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
            response = centralCalls.add_new_Inventory(Item_name: Item_name, lab: Session["Lab"].ToString());
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Inventory()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Inventory' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            List<Lo_Inventory> response = null; 
           ActionResult d =  view_it_Inventory(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
            return View((List<Lo_Inventory>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Inventory(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Inventory' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Lo_Inventory>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_Inventory>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Inventory(" where lab = " + Session["Lab"]);
            return Content(JsonConvert.SerializeObject( ((List<Lo_Inventory>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Inventory(string id,string Item_name  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Inventory' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Item_name = Item_name;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Inventory(string id,string oItem_name,string Item_name )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Inventory' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            string response =null;
                ActionResult xx =  update_Inventory(id:id,oItem_name:  oItem_name,Item_name: Item_name, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Inventory", "Staff");
                }
                else{
                    return RedirectToAction("view_Inventory", "Staff");
                     return View();
                }
                return RedirectToAction("new_Inventory", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult update_Inventory(string id, string oItem_name,string Item_name,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Inventory' )   ").Count ==0){ 
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
            response =  centralCalls.update_Inventory(id:id,oItem_name:  oItem_name,Item_name: Item_name,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult new_Inventory_Movement_allocate()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Inventory Movement allocate' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            ViewBag.Data0 = centralCalls.get_Inventory("SELECT id,CONCAT( item_name ,' (', IFNULL(q,0), ') ') item_name  , lab from lo_inventory a left JOIN (SELECT SUM(quantity * direction) q, inventory FROM lo_inventory_movement GROUP BY inventory  ) a1 ON a.id = a1.inventory where lab = " + Session["Lab"]);
            ViewBag.Data1 = centralCalls.get_authenticate_Staff(" where a.lab = " + Session["Lab"]);
            ViewBag.Data2 = centralCalls.get_authenticate_Staff(" where a.lab = " + Session["Lab"]);
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Inventory_Movement_allocate(string Inventory, string Quantity, string Direction, string By_satff, string To_staff, string Move_date)
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if (centralCalls.get_role_right_Staff("  where role =  " + Session["role"] + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Inventory Movement allocate' )   ").Count == 0)
            { 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
                ActionResult xx = add_Inventory_Movement(Inventory: Inventory, Quantity: Quantity, Direction: "-1", By_satff: Session["userID"].ToString(), To_staff: To_staff, Move_date: Move_date, token: Session["token"].ToString(), role: Session["role"].ToString()); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Inventory_Movement_allocate", "Staff");
        } 




        [AllowAnonymous]
        public ActionResult new_Inventory_Movement_add()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Inventory Movement add' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            ViewBag.Data0 = centralCalls.get_Inventory(" where lab = " + Session["Lab"]);
            ViewBag.Data1 = centralCalls.get_authenticate_Staff(" where a.lab = " + Session["Lab"]);
            ViewBag.Data2 = centralCalls.get_authenticate_Staff(" where a.lab = " + Session["Lab"]);
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Inventory_Movement_add(string Inventory, string Quantity, string Direction, string By_satff, string To_staff, string Move_date)
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Inventory Movement add' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
                ActionResult xx = add_Inventory_Movement(Inventory: Inventory, Quantity: Quantity, Direction: "1", By_satff: Session["userID"].ToString(), To_staff: To_staff, Move_date: Move_date, token: Session["token"].ToString(), role: Session["role"].ToString()); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Inventory_Movement_add", "Staff");
        } 


        [AllowAnonymous]
        public ActionResult add_Inventory_Movement(string Inventory,string Quantity,string Direction,string By_satff,string To_staff,string Move_date,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Inventory Movement add' )   ").Count ==0){
                if (centralCalls.get_role_right_Staff("  where role =  " + Session["role"] + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'new Inventory Movement allocate' )   ").Count == 0)
                { 
                   Session["status"] = "You do not have access to this functionality";
                   Session["response"]  = "You do not have access to this functionality";
                   return Content(Session["status"].ToString() );
                }
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_Inventory_Movement(Inventory: Inventory,Quantity: Quantity,Direction: Direction,By_satff: By_satff,To_staff: To_staff,Move_date: Move_date);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Inventory_Movement()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Inventory Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            List<Lo_Inventory_Movement> response = null; 
           ActionResult d =  view_it_Inventory_Movement(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
            return View((List<Lo_Inventory_Movement_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Inventory_Movement(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'view Inventory Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Lo_Inventory_Movement>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Lo_Inventory_Movement>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Inventory_Movement(" where a1.lab = " + Session["Lab"]);
            return Content(JsonConvert.SerializeObject( ((List<Lo_Inventory_Movement_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Inventory_Movement(string id,string Inventory,string Quantity,string Direction,string By_satff,string To_staff,string Move_date  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Inventory Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
            ViewBag.Data0 =  centralCalls.get_Inventory("");
            ViewBag.Data1 =  centralCalls.get_authenticate_Staff("");
            ViewBag.Data2 =  centralCalls.get_authenticate_Staff("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Inventory = Inventory;
 ViewBag.Quantity = Quantity;
 ViewBag.Direction = Direction;
 ViewBag.By_satff = By_satff;
 ViewBag.To_staff = To_staff;
 ViewBag.Move_date = Move_date;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Inventory_Movement(string id,string oInventory,string oQuantity,string oDirection,string oBy_satff,string oTo_staff,string oMove_date,string Inventory,string Quantity,string Direction,string By_satff,string To_staff,string Move_date )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Staff");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Staff");
            }
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Inventory Movement' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Staff");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Staff");
                }
            string response =null;
                ActionResult xx =  update_Inventory_Movement(id:id,oInventory:  oInventory,oQuantity:  oQuantity,oDirection:  oDirection,oBy_satff:  oBy_satff,oTo_staff:  oTo_staff,oMove_date:  oMove_date,Inventory: Inventory,Quantity: Quantity,Direction: Direction,By_satff: By_satff,To_staff: To_staff,Move_date: Move_date, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Staff");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Inventory_Movement", "Staff");
                }
                else{
                    return RedirectToAction("view_Inventory_Movement", "Staff");
                     return View();
                }
                return RedirectToAction("view_Inventory_Movement", "Staff");
        } 

        [AllowAnonymous]
        public ActionResult update_Inventory_Movement(string id, string oInventory,string oQuantity,string oDirection,string oBy_satff,string oTo_staff,string oMove_date,string Inventory,string Quantity,string Direction,string By_satff,string To_staff,string Move_date,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Staff("  where role =  "  + Session["role"]   + " and  `right` = (select id from Lo_right_Staff where  replace(rightName,'_',' ') = 'edit Inventory Movement' )   ").Count ==0){ 
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
            response =  centralCalls.update_Inventory_Movement(id:id,oInventory:  oInventory,oQuantity:  oQuantity,oDirection:  oDirection,oBy_satff:  oBy_satff,oTo_staff:  oTo_staff,oMove_date:  oMove_date,Inventory: Inventory,Quantity: Quantity,Direction: Direction,By_satff: By_satff,To_staff: To_staff,Move_date: Move_date,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult receipt(string  id  , 
            string Test_Date  , 
            string  patient  , 
            string price  )
        {

            Audit.protocol();
            List<Lo_Tests_data> d =  centralCalls.get_Tests(" where a1.lab = " + Session["Lab"] + " and a.id = " + id);
             List<Lo_Patient_data> p = centralCalls.get_Patient(" where a.id = " + d[0].Patient );

             ViewBag.Data1 = centralCalls.get_Test_Type2(" where a.lab = " + Session["Lab"], id);
             ViewBag.firstname = p[0].First_name;
             ViewBag.email = p[0].Email;
             ViewBag.dateValue = Test_Date;
            ViewBag.totalBill = price;


            ViewBag.id=id;
            ViewBag.Test_Date=Test_Date;
            ViewBag.patient=patient;
            ViewBag.price=price;

            return View();
        }




        [AllowAnonymous]

        public ActionResult receipt1(string id,
            string Test_Date,
            string patient,
            string price)
        {//ExportInvoiceX_ receipt
            Audit.protocol();

            List<Lo_Tests_data> d = centralCalls.get_Tests(" where a1.lab = " + Session["Lab"] + " and a.id = " + id);
            List<Lo_Patient_data> p = centralCalls.get_Patient(" where a.id = " + d[0].Patient);

            ViewBag.Data1 = centralCalls.get_Test_Type2(" where a.lab = " + Session["Lab"], id);
            ViewBag.firstname = p[0].First_name;
            ViewBag.email = p[0].Email;
            ViewBag.dateValue = Test_Date;
            ViewBag.totalBill = price;

            Session["ViewBag_Data1"] = ViewBag.Data1 ;
            Session["ViewBag_firstname"] = ViewBag.firstname;
            Session["ViewBag_email"] = ViewBag.email;
            Session["ViewBag_dateValue"] = ViewBag.dateValue;
            Session["ViewBag_totalBill"] = ViewBag.totalBill;
            ViewBag.Lab_data = Session["Lab_data"];


            Session["ViewBag.id"] = id;
            Session["ViewBag.Test_Date"] = Test_Date;
            Session["ViewBag.patient"] = patient;
            Session["ViewBag.price"] = price;

            //Session["email"] = email;
            //Session["companyV"] = company;
            //Session["firstname"] = firstname;
            //Session["lastname"] = lastname;
            //Session["noOfToken"] = noOfToken;
            //Session["totalBill"] = totalBill;
            //Session["modeOfPay"] = modeOfPay;
            //Session["amount"] = amount;
            //Session["testType"] = testType; 
            //ViewBag.dateValue = dateValue;
            //ViewBag.dateCOde = dateCOde; 
            //ViewBag.email = email;
            //ViewBag.company = company;
            //ViewBag.firstname = firstname;
            //ViewBag.lastname = lastname;
            //ViewBag.noOfToken = noOfToken;
            //ViewBag.totalBill = totalBill;
            //ViewBag.modeOfPay = modeOfPay;
            //ViewBag.amount = amount;
            //ViewBag.testType = testType;



            string strFileName = "Receipt_" + id + ".pdf";
            //Session["dateCOde"] = dateCOde.Replace("_", "/");
            //Session["dateValue"] = dateValue;
            //ViewBag.dateCOde = Session["dateCOde"];
            //ViewBag.dateValue = Session["dateValue"];
            Session["strFileName"] = strFileName;
            ViewBag.strFileName = Session["strFileName"];

            return new Rotativa.ViewAsPdf("receipt0", "") { FileName = strFileName };

        }
        public ActionResult receipt0()
        {//ExportInvoiceX receipt0
           

            Audit.protocol();
            return View();
        }



        public ActionResult receipt2(string id,
            string Test_Date,
            string patient,
            string price)
        {//ExportInvoiceXX_ receipt0
            Audit.protocol();



            List<Lo_Tests_data> d = centralCalls.get_Tests(" where a1.lab = " + Session["Lab"] + " and a.id = " + id);
            List<Lo_Patient_data> p = centralCalls.get_Patient(" where a.id = " + d[0].Patient);

            ViewBag.Data1 = centralCalls.get_Test_Type2(" where a.lab = " + Session["Lab"], id);
            ViewBag.firstname = p[0].First_name;
            ViewBag.email = p[0].Email;
            ViewBag.dateValue = Test_Date;
            ViewBag.totalBill = price;
            ViewBag.Lab_data =   Session["Lab_data"];

            Session["ViewBag_Data1"] = ViewBag.Data1;
            Session["ViewBag_firstname"] = ViewBag.firstname;
            Session["ViewBag_email"] = ViewBag.email;
            Session["ViewBag_dateValue"] = ViewBag.dateValue;
            Session["ViewBag_totalBill"] = ViewBag.totalBill;


            //Session["email"] = email;
            //Session["companyV"] = company;
            //Session["firstname"] = firstname;
            //Session["lastname"] = lastname;
            //Session["noOfToken"] = noOfToken;
            //Session["totalBill"] = totalBill;
            //Session["modeOfPay"] = modeOfPay;
            //Session["amount"] = amount;
            //Session["testType"] = testType;



            //ViewBag.dateValue = dateValue;
            //ViewBag.dateCOde = dateCOde;


            //ViewBag.email = email;
            //ViewBag.company = company;
            //ViewBag.firstname = firstname;
            //ViewBag.lastname = lastname;
            //ViewBag.noOfToken = noOfToken;
            //ViewBag.totalBill = totalBill;
            //ViewBag.modeOfPay = modeOfPay;
            //ViewBag.amount = amount;
            //ViewBag.testType = testType;



            // string dateCOde = System.DateTime.Now.ToString("MMddyyyy_hhmmss_fff");
            // string dateValue = System.DateTime.Now.ToString("dd/MMM/yyyy");
            string strFileName = Server.MapPath("~/Receipt_" + id + ".pdf");
            Session["strFileName"] = strFileName;
            ViewBag.strFileName = Session["strFileName"];
            var x = new Rotativa.ViewAsPdf("receipt0", "") { FileName = strFileName };
            var byteArray = x.BuildPdf(ControllerContext);
            System.IO.File.WriteAllBytes(strFileName, byteArray);



            string mailSubject = "Receipt " + Session["Lab_data"];
            string mailBody = "Hi " + p[0].First_name + "<br><br>Please find attached your receipt<br><br><br>Regards<br><br>";
            Audit.SendMail(p[0].Email, mailSubject, mailBody, "add profile", strFileName);

            return RedirectToAction("view_Patient_Tests", "staff");

        }
   
    }
} 
