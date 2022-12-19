using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/**
 * Author: Baby Bear
 * Created At: 12/16/2022
 */
namespace BugTracker
{
    public partial class Form1 : Form
    {

        private int selectedBugId = 0;

        public Form1()
        {

            InitializeComponent();


            using (BugTrackerDb dbContext = new MyContextFactory().Create())
            {
                // Initialize Database and Tables
                // Severity and Priority
                if(dbContext.Severities.Count() == 0)
                {
                    dbContext.Severities.Add(new Severity { Name = "Level 1", Desc = 1 });
                    dbContext.Severities.Add(new Severity { Name = "Level 2", Desc = 2 });

                    dbContext.Priorities.Add(new Priority { Name = "Level 1", Desc = 1 });
                    dbContext.Priorities.Add(new Priority { Name = "Level 2", Desc = 2 });
                    dbContext.Priorities.Add(new Priority { Name = "Level 3", Desc = 3 });

                    dbContext.Bugs.Add(new Bug
                    {
                        Name = "New Bub",
                        Description = "This is a bug.",
                        CreatorId = 1,
                        PriorityId = 1,
                        SeverityId = 1,
                        CreationDate = DateTime.Now,
                        LastUpdateDate = DateTime.Now,
                        Solved = 0
                    });

                    dbContext.Bugs.Add(new Bug
                    {
                        Name = "Second Bub",
                        Description = "This is second bug.",
                        CreatorId = 1,
                        PriorityId = 1,
                        SeverityId = 1,
                        CreationDate = DateTime.Now,
                        LastUpdateDate = DateTime.Now,
                        Solved = 0
                    });

                    dbContext.SaveChanges();
                }

                var query = from t in dbContext.Priorities
                            orderby t.Desc
                            select t;

                foreach (var t in query)
                {
                    comboBox_bug_prio.Items.Add(t.Name);
                }

                var severities = from t in dbContext.Severities
                                 orderby t.Desc
                                 select t;

                foreach (var t in severities)
                {
                    comboBox_bug_seve.Items.Add(t.Name);
                }

                var bugs = from t in dbContext.Bugs
                           orderby t.CreationDate descending, t.LastUpdateDate descending
                           select t;

                //dataGridView_selections.DataSource = bugs.ToList();

                var i = 1;
                foreach (var t in bugs)
                {
                    dataGridView_selections.Rows.Add(new object[] { i++, t.Name, t.Description, t.Id });
                }

                //

                dateTimePicker_selection_cre_from.Value = DateTime.Now.AddYears(-1);
                dateTimePicker_selection_cre_to.Value = DateTime.Now;
                dateTimePicker_selection_lu_from.Value = DateTime.Now.AddYears(-1);
                dateTimePicker_selection_lu_to.Value = DateTime.Now;
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private string getConnectionString()
        {
            // DB Connect
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.IntegratedSecurity = true;
            builder.DataSource = "localhost";
            builder.UserID = "baby";
            builder.Password = "babybear";
            builder.InitialCatalog = "BugTracker";

            return builder.ConnectionString;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv == null)
                return;
            //if (dataGridView_selections.SelectedRows.Count > 0)
            {
                //
                selectedBugId = (int)dataGridView_selections.Rows[e.RowIndex].Cells[3].Value;


                tabControl1.SelectTab(tabPage_bug);
                //
                loadBugInfo();

                //
            }
        }

        private void button_selection_search_Click(object sender, EventArgs e)
        {
            // Search action
            var searchBidId = (int)numericUpDown_selection_bugid.Value;
            var searchCDFrom = dateTimePicker_selection_cre_from.Value;
            var searchCDTo = dateTimePicker_selection_cre_to.Value;
            var searchLDFrom = dateTimePicker_selection_lu_from.Value;
            var searchLDTo = dateTimePicker_selection_lu_to.Value;
            var searchName = textBox_selection_name.Text;
            var searchDisc = textBox_selection_desc.Text;
            var searchPri = comboBox_selection_pri.Text;
            var searchSev = comboBox_selection_sev.Text;
            var searchSolv = comboBox_selection_solv.Text;

            using (BugTrackerDb dbContext = new MyContextFactory().Create())
            {
                var bugs = from t in dbContext.Bugs
                           select t;

                if(searchBidId != 0)
                {
                    bugs = bugs.Where(b => b.Id == searchBidId);
                }

                bugs = bugs.Where(b => b.CreationDate >= searchCDFrom);
                bugs = bugs.Where(b => DbFunctions.TruncateTime(b.CreationDate) <= searchCDTo);
                bugs = bugs.Where(b => b.LastUpdateDate >= searchLDFrom);
                bugs = bugs.Where(b => DbFunctions.TruncateTime(b.LastUpdateDate) <= searchLDTo);

                if (!string.IsNullOrEmpty(searchName))
                {
                    bugs = bugs.Where(b => b.Name.Contains(searchName));
                }
                if (!string.IsNullOrEmpty(searchDisc))
                {
                    bugs = bugs.Where(b => b.Description.Contains(searchDisc));
                }

                if (!string.IsNullOrEmpty(searchPri))
                {
                    var prio = dbContext.Priorities.Where(p => p.Name == searchPri).FirstOrDefault();

                    bugs = bugs.Where(b => b.PriorityId == prio.Id);
                }
                if (!string.IsNullOrEmpty(searchSev))
                {
                    var serev = dbContext.Severities.Where(p => p.Name == searchSev).FirstOrDefault();

                    bugs = bugs.Where(b => b.SeverityId == serev.Id);
                }
                if (!string.IsNullOrEmpty(searchSolv))
                {
                    bugs = bugs.Where(b => b.Solved == (searchSolv == "Yes" ? 1 : 0));
                }


                bugs = bugs.OrderByDescending(b => b.CreationDate).OrderByDescending(b => b.LastUpdateDate);


                //dataGridView_selections.DataSource = bugs.ToList();

                dataGridView_selections.Rows.Clear();

                var i = 1;
                foreach (var t in bugs)
                {
                    dataGridView_selections.Rows.Add(new object[] { i++, t.Name, t.Description, t.Id });
                }
            }

        }

        private void button_bug_cancel_Click(object sender, EventArgs e)
        {
            loadBugInfo();

            disableEditableBugInfo();
        }

        private void loadBugInfo()
        {

            dataGridView3.Rows.Clear();

            using (BugTrackerDb dbContext = new MyContextFactory().Create())
            {
                var bug = dbContext.Bugs.Where(b => b.Id == selectedBugId).FirstOrDefault();

                textBox_bug_name.Text = bug.Name;
                textBox_bug_desc.Text = bug.Description;

                var prio = dbContext.Priorities.Where(p => p.Id == bug.PriorityId).FirstOrDefault();

                comboBox_bug_prio.Text = prio.Name;

                var sever = dbContext.Severities.Where(s => s.Id == bug.SeverityId).FirstOrDefault();
                comboBox_bug_seve.Text = sever.Name;
                comboBox_bug_solve.Text = bug.Solved == 1 ? "Yes" : "No";

                var messages = from t in dbContext.Messages
                               where t.BugId == selectedBugId
                               orderby t.CreationDate descending
                               select t;

                //dataGridView_selections.DataSource = bugs.ToList();


                foreach (var t in messages)
                {
                    dataGridView3.Rows.Add(new object[] { t.Title, t.CreatorId, t.CreationDate });
                }

                var logs = from t in dbContext.Logs
                           where t.BugId == selectedBugId
                           orderby t.CreationDate descending
                           select t;

                //dataGridView_selections.DataSource = bugs.ToList();

                foreach (var t in logs)
                {
                    richTextBox2.Text += t.Text + "(" + t.CreationDate + ")/n";
                }
            }
        }

        private void button_bug_save_Click(object sender, EventArgs e)
        {
            using (BugTrackerDb dbContext = new MyContextFactory().Create())
            {
                var prio = dbContext.Priorities.Where(p => p.Name == comboBox_bug_prio.Text).FirstOrDefault();

                var sever = dbContext.Priorities.Where(p => p.Name == comboBox_bug_seve.Text).FirstOrDefault();

                var bug = dbContext.Bugs.Where(p => p.Id == selectedBugId).FirstOrDefault();

                bug.Name = textBox_bug_name.Text;
                bug.Description = textBox_bug_desc.Text;
                bug.PriorityId = prio.Id;
                bug.SeverityId = sever.Id;
                bug.LastUpdateDate = DateTime.Now;
                bug.Solved = comboBox_bug_solve.Text == "Yes" ? 1 : 0;

                // Log
                dbContext.Logs.Add(new Log
                {
                    BugId = selectedBugId,
                    Text = "Name : " + bug.Name + ", Description : " + bug.Description,
                    CreationDate = DateTime.Now
                });

                dbContext.SaveChanges();

                disableEditableBugInfo();

                button_selection_search_Click(null, null);

                

            }
        }

        private void enableEditableBugInfo()
        {
            textBox_bug_name.ReadOnly = false;
            textBox_bug_desc.ReadOnly = false;
            comboBox_bug_prio.Enabled = true;
            comboBox_bug_seve.Enabled = true;
            comboBox_bug_solve.Enabled = true;
        }
        private void disableEditableBugInfo()
        {
            textBox_bug_name.ReadOnly = true;
            textBox_bug_desc.ReadOnly = true;
            comboBox_bug_prio.Enabled = false;
            comboBox_bug_seve.Enabled = false;
            comboBox_bug_solve.Enabled = false;
        }

        private void button_bug_edit_Click(object sender, EventArgs e)
        {
            enableEditableBugInfo();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            tabControl1.SelectTab(tabPage_message);
        }

        private void button_message_save_Click(object sender, EventArgs e)
        {
            errorProvider_message_cre.Clear();

            if(selectedBugId == 0)
            {
                errorProvider_message_cre.SetError(button_message_save, "Not selected bug!");
                return;
            }


            using (BugTrackerDb dbContext = new MyContextFactory().Create())
            {
                var creatorId = (int)numericUpDown_message_cre.Value;

                var p = dbContext.Persons.Where(x => x.Id == creatorId).FirstOrDefault();

                if (creatorId == 0 || (p = dbContext.Persons.Where(x => x.Id == creatorId).FirstOrDefault()) != null)
                {
                    errorProvider_message_cre.SetError(numericUpDown_message_cre, "Already exists ID!");
                    return;
                }


                dbContext.Messages.Add(new Message
                {
                    BugId = selectedBugId,
                    Title = textBox_message_tit.Text,
                    Text = richTextBox_message_text.Text,
                    CreationDate = DateTime.Now,
                    CreatorId = (int)numericUpDown_message_cre.Value
                });

                dbContext.SaveChanges();

                textBox_message_tit.Clear();
                richTextBox_message_text.Clear();
                numericUpDown_message_cre.Value = 0;
            }

            loadBugInfo();
        }

        private void button_person_save_Click(object sender, EventArgs e)
        {
            using (BugTrackerDb dbContext = new MyContextFactory().Create())
            {
                dbContext.Persons.Add(new Person
                {
                    FirstName = textBox_person_fn.Text,
                    LastName = textBox_per_ln.Text,
                    JobTitle = textBox_per_jt.Text,
                    Salary = textBox_per_sala.Text,
                    YearsOfExperience = textBox_per_ye.Text,
                    HiredDate = textBox_per_hd.Text,
                    Address = textBox_per_address.Text,
                    Email = textBox_per_email.Text,
                    DateOfBirth = dateTimePicker_per_birth.Text
                }) ;
            }
        }
    }
}
