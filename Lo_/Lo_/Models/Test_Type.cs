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
        public string add_Test_Type(Lo_Test_Type new_Test_Type, string Price, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_Test_Type>(new_Test_Type);

                 Lo_Test_Type_Price p = new Lo_Test_Type_Price();
                 p.Enter_date = DateTime.Now;
                 //p.Test_name = new_Test_Type.Test_name;
                 p.Price = float.Parse(Price);
                 p.Test_type = long.Parse(x.ToString());
                 context.Insert<Lo_Test_Type_Price>(p);

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
         public string update_Test_Type(Lo_Test_Type new_Test_Type,string price)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_Test_Type);


                 Lo_Test_Type_Price p = new Lo_Test_Type_Price();
                 p.Enter_date = DateTime.Now;
                 //p.Test_name = new_Test_Type.Test_name;
                 p.Price = float.Parse(price);
                 p.Test_type = new_Test_Type.Id;
                 context.Insert<Lo_Test_Type_Price>(p);

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
             var actual = context.Fetch<Lo_Test_Type_data>("select a.id , a2.id id2, a.test_name , a2.price , a.lab , a1.Lab  lab_data    from Lo_Test_Type a  inner join  Lo_Lab a1 on a.lab = a1.id inner join  (SELECT * from lo_test_type_price WHERE id IN ( SELECT id FROM (SELECT MAX(id) id,test_type FROM lo_test_type_price GROUP BY test_type  ) a )) a2 on a.id = a2.test_type" + sql);
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

    [Column("id2")]  
    public long Id2  
    {  
        get { return _Id2; }  
        set { _Id2 = value;  }  
    }  
    long _Id2;

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
