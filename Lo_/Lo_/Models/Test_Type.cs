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
    public class Test_Type 
    { 
        public string add_Test_Type(Lo_Test_Type new_Test_Type, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_Test_Type>(new_Test_Type);
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
         public string update_Test_Type(Lo_Test_Type new_Test_Type)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_Test_Type);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_Test_Type_data> get_Test_Type_linked(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Test_Type_data>( "select a.id , a.test_name , a.price , a.lab , a1.Lab  lab_data    from Lo_Test_Type a  inner join  Lo_Lab a1 on a.lab = a1.id "  + sql);
             return actual;
         }  
         public List<Lo_Test_Type> get_Test_Type(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Test_Type>( sql);
             return actual;
         }  
     }
 public partial class Lo_Test_Type_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("test_name")]  
    public string Test_name  
    {  
        get { return _Test_name; }  
        set { _Test_name = value;  }  
    }  
    string _Test_name;
    [Column("price")]  
    public string Price  
    {  
        get { return _Price; }  
        set { _Price = value;  }  
    }  
    string _Price;
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
