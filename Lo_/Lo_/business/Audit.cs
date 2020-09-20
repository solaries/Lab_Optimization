using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography; 
using System.Text;
using Lo.Data.Models;

namespace Lo.BusinessLogic
{ 
        public enum eventzz 
        { 
        SUCCESSFUL_AUTHENTICATE_ADMIN_ADD = 1,
FAILED_AUTHENTICATE_ADMIN_ADD = 2,
ERROR_AUTHENTICATE_ADMIN_ADD = 3,
SUCCESSFUL_AUTHENTICATE_ADMIN_GET = 4,
FAILED_AUTHENTICATE_ADMIN_GET = 5,
ERROR_AUTHENTICATE_ADMIN_GET = 6,
SUCCESSFUL_AUTHENTICATE_ADMIN_UPDATE = 7,
FAILED_AUTHENTICATE_ADMIN_UPDATE = 8,
ERROR_AUTHENTICATE_ADMIN_UPDATE = 9,
SUCCESSFUL_AUTHENTICATE_STAFF_ADD = 10,
FAILED_AUTHENTICATE_STAFF_ADD = 11,
ERROR_AUTHENTICATE_STAFF_ADD = 12,
SUCCESSFUL_AUTHENTICATE_STAFF_GET = 13,
FAILED_AUTHENTICATE_STAFF_GET = 14,
ERROR_AUTHENTICATE_STAFF_GET = 15,
SUCCESSFUL_AUTHENTICATE_STAFF_UPDATE = 16,
FAILED_AUTHENTICATE_STAFF_UPDATE = 17,
ERROR_AUTHENTICATE_STAFF_UPDATE = 18,
SUCCESSFUL_AUTHENTICATE_SUPERADMIN_ADD = 19,
FAILED_AUTHENTICATE_SUPERADMIN_ADD = 20,
ERROR_AUTHENTICATE_SUPERADMIN_ADD = 21,
SUCCESSFUL_AUTHENTICATE_SUPERADMIN_GET = 22,
FAILED_AUTHENTICATE_SUPERADMIN_GET = 23,
ERROR_AUTHENTICATE_SUPERADMIN_GET = 24,
SUCCESSFUL_AUTHENTICATE_SUPERADMIN_UPDATE = 25,
FAILED_AUTHENTICATE_SUPERADMIN_UPDATE = 26,
ERROR_AUTHENTICATE_SUPERADMIN_UPDATE = 27,
SUCCESSFUL_INVENTORY_ADD = 28,
FAILED_INVENTORY_ADD = 29,
ERROR_INVENTORY_ADD = 30,
SUCCESSFUL_INVENTORY_GET = 31,
FAILED_INVENTORY_GET = 32,
ERROR_INVENTORY_GET = 33,
SUCCESSFUL_INVENTORY_UPDATE = 34,
FAILED_INVENTORY_UPDATE = 35,
ERROR_INVENTORY_UPDATE = 36,
SUCCESSFUL_INVENTORY_MOVEMENT_ADD = 37,
FAILED_INVENTORY_MOVEMENT_ADD = 38,
ERROR_INVENTORY_MOVEMENT_ADD = 39,
SUCCESSFUL_INVENTORY_MOVEMENT_GET = 40,
FAILED_INVENTORY_MOVEMENT_GET = 41,
ERROR_INVENTORY_MOVEMENT_GET = 42,
SUCCESSFUL_INVENTORY_MOVEMENT_UPDATE = 43,
FAILED_INVENTORY_MOVEMENT_UPDATE = 44,
ERROR_INVENTORY_MOVEMENT_UPDATE = 45,
SUCCESSFUL_LAB_ADD = 46,
FAILED_LAB_ADD = 47,
ERROR_LAB_ADD = 48,
SUCCESSFUL_LAB_GET = 49,
FAILED_LAB_GET = 50,
ERROR_LAB_GET = 51,
SUCCESSFUL_LAB_UPDATE = 52,
FAILED_LAB_UPDATE = 53,
ERROR_LAB_UPDATE = 54,
SUCCESSFUL_PATIENT_ADD = 55,
FAILED_PATIENT_ADD = 56,
ERROR_PATIENT_ADD = 57,
SUCCESSFUL_PATIENT_GET = 58,
FAILED_PATIENT_GET = 59,
ERROR_PATIENT_GET = 60,
SUCCESSFUL_PATIENT_UPDATE = 61,
FAILED_PATIENT_UPDATE = 62,
ERROR_PATIENT_UPDATE = 63,
SUCCESSFUL_RIGHT_ADMIN_ADD = 64,
FAILED_RIGHT_ADMIN_ADD = 65,
ERROR_RIGHT_ADMIN_ADD = 66,
SUCCESSFUL_RIGHT_ADMIN_GET = 67,
FAILED_RIGHT_ADMIN_GET = 68,
ERROR_RIGHT_ADMIN_GET = 69,
SUCCESSFUL_RIGHT_ADMIN_UPDATE = 70,
FAILED_RIGHT_ADMIN_UPDATE = 71,
ERROR_RIGHT_ADMIN_UPDATE = 72,
SUCCESSFUL_RIGHT_STAFF_ADD = 73,
FAILED_RIGHT_STAFF_ADD = 74,
ERROR_RIGHT_STAFF_ADD = 75,
SUCCESSFUL_RIGHT_STAFF_GET = 76,
FAILED_RIGHT_STAFF_GET = 77,
ERROR_RIGHT_STAFF_GET = 78,
SUCCESSFUL_RIGHT_STAFF_UPDATE = 79,
FAILED_RIGHT_STAFF_UPDATE = 80,
ERROR_RIGHT_STAFF_UPDATE = 81,
SUCCESSFUL_ROLE_ADMIN_ADD = 82,
FAILED_ROLE_ADMIN_ADD = 83,
ERROR_ROLE_ADMIN_ADD = 84,
SUCCESSFUL_ROLE_ADMIN_GET = 85,
FAILED_ROLE_ADMIN_GET = 86,
ERROR_ROLE_ADMIN_GET = 87,
SUCCESSFUL_ROLE_ADMIN_UPDATE = 88,
FAILED_ROLE_ADMIN_UPDATE = 89,
ERROR_ROLE_ADMIN_UPDATE = 90,
SUCCESSFUL_ROLE_RIGHT_ADMIN_ADD = 91,
FAILED_ROLE_RIGHT_ADMIN_ADD = 92,
ERROR_ROLE_RIGHT_ADMIN_ADD = 93,
SUCCESSFUL_ROLE_RIGHT_ADMIN_GET = 94,
FAILED_ROLE_RIGHT_ADMIN_GET = 95,
ERROR_ROLE_RIGHT_ADMIN_GET = 96,
SUCCESSFUL_ROLE_RIGHT_ADMIN_UPDATE = 97,
FAILED_ROLE_RIGHT_ADMIN_UPDATE = 98,
ERROR_ROLE_RIGHT_ADMIN_UPDATE = 99,
SUCCESSFUL_ROLE_RIGHT_STAFF_ADD = 100,
FAILED_ROLE_RIGHT_STAFF_ADD = 101,
ERROR_ROLE_RIGHT_STAFF_ADD = 102,
SUCCESSFUL_ROLE_RIGHT_STAFF_GET = 103,
FAILED_ROLE_RIGHT_STAFF_GET = 104,
ERROR_ROLE_RIGHT_STAFF_GET = 105,
SUCCESSFUL_ROLE_RIGHT_STAFF_UPDATE = 106,
FAILED_ROLE_RIGHT_STAFF_UPDATE = 107,
ERROR_ROLE_RIGHT_STAFF_UPDATE = 108,
SUCCESSFUL_ROLE_STAFF_ADD = 109,
FAILED_ROLE_STAFF_ADD = 110,
ERROR_ROLE_STAFF_ADD = 111,
SUCCESSFUL_ROLE_STAFF_GET = 112,
FAILED_ROLE_STAFF_GET = 113,
ERROR_ROLE_STAFF_GET = 114,
SUCCESSFUL_ROLE_STAFF_UPDATE = 115,
FAILED_ROLE_STAFF_UPDATE = 116,
ERROR_ROLE_STAFF_UPDATE = 117,
SUCCESSFUL_TEST_LIST_ADD = 118,
FAILED_TEST_LIST_ADD = 119,
ERROR_TEST_LIST_ADD = 120,
SUCCESSFUL_TEST_LIST_GET = 121,
FAILED_TEST_LIST_GET = 122,
ERROR_TEST_LIST_GET = 123,
SUCCESSFUL_TEST_LIST_UPDATE = 124,
FAILED_TEST_LIST_UPDATE = 125,
ERROR_TEST_LIST_UPDATE = 126,
SUCCESSFUL_TEST_TYPE_ADD = 127,
FAILED_TEST_TYPE_ADD = 128,
ERROR_TEST_TYPE_ADD = 129,
SUCCESSFUL_TEST_TYPE_GET = 130,
FAILED_TEST_TYPE_GET = 131,
ERROR_TEST_TYPE_GET = 132,
SUCCESSFUL_TEST_TYPE_UPDATE = 133,
FAILED_TEST_TYPE_UPDATE = 134,
ERROR_TEST_TYPE_UPDATE = 135,
SUCCESSFUL_TESTS_ADD = 136,
FAILED_TESTS_ADD = 137,
ERROR_TESTS_ADD = 138,
SUCCESSFUL_TESTS_GET = 139,
FAILED_TESTS_GET = 140,
ERROR_TESTS_GET = 141,
SUCCESSFUL_TESTS_UPDATE = 142,
FAILED_TESTS_UPDATE = 143,
ERROR_TESTS_UPDATE = 144 
        } 
    public class Audit 
    {  
        public static void InsertAudit(EventLog newEvent, string callerFormName)
        {
            var context = Lo.Data.Models.Lo.GetInstance();
            try
            {
                context.Insert<EventLog>(newEvent);
            }
            catch (Exception ex)
            {
            
            }
        }
        public static void InsertAudit(int eventId, string eventDetails, long userId, bool userevent)
        { 
            EventLog audit = new EventLog();
            audit.Description =  eventDetails;
            audit.Eventid = eventId;
            audit.Userid = userId;
            audit.Userevent = userevent;
            audit.Eventdate = DateTime.Now;
            InsertAudit(audit, "");
        }
        public static string GetEncodedHash(string password, string salt)
        {
           MD5 md5 = new MD5CryptoServiceProvider();
           byte [] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
           string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
           return base64digest.Substring(0, base64digest.Length-2);
        }

    public static  void protocol()

    {

        ServicePointManager.SecurityProtocol |= (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

    }

        public static string SendMail(string email, string mailSubject, string mailBody, string callerFormName)
        {
            bool iSBodyHtml = false;
            string result = "";
            try
            {
                  MailMessage mail = new MailMessage();
                  mail.From = new MailAddress(ConfigurationManager.AppSettings["email"]);
                  mail.To.Add(email);
                  mail.Subject = mailSubject;
                  mail.IsBodyHtml = true;
                  mail.Body = mailBody;
                  SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["client"]);
                  smtp.Port = int.Parse(ConfigurationManager.AppSettings["port"]);
                  smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["username_email"], ConfigurationManager.AppSettings["password"]); 
                  smtp.Send(mail); 
            }
            catch (Exception ex)
            { 
                result = ex.Message;
            }
            finally
            { 
            }
            return result;
        }

        public static string GenerateRandom()
        {
            string result = "";
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            result = new String(stringChars);
            return result;
        }    }
}
