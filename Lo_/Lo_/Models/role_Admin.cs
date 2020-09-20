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
    public class role_Admin 
    { 
        public string add_role_Admin(Lo_role_Admin new_role_Admin, string selectedRights, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_role_Admin>(new_role_Admin);
                if(returnID){
                    result =x.ToString().Trim();
                }
                 List<Lo_role_right_Admin> AdminRoleRightList = new List<Lo_role_right_Admin>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                    Lo_role_right_Admin AdminRoleRight = new Lo_role_right_Admin();
                    AdminRoleRight.Role = long.Parse(x.ToString());
                    AdminRoleRight.Right = long.Parse(idList[i]);
                    AdminRoleRightList.Add(AdminRoleRight);
                 }
                 context.InsertBulk<Lo_role_right_Admin>(AdminRoleRightList);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_role_Admin(Lo_role_Admin new_role_Admin, string selectedRights)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_role_Admin);
                 context.DeleteWhere<Lo_role_right_Admin>(" role = " + new_role_Admin.Id.ToString());
                 List<Lo_role_right_Admin> AdminRoleRightList = new List<Lo_role_right_Admin>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                    Lo_role_right_Admin AdminRoleRight = new Lo_role_right_Admin();
                    AdminRoleRight.Role = long.Parse(  new_role_Admin.Id.ToString());
                    AdminRoleRight.Right = long.Parse(idList[i]);
                    AdminRoleRightList.Add(AdminRoleRight);
                 }
                 context.InsertBulk<Lo_role_right_Admin>(AdminRoleRightList);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_role_Admin_data> get_role_Admin_linked(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_role_Admin_data>( "select a.id , a.roleName , a.lab , a1.Lab  lab_data    from Lo_role_Admin a  inner join  Lo_Lab a1 on a.lab = a1.id "  + sql);
             return actual;
         }  
         public List<Lo_role_Admin> get_role_Admin(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_role_Admin>( sql);
             return actual;
         }  
     }
 public partial class Lo_role_Admin_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("roleName")]  
    public string Rolename  
    {  
        get { return _Rolename; }  
        set { _Rolename = value;  }  
    }  
    string _Rolename;
    [Column("lab")]  
    public string Lab  
    {  
        get { return _Lab; }  
        set { _Lab = value;  }  
    }  
    string _Lab;
    [Column("lab_data")]  
    public string Lab_data  
    {  
        get { return _Lab_data; }  
        set { _Lab_data = value;  }  
    }  
    string _Lab_data;
  }  
 
 }
