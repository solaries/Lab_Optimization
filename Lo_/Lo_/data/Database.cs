using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;

namespace Lo.Data.Models 
{
	public partial class Lo : Database
	{
		public Lo() : base("Lo")
		{
			CommonConstruct();
		}
		public virtual void CommonConstruct()
		{
		    Factory = new DefaultFactory();
		}
		public interface IFactory
		{
			Lo GetInstance();
		    void BeginTransaction(Lo database);
		    void CompleteTransaction(Lo database);
		}


        public class DefaultFactory : IFactory
        {
            [ThreadStatic]
            static Stack<Lo> _stack = new Stack<Lo>();

            public Lo GetInstance()
            {
               
			    if (_stack == null)
                { return new  Lo(); }
                else { 
					return _stack.Count > 0 ? _stack.Peek() : new Lo();
                }
			   
			    
            }

            public void BeginTransaction(Lo database)
            {

			 if (_stack == null)
				 {
				  _stack = new  Stack<Lo>();
				 }
                _stack.Push(database);
            }

            public void CompleteTransaction(Lo database)
            {
			 if (_stack == null)
				 {
				  _stack = new Stack <Lo>();
				 }
                _stack.Pop();
            }
        }
		
		public static IFactory Factory { get; set; }

        public static Lo GetInstance()
        {
		 if (Factory == null)
                return new Lo();
			return Factory.GetInstance();
        }

		protected override void OnBeginTransaction()
		{
            Factory.BeginTransaction(this);
		}

        protected override void OnCompleteTransaction()
		{
            Factory.CompleteTransaction(this);
		}

		public class Record<T> where T:new()
		{
			public bool IsNew(Database db) { return db.IsNew(this); }
			public object Insert(Database db) { return db.Insert(this); }  
			
			public int Update(Database db, IEnumerable<string> columns) { return db.Update(this, columns); }
			public static int Update(Database db, string sql, params object[] args) { return db.Update<T>(sql, args); }
			public static int Update(Database db, Sql sql) { return db.Update<T>(sql); }
			public int Delete(Database db) { return db.Delete(this); }
			public static int Delete(Database db, string sql, params object[] args) { return db.Delete<T>(sql, args); }
			public static int Delete(Database db, Sql sql) { return db.Delete<T>(sql); }
			public static int Delete(Database db, object primaryKey) { return db.Delete<T>(primaryKey); }
			public static bool Exists(Database db, object primaryKey) { return db.Exists<T>(primaryKey); }
			public static T SingleOrDefault(Database db, string sql, params object[] args) { return db.SingleOrDefault<T>(sql, args); }
			public static T SingleOrDefault(Database db, Sql sql) { return db.SingleOrDefault<T>(sql); }
			public static T FirstOrDefault(Database db, string sql, params object[] args) { return db.FirstOrDefault<T>(sql, args); }
			public static T FirstOrDefault(Database db, Sql sql) { return db.FirstOrDefault<T>(sql); }
			public static T Single(Database db, string sql, params object[] args) { return db.Single<T>(sql, args); }
			public static T Single(Database db, Sql sql) { return db.Single<T>(sql); }
			public static T First(Database db, string sql, params object[] args) { return db.First<T>(sql, args); }
			public static T First(Database db, Sql sql) { return db.First<T>(sql); }
			public static List<T> Fetch(Database db, string sql, params object[] args) { return db.Fetch<T>(sql, args); }
			public static List<T> Fetch(Database db, Sql sql) { return db.Fetch<T>(sql); }
			public static List<T> Fetch(Database db, long page, long itemsPerPage, string sql, params object[] args) { return db.Fetch<T>(page, itemsPerPage, sql, args); }
			public static List<T> Fetch(Database db, long page, long itemsPerPage, Sql sql) { return db.Fetch<T>(page, itemsPerPage, sql); }
			public static List<T> SkipTake(Database db, long skip, long take, string sql, params object[] args) { return db.SkipTake<T>(skip, take, sql, args); }
			public static List<T> SkipTake(Database db, long skip, long take, Sql sql) { return db.SkipTake<T>(skip, take, sql); }
			public static Page<T> Page(Database db, long page, long itemsPerPage, string sql, params object[] args) { return db.Page<T>(page, itemsPerPage, sql, args); }
			public static Page<T> Page(Database db, long page, long itemsPerPage, Sql sql) { return db.Page<T>(page, itemsPerPage, sql); }
			public static IEnumerable<T> Query(Database db, string sql, params object[] args) { return db.Query<T>(sql, args); }
			public static IEnumerable<T> Query(Database db, Sql sql) { return db.Query<T>(sql); }			
			
			protected HashSet<string> Tracker = new HashSet<string>();
			private void OnLoaded() { Tracker.Clear(); }
			protected void Track(string c) { if (!Tracker.Contains(c)) Tracker.Add(c); }

			public int Update(Database db) 
			{ 
				if (Tracker.Count == 0)
					return db.Update(this); 

				var retv = db.Update(this, Tracker.ToArray());
				Tracker.Clear();
				return retv;
			}
			public void Save(Database db) 
			{
                if (this.IsNew(db))
					Insert(db);
				else
					Update(db);
			}		
		}	
	}
	 [TableName("Lo_event")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class @Event : Lo.Record<@Event>

		{
			[Column("id")] public long Id 
			{ 
				get { return _Id; }
				set { _Id = value; Track("id"); }
			}
			long _Id;
			[Column("eventName")] public string Eventname 
			{ 
				get { return _Eventname; }
				set { _Eventname = value; Track("eventName"); }
			}
			string _Eventname;
		
			public static IEnumerable<@Event> Query(Database db, string[] columns = null, long[] Id = null)
            {
                var sql = new Sql();

                if (columns != null)
                    sql.Select(columns);

                sql.From("Lo_event (NOLOCK)");


				if (Id != null)
					sql.Where("id IN (@0)", Id);


                return db.Query<@Event>(sql);
            }

		} [TableName("Lo_eventLog")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class EventLog : Lo.Record<EventLog>

		{
			[Column("id")] public long Id 
			{ 
				get { return _Id; }
				set { _Id = value; Track("id"); }
			}
			long _Id;
			[Column("eventid")] public long Eventid 
			{ 
				get { return _Eventid; }
				set { _Eventid = value; Track("eventid"); }
			}
			long _Eventid;
			[Column("description")] public string Description 
			{ 
				get { return _Description; }
				set { _Description = value; Track("description"); }
			}
			string _Description;
			[Column("userEvent")] public bool Userevent 
			{ 
				get { return _Userevent; }
				set { _Userevent = value; Track("userEvent"); }
			}
			bool _Userevent;
			[Column("userid")] public long Userid 
			{ 
				get { return _Userid; }
				set { _Userid = value; Track("userid"); }
			}
			long _Userid;
			[Column("eventDate")] public DateTime Eventdate 
			{ 
				get { return _Eventdate; }
				set { _Eventdate = value; Track("eventDate"); }
			}
			DateTime _Eventdate;
		
			public static IEnumerable<EventLog> Query(Database db, string[] columns = null, long[] Id = null)
            {
                var sql = new Sql();

                if (columns != null)
                    sql.Select(columns);

                sql.From("Lo_eventLog (NOLOCK)");


				if (Id != null)
					sql.Where("id IN (@0)", Id);


                return db.Query<EventLog>(sql);
            }

		} [TableName("Lo_authenticate_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_authenticate_Admin : Lo.Record<Lo_authenticate_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("last_name")]  
     public  string   Last_name  
     {  
         get { return _Last_name; }  
         set { _Last_name = value; Track("last_name"); }  
      }  
      string   _Last_name;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("password")]  
     public  byte[]  Password  
     {  
         get { return _Password; }  
         set { _Password = value; Track("password"); }  
      }  
      byte[]  _Password;  
    
     [Column("password2")]  
     public  byte[]  Password2  
     {  
         get { return _Password2; }  
         set { _Password2 = value; Track("password2"); }  
      }  
      byte[]  _Password2;  
    
     [Column("lab")]  
     public  long  Lab  
     {  
         get { return _Lab; }  
         set { _Lab = value; Track("lab"); }  
      }  
      long  _Lab;  
    
 public static IEnumerable<Lo_authenticate_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_authenticate_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_authenticate_Admin>(sql);
     }
  }


 [TableName("Lo_authenticate_Staff")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_authenticate_Staff : Lo.Record<Lo_authenticate_Staff>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("last_name")]  
     public  string   Last_name  
     {  
         get { return _Last_name; }  
         set { _Last_name = value; Track("last_name"); }  
      }  
      string   _Last_name;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("password")]  
     public  byte[]  Password  
     {  
         get { return _Password; }  
         set { _Password = value; Track("password"); }  
      }  
      byte[]  _Password;  
    
     [Column("password2")]  
     public  byte[]  Password2  
     {  
         get { return _Password2; }  
         set { _Password2 = value; Track("password2"); }  
      }  
      byte[]  _Password2;  
    
     [Column("lab")]  
     public  long  Lab  
     {  
         get { return _Lab; }  
         set { _Lab = value; Track("lab"); }  
      }  
      long  _Lab;  
    
 public static IEnumerable<Lo_authenticate_Staff> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_authenticate_Staff (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_authenticate_Staff>(sql);
     }
  }


 [TableName("Lo_authenticate_SuperAdmin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_authenticate_SuperAdmin : Lo.Record<Lo_authenticate_SuperAdmin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("last_name")]  
     public  string   Last_name  
     {  
         get { return _Last_name; }  
         set { _Last_name = value; Track("last_name"); }  
      }  
      string   _Last_name;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("password")]  
     public  byte[]  Password  
     {  
         get { return _Password; }  
         set { _Password = value; Track("password"); }  
      }  
      byte[]  _Password;  
    
     [Column("password2")]  
     public  byte[]  Password2  
     {  
         get { return _Password2; }  
         set { _Password2 = value; Track("password2"); }  
      }  
      byte[]  _Password2;  
    
 public static IEnumerable<Lo_authenticate_SuperAdmin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_authenticate_SuperAdmin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_authenticate_SuperAdmin>(sql);
     }
  }


 [TableName("Lo_Inventory")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_Inventory : Lo.Record<Lo_Inventory>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  

     [Column("lab")]  
     public long Lab  
     {
         get { return _Lab; }
         set { _Lab = value; Track("lab"); }  
      }
     long _Lab; 
    
     [Column("item_name")]  
     public  string   Item_name  
     {  
         get { return _Item_name; }  
         set { _Item_name = value; Track("item_name"); }  
      }  
      string   _Item_name;  
    
 public static IEnumerable<Lo_Inventory> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_Inventory (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_Inventory>(sql);
     }
  }


 [TableName("Lo_Inventory_Movement")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_Inventory_Movement : Lo.Record<Lo_Inventory_Movement>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("inventory")]  
     public  long  Inventory  
     {  
         get { return _Inventory; }  
         set { _Inventory = value; Track("inventory"); }  
      }  
      long  _Inventory;  
    
     [Column("quantity")]  
     public   Int16   Quantity  
     {  
         get { return _Quantity; }  
         set { _Quantity = value; Track("quantity"); }  
      }  
       Int16   _Quantity;  
    
     [Column("direction")]  
     public   Int16   Direction  
     {  
         get { return _Direction; }  
         set { _Direction = value; Track("direction"); }  
      }  
       Int16   _Direction;  
    
     [Column("by_satff")]  
     public  long  By_satff  
     {  
         get { return _By_satff; }  
         set { _By_satff = value; Track("by_satff"); }  
      }  
      long  _By_satff;  
    
     [Column("to_staff")]  
     public  long  To_staff  
     {  
         get { return _To_staff; }  
         set { _To_staff = value; Track("to_staff"); }  
      }  
      long  _To_staff;  
    
     [Column("move_date")]  
     public  DateTime  Move_date  
     {  
         get { return _Move_date; }  
         set { _Move_date = value; Track("move_date"); }  
      }  
      DateTime  _Move_date;  
    
 public static IEnumerable<Lo_Inventory_Movement> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_Inventory_Movement (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_Inventory_Movement>(sql);
     }
  }


 [TableName("Lo_Lab")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_Lab : Lo.Record<Lo_Lab>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("lab")]  
     public  string   Lab  
     {  
         get { return _Lab; }  
         set { _Lab = value; Track("lab"); }  
      }  
      string   _Lab;  
    
 public static IEnumerable<Lo_Lab> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_Lab (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_Lab>(sql);
     }
  }


 [TableName("Lo_Patient")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_Patient : Lo.Record<Lo_Patient>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("surname")]  
     public  string   Surname  
     {  
         get { return _Surname; }  
         set { _Surname = value; Track("surname"); }  
      }  
      string   _Surname;  
    
     [Column("phone_number")]  
     public  string  Phone_number  
     {  
         get { return _Phone_number; }  
         set { _Phone_number = value; Track("phone_number"); }  
      }  
      string  _Phone_number;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("date_of_birth")]  
     public  DateTime  Date_of_birth  
     {  
         get { return _Date_of_birth; }  
         set { _Date_of_birth = value; Track("date_of_birth"); }  
      }  
      DateTime  _Date_of_birth;  
    
     [Column("lab")]  
     public  long  Lab  
     {  
         get { return _Lab; }  
         set { _Lab = value; Track("lab"); }  
      }  
      long  _Lab;  
    
 public static IEnumerable<Lo_Patient> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_Patient (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_Patient>(sql);
     }
  }


 [TableName("Lo_right_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_right_Admin : Lo.Record<Lo_right_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rightname")]  
     public  string   Rightname  
     {  
         get { return _Rightname; }  
         set { _Rightname = value; Track("rightname"); }  
      }  
      string   _Rightname;  
    
 public static IEnumerable<Lo_right_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_right_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_right_Admin>(sql);
     }
  }


 [TableName("Lo_right_Staff")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_right_Staff : Lo.Record<Lo_right_Staff>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rightname")]  
     public  string   Rightname  
     {  
         get { return _Rightname; }  
         set { _Rightname = value; Track("rightname"); }  
      }  
      string   _Rightname;  
    
 public static IEnumerable<Lo_right_Staff> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_right_Staff (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_right_Staff>(sql);
     }
  }


 [TableName("Lo_role_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_role_Admin : Lo.Record<Lo_role_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rolename")]  
     public  string   Rolename  
     {  
         get { return _Rolename; }  
         set { _Rolename = value; Track("rolename"); }  
      }  
      string   _Rolename;  
    
     [Column("lab")]  
     public  long  Lab  
     {  
         get { return _Lab; }  
         set { _Lab = value; Track("lab"); }  
      }  
      long  _Lab;  
    
 public static IEnumerable<Lo_role_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_role_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_role_Admin>(sql);
     }
  }


 [TableName("Lo_role_right_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_role_right_Admin : Lo.Record<Lo_role_right_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("right")]  
     public  long  Right  
     {  
         get { return _Right; }  
         set { _Right = value; Track("right"); }  
      }  
      long  _Right;  
    
 public static IEnumerable<Lo_role_right_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_role_right_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_role_right_Admin>(sql);
     }
  }


 [TableName("Lo_role_right_Staff")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_role_right_Staff : Lo.Record<Lo_role_right_Staff>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("right")]  
     public  long  Right  
     {  
         get { return _Right; }  
         set { _Right = value; Track("right"); }  
      }  
      long  _Right;  
    
 public static IEnumerable<Lo_role_right_Staff> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_role_right_Staff (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_role_right_Staff>(sql);
     }
  }


 [TableName("Lo_role_Staff")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_role_Staff : Lo.Record<Lo_role_Staff>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rolename")]  
     public  string   Rolename  
     {  
         get { return _Rolename; }  
         set { _Rolename = value; Track("rolename"); }  
      }  
      string   _Rolename;  
    
     [Column("lab")]  
     public  long  Lab  
     {  
         get { return _Lab; }  
         set { _Lab = value; Track("lab"); }  
      }  
      long  _Lab;  
    
 public static IEnumerable<Lo_role_Staff> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_role_Staff (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_role_Staff>(sql);
     }
  }


 [TableName("Lo_Test_List")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_Test_List : Lo.Record<Lo_Test_List>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("test")]  
     public  long  Test  
     {  
         get { return _Test; }  
         set { _Test = value; Track("test"); }  
      }  
      long  _Test;  
    
     [Column("test_type")]  
     public  long  Test_type  
     {  
         get { return _Test_type; }  
         set { _Test_type = value; Track("test_type"); }  
      }  
      long  _Test_type;  
    
 public static IEnumerable<Lo_Test_List> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_Test_List (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_Test_List>(sql);
     }
  }


 [TableName("Lo_Test_Type")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_Test_Type : Lo.Record<Lo_Test_Type>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("test_name")]  
     public  string   Test_name  
     {  
         get { return _Test_name; }  
         set { _Test_name = value; Track("test_name"); }  
      }  
      string   _Test_name;  
    
     //[Column("price")]  
     //public   float   Price  
     //{  
     //    get { return _Price; }  
     //    set { _Price = value; Track("price"); }  
     // }  
     //  float   _Price;  
    
     [Column("lab")]  
     public  long  Lab  
     {  
         get { return _Lab; }  
         set { _Lab = value; Track("lab"); }  
      }  
      long  _Lab;  
    
 public static IEnumerable<Lo_Test_Type> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_Test_Type (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_Test_Type>(sql);
     }
  }



 [TableName("Lo_Test_Type_Price")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_Test_Type_Price : Lo.Record<Lo_Test_Type_Price>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;


     [Column("test_type")]
     public long Test_type
     {
         get { return _Test_type; }
         set { _Test_type = value; Track("test_type"); }
     }
     long _Test_type;  


     //[Column("test_name")]  
     //public  string   Test_name  
     //{  
     //    get { return _Test_name; }  
     //    set { _Test_name = value; Track("test_name"); }  
     // }  
     // string   _Test_name;

      [Column("price")]
      public float Price
      {
          get { return _Price; }
          set { _Price = value; Track("price"); }
      }
      float _Price;  

     [Column("enter_date")]
      public DateTime Enter_date 
     {
         get { return _Enter_date; }
         set { _Enter_date = value; Track("enter_date"); }  
      }
     DateTime _Enter_date;

     public static IEnumerable<Lo_Test_Type_Price> Query(Database db, string[] columns = null, long[] Id = null)
     {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);
     sql.From("Lo_Test_Type_Price (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);

       return db.Query<Lo_Test_Type_Price>(sql);
     }
  }

 [TableName("Lo_Tests")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Lo_Tests : Lo.Record<Lo_Tests>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("patient")]  
     public  long  Patient  
     {  
         get { return _Patient; }  
         set { _Patient = value; Track("patient"); }  
      }  
      long  _Patient;  
    
     [Column("test_date")]  
     public  DateTime  Test_date  
     {  
         get { return _Test_date; }  
         set { _Test_date = value; Track("test_date"); }  
      }  
      DateTime  _Test_date;  
    
 public static IEnumerable<Lo_Tests> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Lo_Tests (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Lo_Tests>(sql);
     }
  }

	}
