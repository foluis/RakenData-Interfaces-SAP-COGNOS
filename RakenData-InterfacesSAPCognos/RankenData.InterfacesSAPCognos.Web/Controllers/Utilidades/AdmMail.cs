using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades
{
    public static class AdmMail
    {

        private static string AutenticationEmailPassword { get; set; }

        private static string AutenticationEmailUser { get; set; }

        private static bool CredentialsNeeded { get; set; }

        private static bool EnableSSL { get; set; }

        private static string FromEmailUser { get; set; }

        private static string SMTP { get; set; }

        private static int SMTPPort { get; set; }

        private static string ToEmailUser { get; set; }

        private static bool IsMailEnabled { get; set; }
        static AdmMail()
        {

            CredentialsNeeded = ConfigurationManager.AppSettings["credentialsNeeded"] == null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["credentialsNeeded"]);
            EnableSSL = ConfigurationManager.AppSettings["enableSSL"] == null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]);
            SMTP = ConfigurationManager.AppSettings["sMTP"] == null ? string.Empty : ConfigurationManager.AppSettings["sMTP"];
            SMTPPort = ConfigurationManager.AppSettings["sMTPPort"] == null ? 0 : Convert.ToInt16(ConfigurationManager.AppSettings["sMTPPort"]);
            FromEmailUser = ConfigurationManager.AppSettings["fromEmailUser"] == null ? string.Empty : ConfigurationManager.AppSettings["fromEmailUser"];
            ToEmailUser = ConfigurationManager.AppSettings["toEmailUser"] == null ? string.Empty : ConfigurationManager.AppSettings["toEmailUser"];
            AutenticationEmailUser = ConfigurationManager.AppSettings["autenticationEmailUser"] == null ? string.Empty : ConfigurationManager.AppSettings["autenticationEmailUser"];
            AutenticationEmailPassword = ConfigurationManager.AppSettings["autenticationEmailPassword"] == null ? string.Empty : ConfigurationManager.AppSettings["autenticationEmailPassword"];
            IsMailEnabled = true;
        }

        public static Result Enviar(MailInfo mail)
        {
            Result result = new Result();

            try
            {
                if (IsMailEnabled)
                {
                    if (string.IsNullOrEmpty(SMTP) || SMTPPort == 0)
                    {
                        throw new MissingFieldException();
                    }

                    SmtpClient mailClient = new SmtpClient(SMTP);
                    mailClient.Port = SMTPPort;
                    mailClient.EnableSsl = EnableSSL;

                    if (CredentialsNeeded)
                    {
                        if (string.IsNullOrEmpty(AutenticationEmailUser) || string.IsNullOrEmpty(AutenticationEmailPassword))
                        {
                            throw new MissingFieldException();
                        }

                        mailClient.Credentials = new NetworkCredential(AutenticationEmailUser, AutenticationEmailPassword);
                    }

                    MailMessage mailMessage = new MailMessage();

                    if (mail.IsHTMLFormat == null)
                        mailMessage.IsBodyHtml = true;
                    else
                        mailMessage.IsBodyHtml = (bool)mail.IsHTMLFormat;

                    if (mail.Priority == null)
                        mail.Priority = MailPriority.Normal;

                 //   mailMessage.From = new MailAddress(mail.From, "Sistema RankenData");
                    mailMessage.From = new MailAddress(FromEmailUser, "Sistema RankenData");
                    if (mail.Cc != null)
                    {
                        foreach (string destinatario in mail.Cc)
                        {
                            mailMessage.CC.Add(new MailAddress(destinatario));
                        }
                    }

                    if (mail.Bcc != null)
                    {
                        foreach (string destinatario in mail.Bcc)
                        {
                            mailMessage.Bcc.Add(new MailAddress(destinatario));
                        }
                    }

                    if (mail.To != null)
                    {
                        string variosDestinatarios = string.Empty;
                        string[] destinatarios = null;
                        foreach (string destinatario in mail.To)
                        {
                            if (destinatario.IndexOf(",") != -1 || destinatario.IndexOf(";") != -1)
                            {
                                variosDestinatarios = destinatario.Replace(",", ";");
                                destinatarios = variosDestinatarios.Split(';');

                                foreach (string item in destinatarios)
                                {
                                    mailMessage.To.Add(new MailAddress(item));
                                }
                            }
                            else
                            {
                                mailMessage.To.Add(new MailAddress(destinatario));
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(ToEmailUser))
                        {
                            throw new MissingFieldException();
                        }

                        //string[] destinationMails = ToEmailUser.Replace(",", ";").Split(';');
                        List<string> destinationMails = mail.To;
                        foreach (string theMail in destinationMails)
                        {
                            mailMessage.To.Add(new MailAddress(theMail));
                        }
                    }

                    mailMessage.Subject = mail.Subject == null ? string.Empty : mail.Subject;

                    mailMessage.Body = mail.Message == null ? string.Empty : mail.Message;

                    if (mail.Attachment != null)
                    {
                        foreach (Attachment adjunto in mail.Attachment)
                        {
                            mailMessage.Attachments.Add(adjunto);
                        }
                    }

                    mailClient.Send(mailMessage);

                    result.WasSuccessful = true;

                }
            }
            catch (MissingFieldException ex)
            {
                Log.WriteLog(string.Format(Environment.NewLine + "**********  PARAMETROS DEL EMAIL NO CONFIGURADOS. REVISE EL APP.CONFIG FILE **********" + Environment.NewLine), EnumTypeLog.Event, false);
                IsMailEnabled = false;
                result.WasSuccessful = false;
                result.Message = ex.Message;
                result.Exception = ex;
            }
            catch (SmtpException ex)
            {
                Log.WriteLog("EL SERVIDOR DE EMAIL NO ES VALIDO. ", EnumTypeLog.Event, true, ex);
                result.WasSuccessful = false;
                result.Message = ex.Message;
                result.Exception = ex;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Log.WriteLog(string.Format("PARAMETROS DEL EMAIL NO CONFIGURADOS. REVISE EL APP.CONFIG FILE. "), EnumTypeLog.Event, false);
                result.WasSuccessful = false;
                result.Message = ex.Message;
                result.Exception = ex;
            }
            catch (Exception ex)
            {
                Log.WriteLog(string.Format("Error enviando correo. credentialsNeeded:[{0}],  enableSSL:[{1}], sMTP:[{2}], sMTP Port:[{3}], user:[{4}], password:[{5}]",
                    CredentialsNeeded, EnableSSL, SMTP, SMTPPort, AutenticationEmailUser, AutenticationEmailPassword),
                    EnumTypeLog.Error, true, ex);
                result.WasSuccessful = false;
                result.Message = ex.Message;
                result.Exception = ex;
            }
            return result;
        }
    }
}