using NPoco;
using Lo.Data;
using Lo.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
 
namespace Lo.Models 
{ 
    public class authenticate_SuperAdmin 
    { 
        public string add_authenticate_SuperAdmin(Lo_authenticate_SuperAdmin new_authenticate_SuperAdmin, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_authenticate_SuperAdmin>(new_authenticate_SuperAdmin);
                if(returnID){
                    result =x.ToString().Trim();
                }
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_authenticate_SuperAdmin(Lo_authenticate_SuperAdmin new_authenticate_SuperAdmin)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_authenticate_SuperAdmin);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_authenticate_SuperAdmin> get_authenticate_SuperAdmin(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_authenticate_SuperAdmin>( sql);
             return actual;
         }  
     }
 
 }
