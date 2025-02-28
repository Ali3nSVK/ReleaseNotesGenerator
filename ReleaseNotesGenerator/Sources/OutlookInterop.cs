using ReleaseNotesGenerator.Utils;
using System;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace ReleaseNotesGenerator.Sources
{
    public static class OutlookInterop
    {
        public static void PrepareEmail(EmailSettings settings)
        {
            try
            {
                Outlook.Application outlookApp = new Outlook.Application();
                Outlook._MailItem oMailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
                Outlook.Inspector oInspector = oMailItem.GetInspector;

                Outlook.Recipients oRecipients = oMailItem.Recipients;
                foreach (var iRecipient in settings.Recipients)
                {
                    Outlook.Recipient oRecipient = oRecipients.Add(iRecipient);
                    oRecipient.Resolve();
                }

                foreach (var iCC in settings.CC)
                {
                    Outlook.Recipient oCC = oRecipients.Add(iCC);
                    oCC.Type = (int)Outlook.OlMailRecipientType.olCC;
                    oCC.Resolve();
                }

                oMailItem.Subject = settings.Subject;
                oMailItem.HTMLBody = settings.Body;

                oMailItem.Display(true);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
