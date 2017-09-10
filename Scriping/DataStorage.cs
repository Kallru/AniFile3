using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scriping
{
    public partial class DataStorage
    {
        private DataTable _storage;
        
        public DataStorage()
        {
            _storage = new DataTable("Standard");
        }

        public void Load(string fileName)
        {
            _storage.ReadXml(fileName);
        }

        public void Save(string fileName)
        {
            _storage.WriteXml(fileName);
        }

        public void Add(Contents contetns)
        {
            var row = _storage.NewRow();
            row["subject"] = contetns.Subject;
            row["resolution"] = contetns.Resolution;
            row["magnet"] = contetns.Magnet;
            row["episode"] = contetns.Episode;
        }
    }
}
