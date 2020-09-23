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
    public class Tests 
    {
        public string add_Tests(Lo_Tests new_Tests, string selectedTests, bool returnID = false) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Insert<Lo_Tests>(new_Tests);



                 List<Lo_Test_List> TestList = new List<Lo_Test_List>();
                 string[] idList = selectedTests.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                     Lo_Test_List Test = new Lo_Test_List();
                     Test.Test = long.Parse(x.ToString());
                     Test.Test_type = long.Parse(idList[i]);
                     TestList.Add(Test);
                 }
                 context.InsertBulk<Lo_Test_List>(TestList);


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
        public string update_Tests(Lo_Tests new_Tests, string selectedTests)
         {
             string result = "";
             try
             {
                 var context = Lo.Data.Models.Lo.GetInstance();
                 var x = context.Update(new_Tests);

                 context.DeleteWhere<Lo_Test_List>(" test = " + new_Tests.Id.ToString());




                 List<Lo_Test_List> TestList = new List<Lo_Test_List>();
                 string[] idList = selectedTests.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                     Lo_Test_List Test = new Lo_Test_List();
                     Test.Test = long.Parse(x.ToString());
                     Test.Test_type = long.Parse(idList[i]);
                     TestList.Add(Test);
                 }
                 context.InsertBulk<Lo_Test_List>(TestList);



             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Lo_Tests_data> get_Tests_linked(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Tests_data>( "select a.id , a.patient , a1.first_name  patient_data  , a.Test_Date   from Lo_Tests a  inner join  Lo_Patient a1 on a.patient = a1.id "  + sql);
             return actual;
         }  
         public List<Lo_Tests> get_Tests(string sql)
         {
             var context = Lo.Data.Models.Lo.GetInstance();
             var actual = context.Fetch<Lo_Tests>( sql);
             return actual;
         }  
     }
 public partial class Lo_Tests_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("patient")]  
    public string Patient  
    {  
        get { return _Patient; }  
        set { _Patient = value;  }  
    }  
    string _Patient;
    [Column("patient_data")]  
    public string Patient_data  
    {  
        get { return _Patient_data; }  
        set { _Patient_data = value;  }  
    }  
    string _Patient_data;
    [Column("Test_Date")]  
    public string Test_date  
    {  
        get { return _Test_date; }  
        set { _Test_date = value;  }  
    }  
    string _Test_date;
  }  
 
 }
