using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace ExcelProgram
{
    public class DataParser
    {
        private char[] _Delimiter = new char[] { '\t' };
        private char[] _Quotes = new char[] { '"' };
        private string _Name;
        private DataTable _Table;

        public string Name
        {
            get { return _Name; }
        }

        public DataTable Table
        {
            get { return _Table; }
            set { _Table = value; }
        }

        public DataParser(string filename)
        {
            StreamReader reader = File.OpenText(filename);
            string line;
            string[] a;

            while (null != (line = reader.ReadLine()))
            {
                a = line.Split(_Delimiter)
                        .Select<string, string>(s => s.Trim(_Quotes))
                        .ToArray();

                // First line of text file contains 
                // schedule name

                if (null == _Name)
                {
                    _Name = a[0];
                    continue;
                }

                // Second line of text file contains 
                // schedule column names

                if (null == _Table)
                {
                    _Table = new DataTable();

                    foreach (string column_name in a)
                    {
                        DataColumn column = new DataColumn();
                        column.DataType = typeof(string);
                        column.ColumnName = column_name;
                        _Table.Columns.Add(column);
                    }

                    _Table.BeginLoadData();

                    continue;
                }

                // Remaining lines define schedula data

                DataRow dr = _Table.LoadDataRow(a, true);
            }
            _Table.EndLoadData();
        }
    }
}
