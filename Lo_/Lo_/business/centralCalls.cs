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
using System.Net;

namespace Lo.BusinessLogic
{ 
    public class centralCalls 
    {  
        private static long getVal( ) {
            if (HttpContext.Current.Session["userID"] == null)
            {
                return 0;
            }
            else {
                return long.Parse(HttpContext.Current.Session["userID"].ToString());
            }
        }
        public static string add_new_authenticate_Admin(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab, bool returnID = false )
        { 
            string response = ""; 
            authenticate_Admin c = new authenticate_Admin();  
            string data = "";
            try
            { 

                Lo_authenticate_Admin cust = new Lo_authenticate_Admin(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Last_name =  Last_name;
                 data += ",Last_name : " + Last_name;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                 
                List<Lo_role_Admin_data> ra =  get_role_Admin(" where a.lab = " + Lab + " order by id asc");
                Role = ra[0].Id.ToString();
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + Role;

                cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + Lab;
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                response = c.add_authenticate_Admin(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_ADMIN_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding authenticate_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_authenticate_Admin_data> get_authenticate_Admin(string sql)
        { 
            List<Lo_authenticate_Admin_data> response = null;
            try
            { 
                authenticate_Admin c = new authenticate_Admin(); 
                response = c.get_authenticate_Admin_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_authenticate_Admin( string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oLab,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab, bool andPassword = true) 
        { 
            string response = ""; 
            authenticate_Admin c = new authenticate_Admin();   
            string data = "";
            try
            { 
                Lo_authenticate_Admin cust = c.get_authenticate_Admin(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Last_name =  Last_name;
                 data += ",Last_name : " + oLast_name + " -> " + Last_name;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;

                 List<Lo_role_Admin_data> ra = get_role_Admin(" where a.lab = " + Lab + " order by id asc");
                 Role = ra[0].Id.ToString();

                 cust.Role = long.Parse(Role == null ? "1" : Role);
                 data += ",Role : " + oRole + " -> " + Role;
                 cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + oLab + " -> " + Lab;
                 if(andPassword)
                 {
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                 }
                response = c.update_authenticate_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_authenticate_Staff(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab, bool returnID = false )
        { 
            string response = ""; 
            authenticate_Staff c = new authenticate_Staff();  
            string data = "";
            try
            { 

                Lo_authenticate_Staff cust = new Lo_authenticate_Staff(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Last_name =  Last_name;
                 data += ",Last_name : " + Last_name;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + Role;
                cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + Lab;
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                response = c.add_authenticate_Staff(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_STAFF_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_STAFF_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_STAFF_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding authenticate_Staff";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_STAFF_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_authenticate_Staff_data> get_authenticate_Staff(string sql)
        { 
            List<Lo_authenticate_Staff_data> response = null;
            try
            { 
                authenticate_Staff c = new authenticate_Staff(); 
                response = c.get_authenticate_Staff_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_STAFF_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_authenticate_Staff( string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oLab,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Lab, bool andPassword = true) 
        { 
            string response = ""; 
            authenticate_Staff c = new authenticate_Staff();   
            string data = "";
            try
            { 
                Lo_authenticate_Staff cust = c.get_authenticate_Staff(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Last_name =  Last_name;
                 data += ",Last_name : " + oLast_name + " -> " + Last_name;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;
                 cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + oRole + " -> " + Role;
                 cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + oLab + " -> " + Lab;
                 if(andPassword)
                 {
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                 }
                response = c.update_authenticate_Staff(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_STAFF_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_STAFF_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_STAFF_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_authenticate_SuperAdmin(string First_name,string Last_name,string Email,string Password,string Password2, bool returnID = false )
        { 
            string response = ""; 
            authenticate_SuperAdmin c = new authenticate_SuperAdmin();  
            string data = "";
            try
            { 

                Lo_authenticate_SuperAdmin cust = new Lo_authenticate_SuperAdmin(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Last_name =  Last_name;
                 data += ",Last_name : " + Last_name;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                response = c.add_authenticate_SuperAdmin(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_SUPERADMIN_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_SUPERADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_SUPERADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding authenticate_SuperAdmin";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_SUPERADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_authenticate_SuperAdmin> get_authenticate_SuperAdmin(string sql, out string msg )
        {
            msg = "";
            List<Lo_authenticate_SuperAdmin> response = null;
            try
            { 
                authenticate_SuperAdmin c = new authenticate_SuperAdmin(); 
                response = c.get_authenticate_SuperAdmin(sql); 
            }
            catch (Exception d)
            {
                msg = d.Message;
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_SUPERADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_authenticate_SuperAdmin( string id, string oFirst_name,string oLast_name,string oEmail,string oPassword,string oPassword2,string First_name,string Last_name,string Email,string Password,string Password2, bool andPassword = true) 
        { 
            string response = ""; 
            authenticate_SuperAdmin c = new authenticate_SuperAdmin();   
            string data = "";
            try
            { 
                Lo_authenticate_SuperAdmin cust = c.get_authenticate_SuperAdmin(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Last_name =  Last_name;
                 data += ",Last_name : " + oLast_name + " -> " + Last_name;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;
                 if(andPassword)
                 {
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                 }
                response = c.update_authenticate_SuperAdmin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_SUPERADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_SUPERADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_SUPERADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Inventory(string Item_name,string lab, bool returnID = false )
        { 
            string response = ""; 
            Inventory c = new Inventory();  
            string data = "";
            try
            { 

                Lo_Inventory cust = new Lo_Inventory(); 
                cust.Item_name =  Item_name;
                data += ",Item_name : " + Item_name;
                cust.Lab =  long.Parse(lab);
                data += ",lab : " + lab;
                response = c.add_Inventory(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_INVENTORY_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_INVENTORY_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_INVENTORY_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Inventory";
                Audit.InsertAudit((int)eventzz.ERROR_INVENTORY_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_Inventory> get_Inventory(string sql)
        { 
            List<Lo_Inventory> response = null;
            try
            { 
                Inventory c = new Inventory(); 
                response = c.get_Inventory(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_INVENTORY_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Inventory( string id, string oItem_name,string Item_name, bool andPassword = true) 
        { 
            string response = ""; 
            Inventory c = new Inventory();   
            string data = "";
            try
            { 
                Lo_Inventory cust = c.get_Inventory(" where id = " + id  )[0]; 
                cust.Item_name =  Item_name;
                data += ",Item_name : " + oItem_name + " -> " + Item_name;
                response = c.update_Inventory(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_INVENTORY_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_INVENTORY_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_INVENTORY_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Inventory_Movement(string Inventory,string Quantity,string Direction,string By_satff,string To_staff,string Move_date, bool returnID = false )
        { 
            string response = ""; 
            Inventory_Movement c = new Inventory_Movement();  
            string data = "";
            try
            { 

                Lo_Inventory_Movement cust = new Lo_Inventory_Movement(); 
                cust.Inventory =  long.Parse( Inventory == null ? "1" : Inventory)  ;
                data += ",Inventory : " + Inventory;
                cust.Quantity =  Int16.Parse( Quantity == null ? "1" : Quantity)  ;
                 data += ",Quantity : " + Quantity;
                cust.Direction =  Int16.Parse( Direction == null ? "1" : Direction)  ;
                 data += ",Direction : " + Direction;
                cust.By_satff =  long.Parse( By_satff == null ? "1" : By_satff)  ;
                 data += ",By_satff : " + By_satff;
                cust.To_staff =  long.Parse( To_staff == null ? "1" : To_staff)  ;
                 data += ",To_staff : " + To_staff;
                 cust.Move_date = System.DateTime.Now;
                response = c.add_Inventory_Movement(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_INVENTORY_MOVEMENT_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_INVENTORY_MOVEMENT_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_INVENTORY_MOVEMENT_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Inventory_Movement";
                Audit.InsertAudit((int)eventzz.ERROR_INVENTORY_MOVEMENT_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_Inventory_Movement_data> get_Inventory_Movement(string sql)
        { 
            List<Lo_Inventory_Movement_data> response = null;
            try
            { 
                Inventory_Movement c = new Inventory_Movement(); 
                response = c.get_Inventory_Movement_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_INVENTORY_MOVEMENT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Inventory_Movement( string id, string oInventory,string oQuantity,string oDirection,string oBy_satff,string oTo_staff,string oMove_date,string Inventory,string Quantity,string Direction,string By_satff,string To_staff,string Move_date, bool andPassword = true) 
        { 
            string response = ""; 
            Inventory_Movement c = new Inventory_Movement();   
            string data = "";
            try
            { 
                Lo_Inventory_Movement cust = c.get_Inventory_Movement(" where id = " + id  )[0]; 
                cust.Inventory =  long.Parse( Inventory == null ? "1" : Inventory)  ;
                data += ",Inventory : " + oInventory + " -> " + Inventory;
                 cust.Quantity =  Int16.Parse( Quantity == null ? "1" : Quantity)  ;
                 data += ",Quantity : " + oQuantity + " -> " + Quantity;
                 cust.Direction =  Int16.Parse( Direction == null ? "1" : Direction)  ;
                 data += ",Direction : " + oDirection + " -> " + Direction;
                 cust.By_satff =  long.Parse( By_satff == null ? "1" : By_satff)  ;
                 data += ",By_satff : " + oBy_satff + " -> " + By_satff;
                 cust.To_staff =  long.Parse( To_staff == null ? "1" : To_staff)  ;
                 data += ",To_staff : " + oTo_staff + " -> " + To_staff;
                response = c.update_Inventory_Movement(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_INVENTORY_MOVEMENT_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_INVENTORY_MOVEMENT_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_INVENTORY_MOVEMENT_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Lab(string Lab, bool returnID = false  )
        { 
            string response = ""; 
            Lab c = new Lab();  
            string data = "";
            try
            { 

                Lo_Lab cust = new Lo_Lab(); 
                cust.Lab =  Lab;
                data += ",Lab : " + Lab;
                response = c.add_Lab(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_LAB_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_LAB_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_LAB_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Lab";
                Audit.InsertAudit((int)eventzz.ERROR_LAB_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_Lab> get_Lab(string sql)
        { 
            List<Lo_Lab> response = null;
            try
            { 
                Lab c = new Lab(); 
                response = c.get_Lab(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_LAB_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Lab( string id, string oLab,string Lab, bool andPassword = true) 
        { 
            string response = ""; 
            Lab c = new Lab();   
            string data = "";
            try
            { 
                Lo_Lab cust = c.get_Lab(" where id = " + id  )[0]; 
                cust.Lab =  Lab;
                data += ",Lab : " + oLab + " -> " + Lab;
                response = c.update_Lab(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_LAB_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_LAB_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_LAB_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Patient(string First_name,string Surname,string Phone_number,string Email,string Date_of_birth,string Lab, bool returnID = false )
        { 
            string response = ""; 
            Patient c = new Patient();  
            string data = "";
            try
            { 

                Lo_Patient cust = new Lo_Patient(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Surname =  Surname;
                 data += ",Surname : " + Surname;
                cust.Phone_number =  Phone_number;
                 data += ",Phone_number : " + Phone_number;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                 cust.Date_of_birth = System.DateTime.Parse(Date_of_birth);

                 data += ",Date_of_birth : " + Date_of_birth;
                cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + Lab;
                response = c.add_Patient(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_PATIENT_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_PATIENT_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_PATIENT_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Patient";
                Audit.InsertAudit((int)eventzz.ERROR_PATIENT_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_Patient_data> get_Patient(string sql)
        { 
            List<Lo_Patient_data> response = null;
            try
            { 
                Patient c = new Patient(); 
                response = c.get_Patient_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_PATIENT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Patient( string id, string oFirst_name,string oSurname,string oPhone_number,string oEmail,string oDate_of_birth,string oLab,string First_name,string Surname,string Phone_number,string Email,string Date_of_birth,string Lab, bool andPassword = true) 
        { 
            string response = ""; 
            Patient c = new Patient();   
            string data = "";
            try
            { 
                Lo_Patient cust = c.get_Patient(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Surname =  Surname;
                 data += ",Surname : " + oSurname + " -> " + Surname;
                 cust.Phone_number =  Phone_number;
                 data += ",Phone_number : " + oPhone_number + " -> " + Phone_number;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;
                 cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + oLab + " -> " + Lab;
                 cust.Date_of_birth = DateTime.Parse(Date_of_birth);
                 data += ",Date_of_birth : " + oDate_of_birth + " -> " + Date_of_birth;
                 response = c.update_Patient(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_PATIENT_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_PATIENT_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_PATIENT_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_right_Admin(string Rightname, bool returnID = false )
        { 
            string response = ""; 
            right_Admin c = new right_Admin();  
            string data = "";
            try
            { 

                Lo_right_Admin cust = new Lo_right_Admin(); 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + Rightname;
                response = c.add_right_Admin(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_ADMIN_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding right_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_right_Admin> get_right_Admin(string sql)
        { 
            List<Lo_right_Admin> response = null;
            try
            { 
                right_Admin c = new right_Admin(); 
                response = c.get_right_Admin(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_right_Admin( string id, string oRightname,string Rightname, bool andPassword = true) 
        { 
            string response = ""; 
            right_Admin c = new right_Admin();   
            string data = "";
            try
            { 
                Lo_right_Admin cust = c.get_right_Admin(" where id = " + id  )[0]; 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + oRightname + " -> " + Rightname;
                response = c.update_right_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_right_Staff(string Rightname, bool returnID = false )
        { 
            string response = ""; 
            right_Staff c = new right_Staff();  
            string data = "";
            try
            { 

                Lo_right_Staff cust = new Lo_right_Staff(); 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + Rightname;
                response = c.add_right_Staff(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_STAFF_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_STAFF_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_STAFF_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding right_Staff";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_STAFF_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_right_Staff> get_right_Staff(string sql)
        { 
            List<Lo_right_Staff> response = null;
            try
            { 
                right_Staff c = new right_Staff(); 
                response = c.get_right_Staff(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_STAFF_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_right_Staff( string id, string oRightname,string Rightname, bool andPassword = true) 
        { 
            string response = ""; 
            right_Staff c = new right_Staff();   
            string data = "";
            try
            { 
                Lo_right_Staff cust = c.get_right_Staff(" where id = " + id  )[0]; 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + oRightname + " -> " + Rightname;
                response = c.update_right_Staff(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_STAFF_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_STAFF_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_STAFF_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_Admin(string Rolename,string Lab, string selectedRights, bool returnID = false )
        { 
            string response = ""; 
            role_Admin c = new role_Admin();  
            string data = "";
            try
            { 

                Lo_role_Admin cust = new Lo_role_Admin(); 
                cust.Rolename =  Rolename;
                data += ",Rolename : " + Rolename;
                cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + Lab;
                response = c.add_role_Admin(cust,selectedRights,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_ADMIN_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_role_Admin_data> get_role_Admin(string sql)
        { 
            List<Lo_role_Admin_data> response = null;
            try
            { 
                role_Admin c = new role_Admin(); 
                response = c.get_role_Admin_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_Admin( string id, string oRolename,string oLab,string Rolename,string Lab, string selectedRights, bool andPassword = true) 
        { 
            string response = ""; 
            role_Admin c = new role_Admin();   
            string data = "";
            try
            { 
                Lo_role_Admin cust = c.get_role_Admin(" where id = " + id  )[0]; 
                cust.Rolename =  Rolename;
                data += ",Rolename : " + oRolename + " -> " + Rolename;
                 cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + oLab + " -> " + Lab;
                response = c.update_role_Admin(cust, selectedRights);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_right_Admin(string Role,string Right, bool returnID = false )
        { 
            string response = ""; 
            role_right_Admin c = new role_right_Admin();  
            string data = "";
            try
            { 

                Lo_role_right_Admin cust = new Lo_role_right_Admin(); 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + Role;
                cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + Right;
                response = c.add_role_right_Admin(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_ADMIN_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_right_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_role_right_Admin_data> get_role_right_Admin(string sql)
        { 
            List<Lo_role_right_Admin_data> response = null;
            try
            { 
                role_right_Admin c = new role_right_Admin(); 
                response = c.get_role_right_Admin_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_right_Admin( string id, string oRole,string oRight,string Role,string Right, bool andPassword = true) 
        { 
            string response = ""; 
            role_right_Admin c = new role_right_Admin();   
            string data = "";
            try
            { 
                Lo_role_right_Admin cust = c.get_role_right_Admin(" where id = " + id  )[0]; 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + oRole + " -> " + Role;
                 cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + oRight + " -> " + Right;
                response = c.update_role_right_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_right_Staff(string Role,string Right, bool returnID = false )
        { 
            string response = ""; 
            role_right_Staff c = new role_right_Staff();  
            string data = "";
            try
            { 

                Lo_role_right_Staff cust = new Lo_role_right_Staff(); 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + Role;
                cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + Right;
                response = c.add_role_right_Staff(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_STAFF_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_STAFF_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_STAFF_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_right_Staff";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_STAFF_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_role_right_Staff_data> get_role_right_Staff(string sql)
        { 
            List<Lo_role_right_Staff_data> response = null;
            try
            { 
                role_right_Staff c = new role_right_Staff(); 
                response = c.get_role_right_Staff_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_STAFF_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_right_Staff( string id, string oRole,string oRight,string Role,string Right, bool andPassword = true) 
        { 
            string response = ""; 
            role_right_Staff c = new role_right_Staff();   
            string data = "";
            try
            { 
                Lo_role_right_Staff cust = c.get_role_right_Staff(" where id = " + id  )[0]; 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + oRole + " -> " + Role;
                 cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + oRight + " -> " + Right;
                response = c.update_role_right_Staff(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_STAFF_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_STAFF_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_STAFF_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_Staff(string Rolename,string Lab, string selectedRights, bool returnID = false )
        { 
            string response = ""; 
            role_Staff c = new role_Staff();  
            string data = "";
            try
            { 

                Lo_role_Staff cust = new Lo_role_Staff(); 
                cust.Rolename =  Rolename;
                data += ",Rolename : " + Rolename;
                cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + Lab;
                response = c.add_role_Staff(cust,selectedRights,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_STAFF_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_STAFF_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_STAFF_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_Staff";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_STAFF_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_role_Staff_data> get_role_Staff(string sql)
        { 
            List<Lo_role_Staff_data> response = null;
            try
            { 
                role_Staff c = new role_Staff(); 
                response = c.get_role_Staff_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_STAFF_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_Staff( string id, string oRolename,string oLab,string Rolename,string Lab, string selectedRights, bool andPassword = true) 
        { 
            string response = ""; 
            role_Staff c = new role_Staff();   
            string data = "";
            try
            { 
                Lo_role_Staff cust = c.get_role_Staff(" where id = " + id  )[0]; 
                cust.Rolename =  Rolename;
                data += ",Rolename : " + oRolename + " -> " + Rolename;
                 cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + oLab + " -> " + Lab;
                response = c.update_role_Staff(cust, selectedRights);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_STAFF_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_STAFF_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_STAFF_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Test_List(string Test,string Test_type, bool returnID = false )
        { 
            string response = ""; 
            Test_List c = new Test_List();  
            string data = "";
            try
            { 

                Lo_Test_List cust = new Lo_Test_List(); 
                cust.Test =  long.Parse( Test == null ? "1" : Test)  ;
                data += ",Test : " + Test;
                cust.Test_type =  long.Parse( Test_type == null ? "1" : Test_type)  ;
                 data += ",Test_type : " + Test_type;
                response = c.add_Test_List(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_TEST_LIST_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_TEST_LIST_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_TEST_LIST_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Test_List";
                Audit.InsertAudit((int)eventzz.ERROR_TEST_LIST_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }


        public static List<Lo_Test_List_data> get_Test_List(string sql)
        { 
            List<Lo_Test_List_data> response = null;
            try
            { 
                Test_List c = new Test_List(); 
                response = c.get_Test_List_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_TEST_LIST_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Test_List( string id, string oTest,string oTest_type,string Test,string Test_type, bool andPassword = true) 
        { 
            string response = ""; 
            Test_List c = new Test_List();   
            string data = "";
            try
            { 
                Lo_Test_List cust = c.get_Test_List(" where id = " + id  )[0]; 
                cust.Test =  long.Parse( Test == null ? "1" : Test)  ;
                data += ",Test : " + oTest + " -> " + Test;
                 cust.Test_type =  long.Parse( Test_type == null ? "1" : Test_type)  ;
                 data += ",Test_type : " + oTest_type + " -> " + Test_type;
                response = c.update_Test_List(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_TEST_LIST_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_TEST_LIST_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_TEST_LIST_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Test_Type(string Test_name,string Price,string Lab, bool returnID = false )
        { 
            string response = ""; 
            Test_Type c = new Test_Type();  
            string data = "";
            try
            { 

                Lo_Test_Type cust = new Lo_Test_Type(); 
                cust.Test_name =  Test_name;
                data += ",Test_name : " + Test_name;
                //cust.Price =   float.Parse(Price);
                // data += ",Price : " + Price;
                cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + Lab;
                response = c.add_Test_Type(cust, Price,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_TEST_TYPE_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_TEST_TYPE_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_TEST_TYPE_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Test_Type";
                Audit.InsertAudit((int)eventzz.ERROR_TEST_TYPE_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }

        public static List<Lo_Test_Type_data> get_Test_Type(string sql)
        { 
            List<Lo_Test_Type_data> response = null;
            try
            { 
                Test_Type c = new Test_Type(); 
                response = c.get_Test_Type_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_TEST_TYPE_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }



        public static List<Lo_Test_Type_data> get_Test_Type2(string sql, string id)
        { 
            List<Lo_Test_Type_data> response = null;
            try
            { 
                Test_Type c = new Test_Type(); 
                response = c.get_Test_Type_linked2(sql, id); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_TEST_TYPE_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Test_Type( string id, string oTest_name,string oPrice,string oLab,string Test_name,string Price,string Lab, bool andPassword = true) 
        { 
            string response = ""; 
            Test_Type c = new Test_Type();   
            string data = "";
            try
            { 
                Lo_Test_Type cust = c.get_Test_Type(" where id = " + id  )[0]; 
                cust.Test_name =  Test_name;
                data += ",Test_name : " + oTest_name + " -> " + Test_name;
                 //cust.Price =   float.Parse(Price);
                 //data += ",Price : " + oPrice + " -> " + Price;
                 cust.Lab =  long.Parse( Lab == null ? "1" : Lab)  ;
                 data += ",Lab : " + oLab + " -> " + Lab;
                 response = c.update_Test_Type(cust, Price);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_TEST_TYPE_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_TEST_TYPE_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_TEST_TYPE_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Tests(string Patient, string Test_date, string selectedTests, bool returnID = false)
        { 
            string response = ""; 
            Tests c = new Tests();  
            string data = "";
            try
            { 

                Lo_Tests cust = new Lo_Tests(); 
                cust.Patient =  long.Parse( Patient == null ? "1" : Patient)  ;
                data += ",Patient : " + Patient;
                 cust.Test_date = System.DateTime.Now;
                 response = c.add_Tests(cust, selectedTests, returnID);
               if( returnID  ){
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_TESTS_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = " creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_TESTS_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Tests";
                Audit.InsertAudit((int)eventzz.ERROR_TESTS_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Lo_Tests_data> get_Tests(string sql)
        { 
            List<Lo_Tests_data> response = null;
            try
            { 
                Tests c = new Tests(); 
                response = c.get_Tests_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_TESTS_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Tests(string id, string oPatient, string oTest_date, string Patient, string Test_date, string selectedTests, bool andPassword = true) 
        { 
            string response = ""; 
            Tests c = new Tests();   
            string data = "";
            try
            { 
                Lo_Tests cust = c.get_Tests(" where id = " + id  )[0]; 
                cust.Patient =  long.Parse( Patient == null ? "1" : Patient)  ;
                data += ",Patient : " + oPatient + " -> " + Patient;
                response = c.update_Tests(cust, selectedTests);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_TESTS_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "TESTS update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_TESTS_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_TESTS_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
    }
}
