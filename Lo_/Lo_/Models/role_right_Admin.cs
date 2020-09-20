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
    public class role_right_Admin 
    { 
        public string add_role_right_Admin(Lo_role_right_Admin new_role_right_Admin, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_role_right_Admin>(new_role_right_Admin);
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
         public string update_role_right_Admin(Lo_role_right_Admin new_role_right_Admin)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_role_right_Admin);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_role_right_Admin_data> get_role_right_Admin_linked(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_role_right_Admin_data>( "select a.id , a.role , a1.roleName  role_data  , a.right , a2.rightName  right_data    from Lo_role_right_Admin a  inner join  Lo_role_Admin a1 on a.role = a1.id  inner join  Lo_right_Admin a2 on a.right = a2.id "  + sql);
             return actual;
         }  
         public List<Lo_role_right_Admin> get_role_right_Admin(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_role_right_Admin>( sql);
             return actual;
         }  
     }
 public partial class Lo_role_right_Admin_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("role")]  
    public string Role  
    {  
        get { return _Role; }  
        set { _Role = value;  }  
    }  
    string _Role;
    [Column("role_data")]  
    public string Role_data  
    {  
        get { return _Role_data; }  
        set { _Role_data = value;  }  
    }  
    string _Role_data;
    [Column("right")]  
    public string Right  
    {  
        get { return _Right; }  
        set { _Right = value;  }  
    }  
    string _Right;
    [Column("right_data")]  
    public string Right_data  
    {  
        get { return _Right_data; }  
        set { _Right_data = value;  }  
    }  
    string _Right_data;
  }  
 
 }
