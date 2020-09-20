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
    public class Inventory_Movement 
    { 
        public string add_Inventory_Movement(Lo_Inventory_Movement new_Inventory_Movement, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_Inventory_Movement>(new_Inventory_Movement);
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
         public string update_Inventory_Movement(Lo_Inventory_Movement new_Inventory_Movement)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_Inventory_Movement);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_Inventory_Movement_data> get_Inventory_Movement_linked(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Inventory_Movement_data>( "select a.id , a.inventory , a1.Item_Name  inventory_data  , a.quantity , a.direction , a.by_satff , a2.first_name  by_satff_data  , a.to_staff , a3.first_name  to_staff_data  , a.move_date   from Lo_Inventory_Movement a  inner join  Lo_Inventory a1 on a.inventory = a1.id  inner join  Lo_authenticate_Staff a2 on a.by_satff = a2.id  inner join  Lo_authenticate_Staff a3 on a.to_staff = a3.id "  + sql);
             return actual;
         }  
         public List<Lo_Inventory_Movement> get_Inventory_Movement(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Inventory_Movement>( sql);
             return actual;
         }  
     }
 public partial class Lo_Inventory_Movement_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("inventory")]  
    public string Inventory  
    {  
        get { return _Inventory; }  
        set { _Inventory = value;  }  
    }  
    string _Inventory;
    [Column("inventory_data")]  
    public string Inventory_data  
    {  
        get { return _Inventory_data; }  
        set { _Inventory_data = value;  }  
    }  
    string _Inventory_data;
    [Column("quantity")]  
    public string Quantity  
    {  
        get { return _Quantity; }  
        set { _Quantity = value;  }  
    }  
    string _Quantity;
    [Column("direction")]  
    public string Direction  
    {  
        get { return _Direction; }  
        set { _Direction = value;  }  
    }  
    string _Direction;
    [Column("by_satff")]  
    public string By_satff  
    {  
        get { return _By_satff; }  
        set { _By_satff = value;  }  
    }  
    string _By_satff;
    [Column("by_satff_data")]  
    public string By_satff_data  
    {  
        get { return _By_satff_data; }  
        set { _By_satff_data = value;  }  
    }  
    string _By_satff_data;
    [Column("to_staff")]  
    public string To_staff  
    {  
        get { return _To_staff; }  
        set { _To_staff = value;  }  
    }  
    string _To_staff;
    [Column("to_staff_data")]  
    public string To_staff_data  
    {  
        get { return _To_staff_data; }  
        set { _To_staff_data = value;  }  
    }  
    string _To_staff_data;
    [Column("move_date")]  
    public string Move_date  
    {  
        get { return _Move_date; }  
        set { _Move_date = value;  }  
    }  
    string _Move_date;
  }  
 
 }
