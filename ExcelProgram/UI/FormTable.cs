using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExcelProgram
{
    public partial class FormTable : Form
    {
        private DataTable _Data;
        private List<DataColumn> _NameCol;
        private List<DataColumn> _AreaCol;
        private List<DataColumn> _CountCol;
        private DataColumn _SelNameCol;
        private DataColumn _SelAreaCol;
        private DataColumn _SelCountCol;
        private static int _NCount;
        private static int _ACount;
        private static int _CCount;

        public static int NCount
        {
            get { return _NCount; }
        }

        public static int ACount
        {
            get { return _ACount; }
        }

        public static int CCount
        {
            get { return _CCount; }
        }


        public FormTable(ref DataParser data)
        {
            InitializeComponent();

            _Data = data.Table;

            _NameCol = new List<DataColumn>();
            _AreaCol = new List<DataColumn>();
            _CountCol = new List<DataColumn>();

            foreach (DataColumn c in _Data.Columns)
            {
                _NameCol.Add(c);
                _AreaCol.Add(c);
                _CountCol.Add(c);
            }

            this.dataGridView1.DataSource = _Data;
            for (int i = 0; i < dataGridView1.Columns.Count; ++i)
            {
                this.dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            this.comboBoxName.DataSource = _NameCol;
            this.comboBoxName.DisplayMember = "ColumnName";
            this.comboBoxArea.DataSource = _AreaCol;
            this.comboBoxArea.DisplayMember = "ColumnName";
            this.comboBoxCount.DataSource = _CountCol;
            this.comboBoxCount.DisplayMember = "ColumnName";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ShapeMaker shapeMaker = new ShapeMaker(_Data, _SelNameCol, _SelAreaCol, _SelCountCol);

            FormFile.NameCol = _SelNameCol.ColumnName;
            FormFile.AreaCol = _SelAreaCol.ColumnName;
            FormFile.CountCol = _SelCountCol.ColumnName;

            this.Close();
        }

        private void comboBoxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelNameCol = this.comboBoxName.SelectedItem as DataColumn;
        }

        private void comboBoxArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelAreaCol = this.comboBoxArea.SelectedItem as DataColumn;
        }

        private void comboBoxCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelCountCol = this.comboBoxCount.SelectedItem as DataColumn;
        }

        private void numericUpDownName_ValueChanged(object sender, EventArgs e)
        {
            _NCount = Convert.ToInt32(this.numericUpDownName.Value);
        }

        private void numericUpDownArea_ValueChanged(object sender, EventArgs e)
        {
            _ACount = Convert.ToInt32(this.numericUpDownArea.Value);
        }

        private void numericUpDownCount_ValueChanged(object sender, EventArgs e)
        {
            _CCount = Convert.ToInt32(this.numericUpDownCount.Value);
        }
    }
}
