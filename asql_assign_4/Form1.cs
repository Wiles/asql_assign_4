using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Text.RegularExpressions;

namespace asql_assign_4
{
    public partial class Form1 : Form
    {
        private Dictionary<String, List<Column>> sources = new Dictionary<string, List<Column>>();
        List<String> sourceOnly = new List<string>();
        List<String> both = new List<string>();


        /// <summary>
        /// Initializes a new instance of the <see cref="Form1" /> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the button1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                pb_progress.Enabled = true;
                pb_progress.Style = ProgressBarStyle.Marquee;
                tb_todo.Clear();
                btn_copy.Enabled = false;
                sources = LoadDB(tb_source.Text);
                Dictionary<String, List<Column>> dest = LoadDB(tb_dest.Text);
                List<String> match = new List<string>();
                List<String> mismatch = new List<string>();

                var source_only = sources.Keys.Select(t => t).Where(t => !dest.ContainsKey(t));
                if (source_only.Count() != 0)
                {
                    tb_todo.Text += "Source only" + Environment.NewLine;
                    foreach (var s in source_only)
                    {
                        tb_todo.Text += "└" + s + Environment.NewLine;
                    }
                }

                var both = sources.Select(t => t.Key).Where(t => dest.ContainsKey(t));
                if (both.Count() != 0)
                {
                    foreach (var b in both)
                    {
                        if (sources[b].Count() == dest[b].Count())
                        {
                            if (CompareList(sources[b], dest[b]))
                            {
                                match.Add(b);
                            }
                            else
                            {
                                mismatch.Add(b);
                            }
                        }
                    }

                    if (match.Count() != 0)
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
                    reset();
                    StringBuilder message = new StringBuilder();
                    message.Append("The following Tables exist in both databases but contain different columns:");
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                pb_progress.Style = ProgressBarStyle.Blocks;

            }
        }

        /// <summary>
        /// Compares the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l1">The first list</param>
        /// <param name="l2">The second list</param>
        /// <returns></returns>
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

        /// <summary>
        /// Loads the DB.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        private Dictionary<String, List<Column>> LoadDB(String connectionString)
        {
            SqlConnection db = new SqlConnection(connectionString);
            Dictionary<String, List<Column>> tableList = new Dictionary<string, List<Column>>();
            db.Open();
            /*
             * This assumes they don't want to copy INFORMATION_SCHEMA or sys tables.
             */
            var tables = db.GetSchema("Tables").AsEnumerable().Select(t => t).Where(t => !t["table_schema"].Equals("INFORMATION_SCHEMA") && !t["table_schema"].Equals("sys")).OrderBy(t => t["table_name"].ToString().ToLower()).OrderBy(t => t["table_schema"].ToString().ToLower());
            
            foreach (var t in tables)
            {
                String table = t["table_schema"] + "." + t["Table_name"];
                SqlCommand cmd = new SqlCommand(String.Format(@"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.Columns WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}'", t["table_schema"], t["table_name"]), db);
                var tableColumns = cmd.ExecuteReader();
                List<Column> rows = new List<Column>();
                
                while(tableColumns.Read())
                {
                    rows.Add(new Column(tableColumns["column_name"].ToString(), tableColumns["data_type"].ToString(), tableColumns["character_maximum_length"].ToString()));
                }
                tableList.Add(table, rows);
                tableColumns.Close();
            }
            return tableList;
        }


        /// <summary>
        /// Handles the Click event of the btn_copy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btn_copy_Click(object sender, EventArgs e)
        {
            btn_copy.Enabled = false;
            CopyDb(tb_source.Text, tb_dest.Text);
        }

        /// <summary>
        /// Copies tables from the source to the destination
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        private void CopyDb(String source, String destination)
        {

            SqlConnection sourceDb = null;
            SqlConnection destDb = null;
            SqlTransaction sourceTrans = null;
            SqlTransaction destTrans = null;
            try
            {
                pb_progress.Style = ProgressBarStyle.Blocks;
                pb_progress.Value = 0;
                pb_progress.Minimum = 0;
                pb_progress.Step = 1;
                pb_progress.Maximum = both.Count + sourceOnly.Count;

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
                        pb_progress.PerformStep();
                    }
                }

                if (sourceOnly != null && sourceOnly.Count > 0)
                {
                    foreach (var table in sourceOnly)
                    {
                        StringBuilder createTable = new StringBuilder();
                        createTable.Append(String.Format(@"create table {0}(", table));
                        foreach (var row in sources[table])
                        {
                            createTable.Append(String.Format(@"{0},", row.toCreateString()));
                        }
                        createTable.Length -= 1;
                        createTable.Append(String.Format(@")"));
                        SqlCommand cmd = new SqlCommand(createTable.ToString(), destDb, destTrans);

                        cmd.ExecuteNonQuery();

                        cmd = new SqlCommand(String.Format(@"select * from {0}", table), sourceDb);
                        cmd.Transaction = sourceTrans;
                        SqlDataReader results = cmd.ExecuteReader();

                        SqlBulkCopy bulkCopy = new SqlBulkCopy(destDb, SqlBulkCopyOptions.Default, destTrans);
                        bulkCopy.DestinationTableName = table;
                        bulkCopy.WriteToServer(results);
                        results.Close();
                        bulkCopy.Close();
                        pb_progress.PerformStep();
                    }
                }

                sourceTrans.Commit();
                destTrans.Commit();
            }
            catch (Exception ex)
            {
                sourceTrans.Rollback();
                destTrans.Rollback();
                sourceDb.Close();
                destDb.Close();

                MessageBox.Show(ex.Message);
            }
            finally
            {
                pb_progress.Value = 0;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the tb_source control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void tb_source_TextChanged(object sender, EventArgs e)
        {
            reset();
        }

        /// <summary>
        /// Handles the TextChanged event of the tb_dest control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void tb_dest_TextChanged(object sender, EventArgs e)
        {
            reset();
        }

        /// <summary>
        /// Returns the form to a neutral state
        /// </summary>
        private void reset()
        {
            tb_todo.Clear();
            btn_copy.Enabled = false;
            sources.Clear();
            sourceOnly.Clear();
            both.Clear();
            pb_progress.Enabled = false;
        }

        /// <summary>
        /// Represents the metadata for a column in a database
        /// </summary>
        private class Column
        {
            public string name { get; set; }
            public string type { get; set; }
            public string size { get; set; }

            public Column(String name, String type, String size)
            {
                this.name = name;
                this.type = type;
                // I think -1 can be used to signify a few things but this is the only use I've run into
                if (size.Equals("-1"))
                {
                    if (type.Equals("nvarchar"))
                    {
                        size = "max";
                    }
                    else
                    {
                        size = null;
                    }
                }
                this.size = size;
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                Column p = obj as Column;
                if ((System.Object)p == null)
                {
                    return false;
                }

                return name.Equals(p.name) && type.Equals(p.type) && size.Equals(p.size);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return (name + type + size).GetHashCode();
            }

            /// <summary>
            /// Create a string usable in a table create statement
            /// </summary>
            /// <returns>create string</returns>
            public string toCreateString()
            {
                return String.Format(@"[{0}] {1} {2}",
                    name,
                    type,
                    !String.IsNullOrEmpty(size) ? "(" + size + ")" : "");
            }
        }
    }
}
