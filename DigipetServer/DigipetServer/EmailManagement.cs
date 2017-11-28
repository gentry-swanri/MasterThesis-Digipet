using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace DigipetServer
{
    class EmailManagement
    {
        public EmailManagement()
        {

        }

        public int SendEmail(string to, string resetPassword, int emailType)
        {
            int result = -1;

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.UseDefaultCredentials = false;

                message.From = new MailAddress("digipetapp@gmail.com");
                message.To.Add(to);
                if (emailType == 1)
                {
                    message.Subject = "Digipet - Forgotten Password";
                    message.Body = "Good day. \n \nThis is the reset password for you. Please enter this password when you want to login to Digipet. Your new password is : " + resetPassword + " \n \nFor security purpose, please change this password after login \n \nBest regards \n \nDigipet Team";
                }else
                {
                    if (emailType == 2)
                    {
                        message.Subject = "Digipet - Change Password";
                        message.Body = "Good day. \n \nWe want to inform you that your password was recently changed. If you feel that you did not changed your password then please contact our team by sending an email to this address. If you are the one who did this then you do not have to worry. \n \nBest regards \n \nDigipetTeam ";
                    }
                }

                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("digipetapp@gmail.com", "");
                smtp.EnableSsl = true;

                smtp.Send(message);

                result = 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public string CreateRandomPassword(int length)
        {
            string newRandomPassword = "";

            Random r = new Random();
            for (int i=0; i<length; i++)
            {
                int newInt = r.Next(10);
                newRandomPassword += newInt;
            }

            return newRandomPassword;
        }
    }
}
