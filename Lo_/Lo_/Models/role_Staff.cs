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
    public class role_Staff 
    { 
        public string add_role_Staff(Lo_role_Staff new_role_Staff, string selectedRights, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_role_Staff>(new_role_Staff);
                if(returnID){
                    result =x.ToString().Trim();
                }
                 List<Lo_role_right_Staff> StaffRoleRightList = new List<Lo_role_right_Staff>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                    Lo_role_right_Staff StaffRoleRight = new Lo_role_right_Staff();
                    StaffRoleRight.Role = long.Parse(x.ToString());
                    StaffRoleRight.Right = long.Parse(idList[i]);
                    StaffRoleRightList.Add(StaffRoleRight);
                 }
                 context.InsertBulk<Lo_role_right_Staff>(StaffRoleRightList);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_role_Staff(Lo_role_Staff new_role_Staff, string selectedRights)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_role_Staff);
                 context.DeleteWhere<Lo_role_right_Staff>(" role = " + new_role_Staff.Id.ToString());
                 List<Lo_role_right_Staff> StaffRoleRightList = new List<Lo_role_right_Staff>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                    Lo_role_right_Staff StaffRoleRight = new Lo_role_right_Staff();
                    StaffRoleRight.Role = long.Parse(  new_role_Staff.Id.ToString());
                    StaffRoleRight.Right = long.Parse(idList[i]);
                    StaffRoleRightList.Add(StaffRoleRight);
                 }
                 context.InsertBulk<Lo_role_right_Staff>(StaffRoleRightList);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_role_Staff_data> get_role_Staff_linked(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_role_Staff_data>( "select a.id , a.roleName , a.lab , a1.Lab  lab_data    from Lo_role_Staff a  inner join  Lo_Lab a1 on a.lab = a1.id "  + sql);
             return actual;
         }  
         public List<Lo_role_Staff> get_role_Staff(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_role_Staff>( sql);
             return actual;
         }  
     }
 public partial class Lo_role_Staff_data  
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
