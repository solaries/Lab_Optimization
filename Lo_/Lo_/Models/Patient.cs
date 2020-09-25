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
    public class Patient 
    { 
        public string add_Patient(Lo_Patient new_Patient, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_Patient>(new_Patient);
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
         public string update_Patient(Lo_Patient new_Patient)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_Patient);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_Patient_data> get_Patient_linked(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Patient_data>("select a.id , concat( a.first_name, ' ', a.Surname) first_name, a.Surname , a.Phone_number , a.Email , DATE_FORMAT(a.Date_Of_Birth, '%d/%b/%Y')  Date_Of_Birth, a.lab , a1.Lab  lab_data    from lo_patient a  inner join  lo_lab a1 on a.lab = a1.id " + sql);
             return actual;
         }  
         public List<Lo_Patient> get_Patient(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Patient>( sql);
             return actual;
         }  
     }
 public partial class Lo_Patient_data  
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
    [Column("Surname")]  
    public string Surname  
    {  
        get { return _Surname; }  
        set { _Surname = value;  }  
    }  
    string _Surname;
    [Column("Phone_number")]  
    public string Phone_number  
    {  
        get { return _Phone_number; }  
        set { _Phone_number = value;  }  
    }  
    string _Phone_number;
    [Column("Email")]  
    public string Email  
    {  
        get { return _Email; }  
        set { _Email = value;  }  
    }  
    string _Email;
    [Column("Date_Of_Birth")]  
    public string Date_of_birth  
    {  
        get { return _Date_of_birth; }  
        set { _Date_of_birth = value;  }  
    }  
    string _Date_of_birth;
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
