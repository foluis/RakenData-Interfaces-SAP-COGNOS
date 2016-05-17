using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades
{
    public static class LDAP
    {
        // static variables connection ldap
        static LdapConnection ldapConnection;
        //static string ldapServer;
        static NetworkCredential credential;

        /// <summary>
        /// Validar usuario y clave en el directorio activo
        /// </summary>
        /// <param name="usuario">usuario del funcionario</param>
        /// <param name="clave">clave del funcionario</param>
        /// <returns></returns>
        public static bool ValidarUsuario(string usuario, string clave)
        {
            string servidorLDAP = ConfigurationManager.AppSettings["servidorLDAP"];
            string usuarioLDAP = ConfigurationManager.AppSettings["usuarioLDAP"];
            string claveLDAP = ConfigurationManager.AppSettings["claveLDAP"];
            string dominio = ConfigurationManager.AppSettings["dominio"];
            // string targetOU = "OU=VPRE,DC=inco,DC=local";

            try
            {
                // Create the new LDAP connection
                ldapConnection = new LdapConnection(servidorLDAP);
                ldapConnection.Credential = credential;

                Console.WriteLine("LdapConnection is created successfully.");
                var networkCredential = new NetworkCredential(usuario, clave);
                ldapConnection.Bind(networkCredential);

                credential = new NetworkCredential(usuarioLDAP, claveLDAP, dominio);
                return true;
            }

            catch (Exception ex)
            {
                string sEx = ex.ToString();
                return false;
            }
        }
    }
}