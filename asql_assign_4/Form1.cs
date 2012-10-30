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
        List<String> sourceOnly;
        List<String> both;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tb_todo.Clear();
            btn_copy.Enabled = false;
            Dictionary<String, List<Row>> source = LoadDB(tb_source.Text);
            Dictionary<String, List<Row>> dest = LoadDB(tb_dest.Text);
            List<String> match = new List<string>();
            List<String> mismatch = new List<string>();

            var source_only = source.Keys.Select(t => t).Where(t => !dest.ContainsKey(t));
            if(source_only.Count() != 0)
            {
                tb_todo.Text += "Source only" + Environment.NewLine;
                foreach (var s in source_only)
                {
                    tb_todo.Text += "└" + s + Environment.NewLine;
                }
            }

            var both = source.Select(t => t.Key).Where(t => dest.ContainsKey(t));
            if (both.Count() != 0)
            {
                foreach (var b in both)
                {
                    if (source[b].Count() == dest[b].Count()) 
                    {
                        if(CompareList(source[b], dest[b]))
                        {
                            match.Add(b);   
                        }
                        else
                        {
                            mismatch.Add(b);
                        }
                    }
                }

                if(match.Count() != 0)
                {
                    tb_todo.Text += "Both" + Environment.NewLine;
                    foreach (var b in match)
                    {
                        tb_todo.Text += "└" + b + Environment.NewLine;
                    }
                }
            }

            if (mismatch.Count() != 0)
            {
                StringBuilder message = new StringBuilder();
                message.Append("The following Tables exist in both databases but contain different columns");
                foreach (var s in mismatch)
                {
                    message.Append(Environment.NewLine);
                    message.Append(s);
                }
                MessageBox.Show(message.ToString());
                return;
            }
            else
            {
                sourceOnly = source_only.ToList<String>();
                this.both = both.ToList<String>();
                btn_copy.Enabled = true;
            }
        }

        private bool CompareList<T>(List<T> l1, List<T> l2)
        {
            foreach (var element in l1)
            {
                if(!l2.Contains(element))
                {
                    return false;
                }
            }
            return true;
        }

        private Dictionary<String, List<Row>> LoadDB(String connectionString)
        {
            Dictionary<String, List<Row>> tableList = new Dictionary<string,List<Row>>();
            SqlConnection db = new SqlConnection(connectionString);
            db.Open();
            var tables = db.GetSchema("Tables").AsEnumerable().Select(t => t).OrderBy(t => t["table_name"].ToString().ToLower()).OrderBy(t => t["table_schema"].ToString().ToLower());
            var columns = db.GetSchema("Columns").AsEnumerable().Select(c => c);

            foreach (var t in tables)
            {
                String table = t["table_schema"] + "." + t["Table_name"];
                List<Row> rows = new List<Row>();
                var tableColumns = columns.Where(c => c["table_schema"].Equals(t["table_schema"])).Where(c => c["table_name"].Equals(t["table_name"])).OrderBy(c => c["column_name"].ToString().ToLower());

                foreach (var column in tableColumns)
                {
                    rows.Add(new Row(column["column_name"].ToString(), column["data_type"].ToString(), column["character_maximum_length"].ToString()));
                }
                tableList.Add(table, rows);
            }
            return tableList;
        }


        private class Row
        {
            public string name { get; set; }
            public string type { get; set; }
            public string size { get; set; }

            public Row(String name, String type, String size)
            {
                this.name = name;
                this.type = type;
                this.size = size;
            }
            
            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                Row p = obj as Row;
                if ((System.Object)p == null)
                {
                    return false;
                }

                return name.Equals(p.name) && type.Equals(p.type) && size.Equals(p.size);
            }

            public override int GetHashCode()
            {
                return (name + type + size).GetHashCode();
            }
        }

        private void btn_copy_Click(object sender, EventArgs e)
        {
            CopyDb(tb_source.Text, tb_dest.Text);
        }

        private void CopyDb(String source, String destination)
        {
            
            SqlConnection sourceDb = null;
            SqlConnection destDb = null;
            SqlTransaction sourceTrans = null;
            SqlTransaction destTrans = null;
            try{
                sourceDb = new SqlConnection(source);
                destDb = new SqlConnection(destination);
                sourceDb.Open();
                destDb.Open();
                sourceTrans = sourceDb.BeginTransaction();
                destTrans = destDb.BeginTransaction();

                if (both != null && both.Count > 0)
                {
                    foreach (var table in both)
                    {
                        SqlCommand cmd = new SqlCommand(String.Format(@"select * from {0}", table), sourceDb);
                        cmd.Transaction = sourceTrans;
                        SqlDataReader results = cmd.ExecuteReader();

                        SqlBulkCopy bulkCopy = new SqlBulkCopy(destDb, SqlBulkCopyOptions.Default, destTrans);
                        bulkCopy.DestinationTableName = table;
                        bulkCopy.WriteToServer(results);
                        results.Close();
                        bulkCopy.Close();
                    }
                }
                sourceTrans.Commit();
                destTrans.Commit();
                btn_copy.Enabled = false;
            }
            catch (Exception ex)
            {
                sourceTrans.Rollback();
                destTrans.Rollback();
                sourceDb.Close();
                destDb.Close();

                MessageBox.Show(ex.Message);
            }
        }

        private void tb_source_TextChanged(object sender, EventArgs e)
        {
            btn_copy.Enabled = false;
        }

        private void tb_dest_TextChanged(object sender, EventArgs e)
        {
            btn_copy.Enabled = false;
        }
    }
}
