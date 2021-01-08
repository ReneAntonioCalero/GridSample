using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace GridSample
{
    public class Record
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<RecordVersion> Versions { get; set; }

        public Record()
        {
            Versions = new List<RecordVersion>();
        }
    }

    public class RecordVersion
    {
        public string Name { get; set; }
    }

    public static class DataHelper
    {
        public static BindingList<Record> GetRecords()
        {
            var list = new BindingList<Record>();

            for (int i = 0; i < 10; i++)
            {
                var record = new Record
                {
                    Id = Guid.NewGuid(),
                    Name = $"Foobar {i + 1}",
                };

                for (int j = 0; j <= i; j++)
                {
                    var version = new RecordVersion {Name = $"v{j + 1}"};
                    record.Versions.Add(version);
                }

                list.Add(record);
            }

            return list;
        }
    }


    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GridControl gridControl1 = new GridControl();
            gridControl1.Parent = this;
            gridControl1.Dock = DockStyle.Fill;

            gridControl1.DataSource = DataHelper.GetRecords();

            GridView masterGridView = new GridView();
            gridControl1.MainView = masterGridView;
            masterGridView.OptionsDetail.EnableMasterViewMode = false;
            GridColumn colDetails = masterGridView.Columns.AddVisible("Details");
            colDetails.UnboundType = DevExpress.Data.UnboundColumnType.Object;

             masterGridView.CustomUnboundColumnData += MasterGridView_CustomUnboundColumnData;

            masterGridView.CustomRowCellEdit += MasterGridView_CustomRowCellEdit;
        }

        Dictionary<int, RepositoryItemGridLookUpEdit> editors = new Dictionary<int, RepositoryItemGridLookUpEdit>();
        private void MasterGridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "Details") {
                int key = view.GetDataSourceRowIndex(e.RowHandle);
                RepositoryItemGridLookUpEdit ri = null;
                if (!editors.TryGetValue(key, out ri)) {
                    ri = new RepositoryItemGridLookUpEdit();
                    ri.DisplayMember = nameof(RecordVersion.Name);
                    ri.ValueMember = nameof(RecordVersion.Name);
                    Record masterRow = view.GetRow(e.RowHandle) as Record;
                    ri.DataSource = masterRow.Versions;
                    editors[key] = ri;
                }
                else
                    e.RepositoryItem = ri;
            }
        }


        Dictionary<int, string> unboundData = new Dictionary<int, string>();
        private void MasterGridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e) {
            if (e.Column.FieldName == "Details") {
                if (e.IsGetData) {
                    if (unboundData.ContainsKey(e.ListSourceRowIndex))
                        e.Value = unboundData[e.ListSourceRowIndex];
                }
                if (e.IsSetData && e.Value != null) {
                    unboundData[e.ListSourceRowIndex] = e.Value.ToString();
                }
            }
        }
    }
}
