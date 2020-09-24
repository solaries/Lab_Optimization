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
    public class Test_List 
    { 
        public string add_Test_List(Lo_Test_List new_Test_List, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_Test_List>(new_Test_List);
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
         public string update_Test_List(Lo_Test_List new_Test_List)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_Test_List);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_Test_List_data> get_Test_List_linked(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Test_List_data>("select  a3.price,a.id , a.Test , a1.Test_Date  Test_data  , a.Test_Type , a2.test_name  Test_Type_data FROM Lo_Test_List a  inner JOIN Lo_Tests a1 on a.Test = a1.id  inner JOIN lo_test_type_price a3 on a.Test_Type = a3.id    inner JOIN  Lo_Test_Type a2 ON a3.Test_Type = a2.id  " + sql);
             return actual;
         }  
         public List<Lo_Test_List> get_Test_List(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Test_List>( sql);
             return actual;
         }  
     }
 public partial class Lo_Test_List_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("Test")]  
    public string Test  
    {  
        get { return _Test; }  
        set { _Test = value;  }  
    }  
    string _Test;
    [Column("Test_data")]  
    public string Test_data  
    {  
        get { return _Test_data; }  
        set { _Test_data = value;  }  
    }  
    string _Test_data;
    [Column("Test_Type")]  
    public string Test_type  
    {  
        get { return _Test_type; }  
        set { _Test_type = value;  }  
    }  
    string _Test_type;


    [Column("price")]  
    public float Price
    {
        get { return _Price; }
        set { _Price = value;   }
    }
    float _Price;  


    [Column("Test_Type_data")]  
    public string Test_type_data  
    {  
        get { return _Test_type_data; }  
        set { _Test_type_data = value;  }  
    }  
    string _Test_type_data;
  }  
 
 }
