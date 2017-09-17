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

        public void CreateStandard()
        {
            _storage.Columns.Clear();
            _storage.Columns.Add(new DataColumn("subject", typeof(string)));
            _storage.Columns.Add(new DataColumn("resolution", typeof(string)));
            _storage.Columns.Add(new DataColumn("magnet", typeof(string)));
            _storage.Columns.Add(new DataColumn("episode", typeof(int)));
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
