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
    public class authenticate_Staff 
    { 
        public string add_authenticate_Staff(Lo_authenticate_Staff new_authenticate_Staff, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_authenticate_Staff>(new_authenticate_Staff);
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
         public string update_authenticate_Staff(Lo_authenticate_Staff new_authenticate_Staff)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_authenticate_Staff);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_authenticate_Staff_data> get_authenticate_Staff_linked(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_authenticate_Staff_data>( "select a.id , a.first_name , a.last_name , a.email , a.role , a1.roleName  role_data  , a.password , a.password2 , a.lab , a2.Lab  lab_data    from lo_authenticate_staff a  inner join  lo_role_staff a1 on a.role = a1.id  inner join  lo_lab a2 on a.lab = a2.id "  + sql);
             return actual;
         }  
         public List<Lo_authenticate_Staff> get_authenticate_Staff(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_authenticate_Staff>( sql);
             return actual;
         }  
     }
 public partial class Lo_authenticate_Staff_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("first_name")]  
    public string First_name  
    {  
        get { return _First_name; }  
        set { _First_name = value;  }  
    }  
    string _First_name;
    [Column("last_name")]  
    public string Last_name  
    {  
        get { return _Last_name; }  
        set { _Last_name = value;  }  
    }  
    string _Last_name;
    [Column("email")]  
    public string Email  
    {  
        get { return _Email; }  
        set { _Email = value;  }  
    }  
    string _Email;
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
    [Column("password")]  
    public byte[] Password  
    {  
        get { return _Password; }  
        set { _Password = value;  }  
    }  
    byte[] _Password;
    [Column("password2")]  
    public byte[] Password2  
    {  
        get { return _Password2; }  
        set { _Password2 = value;  }  
    }  
    byte[] _Password2;
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
