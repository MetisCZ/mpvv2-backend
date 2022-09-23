using System;
using mpvv2.DbModels;

namespace mpvv2.Models
{
    public class Helper
    {
        public static string getDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
        }
        
        public static string getDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static int getInt(string number)
        {
            try { return int.Parse(number); }
            catch (Exception) { return -1; }
        }

        public static string GenerateUuid()
        {
            return System.Guid.NewGuid().ToString("D");
        }
        
        public static bool LogToDatabase(string message)
        {
            try
            {
                var context = new mpvContext();
                Message msg = new Message()
                {
                    Date = DateTime.Now,
                    Message1 = message
                };
                context.Messages.Add(msg);
                if (context.SaveChanges() == 0)
                {
                    Console.Error.WriteLine("Could not save log [{0}] to database, unknown error",message);    
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Could not save log [{0}] to database, error: {1}",message,e.Message);
                return false;
            }
        }
    }
}