using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace WGUTermOrg
{
    class InputValidation
    {
        //Validates field input and email input specifically 
        public static bool CorrectEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", EmailFix, RegexOptions.None,
                    TimeSpan.FromMilliseconds(200));

                string EmailFix(Match match)
                {
                    var idn = new IdnMapping();

                    var domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
            }

            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }

            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static bool IsNull(string fieldEntry) //Handles empty fields
        {
            if (!String.IsNullOrEmpty(fieldEntry))
            {
                return true;
            }
            else
            {
                return false;

            }
        }
    }
}
