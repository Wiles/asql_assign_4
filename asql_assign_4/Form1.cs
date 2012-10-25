using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace asql_assign_4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tb_source_info.Clear();
            SqlConnection db = new SqlConnection(tb_source.Text);
            db.Open();
            var tables = db.GetSchema("Tables").AsEnumerable().Select(t => t).OrderBy(t => t["table_name"].ToString().ToLower()).OrderBy(t => t["table_schema"].ToString().ToLower());
            var columns = db.GetSchema("Columns").AsEnumerable().Select(c => c);
           
            foreach (var t in tables )
            {
                String table = t["table_schema"] + "." + t["Table_name"];
                tb_source_info.Text += table + Environment.NewLine;
                var tableColumns = columns.Where(c => c["table_schema"].Equals(t["table_schema"])).Where(c => c["table_name"].Equals(t["table_name"])).OrderBy(c => c["column_name"].ToString().ToLower());

                foreach (var column in tableColumns)
                {
                    tb_source_info.Text += "└" + column["column_name"] + " [" + column["data_type"] + "(" + column["character_maximum_length"] + ")]" + Environment.NewLine;
                }
            }

        }
    }
}
