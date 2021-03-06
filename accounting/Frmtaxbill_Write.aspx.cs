using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PublicLibsManagement;
using MysqlLib;
using les;
using System.Drawing;

namespace iljin
{
    public partial class Frmtaxbill_Write : ApplicationRoot
    {
        DB_mysql km;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (km == null) km = new DB_mysql();

                les_Tool.Set_TextBoxes_Period_MM_01_To_Now(tb_orderdate, tb_orderdate2);
                Search();
            }
        }

        private void Search()
        {
            if (km == null) km = new DB_mysql();

            object[] objs = { tb_orderdate, tb_orderdate2, txt_customer };
            DataTable dt = PROCEDURE.SELECT("SP_taxbill_write_GetBySearch", objs, km);

            // les_DataGridSystem.Set_DataGrid_From_Dt(grdTable, dt, 1, grdTable.Columns.Count - 1, 0);
            string[] fields = { "hdn_code" };
            les_DataGridSystem.Set_DataGrid_From_Search_Dt(grdTable, dt, fields);
            Button btn;

            for (int i = 0; i < grdTable.Items.Count; i++)
            {
                btn = grdTable.Items[i].FindControl("btn_sent") as Button;

                if (dt.Rows[i]["taxbillserialNo"].ToString() != "")
                {
                    btn.Text = "발행완료";
                    btn.BackColor = Color.Green;
                    btn.Attributes.Add("onclick", "return false;");
                }
                else
                {
                    btn.Attributes.Add("onclick", "return false;");
                }
            }
        }

        protected void btn_sch_Click(object sender, EventArgs e)
        {
            Search();
        }
    }
}