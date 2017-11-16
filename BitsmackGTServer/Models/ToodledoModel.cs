using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Web;
using Toodledo.Client;
using Toodledo.Model;
using Toodledo.Model.API;

namespace BSFullCalendar.Models
{
    public class ToodledoModel
    {
        private Session _session = null;
        public Session Session
        {
            get
            {
                var userid = ConfigurationManager.AppSettings["ToodledoUser"];
                var pw = ConfigurationManager.AppSettings["ToodledoPW"];
                var clientID = ConfigurationManager.AppSettings["ToodledoClientID"];
                return _session ?? Session.Create(userid, pw, clientID);
            }
        }

        public IGeneral General
        {
            get { return (IGeneral)this.Session; }
        }

        public ITasks Tasks
        {
            get { return (ITasks)this.Session; }
        }

        public INotebook Notebook
        {
            get { return (INotebook)this.Session; }
        }

        private IEnumerable<Folder> _folders { get; set; }

        public IEnumerable<Folder> Folders
        {
            get
            {
                if (_folders == null)
                {
                    _folders = General.GetFolders();
                }
                return _folders;
            }
        }

 

        public Folder GetFolder(int id)
        {
            return Folders.FirstOrDefault(x => x.Id == id);
        }
    }
}