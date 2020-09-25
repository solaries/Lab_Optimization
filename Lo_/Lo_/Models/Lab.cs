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
    public class Lab 
    { 
        public string add_Lab(Lo_Lab new_Lab, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_Lab>(new_Lab);

                 Lo_role_Admin a = new Lo_role_Admin();
                 a.Lab = long.Parse(x.ToString());
                 a.Rolename = ".";
                 var y = context.Insert<Lo_role_Admin>(a); 
                 var r = context.Fetch<Lo_right_Admin>();
                 List<Lo_role_right_Admin> rral = new List<Lo_role_right_Admin>();
                 foreach (Lo_right_Admin ra in r)
                 {
                     Lo_role_right_Admin rra = new Lo_role_right_Admin();
                     rra.Right = ra.Id;
                     rra.Role = long.Parse(y.ToString());
                     rral.Add(rra);
                 }
                 if (rral.Count > 0)
                 {
                     context.InsertBulk<Lo_role_right_Admin>(rral);
                 }

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
         public string update_Lab(Lo_Lab new_Lab)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_Lab);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_Lab> get_Lab(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Lab>( sql);
             return actual;
         }  
     }
 
 }
