using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using C_Global;
using C_Event;
using Language;

namespace M_SDO
{
    class Operation_SDO
    {
        public CSocketEvent m_ClientEvent = null;
        public static int iPageSize = 50;
        /// <summary>
        /// 获取服务器地址列表
        /// </summary>
        /// <param name="mEvent">Socket事件</param>
        /// <param name="mContent">消息内容</param>
        /// <returns>请求结果</returns>
        public static CEnum.Message_Body[,] GetServerList(CSocketEvent mEvent, CEnum.Message_Body[] mContent)
        {
            CEnum.Message_Body[,] mReturn = null;

            mReturn =
                mEvent.RequestResult(CEnum.ServiceKey.SERVERINFO_IP_QUERY, CEnum.Msg_Category.COMMON, mContent);

            return mReturn;
        }

        #region 语言库
        /// <summary>
        ///　文字库
        /// </summary>
        ConfigValue config = null;

        /// <summary>
        /// 初始化华文字语言库
        /// </summary>
        private void IntiFontLib()
        {
            config = (ConfigValue)m_ClientEvent.GetInfo("INI");
        }

        #endregion

        public static object DecodeContent(CEnum.TagFormat mFormat, string mContent)
        {
            object mObject = null;

            switch (mFormat)
            {
                case CEnum.TagFormat.TLV_BOOLEAN:
                    mObject = Convert.ToBoolean(mContent);
                    break;
                case CEnum.TagFormat.TLV_INTEGER:
                    mObject = Convert.ToInt32(mContent);
                    break;
                case CEnum.TagFormat.TLV_DATE:
                    mObject = Convert.ToDateTime(mContent).Date;
                    break;
                case CEnum.TagFormat.TLV_TIME:
                    mObject = Convert.ToDateTime(mContent).TimeOfDay;
                    break;
                case CEnum.TagFormat.TLV_EXTEND:
                case CEnum.TagFormat.TLV_MONEY:
                    mObject = Convert.ToDouble(mContent);
                    break;
                case CEnum.TagFormat.TLV_NUMBER:
                    mObject = Convert.ToInt32(mContent);
                    break;
                case CEnum.TagFormat.TLV_STRING:
                    mObject = mContent;
                    break;
                case CEnum.TagFormat.TLV_TIMESTAMP:
                    mObject = Convert.ToDateTime(mContent);
                    break;
            }

            return mObject;
        }

        /// <summary>
        /// 获取信息列表
        /// </summary>
        /// <param name="mEvent">Socket事件</param>
        /// <param name="mKey">请求标记</param>
        /// <param name="mContent">消息内容</param>
        /// <returns>请求结果</returns>
        public static CEnum.Message_Body[,] GetResult(CSocketEvent mEvent, CEnum.ServiceKey mKey, CEnum.Message_Body[] mContent)
        {
            CEnum.Message_Body[,] mReturn = null;

            mReturn =
                mEvent.RequestResult(mKey, CEnum.Msg_Category.SDO_ADMIN, mContent);

            return mReturn;
        }

        /// <summary>
        /// 构造 ComboBox 内容
        /// </summary>
        /// <param name="val">ComboBox 内容</param>
        /// <param name="mCmbBox">ComboBox 控件</param>
        /// <returns>ComboBox 控件</returns>
        public static System.Windows.Forms.ComboBox BuildCombox(CEnum.Message_Body[,] val, System.Windows.Forms.ComboBox mCmbBox)
        {
            try
            {
                for (int i = 0; i < val.GetLength(0); i++)
                {
                    mCmbBox.Items.Add(val[i, 1].oContent);
                }

                mCmbBox.SelectedIndex = 0;
            }
            catch
            {

            }

            return mCmbBox;
        }

        public static System.Windows.Forms.ComboBox BuildComboxs(CSocketEvent mEvent, CEnum.Message_Body[,] val, System.Windows.Forms.ComboBox mCmbBox)
        {
            //try
            //{
            //    for (int i = 0; i < val.GetLength(0); i++)
            //    {
            //        mCmbBox.Items.Add(val[i, 1].oContent + "(" + val[i, 3].oContent + ")");
            //    }

            //    mCmbBox.SelectedIndex = 0;
            //}
            //catch
            //{

            //}

            //return mCmbBox;
            ConfigValue config = (ConfigValue)mEvent.GetInfo("INI");
            try
            {

                ArrayList All = new ArrayList();
                ArrayList DayLimitList = new ArrayList();
                ArrayList ItemCodeList = new ArrayList();
                for (int i = 0; i < val.GetLength(0); i++)
                {
                    mCmbBox.Items.Add((val[i, 5].oContent.ToString() == "0") ? ("(" + val[i, 3].oContent + ")" + config.ReadConfigValue("MSDO", "OP_Code_perpetuity")) : ("(" + val[i, 3].oContent + ")" + "[" + val[i, 5].oContent + config.ReadConfigValue("MSDO", "OP_Code_days"))+"]");
                    //mCmbBox.Items.Add(val[i, 1].oContent + "(" + val[i, 3].oContent + ")");
                    DayLimitList.Add(val[i, 5].oContent.ToString());
                    ItemCodeList.Add(val[i, 2].oContent.ToString());
                }
                All.Add(DayLimitList);
                All.Add(ItemCodeList);
                mCmbBox.SelectedIndex = 0;
                mCmbBox.Tag = All;
            }
            catch
            {

            }

            return mCmbBox;
        }

        public static DataTable BuildDataTable(CSocketEvent mEvent, CEnum.Message_Body[,] val, System.Windows.Forms.DataGridView mGrid, out int PageCount)
        {
            ConfigValue config = (ConfigValue)mEvent.GetInfo("INI");

            DataTable mDataTable = BuildColumn(mEvent, val);

            if (mGrid.Name == "GrdGift")
            {
                for (int i = 0; i < val.GetLength(0); i++)
                {
                    for (int j = 0; j < val.GetLength(1); j++)
                    {
                        if (val[i, j].eName == CEnum.TagName.SDO_BigType)
                        {
                            if (val[i, j].oContent.ToString() == "0")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_dress");
                            if (val[i, j].oContent.ToString() == "1")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_item");
                            if (val[i, j].oContent.ToString() == "2")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_tz");
                        }

                        if (val[i, j].eName == CEnum.TagName.SDO_SmallType)
                        {
                            if (val[i, j].oContent.ToString() == "0")
                            {
                                if (val[i, j - 1].eName == CEnum.TagName.SDO_BigType && val[i, j - 1].oContent.ToString() == "0")
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_head");
                                else
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_item");
                            }

                            if (val[i, j].oContent.ToString() == "100")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_head");

                            if (val[i, j].oContent.ToString() == "1" || val[i, j].oContent.ToString() == "101")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_hair");
                            if (val[i, j].oContent.ToString() == "2" || val[i, j].oContent.ToString() == "102")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_body");
                            if (val[i, j].oContent.ToString() == "3" || val[i, j].oContent.ToString() == "103")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_leg");
                            if (val[i, j].oContent.ToString() == "4" || val[i, j].oContent.ToString() == "104")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_hand");
                            if (val[i, j].oContent.ToString() == "5" || val[i, j].oContent.ToString() == "105")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_feet");
                            if (val[i, j].oContent.ToString() == "6" || val[i, j].oContent.ToString() == "106")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_face");
                            if (val[i, j].oContent.ToString() == "7")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "IC_Code_glass");
                            if (val[i, j].oContent.ToString() == "150")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_onedress");
                            //if (val[i, j].oContent.ToString() == "200" || val[i, j].oContent.ToString() == "201")
                            //val[i, j].oContent = "套装";
                            if (val[i, j].oContent.ToString() == "200")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_ftz");
                            if (val[i, j].oContent.ToString() == "201")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_mtz");
                            if (val[i, j].oContent.ToString() == "202")
                                val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_gift");
                        }
                    }
                }
            }

            mGrid.DataSource = BuildRow(mEvent, val, mDataTable, out PageCount);

            /*
            DataRow mDataRow = null;
            DataTable mDataTable = new DataTable();

            //创建样式
            mDataTable.Columns.Add("索引", typeof(int));
            mDataTable.Columns.Add("服务器名称", typeof(string));
            mDataTable.Columns.Add("角色名称", typeof(string));
            mDataTable.Columns.Add("激活状态", typeof(bool));
            mDataTable.Columns.Add("在线状态", typeof(bool));
            mDataTable.Columns.Add("封停状态", typeof(bool));
            mDataTable.Columns.Add("服务器", typeof(string));
            mDataTable.Columns.Add("房  间", typeof(string));

            //创建记录
            mDataRow = mDataTable.NewRow();
            mDataRow["索引"] = 0;
            mDataRow["服务器名称"] = "华东";
            mDataRow["角色名称"] = "3G";
            mDataRow["激活状态"] = 0;
            mDataRow["在线状态"] = 0;
            mDataRow["封停状态"] = 0;
            mDataRow["服务器"] = 0;
            mDataRow["房  间"] = 0;

            mDataTable.Rows.Add(mDataRow);

            mDataRow = mDataTable.NewRow();
            mDataRow["索引"] = 0;
            mDataRow["服务器名称"] = "华东2";
            mDataRow["角色名称"] = "3G2";
            mDataRow["激活状态"] = 0;
            mDataRow["在线状态"] = 0;
            mDataRow["封停状态"] = 0;
            mDataRow["服务器"] = 0;
            mDataRow["房  间"] = 0;

            mDataTable.Rows.Add(mDataRow);
            
            return mDataTable;
             * */
            return null;
        }

        public static object GetTimes(CEnum.Message_Body[,] val, string iItemID)
        {
            object oResult = null;
            for (int i = 0; i < val.GetLength(0); i++)
            {
                if (iItemID.Equals(val[i, 0].oContent.ToString()))
                {
                    oResult = val[i, 2].oContent;
                    break;
                }
            }

            return oResult;
        }

        public static string ReturnClientIp()
        {
            string ClientIp = "";
            System.Net.IPAddress[] addressList = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            ClientIp = addressList[0].ToString();
            return ClientIp;
        }

        public static object GetLimit(CEnum.Message_Body[,] val, string iItemID)
        {
            object oResult = null;
            for (int i = 0; i < val.GetLength(0); i++)
            {
                if (iItemID.Equals(val[i, 0].oContent.ToString()))
                {
                    oResult = val[i, 3].oContent;
                    break;
                }
            }

            return oResult;
        }

        #region 私有函数
        /// <summary>
        /// 构造 DataGridView 列
        /// </summary>
        /// <param name="val">数据</param>
        /// <param name="mDataTable">DataTable</param>
        /// <returns>DataTable</returns>
        private static DataTable BuildColumn(CSocketEvent mEvent, CEnum.Message_Body[,] val)
        {
            DataTable mDataTable = new DataTable();
            ConfigValue config = (ConfigValue)mEvent.GetInfo("INI");

            for (int i = 0; i < val.GetLength(1); i++)
            {
                if (val[0, i].eName != CEnum.TagName.PageCount)
                {

                    mDataTable.Columns.Add((string)config.ReadConfigValue("GLOBAL", val[0, i].eName.ToString()), typeof(string));

                    //mDataTable.Columns.Add((string)mEvent.DecodeFieldName(val[0, i].eName), typeof(string));
                }
            }

            return mDataTable;
        }

        /// <summary>
        /// 构造 DataGridView 列
        /// </summary>
        /// <param name="val">数据</param>
        /// <param name="mDataRow">DataRow</param>
        /// <returns>DataRow</returns>
        private static DataTable BuildRow(CSocketEvent mEvent, CEnum.Message_Body[,] val, DataTable mTable, out int PageCount)
        {
            try
            {
                PageCount = 0;
                ConfigValue config = (ConfigValue)mEvent.GetInfo("INI");
                for (int i = 0; i < val.GetLength(0); i++)
                {
                    DataRow mRow = mTable.NewRow();

                    for (int j = 0; j < val.GetLength(1); j++)
                    {
                        if (val[i, j].eName != CEnum.TagName.PageCount)
                        {
                            if (val[i, j].eName == CEnum.TagName.SDO_MoneyType)
                            {
                                if (val[i, j].oContent.ToString() == "1")
                                {
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_Mcash");
                                }
                                else if (val[i, j].oContent.ToString() == "0")
                                {
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_Gcash");
                                }
                                else if (val[i, j].oContent.ToString() == "4")
                                {
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_jifen");
                                }
                            }

                            if (val[i, j].eName == CEnum.TagName.SDO_DaysLimit)
                            {
                                if (val[i, j].oContent.ToString() == "0")
                                {
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_yongjiu");
                                }
                                else if (val[i, j].oContent.ToString() == "7")
                                {
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_7days");
                                }
                                else if (val[i, j].oContent.ToString() == "30")
                                {
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_30days");
                                }
                            }
                            if (val[i, j].eName == CEnum.TagName.SDO_Ispad)
                            {
                                if (val[i, j].oContent.ToString() == "0")
                                {
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "AF_Code_BlanketNotEnabled");
                                }
                                else
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "AF_Code_BlanketEnabled");

                            }
                            if (val[i, j].eName == CEnum.TagName.SDO_BuyTimes)
                            {
                                if (val[i, j].oContent.ToString() == "99999")
                                {
                                    val[i, j].oContent = 0;
                                }
                            }
                            if (val[i, j].eName == CEnum.TagName.SDO_TimesLimit)
                            {
                                if (val[i, j].oContent.ToString() == "99999")
                                {
                                    val[i, j].oContent = 0;
                                }
                            }
                            if (val[i, j].eName == CEnum.TagName.SDO_ActiveStatus)
                            {
                                if (val[i, j].oContent.ToString() == "0")
                                {
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "AF_UI_UnActived");
                                }
                                else
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "AF_UI_Actived");
                            }

                            if (val[i, j].eName == CEnum.TagName.SDO_Status)
                            {
                                if(val[i,j].oContent.ToString() == "0")
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "SDO_Single");
                                else if (val[i, j].oContent.ToString() == "1")
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "SDO_AppMarry");
                                else if (val[i, j].oContent.ToString() == "2")
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "SDO_AppDivorce");
                                else if (val[i, j].oContent.ToString() == "3")
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "SDO_Married");
                            }

                            if (val[i, j].eName == CEnum.TagName.SDO_SEX)
                            {
                                if (val[i, j].oContent.ToString().Equals("0"))
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_Fsex");
                                //mRow[(string)mEvent.DecodeFieldName(val[i, j].eName)] = config.ReadConfigValue("MSDO", "OP_Code_Fsex");
                                else
                                    val[i, j].oContent = config.ReadConfigValue("MSDO", "OP_Code_Msex");
                                //mRow[(string)mEvent.DecodeFieldName(val[i, j].eName)] = config.ReadConfigValue("MSDO", "OP_Code_Msex");
                            }

                            if (val[i, j].oContent == null)
                            {
                                mRow[(string)config.ReadConfigValue("GLOBAL", val[i, j].eName.ToString())] = "N/A";
                                //mRow[(string)mEvent.DecodeFieldName(val[i, j].eName)] = "N/A";
                            }
                            else
                            {
                                mRow[(string)config.ReadConfigValue("GLOBAL", val[i, j].eName.ToString())] = val[i, j].oContent.ToString().Trim();
                                //mRow[(string)mEvent.DecodeFieldName(val[i, j].eName)] = val[i, j].oContent;
                            }
                        }
                        else
                        {
                            PageCount = int.Parse(val[i, j].oContent.ToString());
                        }
                    }

                    mTable.Rows.Add(mRow);
                }

                return mTable;
            }
            catch (Exception ex)
            {
                PageCount = 0;
                return null;
            }

        }

        /// <summary>
        /// 获取 ComboBox 选定项索引
        /// </summary>
        /// <param name="val">记录集</param>
        /// <param name="Condition">搜索条件</param>
        /// <returns></returns>
        public static string GetItemAddr(CEnum.Message_Body[,] val, string Condition)
        {
            string strResult = null;

            for (int i = 0; i < val.GetLength(0); i++)
            {
                for (int j = 0; j < val.GetLength(1); j++)
                {
                    if (val[i, j].eName == CEnum.TagName.ServerInfo_IP && val[i, j + 1].oContent.Equals(Condition))
                    {
                        strResult = val[i, j].oContent.ToString();
                    }
                }
            }

            return strResult;
        }

        public static int GetItemID(CEnum.Message_Body[,] val, string Condition)
        {
            int iResult = 0;

            for (int i = 0; i < val.GetLength(0); i++)
            {
                for (int j = 0; j < val.GetLength(1); j++)
                {
                    if (val[i, j].eName == CEnum.TagName.SDO_ItemCode && val[i, j + 1].oContent.Equals(Condition))
                    {
                        iResult = int.Parse(val[i, j].oContent.ToString());
                    }
                }
            }

            return iResult;
        }

        public static int GetItemID(CEnum.Message_Body[,] val, int Condition)
        {
            int iResult = 0;

            try
            {
                iResult = int.Parse(val[Condition, 0].oContent.ToString());
            }
            catch
            {
                iResult = 0;
            }

            return iResult;
        }
        #endregion
    }
}
