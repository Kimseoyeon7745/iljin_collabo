﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PublicLibsManagement;
using MysqlLib;
using les;

namespace iljin.popUp
{
    public partial class poptaxbill : ApplicationRoot
    {
        DB_mysql km;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (km == null) km = new DB_mysql();

                hdn_code.Value = Request.Params.Get("code");
                hdn_cusCode.Value = Request.Params.Get("cusCode");
                hdn_serialNo.Value = Request.Params.Get("serialNo");

                if (hdn_serialNo.Value == "")
                {
                    txt_serialNo.Text = les_Tool_DB.SetCode("tb_taxbill", "serialNo", ConstClass.TAXBILL_CODE_PREFIX, km);
                }
                else
                {
                    txt_serialNo.Text = hdn_serialNo.Value;
                }


                cb_billtypecode.Items.Add(new ListItem("청구", "0"));
                cb_billtypecode.Items.Add(new ListItem("영수", "1"));

                Search_Company();
                Search_Customer();
                Search_Order();
            }
        }

        //공급자 정보
        private void Search_Company()
        {
            if (km == null) km = new DB_mysql();

            DataTable dt = PROCEDURE.SELECT("SP_company_Info", km);

            txt_registration1.Text = dt.Rows[0]["comregistration"].ToString();
            txt_cusName1.Text = dt.Rows[0]["comName"].ToString();
            txt_bossname1.Text = dt.Rows[0]["comBossName"].ToString();
            txt_address.Text = dt.Rows[0]["address1"].ToString() + " / " + dt.Rows[0]["address2"].ToString();
            txt_business.Text = dt.Rows[0]["combusiness"].ToString();
            txt_businessitem.Text = dt.Rows[0]["comType"].ToString();
            txt_email.Text = dt.Rows[0]["commail"].ToString();
        }

        //공급받는자 정보
        private void Search_Customer()
        {
            if (km == null) km = new DB_mysql();

            if (hdn_serialNo.Value == "")
            {
                DataTable dt = PROCEDURE.SELECT("SP_customer_GetByCode", hdn_cusCode.Value, km);

                txt_registration2.Text = dt.Rows[0]["registration"].ToString();
                txt_cusName2.Text = dt.Rows[0]["cusName"].ToString();
                txt_bossname2.Text = dt.Rows[0]["cusBossName"].ToString();
                txt_address2.Text = dt.Rows[0]["address1"].ToString() + " / " + dt.Rows[0]["address2"].ToString();
                txt_business2.Text = dt.Rows[0]["business"].ToString();
                txt_businessitem2.Text = dt.Rows[0]["businessItem"].ToString();
                txt_email2.Text = dt.Rows[0]["email"].ToString();
            }
            else
            {
                DataTable dt = PROCEDURE.SELECT("SP_taxbill_cusinfo_pop_GetByNo", hdn_serialNo.Value, km);

                txt_registration2.Text = dt.Rows[0]["cus_registration"].ToString();
                txt_cusName2.Text = dt.Rows[0]["cus_name"].ToString();
                txt_businessNo2.Text = dt.Rows[0]["cus_employeeNo"].ToString();
                txt_bossname2.Text = dt.Rows[0]["cus_ceo"].ToString();
                txt_address2.Text = dt.Rows[0]["cus_address"].ToString();
                txt_business2.Text = dt.Rows[0]["cus_business"].ToString();
                txt_businessitem2.Text = dt.Rows[0]["cus_business_item"].ToString();
                txt_email2.Text = dt.Rows[0]["cus_email"].ToString();
            }
        }

        //품목정보
        private void Search_Order()
        {
            if (km == null) km = new DB_mysql();

            if (hdn_serialNo.Value != "")
            {
                DataTable dt = PROCEDURE.SELECT("SP_taxbil_pop_GetByNo", hdn_serialNo.Value, km);

                txt_itemName.Text = dt.Rows[0]["itemName"].ToString();
                txt_registrationDate.Text = dt.Rows[0]["registrationDate"].ToString();
                txt_produceCost.Text = dt.Rows[0]["producePrice"].ToString();
                txt_taxCost.Text = dt.Rows[0]["taxCost"].ToString();

                cb_billtypecode.SelectedValue = dt.Rows[0]["billTypeCode"].ToString();
                string chk = dt.Rows[0]["isTaxFree"].ToString();

                if (chk == "1")
                {
                    chk_taxfree.Checked = true;
                    txt_taxCost.Text = (int.Parse(txt_produceCost.Text ) / 10).ToString();
                    txt_taxCost.Attributes.Add("style", "visibility:hidden");
                    txt_totalCost.Text = txt_produceCost.Text;
                }
                else
                {
                    txt_taxCost.Text = (int.Parse(txt_produceCost.Text) / 10).ToString();
                    txt_totalCost.Text = (int.Parse(txt_produceCost.Text) + int.Parse(txt_taxCost.Text)).ToString();
                }
            }
            else
            {
                DataTable dt = PROCEDURE.SELECT_TRAN("SP_taxbill_GetOrderList", hdn_code.Value, km);

                txt_registrationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                string itemName = "";
                int prodCost = 0;
                int taxCost = 0;
                int totalCost = 0;
                int itemCount = 0;

                itemName = dt.Rows[0][1].ToString();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    prodCost += int.Parse(dt.Rows[i][5].ToString());
                    taxCost += int.Parse(dt.Rows[i][6].ToString());
                    itemCount += int.Parse(dt.Rows[i][2].ToString());
                }

                if (itemCount > 1) itemName += " 외";

                txt_itemName.Text = itemName;
                txt_produceCost.Text = prodCost.ToString();
                txt_taxCost.Text = taxCost.ToString();
                txt_totalCost.Text = (prodCost + taxCost).ToString();

            }

        }

        protected void btn_save_Click(object sender, EventArgs e)
        {
            if (km == null) km = new DB_mysql();

            try
            {
                km.BeginTran();

                string tax = txt_taxCost.Text;
                if (chk_taxfree.Checked) tax = "0";

                if (hdn_serialNo.Value == "") //추가
                {
                    string serialNo = les_Tool_DB.SetCode_Tran("tb_taxbill", "serialNo", ConstClass.TAXBILL_CODE_PREFIX, km);

                    object[] cusObjs = { serialNo, txt_registration2, txt_cusName2, txt_businessNo2, txt_bossname2, txt_address2, txt_business2, txt_businessitem2, txt_email2 };

                    PROCEDURE.CUD_TRAN("SP_taxbill_cusinfo_Add", cusObjs, km);

                    object[] objs = { serialNo, hdn_cusCode.Value, cb_billtypecode, chk_taxfree, txt_registrationDate, txt_itemName, txt_produceCost, tax };

                    PROCEDURE.CUD_TRAN("SP_taxbill_Add", objs, km);

                    km.tran_ExSQL_Ret($"UPDATE tb_order_master SET taxbillserialNo = '{serialNo}' WHERE orderCode IN ({hdn_code.Value});");

                    km.Commit();

                    Response.Write("<script>alert('저장되었습니다.'); window.opener.refresh(); window.close();</script>");
                }
                else // 수정
                {
                    object[] cusObjs = { hdn_serialNo, txt_registration2, txt_cusName2, txt_businessNo2, txt_bossname2, txt_address2, txt_business2, txt_businessitem2, txt_email2 };

                    PROCEDURE.CUD_TRAN("SP_taxbill_cusinfo_Update", cusObjs, km);

                    object[] objs = { hdn_serialNo, cb_billtypecode, chk_taxfree, txt_itemName, tax };

                    PROCEDURE.CUD_TRAN("SP_taxbill_Update", objs, km);

                    km.Commit();

                    Response.Write("<script>alert('수정되었습니다.'); window.opener.refresh(); window.close();</script>");
                }
            }
            catch (Exception ex)
            {
                PROCEDURE.ERROR_ROLLBACK(ex.Message, km);

                Response.Write("<script>alert('저장 실패');</script>");
            }
        }
    }
}